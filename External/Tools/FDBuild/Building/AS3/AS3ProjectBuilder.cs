using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using ProjectManager.Projects.AS3;
using ProjectManager.Helpers;
using System.IO;
using FDBuild.Building.AS3;
using System.Collections;
using PluginCore.Helpers;
using ProjectManager.Projects;
using System.Text.RegularExpressions;


namespace ProjectManager.Building.AS3
{
    public class AS3ProjectBuilder : ProjectBuilder
    {
        AS3Project project;
        FlexCompilerShell fcsh;
        string VMARGS = "-Xmx384m -Xmx1024m -Dsun.io.useCanonCaches=false -Duser.language=en";
        string sdkPath;
        string mxmlcPath;
        string asc2Path;
        string fcshPath;
        string ascshPath;
        bool asc2Mode;
        Dictionary<string, string> jvmConfig;

        public AS3ProjectBuilder(AS3Project project, string compilerPath, string ipcName)
            : base(project, compilerPath)
        {
            this.project = project;
            
            DetectFlexSdk(compilerPath);

            bool mxmlcExists = File.Exists(mxmlcPath);
            bool fcshExists = File.Exists(fcshPath);
            bool ascshExists = File.Exists(ascshPath);
            bool asc2Exixts = File.Exists(asc2Path);
            asc2Mode = !fcshExists && (ascshExists || asc2Exixts);
 
            bool hostedInFD = (fcshExists || ascshExists) && ipcName != null && ipcName != "";

            if (hostedInFD)
            {
                fcsh = Activator.GetObject(typeof(FlexCompilerShell),
                    "ipc://" + ipcName + "/FlexCompilerShell") as FlexCompilerShell;
            }

            if (project.OutputType == OutputType.Application || project.OutputType == OutputType.Library)
            {
                if (fcsh != null && !fcshExists && !ascshExists) throw new Exception("Could not locate lib\\fcsh.jar or lib\\ascsh.jar in Flex SDK.");
                if (fcsh == null && !mxmlcExists && !asc2Mode) 
                    throw new Exception("Could not locate lib\\mxmlc.jar or lib\\mxmlc-cli.jar in Flex SDK.");
            }
        }

        #region Grab Flex SDK Path from Project Manager

        private void DetectFlexSdk(string flexsdkPath)
        {
            flexsdkPath = ResolveFlexSdk(flexsdkPath);
            if (Path.GetFileName(flexsdkPath) == "bin")
                flexsdkPath = Path.GetDirectoryName(flexsdkPath);

            sdkPath = flexsdkPath;
            string libPath = Path.Combine(flexsdkPath, "lib");
            mxmlcPath = Path.Combine(libPath, "mxmlc.jar");
            fcshPath = Path.Combine(libPath, "fcsh.jar");
            asc2Path = Path.Combine(libPath, "mxmlc-cli.jar");
            if (File.Exists(mxmlcPath) || File.Exists(fcshPath) || !File.Exists(asc2Path))
            {
                // priority to FCSH
                ascshPath = null;
            }
            else
            {
                ascshPath = Path.Combine(libPath, "ascsh.jar");
                string toolsDir = Path.GetDirectoryName(FDBuildDirectory);
                string lib = Path.Combine(toolsDir, "flexlibs/lib/ascsh.jar");
                if (ShouldCopyASCSH(lib, ascshPath))
                {
                    // try copying the missing JAR in the SDK
                    try
                    {
                        File.Copy(lib, ascshPath);
                        Console.WriteLine("Copied 'ascsch.jar' in the AIR SDK for incremental ASC2 compilation.");
                    }
                    catch
                    {
                        ascshPath = null;
                    }
                }
            }

            jvmConfig = PluginCore.Helpers.JvmConfigHelper.ReadConfig(flexsdkPath);
            if (jvmConfig.ContainsKey("java.args") && jvmConfig["java.args"].Trim().Length > 0)
                VMARGS = jvmConfig["java.args"];
        }

