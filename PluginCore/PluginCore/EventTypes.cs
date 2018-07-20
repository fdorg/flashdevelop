// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Windows.Forms;

namespace PluginCore
{
    /// <summary>
    /// Events without arguments
    /// </summary>
    public class NotifyEvent
    {
        private EventType type;
        private Boolean handled;

        public EventType Type
        {
            get { return this.type; }
        }

        public Boolean Handled
        {
            get { return this.handled; }
            set { this.handled = value; }
        }

        public NotifyEvent(EventType type)
        {
            this.handled = false;
            this.type = type;
        }
    }

    /// <summary>
    /// Events with text data
    /// </summary>
    public class TextEvent : NotifyEvent
    {
        private String value;

        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public TextEvent(EventType type, String value) : base(type)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// Events with number data
    /// </summary>
    public class NumberEvent : NotifyEvent
    {
        private Int32 value;

        public Int32 Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public NumberEvent(EventType type, Int32 value) : base(type)
        {
            this.value = value;
        }

    }

    /// <summary>
    /// Events with Key data
    /// </summary>
    public class KeyEvent : NotifyEvent
    {
        private Keys value;
        private Boolean processKey;

        public Keys Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public Boolean ProcessKey
        {
            get { return this.processKey; }
            set { this.processKey = value; }
        }

        public KeyEvent(EventType type, Keys value) : base(type)
        {
            this.value = value;
        }

    }

    /// <summary>
    /// Events with custom data
    /// </summary>
    public class DataEvent : NotifyEvent
    {
        private Object data;
        private String action;

        public String Action
        {
            get { return this.action; }
        }

        public Object Data
        {
            get { return this.data; }
            set { this.data = value; }
        }

        public DataEvent(EventType type, String action, Object data) : base(type)
        {
            this.action = action;
            this.data = data;
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
