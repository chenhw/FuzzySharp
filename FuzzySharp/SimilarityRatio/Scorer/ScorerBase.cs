using FuzzySharp.PreProcess;
using System;

namespace FuzzySharp.SimilarityRatio.Scorer
{
    public abstract class ScorerBase : IRatioScorer
    {
        public abstract int Score(string input1, string input2);

        public int Score(string input1, string input2, PreprocessMode preprocessMode)
        {
            ArgumentNullException.ThrowIfNull(input1);
            ArgumentNullException.ThrowIfNull(input2);

            var preprocessor = StringPreprocessorFactory.GetPreprocessor(preprocessMode);
            input1 = preprocessor(input1);
            input2 = preprocessor(input2);
            return Score(input1, input2);
        }
    }
}
