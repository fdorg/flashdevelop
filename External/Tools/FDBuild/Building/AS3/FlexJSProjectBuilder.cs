using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PluginCore.Helpers;
using ProjectManager.Building;
using ProjectManager.Building.AS3;
using ProjectManager.Projects.AS3;

namespace FDBuild.Building.AS3
{
    class FlexJSProjectBuilder : ProjectBuilder
    {
        readonly AS3Project project;
        readonly string mxmlcPath;
        readonly Dictionary<string, string> jvmConfig;
        readonly string jvmArgs;

        public FlexJSProjectBuilder(AS3Project project, string compilerPath) : base(project, compilerPath)
        {
            this.project = project;
            mxmlcPath = Path.Combine(compilerPath, "js", "lib", "mxmlc.jar");
            if (!File.Exists(mxmlcPath)) throw new Exception("Could not locate js\\lib\\mxmlc.jar in FlexJS SDK.");
            jvmConfig = JvmConfigHelper.ReadConfig(compilerPath);
            if (jvmConfig.ContainsKey("java.args"))
            {
                var args = jvmConfig["java.args"];
                if (args.Trim().Length > 0) jvmArgs = args;
            }
        }

        protected override void DoBuild(string[] extraClassPaths, bool noTrace)
        {
            string tempFile = null;
            Environment.CurrentDirectory = project.Directory;
            try
            {
                const string objDir = "obj";
                if (!Directory.Exists(objDir)) Directory.CreateDirectory(objDir);
                tempFile = AS3ProjectBuilder.GetTempProjectFile(project);

                //create new config file
                double sdkVersion = AS3ProjectBuilder.ParseVersion(Program.BuildOptions.CompilerVersion ?? "4.0");

                // create compiler configuration file
                var projectName = project.Name.Replace(" ", "");
                var backupConfig = Path.Combine(objDir, projectName + "Config.old");
                var configFileTmp = Path.Combine(objDir, projectName + "Config.tmp");
                var configFile = Path.Combine(objDir, projectName + "Config.xml");

                // backup the old Config.xml to Config.old so we can reference it
                if (File.Exists(configFile)) File.Copy(configFile, backupConfig, true);

                //write "new" config to tmp 
                var config = new FlexConfigWriter(project.GetAbsolutePath(configFileTmp));
                config.WriteConfig(project, sdkVersion, extraClassPaths, !noTrace, false);

                //compare tmp to current
                var configChanged = !File.Exists(backupConfig) || !File.Exists(configFile) || !FileComparer.IsEqual(configFileTmp, configFile);

                //copy temp file to config if there is a change
                if (configChanged)
                {
                    File.Copy(configFileTmp, configFile, true);
                }

                //remove temp
                File.Delete(configFileTmp);

                var mxmlc = new MxmlcArgumentBuilder(project, sdkVersion, false);
                mxmlc.AddConfig(configFile);
                mxmlc.AddOptions(noTrace, false);
                mxmlc.AddOutput(tempFile);
                var mxmlcArgs = mxmlc.ToString();
                Console.WriteLine("mxmlc " + mxmlcArgs);
                CompileWithMxmlc(mxmlcArgs);

                // if we get here, the build was successful
                var output = project.FixDebugReleasePath(project.OutputPathAbsolute);
                var outputDir = Path.GetDirectoryName(output);
                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
                File.Copy(tempFile, output, true);
            }
            finally { if (tempFile != null && File.Exists(tempFile)) File.Delete(tempFile); }
        }

        void CompileWithMxmlc(string mxmlcArgs)
        {
            var javaExe = JvmConfigHelper.GetJavaEXE(jvmConfig, CompilerPath);
            Console.WriteLine("Running java as: " + javaExe);
            var frameworks = Path.Combine(CompilerPath, "frameworks");
            var args = $"-Dflexcompiler=\"{CompilerPath}\" -Dflexlib=\"{frameworks}\" {jvmArgs} -jar \"{mxmlcPath}\"";
            args += " -js-output-type=FLEXJS";
            args += " -sdk-js-lib=\"" + Path.Combine(frameworks, "js", "FlexJS", "generated-sources") + "\" ";
            var index = mxmlcArgs.LastIndexOf("-o ");
            if (index != -1) mxmlcArgs = mxmlcArgs.Remove(index);
            args += mxmlcArgs;
            args += " -targets=SWF";
            args += " " + project.CompileTargets.First();
            args += " -output=" + project.OutputPath;
            if (!ProcessRunner.Run(javaExe, args, false, false)) throw new BuildException("Build halted with errors (mxmlc).");
        }
    }
}
