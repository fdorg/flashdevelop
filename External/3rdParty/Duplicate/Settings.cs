// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using PluginCore;

namespace Duplicate
{
    [Serializable]
    public class Settings
    {
        private Keys increaseShortcut = Keys.Control | Keys.Shift | Keys.Alt | Keys.D;
        private Keys switchShortcut = Keys.Control | Keys.Alt | Keys.D;
        private Keys wordShortcut = Keys.Control | Keys.Alt | Keys.S;
        private List<String> eventsList;
        private List<String> propertiesList;
        private List<String> wordsList;

        [DisplayName("Duplicate Events")]
        [Category("Dictionaries"), Description("Events dictionary list.")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> EventsList
        {
            get { return this.eventsList; }
            set { this.eventsList = value; }
        }

        [DisplayName("Duplicate Properties")]
        [Category("Dictionaries"), Description("Properties dictionary list.")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> PropertiesList
        {
            get { return this.propertiesList; }
            set { this.propertiesList = value; }
        }

        [DisplayName("Direct Switch Words")]
        [Category("Dictionaries"), Description("Words dictionary list.")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> WordsList
        {
            get { return this.wordsList; }
            set { this.wordsList = value; }
        }

        //

        [DisplayName("Duplicate Increase")]
        [Category("Shortcuts"), Description("The Duplicate Increase shortcut."), DefaultValue(Keys.Control | Keys.Shift | Keys.Alt | Keys.D)]
        public Keys IncreaseShortcut
        {
            get { return this.increaseShortcut; }
            set { this.increaseShortcut = value; }
        }

        [DisplayName("Duplicate Switch")]
        [Category("Shortcuts"), Description("The Duplicate Switch shortcut."), DefaultValue(Keys.Control | Keys.Alt | Keys.D)]
        public Keys SwitchShortcut
        {
            get { return this.switchShortcut; }
            set { this.switchShortcut = value; }
        }

        [DisplayName("Direct Switch")]
        [Category("Shortcuts"), Description("The Direct Switch shortcut."), DefaultValue(Keys.Control | Keys.Alt | Keys.S)]
        public Keys WordShortcut
        {
            get { return this.wordShortcut; }
            set { this.wordShortcut = value; }
        }
    }
}
