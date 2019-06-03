using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using PluginCore.Localization;

namespace SourceControl
{
    [Serializable]
    public class Settings
    {
        private string gitPath;
        private string svnPath;
        private string hgPath;
        private string tortoiseSVNProcPath;
        private string tortoiseGitProcPath;
        private string tortoiseHGProcPath;
        private bool enableSVN;
        private bool enableGIT;
        private bool enableHG;


        [DefaultValue(false)]
        [DisplayName("Enable SVN")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.EnableSVN")]
        public bool EnableSVN
        {
            get { return this.enableSVN; }
            set { this.enableSVN = value; }
        }

        [DefaultValue("svn.exe")]
        [DisplayName("SVN Path")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.SVNPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string SVNPath
        {
            get { return this.svnPath ?? "svn.exe"; }
            set { this.svnPath = value; }
        }

        [DefaultValue("TortoiseProc.exe")]
        [DisplayName("TortoiseSVN Proc Path")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.TortoiseSVNProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TortoiseSVNProcPath
        {
            get { return this.tortoiseSVNProcPath ?? "TortoiseProc.exe"; }
            set { this.tortoiseSVNProcPath = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Enable GIT")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.EnableGIT")]
        public bool EnableGIT
        {
            get { return this.enableGIT; }
            set { this.enableGIT = value; }
        }

        [DefaultValue("git.exe")]
        [DisplayName("GIT Path")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.GITPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string GITPath
        {
            get { return this.gitPath ?? "git.exe"; }
            set { this.gitPath = value; }
        }

        [DefaultValue("TortoiseGitProc.exe")]
        [DisplayName("TortoiseGIT Proc Path")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.TortoiseGITProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TortoiseGITProcPath
        {
            get { return this.tortoiseGitProcPath ?? "TortoiseGitProc.exe"; }
            set { this.tortoiseGitProcPath = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Enable HG")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.EnableHG")]
        public bool EnableHG
        {
            get { return this.enableHG; }
            set { this.enableHG = value; }
        }

        [DefaultValue("hg.exe")]
        [DisplayName("HG Path")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.HGPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string HGPath
        {
            get { return this.hgPath ?? "hg.exe"; }
            set { this.hgPath = value; }
        }

        [DefaultValue("thgw.exe")]
        [DisplayName("TortoiseHG Proc Path")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.TortoiseHGProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TortoiseHGProcPath
        {
            get { return this.tortoiseHGProcPath ?? "thgw.exe"; }
            set { this.tortoiseHGProcPath = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Never create a commit when moving, deleting, etc. files ")]
        [LocalizedDescription("SourceControl.Description.NeverAskForCommit")]
        public bool NeverCommit { get; set; }
    }
}
