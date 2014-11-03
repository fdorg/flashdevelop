using System;
using System.Text;
using System.Collections.Generic;
using PluginCore;
using System.IO;

namespace ProjectManager.Projects.Haxe
{
    public class HaxeProjectReader : ProjectReader
    {
        HaxeProject project;

        public HaxeProjectReader(string filename)
            : base(filename, new HaxeProject(filename))
        {
            this.project = base.Project as HaxeProject;
        }

        public new HaxeProject ReadProject()
        {
            return base.ReadProject() as HaxeProject;
        }

        protected override void PostProcess()
        {
            var options = project.MovieOptions;

            if (version > 1)
            {
                bool needSave = false;
                // old projects fix
                if (options.Platform == "NME" && project.TargetBuild == null
                    && project.TestMovieCommand != "" && project.TestMovieBehavior != TestMovieBehavior.OpenDocument)
                {
                    project.TestMovieCommand = "";
                    needSave = true;
                }
                if (options.Platform == "NME")
                {
                    options.Platform = GetBuilder(project.OutputPath);
                    options.Version = "1.0";
                    needSave = true;
                }
                if (options.Platform == null)
                {
                    options.Platform = PlatformData.FLASHPLAYER_PLATFORM;
                    needSave = true;
                }
                if (options.HasPlatformSupport)
                {
                    var platform = options.PlatformSupport;
                    options.TargetBuildTypes = platform.Targets;
                    needSave = true;
                }
                if (needSave)
                {
                    try { project.Save(); }
                    catch { }
                }
                return;
            }

            if (options.MajorVersion > 10)
            {
                // old projects fix
                string platform = null;
                switch (options.MajorVersion)
                {
                    case 11: 
                        platform = "JavaScript"; 
                        options.MajorVersion = 0; 
                        break;
                    case 12: 
                        platform = "Neko"; 
                        options.MajorVersion = 0; 
                        break;
                    case 13: 
                        platform = "PHP"; 
                        options.MajorVersion = 0; 
                        break;
                    case 14: 
                        platform = "C++"; 
                        options.MajorVersion = 0; 
                        break;
                }
                if (platform == null)
                {
                    platform = PlatformData.FLASHPLAYER_PLATFORM;
                    options.MajorVersion = 14;
                }
                options.Platform = platform;
            }
            try { project.Save(); } 
            catch { }
        }

        static string GetBuilder(string projectFile)
        {
            if (string.IsNullOrEmpty(projectFile))
                return "Lime";
            switch (Path.GetExtension(projectFile).ToLower())
            {
                case ".nmml":
                    return "Nme";
                default:
                    return "Lime";
            }
        }

        // process Haxe-specific stuff
        protected override void ProcessNode(string name)
        {
            switch (name)
            {
                case "build": ReadBuildOptions(); break;
                case "library": ReadLibraryAssets(); break;
                case "haxelib": ReadLibraries(); break;
                default: base.ProcessNode(name); break;
            }
        }

        public void ReadLibraries()
        {
            List<string> libraries = new List<string>();

            ReadStartElement("haxelib");
            while (Name == "library")
            {
                libraries.Add(GetAttribute("name"));
                Read();
            }
            ReadEndElement();

            project.CompilerOptions.Libraries = new string[libraries.Count];
            libraries.CopyTo(project.CompilerOptions.Libraries);
        }

        public void ReadBuildOptions()
        {
            HaxeOptions options = project.CompilerOptions;

            ReadStartElement("build");
            while (Name == "option")
            {
                MoveToFirstAttribute();
                switch (Name)
                {
                    case "directives": options.Directives = (Value=="") ? new string[]{} : Value.Split('\n'); break;
                    case "flashStrict": options.FlashStrict = BoolValue; break;
                    case "noInlineOnDebug": options.NoInlineOnDebug = BoolValue; break;
                    case "mainClass": options.MainClass = Value; break;
                    case "enabledebug": options.EnableDebug = BoolValue; break;
                    case "additional": options.Additional = Value.Split('\n'); break;
                }
                Read();
            }
            ReadEndElement();
        }

        public void ReadLibraryAssets()
        {
            ReadStartElement("library");
            while (Name == "asset")
            {
                string path = OSPath(GetAttribute("path"));
                string mode = GetAttribute("mode");

                if (path == null)
                    throw new Exception("All library assets must have a 'path' attribute.");

                LibraryAsset asset = new LibraryAsset(project, path);
                project.LibraryAssets.Add(asset);

                asset.ManualID = GetAttribute("id"); // could be null
                asset.UpdatePath = OSPath(GetAttribute("update")); // could be null
                asset.FontGlyphs = GetAttribute("glyphs"); // could be null

                if (mode != null)
                    asset.SwfMode = (SwfAssetMode)Enum.Parse(typeof(SwfAssetMode), mode, true);

                if (asset.SwfMode == SwfAssetMode.Shared)
                    asset.Sharepoint = GetAttribute("sharepoint"); // could be null

                if (asset.IsImage && GetAttribute("bitmap") != null)
                    asset.BitmapLinkage = Boolean.Parse(GetAttribute("bitmap"));

                Read();
            }
            ReadEndElement();
        }
    }
}
