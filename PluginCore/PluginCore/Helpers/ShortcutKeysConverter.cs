using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace PluginCore.Helpers
{
    /// <summary>
    /// Provides a <see cref="TypeConverter"/> to convert <see cref="ShortcutKeys"/> objects to and from other representations.
    /// </summary>
    public class ShortcutKeysConverter : TypeConverter
    {
        private const string Alt = "Alt+";
        private const string Ctrl = "Ctrl+";
        private const string Shift = "Shift+";

        private static Dictionary<Keys, string> names;
        private static Dictionary<string, Keys> keys;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortcutKeysConverter"/> class.
        /// </summary>
        public ShortcutKeysConverter()
        {

        }

        #endregion

        #region TypeConverter Overrides

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="Type"/> that represents the type you want to convert from.</param>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || sourceType == typeof(Keys) || sourceType == typeof(Keys[]) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="Type"/> that represents the type you want to convert to.</param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType) || destinationType == typeof(Keys) || destinationType == typeof(Keys[]);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return ConvertFromString((string) value);
            }
            if (value is Keys)
            {
                return (ShortcutKeys) (Keys) value;
            }
            if (value is Keys[])
            {
                var array = (Keys[]) value;
                switch (array.Length)
                {
                    case 0: return new ShortcutKeys();
                    case 1: return new ShortcutKeys(array[0]);
                    case 2: return new ShortcutKeys(array[0], array[1]);
                    default:
                        throw new FormatException("Length of the specified array is out of range.");
                }
            }
            if (value == null)
            {
                return ShortcutKeys.None;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="CultureInfo"/>. If <code>null</code> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="Type"/> to convert the <code>value</code> parameter to.</param>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }
            if (value is ShortcutKeys)
            {
                var keys = (ShortcutKeys) value;
                if (destinationType == typeof(string))
                {
                    return ConvertToString(keys);
                }
                if (destinationType == typeof(Keys))
                {
                    return (Keys) keys;
                }
                if (destinationType == typeof(Keys[]))
                {
                    return new[] { keys.First, keys.Second };
                }
            }
            else if (value is Keys)
            {
                var keys = (Keys) value;
                if (destinationType == typeof(string))
                {
                    return ConvertToString(keys);
                }
                if (destinationType == typeof(Keys))
                {
                    return keys;
                }
                if (destinationType == typeof(Keys[]))
                {
                    return new[] { keys, Keys.None };
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts a <see cref="string"/> to <see cref="ShortcutKeys"/>.
        /// </summary>
        /// <param name="value">A <see cref="string"/> to convert.</param>
        public new static ShortcutKeys ConvertFromString(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (keys == null)
            {
                Initialize();
            }
            return ConvertFromStringInternal(value);
        }

        /// <summary>
        /// Converts a <see cref="ShortcutKeys"/> value to <see cref="string"/>.
        /// </summary>
        /// <param name="keys">A <see cref="ShortcutKeys"/> value to convert.</param>
        public static string ConvertToString(ShortcutKeys keys)
        {
            if (names == null)
            {
                Initialize();
            }
            if (keys.IsExtended)
            {
                return ConvertToStringInternal(keys.First) + ", " + ConvertToStringInternal(keys.Second);
            }
            return ConvertToStringInternal(keys.First);
        }

        /// <summary>
        /// Converts a <see cref="Keys"/> value to <see cref="string"/>.
        /// </summary>
        /// <param name="keys">A <see cref="Keys"/> value to convert.</param>
        public static string ConvertToString(Keys keys)
        {
            if (names == null)
            {
                Initialize();
            }
            return ConvertToStringInternal(keys);
        }

        /// <summary>
        /// Converts a <see cref="string"/> to <see cref="ShortcutKeys"/>. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">A <see cref="string"/> to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="ShortcutKeys"/> value equivalent of the value represented in the specified string, if the conversion succeeded, or <see cref="ShortcutKeys.None"/> if the conversion failed.
        /// The conversion fails if the specified string is <see langword="null"/> or <see cref="string.Empty"/>, or is not of the correct format.
        /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        public static bool TryConvertFromString(string value, out ShortcutKeys result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = ShortcutKeys.None;
                return false;
            }
            if (keys == null)
            {
                Initialize();
            }
            return TryConvertFromStringInternal(value, out result);
        }

        #endregion

        #region Private Methods

        private static void Initialize()
        {
            names = new Dictionary<Keys, string>();
            keys = new Dictionary<string, Keys>();
            Add(Keys.Enter, "Enter"); // Keys.Return
            Add(Keys.CapsLock, "CapsLock"); // Keys.Capital
            Add(Keys.HangulMode, "HangulMode"); // Keys.HanguelMode, Keys.KannaMode
            Add(Keys.HanjaMode, "HanjaMode"); // Keys.KanjiMode
            Add(Keys.PageUp, "PgUp"); // Keys.Prior
            Add(Keys.PageDown, "PgDn"); // Keys.Next
            Add(Keys.Insert, "Ins");
            Add(Keys.Delete, "Del");
            Add(Keys.D0, "0");
            Add(Keys.D1, "1");
            Add(Keys.D2, "2");
            Add(Keys.D3, "3");
            Add(Keys.D4, "4");
            Add(Keys.D5, "5");
            Add(Keys.D6, "6");
            Add(Keys.D7, "7");
            Add(Keys.D8, "8");
            Add(Keys.D9, "9");
            Add(Keys.NumPad0, "Num 0");
            Add(Keys.NumPad1, "Num 1");
            Add(Keys.NumPad2, "Num 2");
            Add(Keys.NumPad3, "Num 3");
            Add(Keys.NumPad4, "Num 4");
            Add(Keys.NumPad5, "Num 5");
            Add(Keys.NumPad6, "Num 6");
            Add(Keys.NumPad7, "Num 7");
            Add(Keys.NumPad8, "Num 8");
            Add(Keys.NumPad9, "Num 9");
            Add(Keys.Multiply, "Num *");
            Add(Keys.Add, "Num +");
            Add(Keys.Subtract, "Num -");
            Add(Keys.Decimal, "Num .");
            Add(Keys.Divide, "Num /");
            Add(Keys.OemSemicolon, ";"); // Keys.Oem1
            Add(Keys.Oemplus, "=");
            Add(Keys.Oemcomma, ",");
            Add(Keys.OemMinus, "-");
            Add(Keys.OemPeriod, ".");
            Add(Keys.OemQuestion, "/"); // Keys.Oem2
            Add(Keys.Oemtilde, "`"); // Keys.Oem3
            Add(Keys.OemOpenBrackets, "["); // Keys.Oem4
            Add(Keys.OemPipe, "\\"); // Keys.Oem5
            Add(Keys.OemCloseBrackets, "]"); // Keys.Oem6
            Add(Keys.OemQuotes, "\""); // Keys.Oem7
            Add(Keys.OemBackslash, "\\"); // Keys.Oem102
            Add(Keys.Shift, "Shift");
            Add(Keys.Control, "Ctrl");
            Add(Keys.Alt, "Alt");
        }

        private static void Add(Keys key, string name)
        {
            names[key] = name;
            keys[name] = key;
        }

        private static ShortcutKeys ConvertFromStringInternal(string value)
        {
            int index = 0;
            bool extended = false;
            var first = Keys.None;
            var second = Keys.None;
            int length = value.Length;

            for (int i = 0; i < length; i++)
            {
                switch (value[i])
                {
                    case '+':
                        if (i < 4 || string.CompareOrdinal(value, i - 4, "Num +", 0, 4) != 0)
                        {
                            if (extended)
                            {
                                second |= GetKey(value.Substring(index, i - index));
                            }
                            else
                            {
                                first |= GetKey(value.Substring(index, i - index));
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    throw new FormatException($"Missing part after '{value[i]}'");
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                        }
                        break;
                    case ',':
                        if (index != i)
                        {
                            if (extended)
                            {
                                throw new FormatException($"{nameof(ShortcutKeys)} cannot have more than two parts.");
                            }
                            else
                            {
                                first |= GetKey(value.Substring(index, i - index));
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    throw new FormatException($"Missing part after '{value[i]}'");
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                            extended = true;
                        }
                        break;
                }
            }

            if (extended)
            {
                second |= GetKey(value.Substring(index, length - index));
            }
            else
            {
                first |= GetKey(value.Substring(index, length - index));
            }

            return new ShortcutKeys(first, second);
        }

        private static string ConvertToStringInternal(Keys keys)
        {
            // For performance reasons, instead of using a string or StringBuilder buffer and appending
            // text such as Ctrl, Alt and Shift on it, use the string concatenation to utilize the compiler optimization,
            // which turns them into string.Concat() calls.
            if ((keys & Keys.Control) == Keys.Control)
            {
                if ((keys & Keys.Alt) == Keys.Alt)
                {
                    if ((keys & Keys.Shift) == Keys.Shift)
                    {
                        return Ctrl + Alt + Shift + GetName(keys & Keys.KeyCode);
                    }
                    return Ctrl + Alt + GetName(keys & Keys.KeyCode);
                }
                if ((keys & Keys.Shift) == Keys.Shift)
                {
                    return Ctrl + Shift + GetName(keys & Keys.KeyCode);
                }
                return Ctrl + GetName(keys & Keys.KeyCode);
            }
            if ((keys & Keys.Alt) == Keys.Alt)
            {
                if ((keys & Keys.Shift) == Keys.Shift)
                {
                    return Alt + Shift + GetName(keys & Keys.KeyCode);
                }
                return Alt + GetName(keys & Keys.KeyCode);
            }
            if ((keys & Keys.Shift) == Keys.Shift)
            {
                return Shift + GetName(keys & Keys.KeyCode);
            }
            return GetName(keys);
        }

        private static bool TryConvertFromStringInternal(string value, out ShortcutKeys shortcutKeys)
        {
            int index = 0;
            bool extended = false;
            var first = Keys.None;
            var second = Keys.None;
            var result = Keys.None;
            int length = value.Length;

            for (int i = 0; i < length; i++)
            {
                switch (value[i])
                {
                    case '+':
                        if (i < 4 || string.CompareOrdinal(value, i - 4, "Num +", 0, 4 /*5*/) != 0)
                        {
                            if (extended)
                            {
                                if (!TryGetKey(value.Substring(index, i - index), out result))
                                {
                                    shortcutKeys = ShortcutKeys.None;
                                    return false;
                                }
                                second |= result;
                            }
                            else
                            {
                                if (!TryGetKey(value.Substring(index, i - index), out result))
                                {
                                    shortcutKeys = ShortcutKeys.None;
                                    return false;
                                }
                                first |= result;
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    shortcutKeys = ShortcutKeys.None;
                                    return false;
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                        }
                        break;
                    case ',':
                        if (index != i)
                        {
                            if (extended)
                            {
                                shortcutKeys = ShortcutKeys.None;
                                return false;
                            }
                            else
                            {
                                if (!TryGetKey(value.Substring(index, i - index), out result))
                                {
                                    shortcutKeys = ShortcutKeys.None;
                                    return false;
                                }
                                first |= result;
                            }
                            do
                            {
                                if (++i == length)
                                {
                                    shortcutKeys = ShortcutKeys.None;
                                    return false;
                                }
                            }
                            while (char.IsWhiteSpace(value[i]));
                            index = i--;
                            extended = true;
                        }
                        break;
                }
            }

            if (!TryGetKey(value.Substring(index, length - index), out result))
            {
                shortcutKeys = ShortcutKeys.None;
                return false;
            }

            if (extended)
            {
                second |= result;
            }
            else
            {
                first |= result;
            }

            if (first == 0 && second != 0)
            {
                shortcutKeys = ShortcutKeys.None;
                return false;
            }

            shortcutKeys = new ShortcutKeys(first, second);
            return true;
        }

        private static Keys GetKey(string name)
        {
            Keys key;
            if (keys.TryGetValue(name.Trim(), out key))
            {
                return key;
            }

            try
            {
                return (Keys) Enum.Parse(typeof(Keys), name);
            }
            catch (Exception e)
            {
                throw new FormatException($"'{name}' is not a named constant defined for {nameof(ShortcutKeys)}.", e);
            }
        }

        private static string GetName(Keys keys)
        {
            string name;
            return names.TryGetValue(keys, out name) ? name : keys.ToString();
        }

        private static bool TryGetKey(string name, out Keys result)
        {
            if (keys.TryGetValue(name.Trim(), out result))
            {
                return true;
            }

            try
            {
                result = (Keys) Enum.Parse(typeof(Keys), name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
