using System;
using FuzzySharp.PreProcess;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
    public abstract class TokenInitialismScorerBase : StrategySensitiveScorerBase
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

            // if longer isn't at least 3 times longer than the other, then it's probably not an initialism
            if (lenRatio < 3) return 0;

            var tokens = StringTokenization.SplitOnWhitespace(longer);
            if (tokens.Length == 0)
            {
                return 0;
            }

            var initials = new char[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                initials[i] = tokens[i][0];
            }

            return Scorer(new string(initials), shorter);
        }
    }
}
