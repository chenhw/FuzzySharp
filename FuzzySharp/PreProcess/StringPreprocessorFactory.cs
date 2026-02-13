using System;
using System.Buffers;

namespace FuzzySharp.PreProcess
{
    internal static class StringPreprocessorFactory
    {
        private static readonly SearchValues<char> s_allowedCharacters = SearchValues.Create(" abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        private static string Default(string input)
        {
            ArgumentNullException.ThrowIfNull(input);

            return string.Create(input.Length, input, static (buffer, source) =>
            {
                for (int i = 0; i < source.Length; i++)
                {
                    var c = source[i];
                    buffer[i] = s_allowedCharacters.Contains(c) ? char.ToLowerInvariant(c) : ' ';
                }
            }).Trim();
        }

        public static Func<string, string> GetPreprocessor(PreprocessMode mode)
        {
            return mode switch
            {
                PreprocessMode.Full => Default,
                PreprocessMode.None => static s => s,
                _ => throw new InvalidOperationException($"Invalid string preprocessor mode: {mode}")
            };
        }
    }
}
