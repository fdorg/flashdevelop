using System;
using System.Runtime.InteropServices;

// From: http://www.pinvoke.net/default.aspx/shell32/SHFileOperation.html

namespace PluginCore.Utilities
{
    public class InteropSHFileOperation
    {
        public enum FO_Func : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public FO_Func wFunc;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            public ushort fFlags;
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHFileOperation([In] ref SHFILEOPSTRUCT lpFileOp);

        SHFILEOPSTRUCT _ShFile;
        public FILEOP_FLAGS fFlags;

        public IntPtr hwnd
        {
            set => _ShFile.hwnd = value;
        }
        public FO_Func wFunc
        {
            set => _ShFile.wFunc = value;
        }

        public string pFrom
        {
            set => _ShFile.pFrom = value + '\0' + '\0';
        }
        public string pTo
        {
            set => _ShFile.pTo = value + '\0' + '\0';
        }
        public bool fAnyOperationsAborted
        {
            set => _ShFile.fAnyOperationsAborted = value;
        }
        public IntPtr hNameMappings
        {
            set => _ShFile.hNameMappings = value;
        }
        public string lpszProgressTitle
        {
            set => _ShFile.lpszProgressTitle = value + '\0';
        }

        public InteropSHFileOperation()
        {
            fFlags = new FILEOP_FLAGS();
            _ShFile = new SHFILEOPSTRUCT();
            _ShFile.hwnd = IntPtr.Zero;
            _ShFile.wFunc = FO_Func.FO_COPY;
            _ShFile.pFrom = "";
            _ShFile.pTo = "";
            _ShFile.fAnyOperationsAborted = false;
            _ShFile.hNameMappings = IntPtr.Zero;
            _ShFile.lpszProgressTitle = "";

        }

        public bool Execute()
        {
            _ShFile.fFlags = fFlags.Flag;
            return SHFileOperation(ref _ShFile) == 0;
        }

        public class FILEOP_FLAGS
        {
            [Flags]
            enum FILEOP_FLAGS_ENUM : ushort
            {
                FOF_MULTIDESTFILES = 0x0001,
                FOF_CONFIRMMOUSE = 0x0002,
                FOF_SILENT = 0x0004,  // don't create progress/report
                FOF_RENAMEONCOLLISION = 0x0008,
                FOF_NOCONFIRMATION = 0x0010,  // Don't prompt the user.
                FOF_WANTMAPPINGHANDLE = 0x0020,  // Fill in SHFILEOPSTRUCT.hNameMappings
                // Must be freed using SHFreeNameMappings
                FOF_ALLOWUNDO = 0x0040,
                FOF_FILESONLY = 0x0080,  // on *.*, do only files
                FOF_SIMPLEPROGRESS = 0x0100,  // means don't show names of files
                FOF_NOCONFIRMMKDIR = 0x0200,  // don't confirm making any needed dirs
                FOF_NOERRORUI = 0x0400,  // don't put up error UI
                FOF_NOCOPYSECURITYATTRIBS = 0x0800,  // dont copy NT file Security Attributes
                FOF_NORECURSION = 0x1000,  // don't recurse into directories.
                FOF_NO_CONNECTED_ELEMENTS = 0x2000,  // don't operate on connected elements.
                FOF_WANTNUKEWARNING = 0x4000,  // during delete operation, warn if nuking instead of recycling (partially overrides FOF_NOCONFIRMATION)
                FOF_NORECURSEREPARSE = 0x8000,  // treat reparse points as objects, not containers
            }

            public bool FOF_MULTIDESTFILES = false;
            public bool FOF_CONFIRMMOUSE = false;
            public bool FOF_SILENT = false;
            public bool FOF_RENAMEONCOLLISION = false;
            public bool FOF_NOCONFIRMATION = false;
            public bool FOF_WANTMAPPINGHANDLE = false;
            public bool FOF_ALLOWUNDO = false;
            public bool FOF_FILESONLY = false;
            public bool FOF_SIMPLEPROGRESS = false;
            public bool FOF_NOCONFIRMMKDIR = false;
            public bool FOF_NOERRORUI = false;
            public bool FOF_NOCOPYSECURITYATTRIBS = false;
            public bool FOF_NORECURSION = false;
            public bool FOF_NO_CONNECTED_ELEMENTS = false;
            public bool FOF_WANTNUKEWARNING = false;
            public bool FOF_NORECURSEREPARSE = false;

            public ushort Flag
            {
                get
                {
                    ushort result = 0;
                    if (FOF_MULTIDESTFILES) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_MULTIDESTFILES;
                    if (FOF_CONFIRMMOUSE) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_CONFIRMMOUSE;
                    if (FOF_SILENT) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_SILENT;
                    if (FOF_RENAMEONCOLLISION) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_RENAMEONCOLLISION;
                    if (FOF_NOCONFIRMATION) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCONFIRMATION;
                    if (FOF_WANTMAPPINGHANDLE) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_WANTMAPPINGHANDLE;
                    if (FOF_ALLOWUNDO) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_ALLOWUNDO;
                    if (FOF_FILESONLY) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_FILESONLY;
                    if (FOF_SIMPLEPROGRESS) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_SIMPLEPROGRESS;
                    if (FOF_NOCONFIRMMKDIR) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCONFIRMMKDIR;
                    if (FOF_NOERRORUI) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOERRORUI;
                    if (FOF_NOCOPYSECURITYATTRIBS) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_NOCOPYSECURITYATTRIBS;
                    if (FOF_NORECURSION) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_NORECURSION;
                    if (FOF_NO_CONNECTED_ELEMENTS) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_NO_CONNECTED_ELEMENTS;
                    if (FOF_WANTNUKEWARNING) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_WANTNUKEWARNING;
                    if (FOF_NORECURSEREPARSE) result |= (ushort)FILEOP_FLAGS_ENUM.FOF_NORECURSEREPARSE;
                    return result;
                }
            }
        }
    }
}