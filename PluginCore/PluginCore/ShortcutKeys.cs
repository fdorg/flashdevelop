using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using PluginCore.Helpers;

namespace PluginCore
{
    /// <summary>
    /// Represents an extended shortcut combination.
    /// </summary>
    [Editor(typeof(ShortcutKeysEditor), typeof(UITypeEditor))]
    [Serializable]
    [TypeConverter(typeof(ShortcutKeysConverter))]
    public struct ShortcutKeys
    {
        private Keys m_first;
        private Keys m_second;

        #region Constructors

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
            if (first == 0 && second != 0)
            {
                throw new ArgumentException($"Parameter '{nameof(second)}' must be {nameof(Keys)}.{nameof(Keys.None)} if '{nameof(first)}' is {nameof(Keys)}.{nameof(Keys.None)}.", nameof(second));
            }

            m_first = first;
            m_second = second;
        }

        #endregion

        #region Operators

        public static bool operator ==(ShortcutKeys a, ShortcutKeys b)
        {
            return a.m_first == b.m_first && a.m_second == b.m_second;
        }

        public static bool operator !=(ShortcutKeys a, ShortcutKeys b)
        {
            return a.m_first != b.m_first || a.m_second != b.m_second;
        }

        public static bool operator ==(ShortcutKeys a, Keys b)
        {
            return a.m_first == b && a.m_second == 0;
        }

        public static bool operator !=(ShortcutKeys a, Keys b)
        {
            return a.m_first != b || a.m_second != 0;
        }

        public static bool operator ==(Keys a, ShortcutKeys b)
        {
            return b.m_first == a && b.m_second == 0;
        }

        public static bool operator !=(Keys a, ShortcutKeys b)
        {
            return b.m_first != a || b.m_second != 0;
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
            return ShortcutKeysConverter.ConvertFromString(s);
        }

        /// <summary>
        /// Converts the string representation of <see cref="ShortcutKeys"/> into its equivalent. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="s">A string representation of <see cref="ShortcutKeys"/> to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="ShortcutKeys"/> value equivalent of the value represented in the specified string, if the conversion succeeded, or <see cref="None"/> if the conversion failed.
        /// The conversion fails if the specified string is <see langword="null"/> or <see cref="string.Empty"/>, or is not of the correct format.
        /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        public static bool TryParse(string s, out ShortcutKeys result)
        {
            return ShortcutKeysConverter.TryConvertFromString(s, out result);
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

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="ShortcutKeys"/> is equal to the current <see cref="ShortcutKeys"/>.
        /// </summary>
        /// <param name="obj">The <see cref="ShortcutKeys"/> to compare with the current <see cref="ShortcutKeys"/>.</param>
        public bool Equals(ShortcutKeys obj)
        {
            return this == obj;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="object"/>.</param>
        public override bool Equals(object obj)
        {
            return obj is ShortcutKeys && this == (ShortcutKeys) obj || obj is Keys && this == (Keys) obj;
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
            return ShortcutKeysConverter.ConvertToString(this);
        }

        #endregion
    }
}
