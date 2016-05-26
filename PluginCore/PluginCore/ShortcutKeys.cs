using System;
using System.Windows.Forms;

namespace PluginCore
{
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

        public ShortcutKeys(Keys value)
        {
            m_first = value;
            m_second = 0;
        }

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

        public static ShortcutKeys None
        {
            get { return new ShortcutKeys(); }
        }

        #endregion

        #region Static Methods

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

        public Keys First
        {
            get { return m_first; }
        }

        public Keys Second
        {
            get { return m_second; }
        }

        public bool IsNone
        {
            get { return m_first == 0; }
        }

        public bool IsSimple
        {
            get { return m_second == 0 && m_first != 0; }
        }

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

        public bool Equals(ShortcutKeys obj)
        {
            return m_first == obj.m_first && m_second == obj.m_second;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is ShortcutKeys && Equals((ShortcutKeys) obj);
        }

        public override int GetHashCode()
        {
            return (int) m_first;
        }

        public override string ToString()
        {
            return IsExtended ?
                keysConverter.ConvertToString(m_first) + ", " + keysConverter.ConvertToString(m_second)
                : keysConverter.ConvertToString(m_first);
        }

        #endregion
    }
}
