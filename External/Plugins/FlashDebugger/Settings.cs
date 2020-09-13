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
    [Serializable]
    [DefaultProperty("Path")]
    public class Folder
    {
        string path;

        public Folder() : this(string.Empty)
        {
        }

        public Folder(string value) => path = value;

        public override string ToString() => path;

        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string Path
        {
            get => path;
            set => path = value;
        }
    }

    [Serializable]
    public class Settings
    {
        Folder[] m_SourcePaths = Array.Empty<Folder>();
        bool m_SaveBreakPoints = true;
        bool m_DisablePanelsAutoshow;
        bool m_VerboseOutput;
        bool m_StartDebuggerOnTestMovie = true;
        bool m_BreakOnThrow;
        string  m_SwitchToLayout;
        bool m_CombineInherited;
        bool m_HideStaticMembers;
        bool m_HideFullClassPaths;
        bool m_HideClassIds;
        int m_CopyTreeMaxRecursion = 10;
        int m_CopyTreeMaxChars = 1000000;

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
            get => m_SaveBreakPoints;
            set => m_SaveBreakPoints = value;
        }

        [DisplayName("Disable Panel Auto Show")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.DisablePanelsAutoshow")]
        [DefaultValue(false)]
        public bool DisablePanelsAutoshow
        {
            get => m_DisablePanelsAutoshow;
            set => m_DisablePanelsAutoshow = value;
        }

        [DisplayName("Switch To Layout On Debugger Start")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.SwitchToLayout")]
        [DefaultValue(null)]
        [Editor(typeof(LayoutSelectorEditor), typeof(UITypeEditor))]
        public string SwitchToLayout
        {
            get => m_SwitchToLayout;
            set => m_SwitchToLayout = value;
        }

        [DisplayName("Verbose Output")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.VerboseOutput")]
        [DefaultValue(false)]
        public bool VerboseOutput
        {
            get => m_VerboseOutput;
            set => m_VerboseOutput = value;
        }

        [DisplayName("Source Paths")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.SourcePaths")]
        [Editor(typeof(ArrayEditor), typeof(UITypeEditor))]
        public Folder[] SourcePaths
        {
            get => m_SourcePaths ??= Array.Empty<Folder>();
            set => m_SourcePaths = value;
        }

        [DisplayName("Start Debugger On Test Movie")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.StartDebuggerOnTestMovie")]
        [DefaultValue(true)]
        public bool StartDebuggerOnTestMovie
        {
            get => m_StartDebuggerOnTestMovie;
            set => m_StartDebuggerOnTestMovie = value;
        }

        [DisplayName("Break When Error Is Thrown")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.BreakOnThrow")]
        [DefaultValue(false)]
        public bool BreakOnThrow
        {
            get => m_BreakOnThrow;
            set
            {
                if (value == m_BreakOnThrow) return;
                m_BreakOnThrow = value;
                BreakOnThrowChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public static readonly string DefaultJavaHome = "JAVA_HOME";
        string javaHome = DefaultJavaHome;

        [DisplayName("Java Home Directory")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [Description("The JDK software is installed on your computer, for example, at C:\\Program Files (x86)\\Java\\jdk1.8.0_202 for FlashDevelop.exe or C:\\Program Files\\Java\\jdk1.8.0_202 for FlashDevelop64.exe")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string JavaHome
        {
            get => string.IsNullOrEmpty(javaHome) ? DefaultJavaHome : javaHome;
            set => javaHome = value;
        }

        [DisplayName("Combine Inherited Members")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.CombineInherited")]
        [DefaultValue(false)]
        public bool CombineInherited
        {
            get => m_CombineInherited;
            set
            {
                if (value == m_CombineInherited) return;
                m_CombineInherited = value;
                DataTreeDisplayChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DisplayName("Hide Static Members")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.HideStaticMembers")]
        [DefaultValue(false)]
        public bool HideStaticMembers
        {
            get => m_HideStaticMembers;
            set
            {
                if (value == m_HideStaticMembers) return;
                m_HideStaticMembers = value;
                DataTreeDisplayChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DisplayName("Hide Full Classpath")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.HideFullClasspaths")]
        [DefaultValue(false)]
        public bool HideFullClasspaths
        {
            get => m_HideFullClassPaths;
            set
            {
                if (value == m_HideFullClassPaths) return;
                m_HideFullClassPaths = value;
                DataTreeDisplayChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DisplayName("Hide Class IDs")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.HideClassIds")]
        [DefaultValue(false)]
        public bool HideClassIds
        {
            get => m_HideClassIds;
            set
            {
                if (value == m_HideClassIds) return;
                m_HideClassIds = value;
                DataTreeDisplayChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [DisplayName("Copy Tree Max Level")]
        [LocalizedCategory("FlashDebugger.Category.DataTree")]
        [LocalizedDescription("FlashDebugger.Description.CopyTreeMaxRecursion")]
        [DefaultValue(10)]
        public int CopyTreeMaxRecursion
        {
            get => m_CopyTreeMaxRecursion;
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
            get => m_CopyTreeMaxChars;
            set
            {
                if (value < 10) value = 10;
                m_CopyTreeMaxChars = value;
            }
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            if (m_CopyTreeMaxChars == 0)
                m_CopyTreeMaxChars = 1000000;

            if (m_CopyTreeMaxRecursion == 0)
                m_CopyTreeMaxRecursion = 10;
        }
    }
}