        private bool ShouldCopyASCSH(string lib, string ascsh)
        {
            if (File.Exists(ascsh + ".disabled")) return false;
            if (!File.Exists(ascsh)) return true;
            // should the JAR be upgraded?
            FileInfo infoFrom = new FileInfo(lib);
            FileInfo infoTo = new FileInfo(ascsh);
            if (infoFrom.LastWriteTime > infoTo.LastWriteTime)
            {
                try
                {
                    Console.WriteLine("Found 'ascsh.jar' but it looks obsolete.");
                    File.Delete(ascsh);
                    return true;
                }
                catch { }
            }
            return false;
        }

        private string ResolveFlexSdk(string flexsdkPath)
        {
            if (flexsdkPath != null)
            {
                if (!Directory.Exists(flexsdkPath))
                    Console.WriteLine("Provided Flex SDK path doesn't exist:\n" + flexsdkPath);
                else
                    return flexsdkPath;
            }

            // search for FDBuildHints.txt in the ProjectManager's Data directory
            // when path is not provided in program arguments.
            string toolsDir = Path.GetDirectoryName(FDBuildDirectory);
            string firstRunDir = Path.GetDirectoryName(toolsDir);
            string dataDir = Path.Combine(firstRunDir, "Data");
            string pmDir = Path.Combine(dataDir, "ProjectManager");
            string fdbuildHints = Path.Combine(pmDir, "FDBuildHints.txt");

            if (File.Exists(fdbuildHints))
            {
                using (StreamReader reader = File.OpenText(fdbuildHints))
                {
                    flexsdkPath = reader.ReadLine();
                }
                if (!Directory.Exists(flexsdkPath))
                    Console.WriteLine("Compiler path configured in FDBuildHints.txt doesn't exist:\n" + flexsdkPath);
                else
                {
                    Console.WriteLine("Using compiler defined in FDBuildHints.txt");
                    return flexsdkPath;
                }
            }

            string envPath = Environment.ExpandEnvironmentVariables("%FLEX_HOME%");
            if (!envPath.StartsWith("%"))
            {
                if (!Directory.Exists(envPath))
                    Console.WriteLine("Environment path %FLEX_HOME% doesn't exist:\n" + envPath);
                else
                {
                    Console.WriteLine("Using Flex SDK defined in environment path %FLEX_HOME%");
                    return envPath;
                }
            }
            throw new Exception("Path to Flex SDK could not be resolved:\n" + flexsdkPath);
        }

        #endregion

        protected override void DoBuild(string[] extraClasspaths, bool noTrace)
        {
            string tempFile = null;
            
            Environment.CurrentDirectory = project.Directory;
            try
            {
                string objDir = "obj";
                if (!Directory.Exists(objDir)) Directory.CreateDirectory(objDir);
                tempFile = GetTempProjectFile(project);

                //create new config file
                double sdkVersion = ParseVersion(FDBuild.Program.BuildOptions.CompilerVersion ?? "4.0");

                // create compiler configuration file
                string projectName = project.Name.Replace(" ", "");
                string backupConfig = Path.Combine(objDir, projectName + "Config.old");
                string configFileTmp = Path.Combine(objDir, projectName + "Config.tmp");
                string configFile = Path.Combine(objDir, projectName + "Config.xml");

                // backup the old Config.xml to Config.old so we can reference it
                if (File.Exists(configFile))
                    File.Copy(configFile, backupConfig, true);

                //write "new" config to tmp 
                FlexConfigWriter config = new FlexConfigWriter(project.GetAbsolutePath(configFileTmp));
                config.WriteConfig(project, sdkVersion, extraClasspaths, noTrace == false, asc2Mode);

                //compare tmp to current
                bool configChanged = !File.Exists(backupConfig) || !File.Exists(configFile) || !FileComparer.IsEqual(configFileTmp, configFile);

                //copy temp file to config if there is a change
                if (configChanged){
                    File.Copy(configFileTmp, configFile, true);
                }

                //remove temp
                File.Delete(configFileTmp);

                MxmlcArgumentBuilder mxmlc = new MxmlcArgumentBuilder(project, sdkVersion, asc2Mode);

                mxmlc.AddConfig(configFile);
                mxmlc.AddOptions(noTrace, fcsh != null);
                mxmlc.AddOutput(tempFile);

                string mxmlcArgs = mxmlc.ToString();

                if (asc2Mode)
                {
                    mxmlcArgs = AddBaseConfig(mxmlcArgs);
                    Console.WriteLine("mxmlc-cli " + mxmlcArgs);
                }
                else Console.WriteLine("mxmlc " + mxmlcArgs);

                CompileWithMxmlc(project.Directory, mxmlcArgs, configChanged);

                // if we get here, the build was successful
                string output = project.FixDebugReleasePath(project.OutputPathAbsolute);
                string outputDir = Path.GetDirectoryName(output);
                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
                File.Copy(tempFile, output, true);
            }
            finally { if (tempFile != null && File.Exists(tempFile)) File.Delete(tempFile); }
        }

