using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp.Extractor;
using FuzzySharp.PreProcess;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer;
using FuzzySharp.SimilarityRatio.Scorer.Composite;

namespace FuzzySharp
{
    public static class Process
    {
        private static readonly IRatioScorer s_defaultScorer = ScorerCache.Get<WeightedRatioScorer>();
        private static readonly Func<string, string> s_defaultStringProcessor = StringPreprocessorFactory.GetPreprocessor(PreprocessMode.Full);

        #region ExtractAll
        /// <summary>
        /// Creates a list of ExtractedResult which contain all the choices with
        /// their corresponding score where higher is more similar
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static IEnumerable<ExtractedResult<string>> ExtractAll(
            string query, 
            IEnumerable<string> choices, 
            Func<string, string>? processor = null,
            IRatioScorer? scorer = null,
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            if (processor == null) processor = s_defaultStringProcessor;
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
        }

        /// <summary>
        /// Creates a list of ExtractedResult which contain all the choices with
        /// their corresponding score where higher is more similar
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static IEnumerable<ExtractedResult<T>> ExtractAll<T>(
            T query, 
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer? scorer = null,
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractWithoutOrder(query, choices, processor, scorer, cutoff);
        }
        #endregion

        #region ExtractTop
        /// <summary>
        /// Creates a sorted list of ExtractedResult  which contain the
        /// top limit most similar choices
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="limit">Maximum number of results to return.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static IEnumerable<ExtractedResult<string>> ExtractTop(
            string query,
            IEnumerable<string> choices,
            Func<string, string>? processor = null,
            IRatioScorer? scorer = null,
            int limit = 5,
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            if (processor == null) processor = s_defaultStringProcessor;
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
        }


        /// <summary>
        /// Creates a sorted list of ExtractedResult  which contain the
        /// top limit most similar choices
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="limit">Maximum number of results to return.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static IEnumerable<ExtractedResult<T>> ExtractTop<T>(
            T query, 
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer? scorer = null,
            int limit = 5, 
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractTop(query, choices, processor, scorer, limit, cutoff);
        }
        #endregion

        #region ExtractSorted
        /// <summary>
        /// Creates a sorted list of ExtractedResult with the closest matches first
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static IEnumerable<ExtractedResult<string>> ExtractSorted(
            string query,
            IEnumerable<string> choices,
            Func<string, string>? processor = null,
            IRatioScorer? scorer = null,
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            if (processor == null) processor = s_defaultStringProcessor;
            if (scorer == null) scorer       = s_defaultScorer;
            return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
        }

        /// <summary>
        /// Creates a sorted list of ExtractedResult with the closest matches first
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static IEnumerable<ExtractedResult<T>> ExtractSorted<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer? scorer = null,
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractSorted(query, choices, processor, scorer, cutoff);
        }
        #endregion

        #region ExtractOne
        /// <summary>
        /// Find the single best match above a score in a list of choices.
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static ExtractedResult<string> ExtractOne(
            string query, 
            IEnumerable<string> choices,
            Func<string, string>? processor = null,
            IRatioScorer? scorer = null,
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            if (processor == null) processor = s_defaultStringProcessor;
            if (scorer == null) scorer       = s_defaultScorer;
            return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
        }

