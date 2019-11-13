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
            get => enableSVN;
            set => enableSVN = value;
        }

        [DefaultValue("svn.exe")]
        [DisplayName("SVN Path")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.SVNPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string SVNPath
        {
            get => svnPath ?? "svn.exe";
            set => svnPath = value;
        }

        [DefaultValue("TortoiseProc.exe")]
        [DisplayName("TortoiseSVN Proc Path")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.TortoiseSVNProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TortoiseSVNProcPath
        {
            get => tortoiseSVNProcPath ?? "TortoiseProc.exe";
            set => tortoiseSVNProcPath = value;
        }

        [DefaultValue(false)]
        [DisplayName("Enable GIT")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.EnableGIT")]
        public bool EnableGIT
        {
            get => enableGIT;
            set => enableGIT = value;
        }

        [DefaultValue("git.exe")]
        [DisplayName("GIT Path")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.GITPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string GITPath
        {
            get => gitPath ?? "git.exe";
            set => gitPath = value;
        }

        [DefaultValue("TortoiseGitProc.exe")]
        [DisplayName("TortoiseGIT Proc Path")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.TortoiseGITProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TortoiseGITProcPath
        {
            get => tortoiseGitProcPath ?? "TortoiseGitProc.exe";
            set => tortoiseGitProcPath = value;
        }

        [DefaultValue(false)]
        [DisplayName("Enable HG")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.EnableHG")]
        public bool EnableHG
        {
            get => enableHG;
            set => enableHG = value;
        }

        [DefaultValue("hg.exe")]
        [DisplayName("HG Path")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.HGPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string HGPath
        {
            get => hgPath ?? "hg.exe";
            set => hgPath = value;
        }

        [DefaultValue("thgw.exe")]
        [DisplayName("TortoiseHG Proc Path")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.TortoiseHGProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string TortoiseHGProcPath
        {
            get => tortoiseHGProcPath ?? "thgw.exe";
            set => tortoiseHGProcPath = value;
        }

        [DefaultValue(false)]
        [DisplayName("Never create a commit when moving, deleting, etc. files ")]
        [LocalizedDescription("SourceControl.Description.NeverAskForCommit")]
        public bool NeverCommit { get; set; }
    }
}
