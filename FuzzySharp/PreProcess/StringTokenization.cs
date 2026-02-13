using System;
using System.Buffers;
using System.Collections.Generic;

namespace FuzzySharp.PreProcess
{
    internal static class StringTokenization
    {
        private static readonly SearchValues<char> s_asciiWhitespaces = SearchValues.Create(" \t\r\n\f\v");

        public static string[] SplitOnWhitespace(string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.Length == 0)
            {
                return Array.Empty<string>();
            }

            var tokens = new List<string>();
            ReadOnlySpan<char> span = input.AsSpan();
            int i = 0;

            while (i < span.Length)
            {
                while (i < span.Length && IsWhitespace(span[i]))
                {
                    i++;
                }

                if (i >= span.Length)
                {
                    break;
                }

                int start = i;
                while (i < span.Length && !IsWhitespace(span[i]))
                {
                    i++;
                }

                tokens.Add(input.Substring(start, i - start));
            }

            return tokens.ToArray();
        }

        public static string[] SplitOnAsciiLetterRuns(string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (input.Length == 0)
            {
                return Array.Empty<string>();
            }

            var tokens = new List<string>();
            ReadOnlySpan<char> span = input.AsSpan();
            int i = 0;

            while (i < span.Length)
            {
                while (i < span.Length && !IsAsciiLetter(span[i]))
                {
                    i++;
                }

                if (i >= span.Length)
                {
                    break;
                }

                int start = i;
                while (i < span.Length && IsAsciiLetter(span[i]))
                {
                    i++;
                }

                tokens.Add(input.Substring(start, i - start));
            }

            return tokens.ToArray();
        }

        private static bool IsWhitespace(char c)
        {
            return c <= '\u007f' ? s_asciiWhitespaces.Contains(c) : char.IsWhiteSpace(c);
        }

        private static bool IsAsciiLetter(char c)
        {
            return (uint)((c | 0x20) - 'a') <= ('z' - 'a');
        }
    }
}
