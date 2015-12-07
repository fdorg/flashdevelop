using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Runtime.Serialization;
using LayoutManager.Controls;
using Ookii.Dialogs;
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

        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
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
        private Boolean m_DisablePanelsAutoshow;
        private Boolean m_VerboseOutput;
        private Boolean m_StartDebuggerOnTestMovie = true;
        private Boolean m_BreakOnThrow;
        private String  m_SwitchToLayout;
        private Boolean m_CombineInherited;
        private Boolean m_HideStaticMembers;
        private Boolean m_HideFullClassPaths;
        private Boolean m_HideClassIds;
        private Int32 m_CopyTreeMaxRecursion = 10;
        private Int32 m_CopyTreeMaxChars = 1000000;

        [field: NonSerialized]
        public event EventHandler BreakOnThrowChanged;

        [field: NonSerialized]
        public event EventHandler DataTreeDisplayChanged;

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

        [DisplayName("Combine Inherited Members")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.CombineInherited")]
        [DefaultValue(false)]
        public bool CombineInherited
        {
            get { return m_CombineInherited; }
            set
            {
                if (m_CombineInherited == value)
                    return;

                m_CombineInherited = value;

                if (DataTreeDisplayChanged != null)
                    DataTreeDisplayChanged(this, EventArgs.Empty);
            }
        }

        [DisplayName("Hide Static Members")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.HideStaticMembers")]
        [DefaultValue(false)]
        public bool HideStaticMembers
        {
            get { return m_HideStaticMembers; }
            set
            {
                if (m_HideStaticMembers == value)
                    return;

                m_HideStaticMembers = value;

                if (DataTreeDisplayChanged != null)
                    DataTreeDisplayChanged(this, EventArgs.Empty);
            }
        }

        [DisplayName("Hide Full Classpath")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.HideFullClasspaths")]
        [DefaultValue(false)]
        public bool HideFullClasspaths
        {
            get { return m_HideFullClassPaths; }
            set
            {
                if (m_HideFullClassPaths == value)
                    return;

                m_HideFullClassPaths = value;

                if (DataTreeDisplayChanged != null)
                    DataTreeDisplayChanged(this, EventArgs.Empty);
            }
        }

        [DisplayName("Hide Class IDs")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.HideClassIds")]
        [DefaultValue(false)]
        public bool HideClassIds
        {
            get { return m_HideClassIds; }
            set
            {
                if (m_HideClassIds == value)
                    return;

                m_HideClassIds = value;

                if (DataTreeDisplayChanged != null)
                    DataTreeDisplayChanged(this, EventArgs.Empty);
            }
        }

        [DisplayName("Copy Tree Max Level")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.CopyTreeMaxRecursion")]
        [DefaultValue(10)]
        public int CopyTreeMaxRecursion
        {
            get { return m_CopyTreeMaxRecursion; }
            set
            {
                if (value < 1) value = 1;
                m_CopyTreeMaxRecursion = value;
            }
        }

        [DisplayName("Copy Tree Max Size")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.CopyTreeMaxSize")]
        [DefaultValue(1000000)]
        public int CopyTreeMaxChars
        {
            get { return m_CopyTreeMaxChars; }
            set
            {
                if (value < 10) value = 10;
                m_CopyTreeMaxChars = value;
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (m_CopyTreeMaxChars == 0)
                m_CopyTreeMaxChars = 1000000;

            if (m_CopyTreeMaxRecursion == 0)
                m_CopyTreeMaxRecursion = 10;
        }

    }

}
