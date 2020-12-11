// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace PluginCore
{
    public static class StringExtensions
    {
        public static bool Contains(this string @this, char value) => @this.IndexOf(value) != -1;

        public static bool Contains(this string @this, char value, int startPosition) => @this.IndexOf(value, startPosition) != -1;

        public static bool Contains(this string @this, char value, out int position)
        {
            position = @this.IndexOf(value);
            return position != -1;
        }
       
        public static bool Contains(this string @this, string value, out int position)
        {
            position = @this.IndexOfOrdinal(value);
            return position != -1;
        }

        /// <summary>
        /// Determines whether the beginning of this <see cref="string"/> instance matches the specified Unicode character.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.StartsWith(string)"/> with length-1 string as parameter.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The Unicode character to compare.</param>
        /// <returns><code>true</code> if <code>value</code> matches the beginning of this string; otherwise, <code>false</code>.</returns>
        public static bool StartsWith(this string @this, char value) => @this.Length != 0 && @this[0] == value;

        /// <summary>
        /// Determines whether the beginning of this <see cref="string"/> instance matches the specified string when compared using the ordinal comparison option.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.StartsWith(string)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to compare.</param>
        /// <returns><code>true</code> if <code>value</code> matches the beginning of this string; otherwise, <code>false</code>.</returns>
        public static bool StartsWithOrdinal(this string @this, string value) => @this.StartsWith(value, StringComparison.Ordinal);

        /// <summary>
        /// Determines whether the end of this <see cref="string"/> instance matches the specified Unicode character.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.EndsWith(string)"/> with length-1 string as parameter.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The Unicode character to compare to the substring at the end of this instance.</param>
        /// <returns></returns>
        public static bool EndsWith(this string @this, char value)
        {
            var length = @this.Length;
            return length != 0 && @this[length - 1] == value;
        }

        /// <summary>
        /// Determines whether the end of this <see cref="string"/> instance matches the specified string when compared using the ordinal comparison option.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.EndsWith(string)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to compare to the substring at the end of this instance.</param>
        /// <returns><code>true</code> if <code>value</code> matches the end of this string; otherwise, <code>false</code>.</returns>
        public static bool EndsWithOrdinal(this string @this, string value) => @this.EndsWith(value, StringComparison.Ordinal);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="string"/> object using ordinal comparison option.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.IndexOf(string)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <returns>The index position of the <code>value</code> parameter if that string is found, or <code>-1</code> if it is not. If <code>value</code> is <see cref="string.Empty"/>, the return value is <code>0</code>.</returns>
        public static int IndexOfOrdinal(this string @this, string value) => @this.IndexOf(value, StringComparison.Ordinal);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="string"/> object using ordinal comparison option. The search starts at a specified character position.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.IndexOf(string, int)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <code>value</code> from the start of the current instance if that string is found, or <code>-1</code> if it is not. If <code>value</code> is <see cref="string.Empty"/>, the return value is <code>startIndex</code>.</returns>
        public static int IndexOfOrdinal(this string @this, string value, int startIndex) => @this.IndexOf(value, startIndex, StringComparison.Ordinal);

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="string"/> object using ordinal comparison option. The search starts at a specified character position and examines a specified number of character positions.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.IndexOf(string, int, int)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based index position of <code>value</code> from the start of the current instance if that string is found, or <code>-1</code> if it is not. If <code>value</code> is <see cref="string.Empty"/>, the return value is <code>startIndex</code>.</returns>
        public static int IndexOfOrdinal(this string @this, string value, int startIndex, int count) => @this.IndexOf(value, startIndex, count, StringComparison.Ordinal);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of a specified string within the current <see cref="string"/> object using ordinal comparison option.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.LastIndexOf(string)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <returns>The zero-based starting index position of <code>value</code> if that string is found, or <code>-1</code> if it is not. If <code>value</code> is <see cref="string.Empty"/>, the return value is the last index position in this instance.</returns>
        public static int LastIndexOfOrdinal(this string @this, string value) => @this.LastIndexOf(value, StringComparison.Ordinal);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of a specified string within the current <see cref="string"/> object using ordinal comparison option. The search starts at a specified character position and proceeds backward toward the beginning of the string.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.LastIndexOf(string, int)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <code>startIndex</code> toward the beginning of this instance.</param>
        /// <returns>The zero-based starting index position of <code>value</code> if that string is found, or <code>-1</code> if it is not found or if the current instance equals <see cref="string.Empty"/>. If <code>value</code> is <see cref="string.Empty"/>, the return value is the smaller of <code>startIndex</code> and the last index position in this instance.</returns>
        public static int LastIndexOfOrdinal(this string @this, string value, int startIndex) => @this.LastIndexOf(value, startIndex, StringComparison.Ordinal);

        /// <summary>
        /// Reports the zero-based index of the last occurrence of a specified string within the current <see cref="string"/> object using ordinal comparison option. The search starts at a specified character position and proceeds backward toward the beginning of the string for a specified number of character positions.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.LastIndexOf(string, int, int)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <code>startIndex</code> toward the beginning of this instance.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>The zero-based starting index position of <code>value</code> if that string is found, or <code>-1</code> if it is not found or if the current instance equals <see cref="string.Empty"/>. If <code>value</code> is <see cref="string.Empty"/>, the return value is the smaller of <code>startIndex</code> and the last index position in this instance.</returns>
        public static int LastIndexOfOrdinal(this string @this, string value, int startIndex, int count) => @this.LastIndexOf(value, startIndex, count, StringComparison.Ordinal);
    }

    public static class CollectionExtensions
    {
        /// <summary>Indicates whether the specified collection is <see langword="null" /> or an empty collection ([], {}, etc...).</summary>
        /// <param name="value">The collection to test.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or an empty collection ([], {}, etc...); otherwise, <see langword="false" />.</returns>
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> value)
        {
            switch (value)
            {
                case null: return true;
                case IList<TSource> list: return list.Count == 0;
                default:
                {
                    using var enumerator = value.GetEnumerator();
                    return !enumerator.MoveNext();
                }
            }
        }

        /// <summary>Indicates whether the specified collection is <see langword="null" /> or an empty collection ([], {}, etc...).</summary>
        /// <param name="value">The collection to test.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or an empty collection ([], {}, etc...); otherwise, <see langword="false" />.</returns>
        public static bool IsNullOrEmpty(this IEnumerable value)
        {
            return value switch
            {
                null => true,
                IList list => list.Count == 0,
                _ => !value.GetEnumerator().MoveNext(),
            };
        }

        /// <summary>Indicates whether the specified collection is <see langword="null" /> or an empty collection ([], {}, etc...).</summary>
        /// <param name="value">The collection to test.</param>
        /// <returns>
        /// <see langword="true" /> if the <paramref name="value" /> parameter is <see langword="null" /> or an empty collection ([], {}, etc...); otherwise, <see langword="false" />.</returns>
        public static bool IsNullOrEmpty(this PropertyDescriptorCollection value) => value is null || value.Count == 0;

        public static void ForEach<T>(this T[] @this, Action<T> action)
        {
            foreach (var it in @this)
            {
                action(it);
            }
        }

        public static void ForEach<T, _>(this T[] @this, Func<T, _> action)
        {
            foreach (var it in @this)
            {
                action(it);
            }
        }

        public static void ForEach<T>(this IList<T> @this, Action<T> action)
        {
            var count = @this.Count;
            for (var i = 0; i < count; i++)
            {
                action(@this[i]);
            }
        }

        public static void ForEach<T, _>(this IList<T> @this, Func<T, _> action)
        {
            var count = @this.Count;
            for (var i = 0; i < count; i++)
            {
                action(@this[i]);
            }
        }
    }
}
