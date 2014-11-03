// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace Ookii.Dialogs
{
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    class SafeGDIHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeGDIHandle()
            : base(true)
        {
        }

        internal SafeGDIHandle(IntPtr existingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.DeleteObject(handle);
        }
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    class SafeDeviceHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeDeviceHandle()
            : base(true)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SafeDeviceHandle(IntPtr existingHandle, bool ownsHandle)
            : base(ownsHandle)
        {
            SetHandle(existingHandle);
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.DeleteDC(handle);
        }
    }

    class SafeModuleHandle : SafeHandle
    {
        public SafeModuleHandle()
            : base(IntPtr.Zero, true)
        {
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.FreeLibrary(handle);
        }
    }
}