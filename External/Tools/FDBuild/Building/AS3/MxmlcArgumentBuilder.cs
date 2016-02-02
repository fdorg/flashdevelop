using System;
using ProjectManager.Projects.AS3;

namespace ProjectManager.Building.AS3
{
    class MxmlcArgumentBuilder : ArgumentBuilder
    {
        AS3Project project;
        bool flex45;
        bool flex410;
        bool asc2;

        public MxmlcArgumentBuilder(AS3Project project, double sdkVersion, bool asc2Mode)
        {
            this.project = project;
            flex45 = sdkVersion >= 4.5;
            flex410 = Math.Truncate(sdkVersion) >= 4 && (sdkVersion % 1) * 100 > 9;
            asc2 = asc2Mode;
        }

        public void AddConfig(string path)
        {
            Add("-load-config+=" + path);

            MxmlcOptions options = project.CompilerOptions;
            if (options.LoadConfig != "")
                Add("-load-config+=" + options.LoadConfig);
        }

        public void AddOutput(string path)
        {
            Add("-o", path);
        }

        public void AddOptions(bool releaseMode, bool incremental)
        {
            if (!releaseMode) AddEq("-debug", true);
            if (!asc2 && incremental) AddEq("-incremental", true);

            MxmlcOptions options = project.CompilerOptions;

            if ((asc2 || flex410) && options.AdvancedTelemetry)
            {
                AddEq("-advanced-telemetry", true);
                if (!string.IsNullOrEmpty(options.AdvancedTelemetryPassword))
                    AddEq("-advanced-telemetry-password", options.AdvancedTelemetryPassword);
            }
            if (asc2 && options.InlineFunctions)
                AddEq("-inline", true);
            if (options.LinkReport.Length > 0) Add("-link-report", options.LinkReport);
            if (options.LoadExterns.Length > 0) Add("-load-externs", options.LoadExterns);

            bool hasConfig = false;
            bool hasVersion = false;
            if (options.Additional != null)
            {
                string all = String.Join(" ", options.Additional);
                if (all.IndexOf("configname", StringComparison.Ordinal) > 0) hasConfig = true;
                if (all.IndexOf("swf-version", StringComparison.Ordinal) > 0) hasVersion = true;
            }

            if (!hasConfig)
            {
                if (project.MovieOptions.Platform == "AIR") {
                    AddEq("+configname", "air");
                }
                else if (project.MovieOptions.Platform == "AIR Mobile") {
                    AddEq("+configname", "airmobile");
                }
            }
            if ((asc2 || flex45) && !hasVersion)
            {
                string version = project.MovieOptions.Version;
                string platform = project.MovieOptions.Platform;
                string swfVersion = PluginCore.PlatformData.ResolveSwfVersion(project.Language, platform, version);
                if (swfVersion != null) AddEq("-swf-version", swfVersion);
            }

            if (options.Additional != null)
                Add(options.Additional, releaseMode);
        }

        void AddEq(string argument, string value)
        {
            Add(argument + "=" + value);
        }

        void AddEq(string argument, bool value)
        {
            AddEq(argument, value ? "true" : "false");
        }
    }
}
