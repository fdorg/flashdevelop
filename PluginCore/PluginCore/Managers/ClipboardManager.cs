using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PluginCore.Collections;

namespace PluginCore.Managers
{
    /// <summary>
    /// A manager class for clipboard history.
    /// </summary>
    public static class ClipboardManager
    {
        private static IntPtr hwnd;
        private static FixedSizeQueue<ClipboardTextData> history;

        /// <summary>
        /// Gets a collection of <see cref="IDataObject"/>, each representing clipboard data in history.
        /// </summary>
        public static FixedSizeQueue<ClipboardTextData> History
        {
            get { return history; }
        }

        /// <summary>
        /// Initializes <see cref="ClipboardManager"/> with a <see cref="IMainForm"/> window.
        /// </summary>
        /// <param name="window">A <see cref="T:FlashDevelop.MainForm"/> object.</param>
        public static void Initialize(IMainForm window)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (window.GetType().ToString() == "FlashDevelop.MainForm")
            {
                hwnd = window.Handle;
                if (!UnsafeNativeMethods.AddClipboardFormatListener(hwnd))
                {
                    hwnd = IntPtr.Zero;
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                history = new FixedSizeQueue<ClipboardTextData>(50); // TODO: Add a setting for the history size
                history.Enqueue(new ClipboardTextData(Clipboard.GetDataObject()));
            }
        }

        /// <summary>
        /// Removes the clipboard listener.
        /// </summary>
        public static void Dispose()
        {
            if (!UnsafeNativeMethods.RemoveClipboardFormatListener(hwnd))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            hwnd = IntPtr.Zero;
            history = null;
        }

        /// <summary>
        /// Handles <see cref="Form.WndProc"/> for a clipboard update message.
        /// Returns <see langword="true"/> if new clipboard data is added; <see langword="false"/> otherwise.
        /// </summary>
        /// <param name="m">A <see cref="Message"/> object.</param>
        public static bool HandleWndProc(ref Message m)
        {
            if (m.Msg == UnsafeNativeMethods.WM_CLIPBOARDUPDATE)
            {
                try
                {
                    var dataObject = Clipboard.GetDataObject();
                    history.Enqueue(new ClipboardTextData(dataObject));
                    return true;
                }
                catch (ExternalException) { }
                //catch (System.Threading.ThreadStateException) { }
            }

            return false;
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
    /// Represents text data from clipboard data.
    /// </summary>
    public sealed class ClipboardTextData
    {
        private string format;
        private string rtf;
        private string text;

        internal ClipboardTextData(IDataObject dataObject)
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
                string[] formats = dataObject.GetFormats();
                if (formats.Length > 0)
                {
                    format = formats[0];
                    text = string.Join(";", formats);
                }
                else
                {
                    format = "";
                    text = null;
                }
            }
        }

        /// <summary>
        /// Gets the format of the text data. Text formats are <see cref="DataFormats.Text"/> and <see cref="DataFormats.Rtf"/>.
        /// </summary>
        public string Format
        {
            get { return format; }
        }

        /// <summary>
        /// Gets the <see cref="DataFormats.Rtf"/> value of the <see cref="ClipboardTextData"/>.
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
    }
}
