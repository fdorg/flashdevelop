// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Runtime.CompilerServices;
using PluginCore.Localization;

namespace FlashDebugger
{
    public delegate void PathChangedEventHandler(String path);
	
	[Serializable]
	[DefaultProperty("Path")]
	public class Folder
	{
		private String m_Value;

		public Folder()
		{
			m_Value = "";
		}

		public Folder(String value)
		{
			m_Value = value;
		}

		public override String ToString()
		{
			return m_Value;
		}

		[Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
		public String Path
		{
			get { return m_Value; }
			set { m_Value = value; }
		}
	}

    [Serializable]
    public class Settings
    {
		private Folder[] m_SourcePaths = new Folder[] {};
        private Boolean m_SaveBreakPoints = true;
        private Boolean m_DisablePanelsAutoshow = false;
        private Boolean m_VerboseOutput = false;
        private Boolean m_StartDebuggerOnTestMovie = true;

        [DisplayName("Save Breakpoints")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.SaveBreakPoints")]
		[DefaultValue(true)]
		public bool SaveBreakPoints
		{
			get { return m_SaveBreakPoints; }
			set { m_SaveBreakPoints = value; }
        }

        [DisplayName("Disable Panels Autoshow")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.DisablePanelsAutoshow")]
        [DefaultValue(false)]
        public bool DisablePanelsAutoshow
        {
            get { return m_DisablePanelsAutoshow; }
            set { m_DisablePanelsAutoshow = value; }
        }

        [DisplayName("Verbose Output")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.VerboseOutput")]
        [DefaultValue(false)]
        public bool VerboseOutput
        {
            get { return m_VerboseOutput; }
            set { m_VerboseOutput = value; }
        }

        [DisplayName("Source Paths")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.SourcePaths")]
		[Editor(typeof(ArrayEditor), typeof(UITypeEditor))]
		public Folder[] SourcePaths
		{
			get
			{
				if (m_SourcePaths == null || m_SourcePaths.Length == 0)
				{
                    m_SourcePaths = new Folder[] {};
				}
				return m_SourcePaths;
			}
			set { m_SourcePaths = value; }
		}

        [DisplayName("Start Debugger On Test Movie")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.StartDebuggerOnTestMovie")]
        [DefaultValue(true)]
        public bool StartDebuggerOnTestMovie
        {
            get { return m_StartDebuggerOnTestMovie; }
            set { m_StartDebuggerOnTestMovie = value; }
        }

    }

}
