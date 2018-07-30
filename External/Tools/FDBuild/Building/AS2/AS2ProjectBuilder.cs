using System;
using System.IO;
using ProjectManager.Projects;
using ProjectManager.Projects.AS2;
using FDBuild.Building;
using PluginCore.Helpers;

namespace ProjectManager.Building.AS2
{
    public class AS2ProjectBuilder : ProjectBuilder
    {
        AS2Project project;

        #region Path Helpers
        string MtascPath
        {
            get
            {
                // path given in arguments
                if (CompilerPath != null)
                {
                    if (File.Exists(CompilerPath)) return CompilerPath;
                    if (PlatformHelper.IsRunningOnWindows() && File.Exists(Path.Combine(CompilerPath, "mtasc.exe")))
                        return Path.Combine(CompilerPath, "mtasc.exe");
                    if (!PlatformHelper.IsRunningOnWindows() && File.Exists(Path.Combine(CompilerPath, "mtasc")))
                        return Path.Combine(CompilerPath, "mtasc");
                }

                // assume that mtasc.exe is probably in a directory alongside fdbuild
                string upDirectory = Path.GetDirectoryName(FDBuildDirectory);
                string mtascDir = Path.Combine(upDirectory, "mtasc");
                if (PlatformHelper.IsRunningOnWindows() && File.Exists(Path.Combine(mtascDir, "mtasc.exe")))
                    return Path.Combine(mtascDir, "mtasc.exe");
                if (!PlatformHelper.IsRunningOnWindows() && File.Exists(Path.Combine(mtascDir, "mtasc")))
                    return Path.Combine(mtascDir, "mtasc");

                // hope you have it in your environment path!  
                return "mtasc";      
            }
        }
        #endregion

        public AS2ProjectBuilder(AS2Project project, string compilerPath)
            : base(project, compilerPath)
        {
            this.project = project;
        }

        protected override void DoBuild(string[] extraClasspaths, bool noTrace)
        {
            Environment.CurrentDirectory = project.Directory;

            string outputDir = Path.GetDirectoryName(project.OutputPathAbsolute);
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            SwfmillLibraryBuilder libraryBuilder = new SwfmillLibraryBuilder();

            // before doing anything else, make sure any resources marked as "keep updated"
            // are properly kept up to date if possible
            libraryBuilder.KeepUpdated(project);

            // if we have any resources, build our library file and run swfmill on it
            libraryBuilder.BuildLibrarySwf(project, project.CompilerOptions.Verbose);

            // do we have anything to compile?
            if (project.CompileTargets.Count > 0 || 
                project.CompilerOptions.IncludePackages.Length > 0)
            {
                MtascArgumentBuilder mtasc = new MtascArgumentBuilder(project);
                mtasc.AddCompileTargets();
                mtasc.AddOutput();
                mtasc.AddClassPaths(extraClasspaths);
                mtasc.AddOptions(noTrace);

                if (project.UsesInjection)
                {
                    mtasc.AddInput();
                }
                else
                {
                    mtasc.AddFrame(libraryBuilder.Frame);

                    if (project.LibraryAssets.Count == 0)
                        mtasc.AddHeader(); // mtasc will have to generate its own output SWF
                    else
                        mtasc.AddKeep(); // keep everything you added with swfmill
                }
                
                string mtascArgs = mtasc.ToString();

                if (project.CompilerOptions.Verbose)
                    Console.WriteLine("mtasc " + mtascArgs);

                if (!ProcessRunner.Run(MtascPath, mtascArgs, false, false))
                    throw new BuildException("Build halted with errors (mtasc).");
            }
        }

        private void KeepUpdated()
        {
            foreach (LibraryAsset asset in project.LibraryAssets)
                if (asset.UpdatePath != null)
                {
                    string assetName = Path.GetFileName(asset.Path);
                    string assetPath = project.GetAbsolutePath(asset.Path);
                    string updatePath = project.GetAbsolutePath(asset.UpdatePath);
                    if (File.Exists(updatePath))
                    {
                        // check size/modified
                        FileInfo source = new FileInfo(updatePath);
                        FileInfo dest = new FileInfo(assetPath);

                        if (source.LastWriteTime != dest.LastWriteTime ||
                            source.Length != dest.Length)
                        {
                            Console.WriteLine("Updating asset '" + assetName + "'");
                            File.Copy(updatePath,assetPath,true);
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Warning: asset '"+assetName+"' could "
                            + " not be updated, as the source file could does not exist.");
                    }
                }
        }
    }
}
