using System;
using System.ComponentModel;

namespace PluginCore
{
    [Flags]
    public enum EventType : long
    {
        // You can copy new values from:
        // http://stackoverflow.com/questions/1060760/what-to-do-when-bit-mask-flags-enum-gets-too-large
        FileNew = 1, // TextEvent (file)
        FileOpen = 2, // TextEvent (file)
        FileOpening = 4, // TextEvent (file)
        FileClose = 8, // TextEvent (file)
        FileSwitch = 16, // NotifyEvent
        FileModify = 32, // TextEvent (file)
        FileModifyRO = 64, // TextEvent (file)
        FileSave = 128, // TextEvent (file)
        FileSaving = 256, // TextEvent (file)
        FileReload = 512, // TextEvent (file)
        FileRevert = 1024, // TextEvent (file)
        FileRename = 2048, // TextEvent (old;new)
        FileRenaming = 4096, // TextEvent (old;new)
        FileEncode = 8192, // DataEvent (file, text)
        FileDecode = 16384, // DataEvent (file, null)
        FileEmpty = 32768, // NotifyEvent
        FileTemplate = 65536, // TextEvent (file)
        RestoreSession = 131072, // DataEvent (file, session)
        RestoreLayout = 262144, // TextEvent (file)
        SyntaxChange = 524288, // TextEvent (language)
        SyntaxDetect = 1048576, // TextEvent (language)
        UIStarted = 2097152, // NotifyEvent
        UIRefresh = 4194304, // NotifyEvent
        UIClosing = 8388608, // NotifyEvent
        ApplySettings = 16777216, // NotifyEvent
        SettingChanged = 33554432, // TextEvent (setting)
        ProcessArgs = 67108864, // TextEvent (content)
        ProcessStart = 134217728, // NotifyEvent
        ProcessEnd = 268435456, // TextEvent (result)
        StartArgs = 536870912, // NotifyEvent
        Shortcut = 1073741824, // DataEvent (id, keys)
        Command = 2147483648, // DataEvent (command)
        Trace = 4294967296, // NotifyEvent
        Keys = 8589934592, // KeyEvent (keys)
        Completion = 17179869184, // NotifyEvent
        AppChanges = 34359738368, // NotifyEvent
        ApplyTheme = 68719476736 // NotifyEvent
    }

    public enum UpdateInterval
    {
        Never = -1,
        Monthly = 0,
        Weekly = 1
    }

    public enum SessionType
    {
        Startup = 0,
        Layout = 1,
        External = 2
    }

    public enum CodingStyle
    {
        BracesOnLine = 0,
        BracesAfterLine = 1
    }

    public enum CommentBlockStyle
    {
        Indented = 0,
        NotIndented = 1
    }

    public enum UiRenderMode
    {
        Professional,
        System
    }

    public enum HandlingPriority
    {
        High = 0,
        Normal = 1,
        Low = 2
    }

    public enum TraceType
    {
        ProcessStart = -1,
        ProcessEnd = -2,
        ProcessError = -3,
        Info = 0,
        Debug = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }

    public enum CodePage
    {
        EightBits = 0,
        BigEndian = 1201,
        LittleEndian = 1200,
        [Browsable(false)]
        UTF32 = 65005,
        UTF8 = 65001,
        UTF7 = 65000
    }

}
