using System;
using System.Linq;
using FuzzySharp.PreProcess;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
    public abstract class TokenSortScorerBase : StrategySensitiveScorerBase
    {
        public override int Score(string input1, string input2)
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);
            var sorted1 = string.Join(" ", StringTokenization.SplitOnWhitespace(input1).OrderBy(s => s)).Trim();
            var sorted2 = string.Join(" ", StringTokenization.SplitOnWhitespace(input2).OrderBy(s => s)).Trim();

            return Scorer(sorted1, sorted2);
        }
    }
}
