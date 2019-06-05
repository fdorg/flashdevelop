using System.Collections.Generic;
using Mono.GetOptions;
using System.IO;

namespace FDBuild
{
    public class FDBuildOptions : Options
    {
        readonly List<string> extraClasspaths;
        string language;
        public string ProjectFile;

        public FDBuildOptions(string[] args)
        {
            NoTrace = false;
            ProjectFile = "";
            extraClasspaths = new List<string>();
            ProcessArgs(args);
        }

        [ArgumentProcessor]
        public void SetProject(string file) 
        {
            ProjectFile = file;
        }

        [Option(99, "Add extra classpath", "cp")]
        public string ExtraClasspath
        {
            set { if (!extraClasspaths.Contains(value)) extraClasspaths.Add(value); }
        }

        [Option("Set library base directory", "library")]
        public string LibraryDir;

        [Option("Set compiler executable path", "compiler")]
        public string CompilerPath;

        [Option("Set compiler version (optional)", "version")]
        public string CompilerVersion;

        [Option("Set build target", "target")]
        public string TargetBuild;
        
        [Option("Set Swfmill executable path", "swfmill")]
        public string SwfmillPath;

        [Option("Disable tracing for this build", "notrace")]
        public bool NoTrace = false;

        [Option("Disable execution of pre build commands", "noprebuild")]
        public bool NoPreBuild = false;

        [Option("Disable execution of post build commands", "nopostbuild")]
        public bool NoPostBuild = false;

        [Option("Pause the console after building", "pause")]
        public bool PauseAtEnd = false;

        [Option("Connect to FlashDevelop's remoting services using the specified IPC name (optional)", "ipc")]
        public string IpcName = null;

        public string Language
        {
            set
            {
                if (LibraryDir == null) return;
                if (language != null) extraClasspaths.Remove(Path.Combine(LibraryDir, language, "classes"));
                language = value;
                var library = Path.Combine(LibraryDir, language, "classes");
                // add the library classpath for the language
                if (Directory.Exists(library) && language != "HAXE") extraClasspaths.Add(library);
            }
        }

        public string[] ExtraClasspaths => extraClasspaths.ToArray();
    }
}