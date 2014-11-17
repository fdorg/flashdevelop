using System;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Runtime.CompilerServices;
using PluginCore.Localization;
using LayoutManager.Controls;

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
        private Boolean m_BreakOnThrow = false;
		private String m_SwitchToLayout = null;

        [field: NonSerialized]
        public event EventHandler BreakOnThrowChanged;

        [DisplayName("Save Breakpoints")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.SaveBreakPoints")]
		[DefaultValue(true)]
		public bool SaveBreakPoints
		{
			get { return m_SaveBreakPoints; }
			set { m_SaveBreakPoints = value; }
        }

        [DisplayName("Disable Panel Auto Show")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.DisablePanelsAutoshow")]
        [DefaultValue(false)]
        public bool DisablePanelsAutoshow
        {
            get { return m_DisablePanelsAutoshow; }
            set { m_DisablePanelsAutoshow = value; }
        }

		[DisplayName("Switch To Layout On Debugger Start")]
		[LocalizedCategory("FlashDebugger.Category.Misc")]
		[LocalizedDescription("FlashDebugger.Description.SwitchToLayout")]
		[DefaultValue(null)]
		[Editor(typeof(LayoutSelectorEditor), typeof(UITypeEditor))]
		public String SwitchToLayout
		{
			get { return m_SwitchToLayout; }
			set { m_SwitchToLayout = value; }
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

        [DisplayName("Break When Error Is Thrown")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.BreakOnThrow")]
        [DefaultValue(false)]
        public bool BreakOnThrow
        {
            get { return m_BreakOnThrow; }
            set 
            {
                if (m_BreakOnThrow == value)
                    return;

                m_BreakOnThrow = value;

                if (BreakOnThrowChanged != null)
                    BreakOnThrowChanged(this, EventArgs.Empty);
            }
        }
    }

}
