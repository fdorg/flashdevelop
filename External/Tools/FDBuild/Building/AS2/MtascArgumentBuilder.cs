using System;
using System.IO;
using System.Collections;
using ProjectManager.Projects.AS2;

namespace ProjectManager.Building.AS2
{
    class MtascArgumentBuilder : ArgumentBuilder
    {
        AS2Project project;

        public MtascArgumentBuilder(AS2Project project)
        {
            this.project = project;
        }

        public void AddClassPaths(params string[] extraClassPaths)
        {
            // build classpaths
            ArrayList classPaths = new ArrayList(project.AbsoluteClasspaths);

            foreach (string extraClassPath in extraClassPaths)
                classPaths.Add(extraClassPath);

            foreach (string classPath in classPaths)
                if (Directory.Exists(classPath)) Add("-cp", "\"" + classPath + "\""); // surround with quotes
        }

        public void AddHeader()
        {
            string htmlColor = project.MovieOptions.Background.Substring(1);

            if (htmlColor.Length > 0)
                htmlColor = ":" + htmlColor;

            Add("-header", string.Format("{0}:{1}:{2}{3}",
                project.MovieOptions.Width,
                project.MovieOptions.Height,
                project.MovieOptions.Fps,
                htmlColor));
        }

        public void AddCompileTargets()
        {
            // add project files marked as "always compile"
            foreach (string target in project.CompileTargets)
            {
                // TODO: this is a crappy workaround for the general problem of
                // project "cruft" accumulating.  This will be removed once
                // I add support in the treeview for displaying "missing" project items
                if (File.Exists(target))
                    Add("\"" + target + "\"");
            }
        }

        public void AddInput()
        {
            string path = project.FixDebugReleasePath(project.GetAbsolutePath(project.InputPath));
            Add("-swf", "\"" + path + "\"");
        }

        public void AddOutput()
        {
            string path = project.FixDebugReleasePath(project.OutputPathAbsolute);
            if (project.UsesInjection)
                Add("-out", "\"" + path + "\"");
            else
                Add("-swf", "\"" + path + "\"");
        }

        public void AddFrame(int frame)
        {
            Add("-frame",frame.ToString());
        }
        
        public void AddKeep()
        {
            // always keep existing source - if you add .swf files to the library, expected
            // behavior is to add *everything* in them.
            Add("-keep");           
        }

        public void AddOptions(bool noTrace)
        {
            Add("-version", Math.Max(Math.Min(project.MovieOptions.MajorVersion, 8), 6).ToString());
            
            if (project.CompilerOptions.UseMX)
                Add("-mx");
            
            if (project.CompilerOptions.Infer)
                Add("-infer");

            if (project.CompilerOptions.Strict)
                Add("-strict");

            if (project.CompilerOptions.UseMain)
                Add("-main");

            if (project.CompilerOptions.Verbose)
                Add("-v");

            if (project.CompilerOptions.WarnUnusedImports)
                Add("-wimp");

            if (project.CompilerOptions.ExcludeFile.Length > 0)
                Add("-exclude","\""+project.CompilerOptions.ExcludeFile+"\"");

            if (project.CompilerOptions.GroupClasses)
                Add("-group");

            if (project.UsesInjection)
            {
                Add("-frame",project.CompilerOptions.Frame.ToString());

                if (project.CompilerOptions.Keep)
                    Add("-keep");
            }

            // add project directories marked as "always compile"
            foreach (string target in project.CompileTargets)
                if (Directory.Exists(target))
                {
                    string cp = project.Classpaths.GetClosestParent(target);
                    
                    if (cp == null)
                        throw new Exception("Could not determine the closest classpath off which to compile the directory '" + target + "'.");
                    
                    string relTarget = (cp == ".") ? target : target.Substring(cp.Length + 1);
                    Add("-pack","\"" + relTarget + "\"");
                }

            foreach (string pack in project.CompilerOptions.IncludePackages)
                if (pack.Trim().Length > 0) Add("-pack", pack);

            if (noTrace)
            {
                Add("-trace no");
                return;
            }

            switch (project.CompilerOptions.TraceMode)
            {
                case TraceMode.Disable:
                    Add("-trace no");
                    break;
                case TraceMode.FlashViewer:
                    Add("-trace org.flashdevelop.utils.FlashViewer.trace");
                    Add("org/flashdevelop/utils/FlashViewer.as");
                    break;
                case TraceMode.FlashViewerExtended:
                    Add("-trace org.flashdevelop.utils.FlashViewer.mtrace");
                    Add("org/flashdevelop/utils/FlashViewer.as");
                    break;
                case TraceMode.FlashConnect:
                    Add("-trace org.flashdevelop.utils.FlashConnect.trace");
                    Add("org/flashdevelop/utils/FlashConnect.as");
                    break;
                case TraceMode.FlashConnectExtended:
                    Add("-trace org.flashdevelop.utils.FlashConnect.mtrace");
                    Add("org/flashdevelop/utils/FlashConnect.as");
                    break;
                case TraceMode.CustomFunction:
                    Add("-trace", project.CompilerOptions.TraceFunction);
                    break;
            }
        }
    }
}
