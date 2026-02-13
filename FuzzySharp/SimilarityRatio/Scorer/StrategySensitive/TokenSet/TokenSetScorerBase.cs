using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp.PreProcess;

namespace FuzzySharp.SimilarityRatio.Scorer.StrategySensitive
{
    public abstract class TokenSetScorerBase : StrategySensitiveScorerBase
    {
        public override int Score(string input1, string input2)
        {
            var tokens1 = new HashSet<string>(StringTokenization.SplitOnWhitespace(input1));
            var tokens2 = new HashSet<string>(StringTokenization.SplitOnWhitespace(input2));

            var sortedIntersection = string.Join(" ", tokens1.Intersect(tokens2).OrderBy(s => s)).Trim();
            var sortedDiff1To2     = (sortedIntersection + " " + string.Join(" ", tokens1.Except(tokens2).OrderBy(s => s))).Trim();
            var sortedDiff2To1     = (sortedIntersection + " " + string.Join(" ", tokens2.Except(tokens1).OrderBy(s => s))).Trim();

            return new[]
            {
                Scorer(sortedIntersection, sortedDiff1To2),
                Scorer(sortedIntersection, sortedDiff2To1),
                Scorer(sortedDiff1To2,     sortedDiff2To1)
            }.Max();
        }
    }
}
