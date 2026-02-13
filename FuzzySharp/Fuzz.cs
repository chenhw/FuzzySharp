using System;
using FuzzySharp.PreProcess;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.Composite;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;

namespace FuzzySharp
{
    public static class Fuzz
    {
        private static int ScoreWith<TScorer>(string input1, string input2) where TScorer : IRatioScorer, new()
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);

            return ScorerCache.Get<TScorer>().Score(input1, input2);
        }

        private static int ScoreWith<TScorer>(string input1, string input2, PreprocessMode preprocessMode) where TScorer : IRatioScorer, new()
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);

            return ScorerCache.Get<TScorer>().Score(input1, input2, preprocessMode);
        }

        #region Ratio
        /// <summary>
        /// Calculates a Levenshtein simple ratio between the strings.
        /// This indicates a measure of similarity
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int Ratio(string input1, string input2)
        {
            return ScoreWith<DefaultRatioScorer>(input1, input2);
        }

        /// <summary>
        /// Calculates a Levenshtein simple ratio between the strings.
        /// This indicates a measure of similarity
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int Ratio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<DefaultRatioScorer>(input1, input2, preprocessMode);
        }
        #endregion

        #region PartialRatio
        /// <summary>
        /// Inconsistent substrings lead to problems in matching. This ratio
        /// uses a heuristic called "best partial" for when two strings
        /// are of noticeably different lengths.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialRatio(string input1, string input2)
        {
            return ScoreWith<PartialRatioScorer>(input1, input2);
        }

        /// <summary>
        /// Inconsistent substrings lead to problems in matching. This ratio
        /// uses a heuristic called "best partial" for when two strings
        /// are of noticeably different lengths.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<PartialRatioScorer>(input1, input2, preprocessMode);
        }
        #endregion

        #region TokenSortRatio
        /// <summary>
        /// Find all whitespace-delimited tokens in the string and sort
        /// those tokens and then take ratio of resulting
        /// joined strings.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenSortRatio(string input1, string input2)
        {
            return ScoreWith<TokenSortScorer>(input1, input2);
        }

        /// <summary>
        /// Find all whitespace-delimited tokens in the string and sort
        /// those tokens and then take ratio of resulting
        /// joined strings.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenSortRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<TokenSortScorer>(input1, input2, preprocessMode);
        }

        /// <summary>
        /// Find all whitespace-delimited tokens in the string and sort
        /// those tokens and then take ratio of resulting
        /// joined strings.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenSortRatio(string input1, string input2)
        {
            return ScoreWith<PartialTokenSortScorer>(input1, input2);
        }

        /// <summary>
        /// Find all whitespace-delimited tokens in the string and sort
        /// those tokens and then take ratio of resulting
        /// joined strings.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenSortRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<PartialTokenSortScorer>(input1, input2, preprocessMode);
        }
        #endregion

        #region TokenSetRatio
        /// <summary>
        /// Splits the strings into tokens and computes intersections and remainders
        /// between the tokens of the two strings.A comparison string is then
        /// built up and is compared using the simple ratio algorithm.
        /// Useful for strings where words appear redundantly.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenSetRatio(string input1, string input2)
        {
            return ScoreWith<TokenSetScorer>(input1, input2);
        }

        /// <summary>
        /// Splits the strings into tokens and computes intersections and remainders
        /// between the tokens of the two strings.A comparison string is then
        /// built up and is compared using the simple ratio algorithm.
        /// Useful for strings where words appear redundantly.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenSetRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<TokenSetScorer>(input1, input2, preprocessMode);
        }

        /// <summary>
        /// Splits the strings into tokens and computes intersections and remainders
        /// between the tokens of the two strings.A comparison string is then
        /// built up and is compared using the simple ratio algorithm.
        /// Useful for strings where words appear redundantly.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenSetRatio(string input1, string input2)
        {
            return ScoreWith<PartialTokenSetScorer>(input1, input2);
        }

        /// <summary>
        /// Splits the strings into tokens and computes intersections and remainders
        /// between the tokens of the two strings.A comparison string is then
        /// built up and is compared using the simple ratio algorithm.
        /// Useful for strings where words appear redundantly.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenSetRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<PartialTokenSetScorer>(input1, input2, preprocessMode);
        }
        #endregion

        #region TokenDifferenceRatio
        /// <summary>
        /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
        /// but the strings themselves)
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenDifferenceRatio(string input1, string input2)
        {
            return ScoreWith<TokenDifferenceScorer>(input1, input2);
        }

        /// <summary>
        /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
        /// but the strings themselves)
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenDifferenceRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<TokenDifferenceScorer>(input1, input2, preprocessMode);
        }

        /// <summary>
        /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
        /// but the strings themselves)
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenDifferenceRatio(string input1, string input2)
        {
            return ScoreWith<PartialTokenDifferenceScorer>(input1, input2);
        }

        /// <summary>
        /// Splits the strings into tokens and computes the ratio on those tokens (not the individual chars,
        /// but the strings themselves)
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenDifferenceRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<PartialTokenDifferenceScorer>(input1, input2, preprocessMode);
        }
        #endregion

        #region TokenInitialismRatio
        /// <summary>
        /// Splits longer string into tokens and takes the initialism and compares it to the shorter
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenInitialismRatio(string input1, string input2)
        {
            return ScoreWith<TokenInitialismScorer>(input1, input2);
        }

        /// <summary>
        /// Splits longer string into tokens and takes the initialism and compares it to the shorter
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenInitialismRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<TokenInitialismScorer>(input1, input2, preprocessMode);
        }

        /// <summary>
        /// Splits longer string into tokens and takes the initialism and compares it to the shorter
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenInitialismRatio(string input1, string input2)
        {
            return ScoreWith<PartialTokenInitialismScorer>(input1, input2);
        }

        /// <summary>
        /// Splits longer string into tokens and takes the initialism and compares it to the shorter
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenInitialismRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<PartialTokenInitialismScorer>(input1, input2, preprocessMode);
        }
        #endregion

        #region TokenAbbreviationRatio
        /// <summary>
        /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
        /// of the other strings tokens. One string must have all its characters in order in the other string
        /// to even be considered.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenAbbreviationRatio(string input1, string input2)
        {
            return ScoreWith<TokenAbbreviationScorer>(input1, input2);
        }

        /// <summary>
        /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
        /// of the other strings tokens. One string must have all its characters in order in the other string
        /// to even be considered.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int TokenAbbreviationRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<TokenAbbreviationScorer>(input1, input2, preprocessMode);
        }

        /// <summary>
        /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
        /// of the other strings tokens. One string must have all its characters in order in the other string
        /// to even be considered.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenAbbreviationRatio(string input1, string input2)
        {
            return ScoreWith<PartialTokenAbbreviationScorer>(input1, input2);
        }

        /// <summary>
        /// Similarity ratio that attempts to determine whether one strings tokens are an abbreviation
        /// of the other strings tokens. One string must have all its characters in order in the other string
        /// to even be considered.
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int PartialTokenAbbreviationRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<PartialTokenAbbreviationScorer>(input1, input2, preprocessMode);
        }
        #endregion

        #region WeightedRatio
        /// <summary>
        /// Calculates a weighted ratio between the different algorithms for best results
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int WeightedRatio(string input1, string input2)
        {
            return ScoreWith<WeightedRatioScorer>(input1, input2);
        }

        /// <summary>
        /// Calculates a weighted ratio between the different algorithms for best results
        /// </summary>
        /// <param name="input1">First input string.</param>
        /// <param name="input2">Second input string.</param>
        /// <param name="preprocessMode">Preprocessing mode applied before scoring.</param>
        /// <returns>Similarity score from 0 to 100.</returns>
        public static int WeightedRatio(string input1, string input2, PreprocessMode preprocessMode)
        {
            return ScoreWith<WeightedRatioScorer>(input1, input2, preprocessMode);
        }
        #endregion
    }
}
