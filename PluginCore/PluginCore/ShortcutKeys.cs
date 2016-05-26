using System;
using System.Windows.Forms;

namespace PluginCore
{
    /// <summary>
    /// Represents an extended shortcut combination.
    /// </summary>
    public struct ShortcutKeys
    {
        private static KeysConverter keysConverter;
        private Keys m_first;
        private Keys m_second;

        #region Constructors

        static ShortcutKeys()
        {
            keysConverter = new KeysConverter();
        }

        /// <summary>
        /// Creates a simple <see cref="ShortcutKeys"/> with the specified <see cref="Keys"/> value.
        /// </summary>
        /// <param name="value">A <see cref="Keys"/> value.</param>
        public ShortcutKeys(Keys value)
        {
            m_first = value;
            m_second = 0;
        }

        /// <summary>
        /// Creates an extended <see cref="ShortcutKeys"/> with the specified <see cref="Keys"/> values.
        /// </summary>
        /// <param name="first">The <see cref="Keys"/> value of first part of the shortcut keys combination.</param>
        /// <param name="second">The <see cref="Keys"/> value of the second part of the shortcut keys combination.</param>
        public ShortcutKeys(Keys first, Keys second)
        {
            if (first == 0 && second != 0) throw new ArgumentException("Parameter second must be Keys.None if first is Keys.None.", "second");
            m_first = first;
            m_second = second;
        }

        #endregion

        #region Operators

        public static bool operator ==(ShortcutKeys a, ShortcutKeys b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ShortcutKeys a, ShortcutKeys b)
        {
            return !a.Equals(b);
        }

        public static implicit operator Keys(ShortcutKeys value)
        {
            return value.IsExtended ? 0 : value.m_first;
        }

        public static implicit operator ShortcutKeys(Keys value)
        {
            return new ShortcutKeys(value);
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Gets a <see cref="ShortcutKeys"/> value that represents no shortcuts.
        /// </summary>
        public static ShortcutKeys None
        {
            get { return new ShortcutKeys(); }
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Converts the string representation of <see cref="ShortcutKeys"/> into its equivalent.
        /// </summary>
        /// <param name="s">A string representation of <see cref="ShortcutKeys"/> to convert.</param>
        public static ShortcutKeys Parse(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            ShortcutKeys value;
            int index = s.IndexOf(',');
            if (index >= 0)
            {
                value.m_first = (Keys) keysConverter.ConvertFromString(s.Substring(0, index));
                value.m_second = (Keys) keysConverter.ConvertFromString(s.Substring(index + 2));
            }
            else
            {
                value.m_first = (Keys) keysConverter.ConvertFromString(s);
                value.m_second = 0;
            }
            return value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the first part of an extended <see cref="ShortcutKeys"/>, or the value of a simple <see cref="ShortcutKeys"/>.
        /// </summary>
        public Keys First
        {
            get { return m_first; }
        }

        /// <summary>
        /// Gets the second part of an extended <see cref="ShortcutKeys"/>, or <see cref="Keys.None"/> if simple.
        /// </summary>
        public Keys Second
        {
            get { return m_second; }
        }

        /// <summary>
        /// Gets whether the <see cref="ShortcutKeys"/> represents no shortcuts.
        /// </summary>
        public bool IsNone
        {
            get { return m_first == 0; }
        }

        /// <summary>
        /// Gets whether the <see cref="ShortcutKeys"/> represents a simple shortcut.
        /// </summary>
        public bool IsSimple
        {
            get { return m_second == 0 && m_first != 0; }
        }

        /// <summary>
        /// Gets whether the <see cref="ShortcutKeys"/> represents an extended shortcut.
        /// </summary>
        public bool IsExtended
        {
            get { return m_second != 0; }
        }

        internal object Value
        {
            get
            {
                if (IsExtended)
                {
                    return this;
                }
                return m_first;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="ShortcutKeys"/> is equal to the current <see cref="ShortcutKeys"/>.
        /// </summary>
        /// <param name="obj">The <see cref="ShortcutKeys"/> to compare with the current <see cref="ShortcutKeys"/>.</param>
        public bool Equals(ShortcutKeys obj)
        {
            return m_first == obj.m_first && m_second == obj.m_second;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
        public override bool Equals(object obj)
        {
            return obj != null && obj is ShortcutKeys && Equals((ShortcutKeys) obj);
        }

        /// <summary>
        /// Gets a hash code.
        /// </summary>
        public override int GetHashCode()
        {
            return (int) m_first;
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="ShortcutKeys"/>.
        /// </summary>
        public override string ToString()
        {
            return IsExtended ?
                keysConverter.ConvertToString(m_first) + ", " + keysConverter.ConvertToString(m_second)
                : keysConverter.ConvertToString(m_first);
        }

        #endregion
    }
}
