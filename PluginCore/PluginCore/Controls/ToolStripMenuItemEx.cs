using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Managers;

namespace PluginCore.Controls
{
    /// <summary>
    /// An extension of <see cref="ToolStripMenuItem"/> that supports the extended shortcut keys.
    /// </summary>
    public class ToolStripMenuItemEx : ToolStripMenuItem
    {
        private ShortcutKeys m_shortcutKeys;
        private ToolStrip lastOwner;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class.
        /// </summary>
        public ToolStripMenuItemEx() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class that displays the specified <see cref="Image"/>.
        /// </summary>
        /// <param name="image">The <see cref="Image"/> to display on the control.</param>
        public ToolStripMenuItemEx(Image image) : base(image) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class that displays the specified text.
        /// </summary>
        /// <param name="text">The text to display on the menu item.</param>
        public ToolStripMenuItemEx(string text) : base(text) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class that displays the specified text and image.
        /// </summary>
        /// <param name="text">The text to display on the menu item.</param>
        /// <param name="image">The <see cref="Image"/> to display on the control.</param>
        public ToolStripMenuItemEx(string text, Image image) : base(text, image) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class that displays the specified text and image and that does the specified action when the <see cref="ToolStripMenuItemEx"/> is clicked.
        /// </summary>
        /// <param name="text">The text to display on the menu item.</param>
        /// <param name="image">The <see cref="Image"/> to display on the control.</param>
        /// <param name="onClick">An event handler that raises the <see cref="Control.Click"/> event when the control is clicked.</param>
        public ToolStripMenuItemEx(string text, Image image, EventHandler onClick) : base(text, image, onClick) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class that displays the specified text and image and that contains the specified <see cref="ToolStripItem"/> collection.
        /// </summary>
        /// <param name="text">The text to display on the menu item.</param>
        /// <param name="image">The <see cref="Image"/> to display on the control.</param>
        /// <param name="dropDownItems">The menu items to display when the control is clicked.</param>
        public ToolStripMenuItemEx(string text, Image image, params ToolStripItem[] dropDownItems) : base(text, image, dropDownItems) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class that displays the specified text and image, does the specified action when the <see cref="ToolStripMenuItemEx"/> is clicked, and displays the specified shortcut keys.
        /// </summary>
        /// <param name="text">The text to display on the menu item.</param>
        /// <param name="image">The <see cref="Image"/> to display on the control.</param>
        /// <param name="onClick">An event handler that raises the <see cref="Control.Click"/> event when the control is clicked.</param>
        /// <param name="shortcutKeys">A <see cref="global::PluginCore.ShortcutKeys"/> value that represents the shortcut key for the <see cref="ToolStripMenuItemEx"/>.</param>
        public ToolStripMenuItemEx(string text, Image image, EventHandler onClick, ShortcutKeys shortcutKeys) : base(text, image, onClick) { ShortcutKeys = shortcutKeys; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolStripMenuItemEx"/> class with the specified name that displays the specified text and image that does the specified action when the <see cref="ToolStripMenuItemEx"/> is clicked.
        /// </summary>
        /// <param name="text">The text to display on the menu item.</param>
        /// <param name="image">The <see cref="Image"/> to display on the control.</param>
        /// <param name="onClick">>An event handler that raises the <see cref="Control.Click"/> event when the control is clicked.</param>
        /// <param name="name">The name of the menu item.</param>
        public ToolStripMenuItemEx(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name) { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the shortcut keys associated with the <see cref="ToolStripMenuItemEx"/>.
        /// </summary>
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
                if (m_shortcutKeys != value)
                {
                    if (value.IsExtended)
                    {
                        if (m_shortcutKeys.IsSimple)
                        {
                            base.ShortcutKeys = 0;
                        }
                        if (Owner != null)
                        {
                            if (m_shortcutKeys.IsExtended)
                            {
                                Owner.Shortcuts().Remove(m_shortcutKeys);
                            }
                            if (!value.IsNone)
                            {
                                var Owner_Shortcuts = Owner.Shortcuts();
                                if (Owner_Shortcuts.Contains(value))
                                {
                                    Owner_Shortcuts[value] = this;
                                }
                                else
                                {
                                    Owner_Shortcuts.Add(value, this);
                                }
                            }
                        }
                        m_shortcutKeys = value;
                        ShortcutKeyDisplayString = m_shortcutKeys.IsNone ? null : m_shortcutKeys.ToString();
                    }
                    else
                    {
                        if (m_shortcutKeys.IsExtended)
                        {
                            if (Owner != null)
                            {
                                Owner.Shortcuts().Remove(m_shortcutKeys);
                            }
                        }
                        m_shortcutKeys = value;
                        base.ShortcutKeys = m_shortcutKeys.First;
                        ShortcutKeyDisplayString = m_shortcutKeys.IsNone ? null : m_shortcutKeys.ToString();
                    }
                }
            }
        }

        #endregion

        #region Methods

        internal bool ProcessCmdKeyInternal(ref Message m, ShortcutKeys keyData)
        {
            if (Enabled && m_shortcutKeys == keyData && !HasDropDownItems)
            {
                PerformClick();
                return true;
            }
            return false;
        }

        //protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        //{
        //    if (m_shortcutKeys.IsSimple && ProcessCmdKeyInternal(ref m, keyData))
        //    {
        //        return true;
        //    }
        //    return base.ProcessCmdKey(ref m, keyData);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (lastOwner != null)
                {
                    if (m_shortcutKeys.IsExtended)
                    {
                        var lastOwner_Shortcuts = lastOwner.Shortcuts();
                        if (lastOwner_Shortcuts.Contains(m_shortcutKeys))
                        {
                            lastOwner_Shortcuts.Remove(m_shortcutKeys);
                        }
                    }
                    lastOwner = null;
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            if (m_shortcutKeys.IsExtended)
            {
                if (lastOwner != null)
                {
                    lastOwner.Shortcuts().Remove(m_shortcutKeys);
                }
                if (Owner != null)
                {
                    var Owner_Shortcuts = Owner.Shortcuts();
                    if (Owner_Shortcuts.Contains(m_shortcutKeys))
                    {
                        Owner_Shortcuts[m_shortcutKeys] = this;
                    }
                    else
                    {
                        Owner_Shortcuts.Add(m_shortcutKeys, this);
                    }
                }
            }
            lastOwner = Owner;
            base.OnOwnerChanged(e);
        }

        #endregion
    }
}