        /// <summary>
        /// Find the single best match above a score in a list of choices.
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <param name="processor">Function projecting a choice to the compared string.</param>
        /// <param name="scorer">Scoring strategy used to compute similarity.</param>
        /// <param name="cutoff">Minimum score threshold to include a result.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static ExtractedResult<T> ExtractOne<T>(
            T query,
            IEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer? scorer = null,
            int cutoff = 0)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            if (scorer == null) scorer       = s_defaultScorer;
            return ResultExtractor.ExtractOne(query, choices, processor, scorer, cutoff);
        }

        /// <summary>
        /// Find the single best match above a score in a list of choices.
        /// </summary>
        /// <param name="query">Query value to match against choices.</param>
        /// <param name="choices">Candidate choices to evaluate.</param>
        /// <returns>Scored extraction results for the requested operation.</returns>
        public static ExtractedResult<string> ExtractOne(string query, params string[] choices)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            return ResultExtractor.ExtractOne(query, choices, s_defaultStringProcessor, s_defaultScorer);
        }

        /// <summary>
        /// Asynchronously evaluates all choices and returns those meeting the cutoff score.
        /// </summary>
        /// <param name="query">Query string to match.</param>
        /// <param name="choices">Asynchronous choice sequence.</param>
        /// <param name="processor">Optional string preprocessor.</param>
        /// <param name="scorer">Optional scorer implementation.</param>
        /// <param name="cutoff">Minimum score threshold.</param>
        /// <param name="cancellationToken">Cancellation token for asynchronous enumeration.</param>
        /// <returns>Asynchronously produced scored matches.</returns>
        public static IAsyncEnumerable<ExtractedResult<string>> ExtractAllAsync(
            string query,
            IAsyncEnumerable<string> choices,
            Func<string, string>? processor = null,
            IRatioScorer? scorer = null,
            int cutoff = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            if (processor == null) processor = s_defaultStringProcessor;
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractWithoutOrderAsync(query, choices, processor, scorer, cutoff, cancellationToken);
        }

        /// <summary>
        /// Asynchronously evaluates all choices and returns those meeting the cutoff score.
        /// </summary>
        /// <typeparam name="T">Choice element type.</typeparam>
        /// <param name="query">Query item to match.</param>
        /// <param name="choices">Asynchronous choice sequence.</param>
        /// <param name="processor">Projection from choice to compared string.</param>
        /// <param name="scorer">Optional scorer implementation.</param>
        /// <param name="cutoff">Minimum score threshold.</param>
        /// <param name="cancellationToken">Cancellation token for asynchronous enumeration.</param>
        /// <returns>Asynchronously produced scored matches.</returns>
        public static IAsyncEnumerable<ExtractedResult<T>> ExtractAllAsync<T>(
            T query,
            IAsyncEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer? scorer = null,
            int cutoff = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractWithoutOrderAsync(query, choices, processor, scorer, cutoff, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the best match or throws if no matches satisfy the cutoff.
        /// </summary>
        /// <param name="query">Query string to match.</param>
        /// <param name="choices">Asynchronous choice sequence.</param>
        /// <param name="processor">Optional string preprocessor.</param>
        /// <param name="scorer">Optional scorer implementation.</param>
        /// <param name="cutoff">Minimum score threshold.</param>
        /// <param name="cancellationToken">Cancellation token for asynchronous enumeration.</param>
        /// <returns>The highest-scoring match.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no choice meets the cutoff.</exception>
        public static ValueTask<ExtractedResult<string>> ExtractOneAsync(
            string query,
            IAsyncEnumerable<string> choices,
            Func<string, string>? processor = null,
            IRatioScorer? scorer = null,
            int cutoff = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            if (processor == null) processor = s_defaultStringProcessor;
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractOneAsync(query, choices, processor, scorer, cutoff, cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the best match or throws if no matches satisfy the cutoff.
        /// </summary>
        /// <typeparam name="T">Choice element type.</typeparam>
        /// <param name="query">Query item to match.</param>
        /// <param name="choices">Asynchronous choice sequence.</param>
        /// <param name="processor">Projection from choice to compared string.</param>
        /// <param name="scorer">Optional scorer implementation.</param>
        /// <param name="cutoff">Minimum score threshold.</param>
        /// <param name="cancellationToken">Cancellation token for asynchronous enumeration.</param>
        /// <returns>The highest-scoring match.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no choice meets the cutoff.</exception>
        public static ValueTask<ExtractedResult<T>> ExtractOneAsync<T>(
            T query,
            IAsyncEnumerable<T> choices,
            Func<T, string> processor,
            IRatioScorer? scorer = null,
            int cutoff = 0,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);
            ArgumentNullException.ThrowIfNull(choices);
            ArgumentNullException.ThrowIfNull(processor);
            if (scorer == null) scorer = s_defaultScorer;
            return ResultExtractor.ExtractOneAsync(query, choices, processor, scorer, cutoff, cancellationToken);
        }
        #endregion
    }
}
