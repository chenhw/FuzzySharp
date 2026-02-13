using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp.PreProcess;
using FuzzySharp.Utils;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
    public abstract class TokenAbbreviationScorerBase : StrategySensitiveScorerBase
    {
        public override int Score(string input1, string input2)
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);
            string shorter;
            string longer;

            if (input1.Length < input2.Length)
            {
                shorter = input1;
                longer  = input2;
            }
            else
            {
                shorter = input2;
                longer  = input1;
            }

            if (shorter.Length == 0)
            {
                return 0;
            }

            double lenRatio = ((double)longer.Length) / shorter.Length;

            // if longer isn't at least 1.5 times longer than the other, then its probably not an abbreviation
            if (lenRatio < 1.5) return 0;

            // numbers can't be abbreviations for other numbers, though that would be hilarious. "Yes, 4 - as in 4,238"
            var tokensLonger = StringTokenization.SplitOnAsciiLetterRuns(longer);
            var tokensShorter = StringTokenization.SplitOnAsciiLetterRuns(shorter);

            // more than 4 tokens and it's probably not an abbreviation (and could get costly)
            if (tokensShorter.Length > 4)
            {
                return 0;
            }

            string[] moreTokens;
            string[] fewerTokens;

            if (tokensLonger.Length > tokensShorter.Length)
            {
                moreTokens = tokensLonger;
                fewerTokens = tokensShorter;
            }
            else
            {
                moreTokens = tokensShorter;
                fewerTokens = tokensLonger;
            }

            if (fewerTokens.Length == 0)
            {
                return 0;
            }

            var allPermutations = moreTokens.PermutationsOfSize(fewerTokens.Length);

            List<int> allScores = new List<int>();
            foreach (var permutation in allPermutations)
            {
                double sum = 0;
                for (int i = 0; i < fewerTokens.Length; i++)
                {
                    var i1 = permutation[i];
                    var i2 = fewerTokens[i];
                    if (StringContainsInOrder(i1, i2)) // all chars of i2 appear in order within i1
                    {
                        var score = Scorer(i1, i2);
                        sum += score;
                    }
                }
                allScores.Add((int) (sum / fewerTokens.Length));
            }
            
            return allScores.Count==0?0:allScores.Max();
        }

        /// <summary>
        /// Does s2 have all its characters appear in order in s1? (Basically, is s2 a potential abbreviation of s1?)
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private bool StringContainsInOrder(string s1, string s2)
        {
            if (s1.Length < s2.Length) return false;
            int s2_idx = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                if (s2[s2_idx] == s1[i])
                    s2_idx++;
                if (s2_idx == s2.Length)
                    return true;
                if (i + s2.Length - s2_idx == s1.Length)
                    return false;
            }
            return false;
        }
    }
}
