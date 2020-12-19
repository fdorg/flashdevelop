using System;
using System.Runtime.InteropServices;

namespace ScintillaNet
{
    /// <summary>
    /// Compatible with Windows NMHDR.
    /// hwndFrom is really an environment specific window handle or pointer
    /// but most clients of Scintilla.h do not have this type visible.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NotifyHeader
    {
        /// <summary>
        /// environment specific window handle/pointer
        /// </summary>
        public IntPtr hwndFrom;

        /// <summary>
        /// CtrlID of the window issuing the notification
        /// </summary>
        public IntPtr idFrom;

        /// <summary>
        /// The SCN_* notification Code
        /// </summary>
        public uint code;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCNotification
    {
        public NotifyHeader nmhdr;
        public int position;             /* SCN_STYLENEEDED, SCN_DOUBLECLICK, SCN_MODIFIED, SCN_MARGINCLICK, SCN_NEEDSHOWN, SCN_DWELLSTART, SCN_DWELLEND, SCN_CALLTIPCLICK, SCN_HOTSPOTCLICK, SCN_HOTSPOTDOUBLECLICK, SCN_HOTSPOTRELEASECLICK, SCN_INDICATORCLICK, SCN_INDICATORRELEASE, SCN_USERLISTSELECTION, SCN_AUTOCSELECTION */
        public char ch;                  /* SCN_CHARADDED, SCN_KEY, SCN_AUTOCCOMPLETE, SCN_AUTOCSELECTION, SCN_USERLISTSELECTION */
        public int modifiers;            /* SCN_KEY, SCN_DOUBLECLICK, SCN_HOTSPOTCLICK, SCN_HOTSPOTDOUBLECLICK, SCN_HOTSPOTRELEASECLICK, SCN_INDICATORCLICK, SCN_INDICATORRELEASE */
        public int modificationType;     /* SCN_MODIFIED - modification types are name "SC_MOD_*" */
        public IntPtr text;              /* SCN_MODIFIED, SCN_USERLISTSELECTION, SCN_AUTOCSELECTION, SCN_URIDROPPED */
        public int length;               /* SCN_MODIFIED */
        public int linesAdded;           /* SCN_MODIFIED */
        public int message;              /* SCN_MACRORECORD */
        public IntPtr wParam;            /* SCN_MACRORECORD */
        public IntPtr lParam;            /* SCN_MACRORECORD */
        public int line;                 /* SCN_MACRORECORD */
        public int foldLevelNow;         /* SCN_MACRORECORD */
        public int foldLevelPrev;        /* SCN_MACRORECORD */
        public int margin;               /* SCN_MARGINCLICK */
        public int listType;             /* SCN_USERLISTSELECTION */
        public int x;                    /* SCN_DWELLSTART, SCN_DWELLEND */
        public int y;                    /* SCN_DWELLSTART, SCN_DWELLEND */
        public int Token;                /* SCN_MODIFIED with SC_MOD_CONTAINER */
        public int AnnotationLinesAdded; /* SC_MOD_CHANGEANNOTATION */
        public int Updated;              /* SCN_UPDATEUI */
        public int ListCompletionMethod; /* SCN_AUTOCSELECTION, SCN_AUTOCCOMPLETED, SCN_USERLISTSELECTION */
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CharacterRange
    {
        public int cpMin;
        public int cpMax;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TextRange
    {
        public CharacterRange chrg;
        public IntPtr lpstrText;
    }

    public struct TextToFind
    {
        public CharacterRange chrg;
        public IntPtr lpstrText;
        public CharacterRange chrgText;
    }

    public struct RangeToFormat
    {
        public IntPtr hdc;
        public IntPtr hdcTarget;
        public Rect rc;
        public Rect rcPage;
        public CharacterRange chrg;
    }
    
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    
}
