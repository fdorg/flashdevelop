using System;
using System.ComponentModel;
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
        private ShortcutKeys value;
        private String command;
        private Boolean processKey;

        /// <summary>
        /// Gets the <see cref="System.Windows.Forms.Keys"/> value associated with this <see cref="KeyEvent"/>.
        /// <para/>
        /// [deprecated] Use the <see cref="Keys"/> property instead.
        /// </summary>
        [Obsolete("This property has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Keys Value
        {
            get { return this.value; }
            set { }
        }

        /// <summary>
        /// Gets the <see cref="ShortcutKeys"/> value associated with this <see cref="KeyEvent"/>.
        /// </summary>
        public ShortcutKeys Keys
        {
            get { return this.value; }
            //set { this.value = value; }
        }

        /// <summary>
        /// Gets the shortcut command string associated with this <see cref="KeyEvent"/>.
        /// </summary>
        public String Command
        {
            get { return this.command; }
        }

        /// <summary>
        /// Gets or sets whether to process the keys associated with this <see cref="KeyEvent"/>.
        /// <para/>
        /// This property is currently not used by any of the default plugins. Prefer using the <see cref="NotifyEvent.Handled"/> property.
        /// </summary>
        public Boolean ProcessKey
        {
            get { return this.processKey; }
            set { this.processKey = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// <para/>
        /// [deprecated] Use the <see cref="KeyEvent(EventType, ShortcutKeys)"/> constructor instead.
        /// </summary>
        [Obsolete("This constructor has been deprecated.", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public KeyEvent(EventType type, Keys value) : this(type, (ShortcutKeys) value) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// </summary>
        public KeyEvent(EventType type, ShortcutKeys value) : base(type)
        {
            this.value = value;
            this.command = PluginBase.MainForm.GetShortcutId(this.value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// </summary>
        public KeyEvent(EventType type, ShortcutKeys value, String command) : base(type)
        {
            this.value = value;
            this.command = command;
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