        private string AddBaseConfig(string args)
        {
            Match m = Regex.Match(args, "configname[\\s=]+([a-z0-9_-]+)", RegexOptions.IgnoreCase);
            string baseConfig = "flex";
            if (m.Success) baseConfig = m.Groups[1].Value;
            string configFile = sdkPath + "/frameworks/" + baseConfig + "-config.xml";
            if (File.Exists(configFile))
                args = "-load-config=\"" + configFile + "\" " + args;
            return args;
        }

        static public double ParseVersion(string version)
        {
            string[] p = version.Split('.');
            if (p.Length == 0) return 0;
            double major = 0;
            double.TryParse(p[0], out major);
            if (p.Length == 1) return major;
            double minor = 0;
            double.TryParse(p[1], out minor);
            return major + (minor < 10 ? minor / 10 : minor / 100);
        }

        public void CompileWithMxmlc(string workingdir, string arguments, bool configChanged)
        {
            if (fcsh != null)
            {
                string output;
                string[] errors;
                string[] warnings;
                string jar = ascshPath != null ? ascshPath : fcshPath;
                string jvmarg = VMARGS + " -Dapplication.home=\"" + sdkPath 
                    //+ "\" -Dflexlib=\"" + Path.Combine(sdkPath, "frameworks")
                    + "\" -jar \"" + jar + "\"";
                fcsh.Compile(workingdir, configChanged, arguments, out output, out errors, out warnings, jvmarg, JvmConfigHelper.GetJavaEXE(jvmConfig, sdkPath));

                string[] lines = output.Split('\n');
                foreach (string line in lines)
                {
                    if (!line.StartsWith("Recompile:") && !line.StartsWith("Reason:"))
                        Console.Write(line);
                }
                foreach (string warning in warnings)
                    Console.Error.WriteLine(warning);
                foreach (string error in errors)
                    Console.Error.WriteLine(error);

                if (errors.Length > 0)
                    throw new BuildException("Build halted with errors (fcsh).");
            }
            else
            {
                string jar = asc2Mode ? asc2Path : mxmlcPath;
                string javaExe = JvmConfigHelper.GetJavaEXE(jvmConfig, sdkPath);
                Console.WriteLine("Running java as: " + javaExe);
                string jvmarg = VMARGS + " -jar \"" + jar + "\" +flexlib=\"" + Path.Combine(sdkPath, "frameworks") + "\" ";
                if (!ProcessRunner.Run(javaExe, jvmarg + arguments, false, asc2Mode))
                    throw new BuildException("Build halted with errors (" + (asc2Mode ? "mxmlc-cli" : "mxmlc") + ").");
            }
        }

        // each project needs to have its own temporary build target that is consistent
        // between builds.  This is necessary because the arguments to the FlexCompilerShell
        // (if you're using it) have to be identical between builds to enable incremental compiling.
        //private string GetTempProjectFile(AS3Project project)
        private string GetTempProjectFile(AS3Project project)
        {
            // this serves two purposes - randomize the filename, so two identically-named
            // projects don't get the same temp build target, and also provide an extra
            // method of forcing a recompile after a project modification.
            string projectName = project.Name.Replace(" ", "");
            long modified = File.GetLastWriteTime(project.ProjectPath).Ticks;
            string tempFileName = projectName + modified;
            string tempPath = "obj";
            string tempFile = Path.Combine(tempPath, tempFileName);
            File.Create(tempFile).Close();
            return tempFile;
        }
    }
}
