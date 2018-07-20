using System;

namespace PluginCore
{
    public static class StringExtensions
    {
        public static bool Contains(this string @this, char value) => @this.IndexOf(value) >= 0;

        /// <summary>
        /// Determines whether the beginning of this <see cref="string"/> instance matches the specified Unicode character.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.StartsWith(string)"/> with length-1 string as parameter.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The Unicode character to compare.</param>
        /// <returns><code>true</code> if <code>value</code> matches the beginning of this string; otherwise, <code>false</code>.</returns>
        public static bool StartsWith(this string @this, char value)
        {
            return @this.Length != 0 && @this[0] == value;
        }

        /// <summary>
        /// Determines whether the beginning of this <see cref="string"/> instance matches the specified string when compared using the ordinal comparison option.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.StartsWith(string)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to compare.</param>
        /// <returns><code>true</code> if <code>value</code> matches the beginning of this string; otherwise, <code>false</code>.</returns>
        public static bool StartsWithOrdinal(this string @this, string value)
        {
            return @this.StartsWith(value, StringComparison.Ordinal);
        }

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
            int length = @this.Length;
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
        public static bool EndsWithOrdinal(this string @this, string value)
        {
            return @this.EndsWith(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="string"/> object using ordinal comparison option.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.IndexOf(string)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <returns>The index position of the <code>value</code> parameter if that string is found, or <code>-1</code> if it is not. If <code>value</code> is <see cref="string.Empty"/>, the return value is <code>0</code>.</returns>
        public static int IndexOfOrdinal(this string @this, string value)
        {
            return @this.IndexOf(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="string"/> object using ordinal comparison option. The search starts at a specified character position.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.IndexOf(string, int)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of <code>value</code> from the start of the current instance if that string is found, or <code>-1</code> if it is not. If <code>value</code> is <see cref="string.Empty"/>, the return value is <code>startIndex</code>.</returns>
        public static int IndexOfOrdinal(this string @this, string value, int startIndex)
        {
            return @this.IndexOf(value, startIndex, StringComparison.Ordinal);
        }

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
        public static int IndexOfOrdinal(this string @this, string value, int startIndex, int count)
        {
            return @this.IndexOf(value, startIndex, count, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of a specified string within the current <see cref="string"/> object using ordinal comparison option.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.LastIndexOf(string)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <returns>The zero-based starting index position of <code>value</code> if that string is found, or <code>-1</code> if it is not. If <code>value</code> is <see cref="string.Empty"/>, the return value is the last index position in this instance.</returns>
        public static int LastIndexOfOrdinal(this string @this, string value)
        {
            return @this.LastIndexOf(value, StringComparison.Ordinal);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of a specified string within the current <see cref="string"/> object using ordinal comparison option. The search starts at a specified character position and proceeds backward toward the beginning of the string.
        /// <para/>
        /// WARNING! Always use this method instead of <see cref="string.LastIndexOf(string, int)"/>.
        /// </summary>
        /// <param name="this">A <see cref="string"/> object.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position. The search proceeds from <code>startIndex</code> toward the beginning of this instance.</param>
        /// <returns>The zero-based starting index position of <code>value</code> if that string is found, or <code>-1</code> if it is not found or if the current instance equals <see cref="string.Empty"/>. If <code>value</code> is <see cref="string.Empty"/>, the return value is the smaller of <code>startIndex</code> and the last index position in this instance.</returns>
        public static int LastIndexOfOrdinal(this string @this, string value, int startIndex)
        {
            return @this.LastIndexOf(value, startIndex, StringComparison.Ordinal);
        }

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
        public static int LastIndexOfOrdinal(this string @this, string value, int startIndex, int count)
        {
            return @this.LastIndexOf(value, startIndex, count, StringComparison.Ordinal);
        }
    }
}
