using System;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public class ToolStripMenuItemEx : ToolStripMenuItem
    {
        private ShortcutKeys m_shortcutKeys;
        private ToolStrip lastOwner;

        #region Constructors

        public ToolStripMenuItemEx() : base() { }
        public ToolStripMenuItemEx(Image image) : base(image) { }
        public ToolStripMenuItemEx(string text) : base(text) { }
        public ToolStripMenuItemEx(string text, Image image) : base(text, image) { }
        public ToolStripMenuItemEx(string text, Image image, EventHandler onClick) : base(text, image, onClick) { }
        public ToolStripMenuItemEx(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems) { }
        public ToolStripMenuItemEx(string text, Image image, EventHandler onClick, ShortcutKeys shortcutKeys) : base(text, image, onClick) { ShortcutKeys = shortcutKeys; }
        public ToolStripMenuItemEx(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name) { }

        #endregion

        #region Properties

        public new ShortcutKeys ShortcutKeys
        {
            get
            {
                return m_shortcutKeys;
            }
            set
            {
                if (!value.IsNone && !ShortcutKeysManager.IsValidShortcut(value))
                {
                    throw new ArgumentException("Passed value is not a valid shortcut.", "value");
                }
                if (!m_shortcutKeys.Equals(value))
                {
                    if (Owner != null)
                    {
                        if (!m_shortcutKeys.IsNone)
                        {
                            Owner.Shortcuts().Remove(m_shortcutKeys.Value);
                        }
                        if (!value.IsNone)
                        {
                            var Owner_Shortcuts = Owner.Shortcuts();
                            if (Owner_Shortcuts.Contains(value.Value))
                            {
                                Owner_Shortcuts[value.Value] = this;
                            }
                            else
                            {
                                Owner_Shortcuts.Add(value.Value, this);
                            }
                        }
                    }
                    m_shortcutKeys = value;
                    ShortcutKeyDisplayString = m_shortcutKeys.ToString();
                }
            }
        }

        #endregion

        #region Methods


        internal bool ProcessCmdKeyInternal(ref Message m, ShortcutKeys keyData)
        {
            if (Enabled && m_shortcutKeys.Equals(keyData) && !HasDropDownItems)
            {
                PerformClick();
                return true;
            }
            return false;
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (m_shortcutKeys.IsSimple && ProcessCmdKeyInternal(ref m, keyData))
            {
                return true;
            }
            return base.ProcessCmdKey(ref m, keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (lastOwner != null && !m_shortcutKeys.IsNone)
                {
                    var shortcutKeys = m_shortcutKeys.Value;
                    var lastOwner_Shortcuts = lastOwner.Shortcuts();
                    if (lastOwner_Shortcuts.Contains(shortcutKeys))
                    {
                        lastOwner_Shortcuts.Remove(shortcutKeys);
                    }
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            if (!m_shortcutKeys.IsNone)
            {
                var shortcutKeys = m_shortcutKeys.Value;
                if (lastOwner != null)
                {
                    lastOwner.Shortcuts().Remove(shortcutKeys);
                }
                if (Owner != null)
                {
                    var Owner_Shortcuts = Owner.Shortcuts();
                    if (Owner_Shortcuts.Contains(shortcutKeys))
                    {
                        Owner_Shortcuts[shortcutKeys] = this;
                    }
                    else
                    {
                        Owner_Shortcuts.Add(shortcutKeys, this);
                    }
                }
            }
            lastOwner = Owner;
            base.OnOwnerChanged(e);
        }

        #endregion
    }
}
