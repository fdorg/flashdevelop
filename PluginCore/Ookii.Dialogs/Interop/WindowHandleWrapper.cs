// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.

using System;
using System.Windows.Forms;

namespace Ookii.Dialogs.Interop
{
    internal class WindowHandleWrapper : IWin32Window
    {
        readonly IntPtr _handle;

        public WindowHandleWrapper(IntPtr handle)
        {
            _handle = handle;
        }

        #region IWin32Window Members

        public IntPtr Handle => _handle;

        #endregion
    }
}
