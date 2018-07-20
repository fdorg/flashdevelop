using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PluginCore.Utilities
{
    public class PathWalker
    {
        public delegate void PathWalkerCompleteHandler(PathWalker sender, List<string> foundFiles);

        public event PathWalkerCompleteHandler OnComplete;

        private String basePath;
        private String fileMask;
        private Boolean recursive;
        private List<String> knownPathes;
        private List<String> foundFiles;
        private Regex reUnsafeMask = new Regex("^\\*(\\.[a-z0-9]{3})$"); 

        public PathWalker(String basePath, String fileMask, Boolean recursive)
        {
            this.basePath = basePath;
            this.fileMask = fileMask;
            this.recursive = recursive;
        }

        /// <summary>
        /// Gets a list of the files
        /// </summary>
        public List<String> GetFiles()
        {
            this.foundFiles = new List<String>();
            this.knownPathes = new List<String>();
            this.ExploreFolder(basePath);
            return this.foundFiles;
        }

        /// <summary>
        /// Gets a list of the files
        /// </summary>
        public void GetFilesAsync()
        {
            this.foundFiles = new List<String>();
            this.knownPathes = new List<String>();

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Form owner = PluginBase.MainForm as Form;
            if (owner.InvokeRequired)
            {
                owner.BeginInvoke((MethodInvoker)delegate { bg_RunWorkerCompleted(sender, e); });
                return;
            }
            if (OnComplete != null) OnComplete(this, foundFiles);
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.ExploreFolder(basePath);
            }
            catch { }
        }
        
        /// <summary>
        /// Explores the content of the folder
        /// </summary> 
        private void ExploreFolder(String path)
        {
            //Avoids performing the split if there are no semi-colons or if fileMask is null which would throw a null-object-reference exception
            //Not sure if the fileMask NULL case is handled outside, I'll leave that for you to decide whether or not to keep
            //I suppose if it is handled, you could just perform the split operation always.  Probably no real measurable performance impact either way.
            if (fileMask != null && fileMask.Contains(";"))
            {
                this.ExploreFolderWithMasks(path, fileMask.Split(new Char[1]{';'}, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                this.ExploreFolderWithMasks(path, new String[1] { fileMask });
            }
        }

        /// <summary>
        /// Explores the content of the folder using the given set of file masks.
        /// </summary>
        /// <param name="path">The root folder to explore.</param>
        /// <param name="masks">A collection of file masks to match against.</param>
        private void ExploreFolderWithMasks(String path, String[] masks)
        {
            this.knownPathes.Add(path);

            //checks the directory for each mask provided
            foreach (String mask in masks)
            {
                String[] files = Directory.GetFiles(path, mask);
                String control = mask.Length == 5 ? getMaskControl(mask) : null;

                foreach (String file in files)
                {
                    //prevent too generous extension matching: *.hxp matching .hxproj
                    if (control != null && Path.GetExtension(file).ToLower() != control) 
                        continue;

                    //prevents the addition of the same file multiple times if it happens to match multiple masks
                    if (!this.foundFiles.Contains(file))
                    {
                        this.foundFiles.Add(file);
                    }
                }
            }
            if (!recursive) return;
            String[] dirs = Directory.GetDirectories(path);
            foreach (String dir in dirs)
            {
                try
                {
                    if (!this.knownPathes.Contains(dir))
                    {
                        FileInfo info = new FileInfo(dir);
                        if ((info.Attributes & FileAttributes.Hidden) == 0)
                            this.ExploreFolderWithMasks(dir, masks);
                    }
                }
                catch { /* Might be system folder.. */ };
            }
        }

        private string getMaskControl(string mask)
        {
            Match m = reUnsafeMask.Match(mask);
            if (m.Success) return m.Groups[1].Value;
            else return null;
        }

    }

}
