// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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

        readonly string basePath;
        readonly string fileMask;
        readonly bool recursive;
        List<string> knownPathes;
        List<string> foundFiles;
        readonly Regex reUnsafeMask = new Regex("^\\*(\\.[a-z0-9]{3})$"); 

        public PathWalker(string basePath, string fileMask, bool recursive)
        {
            this.basePath = basePath;
            this.fileMask = fileMask;
            this.recursive = recursive;
        }

        /// <summary>
        /// Gets a list of the files
        /// </summary>
        public List<string> GetFiles()
        {
            foundFiles = new List<string>();
            knownPathes = new List<string>();
            ExploreFolder(basePath);
            return foundFiles;
        }

        /// <summary>
        /// Gets a list of the files
        /// </summary>
        public void GetFilesAsync()
        {
            foundFiles = new List<string>();
            knownPathes = new List<string>();

            var bg = new BackgroundWorker();
            bg.DoWork += bg_DoWork;
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (PluginBase.MainForm is Form owner && owner.InvokeRequired)
            {
                owner.BeginInvoke((MethodInvoker)delegate { bg_RunWorkerCompleted(sender, e); });
                return;
            }
            OnComplete?.Invoke(this, foundFiles);
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ExploreFolder(basePath);
            }
            catch { }
        }
        
        /// <summary>
        /// Explores the content of the folder
        /// </summary> 
        void ExploreFolder(string path)
        {
            //Avoids performing the split if there are no semi-colons or if fileMask is null which would throw a null-object-reference exception
            //Not sure if the fileMask NULL case is handled outside, I'll leave that for you to decide whether or not to keep
            //I suppose if it is handled, you could just perform the split operation always.  Probably no real measurable performance impact either way.
            if (fileMask != null && fileMask.Contains(';'))
            {
                ExploreFolderWithMasks(path, fileMask.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                ExploreFolderWithMasks(path, new[] {fileMask});
            }
        }

        /// <summary>
        /// Explores the content of the folder using the given set of file masks.
        /// </summary>
        /// <param name="path">The root folder to explore.</param>
        /// <param name="masks">A collection of file masks to match against.</param>
        void ExploreFolderWithMasks(string path, string[] masks)
        {
            knownPathes.Add(path);

            //checks the directory for each mask provided
            foreach (var mask in masks)
            {
                var files = Directory.GetFiles(path, mask);
                var control = mask.Length == 5 ? getMaskControl(mask) : null;

                foreach (var file in files)
                {
                    //prevent too generous extension matching: *.hxp matching .hxproj
                    if (control != null && Path.GetExtension(file).ToLower() != control) 
                        continue;

                    //prevents the addition of the same file multiple times if it happens to match multiple masks
                    if (!foundFiles.Contains(file))
                    {
                        foundFiles.Add(file);
                    }
                }
            }
            if (!recursive) return;
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                try
                {
                    if (!knownPathes.Contains(dir))
                    {
                        var info = new FileInfo(dir);
                        if ((info.Attributes & FileAttributes.Hidden) == 0)
                            ExploreFolderWithMasks(dir, masks);
                    }
                }
                catch { /* Might be system folder.. */ };
            }
        }

        string getMaskControl(string mask)
        {
            var m = reUnsafeMask.Match(mask);
            return m.Success ? m.Groups[1].Value : null;
        }

    }

}
