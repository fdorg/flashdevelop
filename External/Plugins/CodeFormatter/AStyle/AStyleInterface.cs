using System;
using System.Runtime.InteropServices;
using PluginCore.Managers;

namespace CodeFormatter
{
    public class AStyleInterface
    {
        #region Imports

        /// http://astyle.sourceforge.net/astyle.html
        /// Cannot use String as a return value because Mono runtime will attempt to free the returned pointer resulting in a runtime crash.

        [DllImport("AStyle.dll", EntryPoint = "AStyleGetVersion")]
        private static extern IntPtr AStyleGetVersion_32();

        [DllImport("AStyle64.dll", EntryPoint = "AStyleGetVersion")]
        private static extern IntPtr AStyleGetVersion_64();

        [DllImport("AStyle.dll", EntryPoint = "AStyleMainUtf16", CharSet = CharSet.Unicode)]
        private static extern IntPtr AStyleMainUtf16_32([MarshalAs(UnmanagedType.LPWStr)] String sIn, [MarshalAs(UnmanagedType.LPWStr)] String sOptions, AStyleErrorDelgate errorFunc, AStyleMemAllocDelgate memAllocFunc);

        [DllImport("AStyle64.dll", EntryPoint = "AStyleMainUtf16", CharSet = CharSet.Unicode)]
        private static extern IntPtr AStyleMainUtf16_64([MarshalAs(UnmanagedType.LPWStr)] String sIn, [MarshalAs(UnmanagedType.LPWStr)] String sOptions, AStyleErrorDelgate errorFunc, AStyleMemAllocDelgate memAllocFunc);

        private static IntPtr AStyleMainUtf16(String textIn, String options, AStyleErrorDelgate AStyleError, AStyleMemAllocDelgate AStyleMemAlloc)
        {
            return (IntPtr.Size == 4 ? AStyleMainUtf16_32(textIn, options, AStyleError, AStyleMemAlloc) : AStyleMainUtf16_64(textIn, options, AStyleError, AStyleMemAlloc));
        }

        private static IntPtr AStyleGetVersion()
        {
            return (IntPtr.Size == 4 ? AStyleGetVersion_32() : AStyleGetVersion_64());
        }

        #endregion

        /// AStyleMainUtf16 callbacks.
        private delegate IntPtr AStyleMemAllocDelgate(int size);
        private delegate void AStyleErrorDelgate(int errorNum, [MarshalAs(UnmanagedType.LPStr)]String error);

        /// AStyleMainUtf16 Delegates.
        private AStyleMemAllocDelgate AStyleMemAlloc;
        private AStyleErrorDelgate AStyleError;

        /// AStyleMainUtf16 Constants.
        public const String DefaultOptions = "--indent-namespaces --indent-preproc-block --indent-preproc-cond --indent-switches --indent-cases --delete-empty-lines --pad-header --keep-one-line-blocks --keep-one-line-statements --close-templates";

        /// <summary>
        /// Declare callback functions.
        /// </summary>
        public AStyleInterface()
        {
            AStyleMemAlloc = new AStyleMemAllocDelgate(OnAStyleMemAlloc);
            AStyleError = new AStyleErrorDelgate(OnAStyleError);
        }

        /// <summary>
        /// Call the AStyleMainUtf16 function in Artistic Style. An empty string is returned on error.
        /// </summary>
        public String FormatSource(String textIn, String options)
        {
            // Memory space is allocated by OnAStyleMemAlloc, a callback function
            String sTextOut = String.Empty;
            try
            {
                IntPtr pText = AStyleMainUtf16(textIn, options, AStyleError, AStyleMemAlloc);
                if (pText != IntPtr.Zero)
                {
                    sTextOut = Marshal.PtrToStringUni(pText);
                    Marshal.FreeHGlobal(pText);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return sTextOut;
        }

        /// <summary>
        /// Get the Artistic Style version number.
        /// </summary>
        public String GetVersion()
        {
            String sVersion = String.Empty;
            try
            {
                IntPtr pVersion = AStyleGetVersion();
                if (pVersion != IntPtr.Zero) sVersion = Marshal.PtrToStringAnsi(pVersion);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
            return sVersion;
        }

        /// <summary>
        /// Allocate the memory for the Artistic Style return string.
        /// </summary>
        private IntPtr OnAStyleMemAlloc(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        /// <summary>
        ///  Display errors from Artistic Style.
        /// </summary>
        private void OnAStyleError(int errorNumber, String errorMessage)
        {
            ErrorManager.ShowError(errorMessage, null);
        }

    }

}
