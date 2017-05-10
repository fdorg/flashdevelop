﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Collections;

namespace FlashDevelop.Managers
{
    /// <summary>
    /// A manager class for clipboard history.
    /// </summary>
    internal static class ClipboardManager
    {
        private static IntPtr hwnd;
        private static FixedSizeQueue<ClipboardTextData> history;

        /// <summary>
        /// Gets a collection of <see cref="IDataObject"/>, each representing clipboard data in history.
        /// </summary>
        internal static FixedSizeQueue<ClipboardTextData> History
        {
            get { return history; }
        }

        /// <summary>
        /// Initializes <see cref="ClipboardManager"/> and places the <see cref="MainForm"/> window in the system-maintained clipboard format listener list.
        /// </summary>
        /// <param name="window">A <see cref="MainForm"/> object.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="NotSupportedException"/>
        internal static void Initialize(MainForm window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }
            else if (history != null)
            {
                throw new InvalidOperationException(nameof(ClipboardManager) + " is already initialized.");
            }

            if (Win32.ShouldUseWin32())
            {
                try
                {
                    hwnd = window.Handle;
                    if (!UnsafeNativeMethods.AddClipboardFormatListener(hwnd))
                    {
                        hwnd = IntPtr.Zero;
                        var ex = new Win32Exception(Marshal.GetLastWin32Error());
                        throw new NotSupportedException(ex.Message, ex);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    hwnd = IntPtr.Zero;
                }
            }

            history = new FixedSizeQueue<ClipboardTextData>(Globals.Settings.ClipboardHistorySize);

            try
            {
                var dataObject = Clipboard.GetDataObject();
                if (ClipboardTextData.IsTextFormat(dataObject))
                {
                    history.Enqueue(new ClipboardTextData(dataObject));
                }
            }
            catch (ExternalException) { }
            catch (ThreadStateException) { }
        }

        /// <summary>
        /// Disposes <see cref="ClipboardManager"/> and removes the <see cref="MainForm"/> window from the system-maintained clipboard format listener list.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="NotSupportedException"/>
        internal static void Dispose()
        {
            if (history == null)
            {
                throw new InvalidOperationException(nameof(ClipboardManager) + " is either not initialized or already disposed.");
            }
            else if (hwnd != IntPtr.Zero)
            {
                if (!UnsafeNativeMethods.RemoveClipboardFormatListener(hwnd))
                {
                    var ex = new Win32Exception(Marshal.GetLastWin32Error());
                    throw new NotSupportedException(ex.Message, ex);
                }
            }

            hwnd = IntPtr.Zero;
            history = null;
        }

        /// <summary>
        /// Handles <see cref="Form.WndProc"/> for a clipboard update message.
        /// Returns <see langword="true"/> if new clipboard data is added; <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="m">A <see cref="Message"/> object.</param>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ThreadStateException"/>
        internal static bool HandleWndProc(ref Message m)
        {
            if (m.Msg == UnsafeNativeMethods.WM_CLIPBOARDUPDATE)
            {
                try
                {
                    var dataObject = Clipboard.GetDataObject();
                    if (ClipboardTextData.IsTextFormat(dataObject))
                    {
                        history.Enqueue(new ClipboardTextData(dataObject));
                        return true;
                    }
                }
                catch (NullReferenceException ex)
                {
                    throw new InvalidOperationException(nameof(ClipboardManager) + " is either not initialized or disposed.", ex);
                }
                catch (ExternalException) { }
                //catch (ThreadStateException) { }
            }

            return false;
        }

        /// <summary>
        /// Apply the modified settings.
        /// </summary>
        internal static void ApplySettings()
        {
            if (history != null)
            {
                history.Capacity = Globals.Settings.ClipboardHistorySize;
            }
        }

        private static class UnsafeNativeMethods
        {
            /// <summary>
            /// Sent when the contents of the clipboard have changed.
            /// </summary>
            internal const int WM_CLIPBOARDUPDATE = 0x031D;

            /// <summary>
            /// Places the given window in the system-maintained clipboard format listener list.
            /// </summary>
            /// <param name="hwnd">A handle to the window to be placed in the clipboard format listener list.</param>
            /// <returns>Returns <see langword="true"/> if successful, <see langword="false"/> otherwise. Call GetLastError for additional details.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool AddClipboardFormatListener([In] IntPtr hwnd);

            /// <summary>
            /// Removes the given window from the system-maintained clipboard format listener list.
            /// </summary>
            /// <param name="hwnd">A handle to the window to remove from the clipboard format listener list.</param>
            /// <returns>Returns <see langword="true"/> if successful, <see langword="false"/> otherwise. Call GetLastError for additional details.</returns>
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool RemoveClipboardFormatListener([In] IntPtr hwnd);
        }
    }

    /// <summary>
    /// Represents text data from the clipboard.
    /// </summary>
    public sealed class ClipboardTextData
    {
        private string format;
        private string rtf;
        private string text;

        /// <summary>
        /// Creates a new instance of <see cref="ClipboardTextData"/> with the specified <see cref="IDataObject"/>.
        /// </summary>
        /// <param name="dataObject">An <see cref="IDataObject"/> containing clipboard text data.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public ClipboardTextData(IDataObject dataObject)
        {
            if (dataObject == null)
            {
                throw new ArgumentNullException(nameof(dataObject));
            }

            Initialize(dataObject);
        }

        /// <summary>
        /// Gets the format of the <see cref="ClipboardTextData"/>.
        /// </summary>
        public string Format
        {
            get { return format; }
        }

        /// <summary>
        /// Gets the <see cref="DataFormats.Rtf"/> value of the <see cref="ClipboardTextData"/>, or <see langword="null"/> if <see cref="Format"/> is not <see cref="DataFormats.Rtf"/>.
        /// </summary>
        public string Rtf
        {
            get { return rtf; }
        }

        /// <summary>
        /// Gets the <see cref="DataFormats.Text"/> value of the <see cref="ClipboardTextData"/>.
        /// </summary>
        public string Text
        {
            get { return text; }
        }

        /// <summary>
        /// Determines whether data stored in the <see cref="IDataObject"/> is associated with a text format.
        /// </summary>
        public static bool IsTextFormat(IDataObject dataObject)
        {
            if (dataObject == null)
            {
                return false;
            }

            return dataObject.GetDataPresent(DataFormats.Text)/*
                || dataObject.GetDataPresent(DataFormats.UnicodeText)
                || dataObject.GetDataPresent(DataFormats.OemText)
                || dataObject.GetDataPresent(DataFormats.Locale)
                || dataObject.GetDataPresent(DataFormats.Html)
                || dataObject.GetDataPresent(DataFormats.Rtf)
                || dataObject.GetDataPresent(DataFormats.CommaSeparatedValue)
                || dataObject.GetDataPresent(DataFormats.StringFormat)*/;
        }

        private void Initialize(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(DataFormats.Rtf))
            {
                format = DataFormats.Rtf;
                rtf = (string) dataObject.GetData(DataFormats.Rtf);
                text = (string) dataObject.GetData(DataFormats.Text);
            }
            else if (dataObject.GetDataPresent(DataFormats.Text))
            {
                format = DataFormats.Text;
                rtf = null;
                text = (string) dataObject.GetData(DataFormats.Text);
            }
            else
            {
                throw new ArgumentException("Specified " + nameof(IDataObject) + " does not contain any text data.");
            }
        }
    }
}
