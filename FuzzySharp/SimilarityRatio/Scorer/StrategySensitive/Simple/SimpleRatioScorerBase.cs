using System;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
    public abstract class SimpleRatioScorerBase : StrategySensitiveScorerBase
    {
        public override int Score(string input1, string input2)
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);
            return Scorer(input1, input2);
        }
    }
}
