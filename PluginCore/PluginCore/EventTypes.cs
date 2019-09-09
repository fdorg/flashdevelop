// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Windows.Forms;

namespace PluginCore
{
    /// <summary>
    /// Events without arguments
    /// </summary>
    public class NotifyEvent
    {
        public EventType Type { get; }

        public bool Handled { get; set; }

        public NotifyEvent(EventType type)
        {
            Handled = false;
            Type = type;
        }
    }

    /// <summary>
    /// Events with text data
    /// </summary>
    public class TextEvent : NotifyEvent
    {
        public string Value { get; set; }

        public TextEvent(EventType type, string value) : base(type)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Events with number data
    /// </summary>
    public class NumberEvent : NotifyEvent
    {
        public int Value { get; set; }

        public NumberEvent(EventType type, int value) : base(type)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Events with Key data
    /// </summary>
    public class KeyEvent : NotifyEvent
    {
        public Keys Value { get; set; }

        public bool ProcessKey { get; set; }

        public KeyEvent(EventType type, Keys value) : base(type)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Events with custom data
    /// </summary>
    public class DataEvent : NotifyEvent
    {
        public string Action { get; }

        public object Data { get; set; }

        public DataEvent(EventType type, string action, object data) : base(type)
        {
            Action = action;
            Data = data;
        }
    }

    public class TextDataEvent : TextEvent
    {
        public object Data { get; }

        public TextDataEvent(EventType type, string text, object data) : base(type, text)
        {
            Data = data;
        }
    }
}