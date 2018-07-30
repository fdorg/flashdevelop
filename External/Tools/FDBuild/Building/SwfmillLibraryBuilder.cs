using System;
using ProjectManager.Projects;
using ProjectManager.Building;
using System.IO;

namespace FDBuild.Building
{
    class SwfmillLibraryBuilder
    {
        static public string ExecutablePath;

        public int Frame;

        public void KeepUpdated(Project project)
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
                            File.Copy(updatePath, assetPath, true);
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("Warning: asset '" + assetName + "' could "
                            + " not be updated, as the source file could does not exist.");
                    }
                }
        }

        public void BuildLibrarySwf(Project project, bool verbose)
        {
            // compile into frame 1 unless you're using shared libraries or preloaders
            Frame = 1;

            string swfPath = project.LibrarySWFPath;

            // delete existing output file if it exists
            if (File.Exists(swfPath))
                File.Delete(swfPath);

            // if we have any resources, build our library file and run swfmill on it
            if (!project.UsesInjection && project.LibraryAssets.Count > 0)
            {
                // ensure obj directory exists
                if (!Directory.Exists("obj"))
                    Directory.CreateDirectory("obj");

                string projectName = project.Name.Replace(" ", "");
                string backupLibraryPath = Path.Combine("obj", projectName + "Library.old");
                string relLibraryPath = Path.Combine("obj", projectName + "Library.xml");
                string backupSwfPath = Path.Combine("obj", projectName + "Resources.old");
                string arguments = $"simple \"{relLibraryPath}\" \"{swfPath}\"";

                SwfmillLibraryWriter swfmill = new SwfmillLibraryWriter(relLibraryPath);
                swfmill.WriteProject(project);

                if (swfmill.NeedsMoreFrames) Frame = 3;

                // compare the Library.xml with the one we generated last time.
                // if they're identical, and we have a Resources.swf, then we can
                // just assume that Resources.swf is up to date.
                if (File.Exists(backupSwfPath) && File.Exists(backupLibraryPath) &&
                    FileComparer.IsEqual(relLibraryPath, backupLibraryPath))
                {
                    // just copy the old one over!
                    File.Copy(backupSwfPath, swfPath, true);
                }
                else
                {
                    // delete old resource SWF as it's not longer valid
                    if (File.Exists(backupSwfPath))
                        File.Delete(backupSwfPath);

                    Console.WriteLine("Compiling resources");

                    if (verbose)
                        Console.WriteLine("swfmill " + arguments);

                    if (!ProcessRunner.Run(ExecutablePath, arguments, true, false))
                        throw new BuildException("Build halted with errors (swfmill).");

                    // ok, we just generated a swf with all our resources ... save it for
                    // reuse if no resources changed next time we compile
                    File.Copy(swfPath, backupSwfPath, true);
                    File.Copy(relLibraryPath, backupLibraryPath, true);
                }
            }
        }
    }
}
