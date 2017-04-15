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
        private static readonly int PropShortcutKeys;
        private ToolStrip lastOwner;
        private ShortcutKeys shortcutKeys;
        private object properties;

        #region Constructors

        static ToolStripMenuItemEx()
        {
            PropShortcutKeys = ShortcutKeysManager.ToolStripMenuItem_PropShortcutKeys();
        }

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

        internal object Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = this.Properties();
                }
                return properties;
            }
        }

        /// <summary>
        /// Gets or sets the shortcut keys associated with the <see cref="ToolStripMenuItemEx"/>.
        /// </summary>
        public new ShortcutKeys ShortcutKeys
        {
            get
            {
                return shortcutKeys;
            }
            set
            {
                if (!value.IsNone && !ShortcutKeysManager.IsValidShortcut(value))
                {
                    throw new ArgumentException("Passed value is not a valid shortcut.", nameof(value));
                }
                if (shortcutKeys != value)
                {
                    if (Owner != null)
                    {
                        var Owner_Shortcuts = Owner.Shortcuts();
                        object key = shortcutKeys.IsExtended ? (object) shortcutKeys : (object) shortcutKeys.First;
                        if (Owner_Shortcuts[key] == this)
                        {
                            Owner_Shortcuts.Remove(key);
                        }
                        key = value.IsExtended ? (object) value : (object) value.First;
                        if (!value.IsNone)
                        {
                            Owner_Shortcuts[key] = this;
                        }
                    }
                    this.Properties_SetInteger(PropShortcutKeys, (int) (Keys) value);
                    ShortcutKeyDisplayString = value.IsNone ? null : value.ToString();
                    shortcutKeys = value;
                }
            }
        }

        #endregion

        #region Methods

        internal bool ProcessCmdKeyInternal(ref Message m, ShortcutKeys keyData)
        {
            if (Enabled && shortcutKeys == keyData && !HasDropDownItems)
            {
                PerformClick();
                return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && lastOwner != null)
            {
                if (shortcutKeys.IsExtended)
                {
                    var lastOwner_Shortcuts = lastOwner.Shortcuts();
                    if (lastOwner_Shortcuts[shortcutKeys] == this)
                    {
                        lastOwner_Shortcuts.Remove(shortcutKeys);
                    }
                }
                lastOwner = null;
            }
            base.Dispose(disposing);
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            if (!shortcutKeys.IsNone)
            {
                object key = shortcutKeys.IsExtended ? (object) shortcutKeys : (object) shortcutKeys.First;
                if (lastOwner != null)
                {
                    var lastOwner_Shortcuts = lastOwner.Shortcuts();
                    if (lastOwner_Shortcuts[key] == this)
                    {
                        lastOwner_Shortcuts.Remove(key);
                    }
                }
                if (Owner != null)
                {
                    Owner.Shortcuts()[key] = this;
                }
                this.Properties_SetInteger(PropShortcutKeys, 0);
                base.OnOwnerChanged(e);
                this.Properties_SetInteger(PropShortcutKeys, (int) (Keys) shortcutKeys);
            }
            else
            {
                base.OnOwnerChanged(e);
            }
            lastOwner = Owner;
        }

        #endregion
    }
}
