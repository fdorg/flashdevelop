// Copyright � Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.

using System;
using System.Windows.Forms;

namespace Ookii.Dialogs.Interop
{
    class WindowHandleWrapper : IWin32Window
    {
        private readonly IntPtr _handle;

        public WindowHandleWrapper(IntPtr handle)
        {
            _handle = handle;
        }

        #region IWin32Window Members

        public IntPtr Handle => _handle;

        #endregion
    }
}
