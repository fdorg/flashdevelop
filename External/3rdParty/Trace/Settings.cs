// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PluginCore.Localization;

namespace Trace
{
    [Serializable]
    public class Settings
    {
        private Boolean showPackageName = false;
        private Boolean showClassName = true;
        private Boolean insertNewLine = false;
        private Boolean compactMode = false;
        private String alternateFunction = "MyLogger.log";

        private Keys simpleShortcut = Keys.Control | Keys.D0;
        private Keys forinShortcut = Keys.Control | Keys.Shift | Keys.D0;
        private Keys alternateSimpleShortcut = Keys.Control | Keys.D9;
        private Keys alternateForinShortcut = Keys.Control | Keys.Shift | Keys.D9;

        //
        // Options
        //

        [Category("Options"), DisplayName("1. Show Package Name")]
        [Description("Show package name when trace method."), DefaultValue(false)]
        public Boolean ShowPackageName
        {
            get { return this.showPackageName; }
            set { this.showPackageName = value; }
        }

        [Category("Options"), DisplayName("2. Show Class Name")]
        [Description("Show class name when trace method."), DefaultValue(true)]
        public Boolean ShowClassName
        {
            get { return this.showClassName; }
            set { this.showClassName = value; }
        }

        [Category("Options"), DisplayName("3. Insert New Line")]
        [Description("Insert new line after trace."), DefaultValue(false)]
        public Boolean InsertNewLine
        {
            get { return this.insertNewLine; }
            set { this.insertNewLine = value; }
        }

        [Category("Options"), DisplayName("4. Compact Mode")]
        [Description("Compact mode."), DefaultValue(false)]
        public Boolean CompactMode
        {
            get { return this.compactMode; }
            set { this.compactMode = value; }
        }

        [Category("Options"), DisplayName("5. Alternate Function")]
        [Description("Alternate Function."), DefaultValue("MyLogger.log")]
        public String AlternateFunction
        {
            get { return this.alternateFunction; }
            set { this.alternateFunction = value; }
        }

        //
        // Shortcuts
        //

        [Category("Shortcuts"), DisplayName("1. Trace Simple")]
        [Description("Trace simple shortcut."), DefaultValue(Keys.Control | Keys.D0)]
        public Keys TraceSimple
        {
            get { return this.simpleShortcut; }
            set { this.simpleShortcut = value; }
        }

        [Category("Shortcuts"), DisplayName("2. Trace For..in")]
        [Description("Trace for..in shortcut."), DefaultValue(Keys.Control | Keys.Shift | Keys.D0)]
        public Keys TraceForIn
        {
            get { return this.forinShortcut; }
            set { this.forinShortcut = value; }
        }

        [Category("Shortcuts"), DisplayName("3. Trace Alternate Simple")]
        [Description("Trace alternate simple shortcut."), DefaultValue(Keys.Control | Keys.D9)]
        public Keys TraceAlternateSimple
        {
            get { return this.alternateSimpleShortcut; }
            set { this.alternateSimpleShortcut = value; }
        }

        [Category("Shortcuts"), DisplayName("4. Trace Alternate For..in")]
        [Description("Trace alternate for..in shortcut."), DefaultValue(Keys.Control | Keys.Shift | Keys.D9)]
        public Keys TraceAlternateForIn
        {
            get { return this.alternateForinShortcut; }
            set { this.alternateForinShortcut = value; }
        }
    }

}
