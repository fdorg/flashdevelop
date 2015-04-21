using System;

namespace ScintillaNet
{
    public struct NotifyHeader
    {
        public IntPtr hwndFrom;
        public uint idFrom;
        public uint code;
    };

    public struct SCNotification
    {
        public NotifyHeader nmhdr;
        public int position;
        public int ch;
        public int modifiers;
        public int modificationType;
        public IntPtr text;
        public int length;
        public int linesAdded;
        public int message;
        public IntPtr wParam;
        public IntPtr lParam;
        public int line;
        public int foldLevelNow;
        public int foldLevelPrev;
        public int margin;
        public int listType;
        public int x;
        public int y;
    };
    
    public struct CharacterRange
    {
        public int cpMin;
        public int cpMax;
    };
    
    public struct TextRange
    {
        public CharacterRange chrg;
        public IntPtr lpstrText;
    };

    public struct TextToFind
    {
        public CharacterRange chrg;
        public IntPtr lpstrText;
        public CharacterRange chrgText;
    };

    public struct RangeToFormat
    {
        public IntPtr hdc;
        public IntPtr hdcTarget;
        public Rect rc;
        public Rect rcPage;
        public CharacterRange chrg;
    };
    
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    };
    
}
