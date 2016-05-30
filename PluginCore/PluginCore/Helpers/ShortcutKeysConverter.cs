using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PluginCore.Helpers
{
    /// <summary>
    /// A converter class for <see cref="ShortcutKeys"/>.
    /// </summary>
    public static class ShortcutKeysConverter
    {
        private const string Alt = "Alt+";
        private const string Ctrl = "Ctrl+";
        private const string Shift = "Shift+";

        private static Dictionary<Keys, string> names;
        private static Dictionary<string, Keys> keys;

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
        /// Converts a <see cref="string"/> to <see cref="ShortcutKeys"/>.
        /// </summary>
        /// <param name="value">A <see cref="string"/> to convert.</param>
        /// <returns></returns>
        public static ShortcutKeys ConvertFromString(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            if (keys == null)
            {
                Initialize();
            }

            return ConvertFromStringInternal(value);
        }

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
            Add(Keys.Oemtilde, "~"); // Keys.Oem3
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
                        return Ctrl + Alt + Shift + GetString(keys & Keys.KeyCode);
                    }
                    return Ctrl + Alt + GetString(keys & Keys.KeyCode);
                }
                if ((keys & Keys.Shift) == Keys.Shift)
                {
                    return Ctrl + Shift + GetString(keys & Keys.KeyCode);
                }
                return Ctrl + GetString(keys & Keys.KeyCode);
            }
            if ((keys & Keys.Alt) == Keys.Alt)
            {
                if ((keys & Keys.Shift) == Keys.Shift)
                {
                    return Alt + Shift + GetString(keys & Keys.KeyCode);
                }
                return Alt + GetString(keys & Keys.KeyCode);
            }
            if ((keys & Keys.Shift) == Keys.Shift)
            {
                return Shift + GetString(keys & Keys.KeyCode);
            }
            return GetString(keys);
        }

        private unsafe static ShortcutKeys ConvertFromStringInternal(string value)
        {
            // For performance reasons, use an unsafe context and char pointers.
            int index = 0;
            bool extended = false;
            var first = Keys.None;
            var second = Keys.None;
            var length = value.Length;

            fixed (char* c = value)
            {
                for (int i = 0; i < length; i++)
                {
                    switch (*(c + i))
                    {
                        case '+':
                            char* o = c + i;
                            if (i < 4 || *(o - 4) != 'N' || *(o - 3) != 'u' || *(o - 2) != 'm' || *(o - 1) != ' ')
                            {
                                if (extended) second |= GetKey(new string(c, index, i - index));
                                else first |= GetKey(new string(c, index, i - index));
                                index = i + 1;
                            }
                            break;
                        case ',':
                            if (index != i)
                            {
                                if (extended) throw new FormatException();
                                first |= GetKey(new string(c, index, i - index));
                                do
                                {
                                    if (++i == length) throw new FormatException();
                                }
                                while (char.IsWhiteSpace(*(c + i)));
                                index = i--;
                                extended = true;
                            }
                            break;
                    }
                }

                if (extended) second |= GetKey(new string(c, index, length - index));
                else first |= GetKey(new string(c, index, length - index));
            }

            return new ShortcutKeys(first, second);
        }

        private static string GetString(Keys keys)
        {
            string name;
            return names.TryGetValue(keys, out name) ? name : keys.ToString();
        }

        private static Keys GetKey(string value)
        {
            Keys key;
            return keys.TryGetValue(value.Trim(), out key) ? key : (Keys) Enum.Parse(typeof(Keys), value);
        }
    }
}
