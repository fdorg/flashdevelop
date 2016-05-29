using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PluginCore.Controls;

namespace PluginCore.Managers
{
    /// <summary>
    /// A static manager class for advanced shortcut keys.
    /// </summary>
    public static class ShortcutKeysManager
    {
        private static PropertyInfo p_Shortcuts;
        private static PropertyInfo p_ToolStrips;
        private static PropertyInfo p_IsAssignedToDropDownItem;
        private static MethodInfo m_GetToplevelOwnerToolStrip;

        private static IList toolStrips;

        #region Properties

        internal static IList ToolStrips
        {
            get
            {
                if (toolStrips == null)
                {
                    if (p_ToolStrips == null)
                    {
                        p_ToolStrips = typeof(ToolStripManager).GetProperty("ToolStrips", BindingFlags.Static | BindingFlags.NonPublic);
                    }
                    toolStrips = (IList) p_ToolStrips.GetValue(null, null);
                }
                return toolStrips;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns an updated <see cref="ShortcutKeys"/> value with the specified input <see cref="Keys"/> value.
        /// </summary>
        /// <param name="currentShortcutKeys">The <see cref="ShortcutKeys"/> value to update.</param>
        /// <param name="input">The <see cref="Keys"/> value to update with.</param>
        public static ShortcutKeys UpdateShortcutKeys(ShortcutKeys currentShortcutKeys, Keys input)
        {
            if (currentShortcutKeys.IsSimple &&
                IsValidExtendedShortcutFirst(currentShortcutKeys.First) &&
                IsValidExtendedShortcutSecond(input))
            {
                return new ShortcutKeys(currentShortcutKeys.First, input);
            }
            return input;
        }

        /// <summary>
        /// Updates the <see cref="ShortcutKeys"/> value with the specified input <see cref="Keys"/> value.
        /// </summary>
        /// <param name="shortcutKeys">The reference to the <see cref="ShortcutKeys"/> value to update.</param>
        /// <param name="input">The <see cref="Keys"/> value to update with.</param>
        public static void UpdateShortcutKeys(ref ShortcutKeys shortcutKeys, Keys input)
        {
            if (shortcutKeys.IsSimple &&
                IsValidExtendedShortcutFirst(shortcutKeys.First) &&
                IsValidExtendedShortcutSecond(input))
            {
                shortcutKeys = new ShortcutKeys(shortcutKeys.First, input);
            }
            else
            {
                shortcutKeys = input;
            }
        }

        /// <summary>
        /// Retrieves a value indicating whether the specified shortcut key is used by any of the <see cref="ToolStrip"/> controls of a form.
        /// </summary>
        /// <param name="shortcut">The shortcut key for which to search.</param>
        public static bool IsShortcutDefined(ShortcutKeys shortcut)
        {
            foreach (ToolStrip strip in ToolStrips)
            {
                if (strip != null && strip.Shortcuts().Contains(shortcut))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is valid.
        /// </summary>
        /// <param name="shortcut">The shortcut key to test for validity.</param>
        /// <returns></returns>
        public static bool IsValidShortcut(ShortcutKeys shortcut)
        {
            if (shortcut.IsExtended)
            {
                return IsValidExtendedShortcutFirst(shortcut.First) && IsValidExtendedShortcutSecond(shortcut.Second);
            }
            return IsValidSimpleShortcut(shortcut.First);
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is a valid simple shortcut.
        /// </summary>
        /// <param name="first">The shortcut key to test for validity.</param>
        public static bool IsValidSimpleShortcut(Keys first)
        {
            return ToolStripManager.IsValidShortcut(first);
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is valid for the first part of an extended shortcut.
        /// </summary>
        /// <param name="first">The shortcut key to test for validity.</param>
        public static bool IsValidExtendedShortcutFirst(Keys first)
        {
            if (first == 0)
            {
                return false;
            }
            switch (first & Keys.KeyCode)
            {
                case Keys.None:
                case Keys.ShiftKey:
                case Keys.ControlKey:
                case Keys.Menu:
                    return false;
            }
            switch (first & Keys.Modifiers)
            {
                case Keys.None:
                case Keys.Shift:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Retrieves a value indicating whether a defined shortcut key is valid for the second part of an extended shortcut.
        /// </summary>
        /// <param name="first">The shortcut key to test for validity.</param>
        public static bool IsValidExtendedShortcutSecond(Keys second)
        {
            if (second == 0)
            {
                return false;
            }
            switch (second & Keys.KeyCode)
            {
                case Keys.None:
                case Keys.ShiftKey:
                case Keys.ControlKey:
                case Keys.Menu:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Processes a command key. Do not call this method.
        /// </summary>
        /// <param name="m">A <see cref="Message"/>, passed by reference, that represents the window message to process.</param>
        /// <param name="keyData">A <see cref="ShortcutKeys"/> value that represents the key to process.</param>
        public static bool ProcessCmdKey(ref Message m, ShortcutKeys keyData)
        {
            if (IsValidShortcut(keyData))
            {
                return ProcessShortcut(ref m, keyData);
            }
            return false;
        }

        internal static bool ProcessShortcut(ref Message m, ShortcutKeys shortcut)
        {
            if (!IsThreadUsingToolStrips() || !shortcut.IsExtended)
            {
                return false;
            }
            var control = Control.FromChildHandle(m.HWnd);
            var parent = control;
            if (parent == null)
            {
                return false;
            }
            do
            {
                if (parent.ContextMenuStrip != null)
                {
                    var parent_ContextMenuStrip_Shortcuts = parent.ContextMenuStrip.Shortcuts();
                    if (parent_ContextMenuStrip_Shortcuts.Contains(shortcut))
                    {
                        var item = parent_ContextMenuStrip_Shortcuts[shortcut] as ToolStripMenuItemEx;
                        if (item != null && item.ProcessCmdKeyInternal(ref m, shortcut))
                        {
                            return true;
                        }
                    }
                }
                parent = parent.Parent;
            }
            while (parent != null);
            if (parent != null)
            {
                control = parent;
            }
            bool handled = false;
            bool prune = false;
            for (int i = 0; i < ToolStrips.Count; i++)
            {
                var strip = ToolStrips[i] as ToolStrip;
                bool flag = false;
                bool isAssignedToDropDownItem = false;
                if (strip == null)
                {
                    prune = true;
                    continue;
                }
                if (control == null || strip != control.ContextMenuStrip && strip.Shortcuts().Contains(shortcut))
                {
                    if (strip.IsDropDown)
                    {
                        var down = strip as ToolStripDropDown;
                        var firstDropDown = down.GetFirstDropDown() as ContextMenuStrip;
                        if (firstDropDown != null)
                        {
                            isAssignedToDropDownItem = firstDropDown.IsAssignedToDropDownItem();
                            if (!isAssignedToDropDownItem)
                            {
                                if (firstDropDown != control.ContextMenuStrip)
                                {
                                    continue;
                                }
                                flag = true;
                            }
                        }
                    }
                    if (!flag)
                    {
                        var toplevelOwnerToolStrip = strip.GetToplevelOwnerToolStrip();
                        if (toplevelOwnerToolStrip != null && control != null)
                        {
                            var rootHWnd = GetRootHWnd(toplevelOwnerToolStrip);
                            var controlRef = GetRootHWnd(control);
                            flag = rootHWnd.Handle == controlRef.Handle;
                            if (flag)
                            {
                                var form = Control.FromHandle(controlRef.Handle) as Form;
                                if (form != null && form.IsMdiContainer)
                                {
                                    var form2 = toplevelOwnerToolStrip.FindForm();
                                    if (form2 != form && form2 != null)
                                    {
                                        flag = form2 == form.ActiveMdiChild;
                                    }
                                }
                            }
                        }
                    }
                    if (flag || isAssignedToDropDownItem)
                    {
                        var item = strip.Shortcuts()[shortcut] as ToolStripMenuItemEx;
                        if (item != null && item.ProcessCmdKeyInternal(ref m, shortcut))
                        {
                            handled = true;
                            break;
                        }
                    }
                }
            }
            if (prune)
            {
                PruneToolStripList();
            }
            return handled;
        }

        internal static bool IsThreadUsingToolStrips()
        {
            return ToolStrips != null && ToolStrips.Count > 0;
        }

        internal static void PruneToolStripList()
        {
            if (IsThreadUsingToolStrips())
            {
                for (int i = toolStrips.Count - 1; i >= 0; i--)
                {
                    if (toolStrips[i] == null)
                    {
                        toolStrips.RemoveAt(i);
                    }
                }
            }
        }

        #endregion

        #region Reflections

        internal static Hashtable Shortcuts(this ToolStrip @this)
        {
            if (p_Shortcuts == null)
            {
                p_Shortcuts = typeof(ToolStrip).GetProperty("Shortcuts", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (Hashtable) p_Shortcuts.GetValue(@this, null);
        }

        internal static bool IsAssignedToDropDownItem(this ToolStripDropDown @this)
        {
            if (p_IsAssignedToDropDownItem == null)
            {
                p_IsAssignedToDropDownItem = typeof(ToolStripDropDown).GetProperty("IsAssignedToDropDownItem", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (bool) p_IsAssignedToDropDownItem.GetValue(@this, null);
        }

        internal static ToolStripDropDown GetFirstDropDown(this ToolStripDropDown @this)
        {
            var down = @this;
            for (var down2 = down.OwnerToolStrip() as ToolStripDropDown; down2 != null; down2 = down.OwnerToolStrip() as ToolStripDropDown)
            {
                down = down2;
            }
            return down;
        }

        internal static ToolStrip OwnerToolStrip(this ToolStripDropDown @this)
        {
            var ownerItem = @this.OwnerItem;
            if (ownerItem != null)
            {
                var parentInternal = ownerItem.GetCurrentParent();
                if (parentInternal != null)
                {
                    return parentInternal;
                }
                if (ownerItem.Placement == ToolStripItemPlacement.Overflow && ownerItem.Owner != null)
                {
                    return ownerItem.Owner.OverflowButton.DropDown;
                }
                return ownerItem.Owner;
            }
            return null;
        }

        internal static ToolStrip GetToplevelOwnerToolStrip(this ToolStrip @this)
        {
            if (m_GetToplevelOwnerToolStrip == null)
            {
                m_GetToplevelOwnerToolStrip = typeof(ToolStrip).GetMethod("GetToplevelOwnerToolStrip", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (ToolStrip) m_GetToplevelOwnerToolStrip.Invoke(@this, null);
        }

        internal static HandleRef GetRootHWnd(Control control)
        {
            return new HandleRef(control, GetAncestor(new HandleRef(new HandleRef(control, control.Handle), control.Handle), 2));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetAncestor(HandleRef hWnd, int flags);

        #endregion
    }
}
