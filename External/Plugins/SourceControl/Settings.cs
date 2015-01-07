using System;
using System.ComponentModel;
using System.Windows.Forms.Design;
using PluginCore.Localization;
using System.Drawing.Design;

namespace SourceControl
{
    [Serializable]
    public class Settings
    {
        private String gitPath;
        private String svnPath;
        private String hgPath;
        private String tortoiseSVNProcPath;
        private String tortoiseGitProcPath;
        private String tortoiseHGProcPath;
        private Boolean enableSVN;
        private Boolean enableGIT;
        private Boolean enableHG;

        [DefaultValue(false)]
        [DisplayName("Enable SVN")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.EnableSVN")]
        public Boolean EnableSVN
        {
            get { return this.enableSVN; }
            set { this.enableSVN = value; }
        }

        [DefaultValue("svn")]
        [DisplayName("SVN Path")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.SVNPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String SVNPath
        {
            get { return this.svnPath ?? "svn.exe"; }
            set { this.svnPath = value; }
        }

        [DefaultValue("TortoiseProc.exe")]
        [DisplayName("TortoiseSVN Proc Path")]
        [LocalizedCategory("SourceControl.Category.SVN")]
        [LocalizedDescription("SourceControl.Description.TortoiseSVNProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String TortoiseSVNProcPath
        {
            get { return this.tortoiseSVNProcPath ?? "TortoiseProc.exe"; }
            set { this.tortoiseSVNProcPath = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Enable GIT")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.EnableGIT")]
        public Boolean EnableGIT
        {
            get { return this.enableGIT; }
            set { this.enableGIT = value; }
        }

        [DefaultValue("git")]
        [DisplayName("GIT Path")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.GITPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String GITPath
        {
            get { return this.gitPath ?? "git"; }
            set { this.gitPath = value; }
        }

        [DefaultValue("TortoiseProc.exe")]
        [DisplayName("TortoiseGIT Proc Path")]
        [LocalizedCategory("SourceControl.Category.GIT")]
        [LocalizedDescription("SourceControl.Description.TortoiseGITProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String TortoiseGITProcPath
        {
            get { return this.tortoiseGitProcPath ?? "TortoiseProc.exe"; }
            set { this.tortoiseGitProcPath = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Enable HG")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.EnableHG")]
        public Boolean EnableHG
        {
            get { return this.enableHG; }
            set { this.enableHG = value; }
        }

        [DefaultValue("hg")]
        [DisplayName("HG Path")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.HGPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String HGPath
        {
            get { return this.hgPath ?? "hg"; }
            set { this.hgPath = value; }
        }

        [DefaultValue("thgw")]
        [DisplayName("TortoiseHG Proc Path")]
        [LocalizedCategory("SourceControl.Category.HG")]
        [LocalizedDescription("SourceControl.Description.TortoiseHGProcPath")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String TortoiseHGProcPath
        {
            get { return this.tortoiseHGProcPath ?? "thgw"; }
            set { this.tortoiseHGProcPath = value; }
        }

    }

}
