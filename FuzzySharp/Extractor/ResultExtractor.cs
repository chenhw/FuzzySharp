using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp.Extensions;
using FuzzySharp.SimilarityRatio.Scorer;

namespace FuzzySharp.Extractor
{
    public static class ResultExtractor
    {
        public static IEnumerable<ExtractedResult<T>> ExtractWithoutOrder<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            ArgumentNullException.ThrowIfNull(scorer);

            return ExtractWithoutOrderIterator(query, choices, processor, scorer, cutoff);
        }

        private static IEnumerable<ExtractedResult<T>> ExtractWithoutOrderIterator<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer scorer, int cutoff)
        {
            int index = 0;
            var processedQuery = processor(query);
            foreach (var choice in choices)
            {
                int score = scorer.Score(processedQuery, processor(choice));
                if (score >= cutoff)
                {
                    yield return new ExtractedResult<T>(choice, score, index);
                }

                index++;
            }
        }

        public static ExtractedResult<T> ExtractOne<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(calculator);
            using var enumerator = ExtractWithoutOrder(query, choices, processor, calculator, cutoff).GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new InvalidOperationException("Sequence contains no matching elements.");
            }

            var best = enumerator.Current;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Score > best.Score)
                {
                    best = enumerator.Current;
                }
            }

            return best;
        }

        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(calculator);
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff).OrderByDescending(r => r.Score);
        }

        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(T query, IEnumerable<T> choices, Func<T, string> processor, IRatioScorer calculator, int limit, int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(calculator);
            return ExtractWithoutOrder(query, choices, processor, calculator, cutoff).MaxN(limit).Reverse();
        }

        public static IAsyncEnumerable<ExtractedResult<T>> ExtractWithoutOrderAsync<T>(
            T query,
            IAsyncEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer,
            int cutoff = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            ArgumentNullException.ThrowIfNull(scorer);

            return ExtractWithoutOrderAsyncIterator(query, choices, processor, scorer, cutoff, cancellationToken);
        }

        private static async IAsyncEnumerable<ExtractedResult<T>> ExtractWithoutOrderAsyncIterator<T>(
            T query,
            IAsyncEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer,
            int cutoff,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            int index = 0;
            var processedQuery = processor(query);

            await foreach (var choice in choices.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                int score = scorer.Score(processedQuery, processor(choice));
                if (score >= cutoff)
                {
                    yield return new ExtractedResult<T>(choice, score, index);
                }

                index++;
            }
        }

        public static async ValueTask<ExtractedResult<T>> ExtractOneAsync<T>(
            T query,
            IAsyncEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer scorer,
            int cutoff = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            ArgumentNullException.ThrowIfNull(scorer);

            ExtractedResult<T>? best = null;

            await foreach (var result in ExtractWithoutOrderAsync(query, choices, processor, scorer, cutoff, cancellationToken).ConfigureAwait(false))
            {
                if (best is null || result.Score > best.Score)
                {
                    best = result;
                }
            }

            if (best is null)
            {
                throw new InvalidOperationException("Sequence contains no matching elements.");
            }

            return best;
        }
    }
}
