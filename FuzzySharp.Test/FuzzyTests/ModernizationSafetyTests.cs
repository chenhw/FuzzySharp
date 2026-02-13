using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using FuzzySharp.PreProcess;
using NUnit.Framework;

namespace FuzzySharp.Test.FuzzyTests
{
    [TestFixture]
    public class ModernizationSafetyTests
    {
        [Test]
        public void StringTokenization_SplitOnWhitespace_HandlesMixedWhitespace()
        {
            var input = "  alpha\tbeta \r\n gamma  ";
            var tokens = StringTokenization.SplitOnWhitespace(input);

            CollectionAssert.AreEqual(new[] { "alpha", "beta", "gamma" }, tokens);
        }

        [Test]
        public void StringTokenization_SplitOnAsciiLetterRuns_ExtractsLettersOnly()
        {
            var input = "bl 420 baseline_section-OK 中文 test";
            var tokens = StringTokenization.SplitOnAsciiLetterRuns(input);

            CollectionAssert.AreEqual(new[] { "bl", "baseline", "section", "OK", "test" }, tokens);
        }

        [Test]
        public void Fuzz_NullInputs_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Fuzz.Ratio(null!, "abc"));
            Assert.Throws<ArgumentNullException>(() => Fuzz.Ratio("abc", null!));
            Assert.Throws<ArgumentNullException>(() => Fuzz.PartialRatio(null!, "abc"));
        }

        [Test]
        public void Process_NullInputs_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Process.ExtractAll(null!, new[] { "a", "b" }).ToList());
            Assert.Throws<ArgumentNullException>(() => Process.ExtractAll("a", (IEnumerable<string>)null!).ToList());
            Assert.Throws<ArgumentNullException>(() => Process.ExtractOne(null!, new[] { "a", "b" }));
        }

        [Test]
        public void Levenshtein_PublicStringApis_NullInputs_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Levenshtein.GetRatio(null!, "abc"));
            Assert.Throws<ArgumentNullException>(() => Levenshtein.GetMatchingBlocks("abc", null!));
            Assert.Throws<ArgumentNullException>(() => Levenshtein.EditDistance(null!, "abc"));
        }

        [Test]
        public async Task ExtractOneAsync_WhenNoMatch_ThrowsInvalidOperationException()
        {
            var choices = ToAsync(new[] { "alpha", "beta", "gamma" });

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                _ = await Process.ExtractOneAsync("zzz", choices, cutoff: 101);
            });

            ClassicAssert.AreEqual("Sequence contains no matching elements.", ex!.Message);
        }

        [Test]
        public void ExtractOne_WhenNoMatch_ThrowsInvalidOperationException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                _ = Process.ExtractOne("zzz", new[] { "alpha", "beta", "gamma" }, cutoff: 101);
            });

            ClassicAssert.AreEqual("Sequence contains no matching elements.", ex!.Message);
        }

        [Test]
        public async Task ExtractAllAsync_Cutoff_FiltersExpectedResults()
        {
            var choices = ToAsync(new[] { "dallas cowboys", "new york jets", "new york giants" });
            var results = new List<string>();

            await foreach (var item in Process.ExtractAllAsync("cowboys", choices, cutoff: 80))
            {
                results.Add(item.Value);
            }

            CollectionAssert.AreEquivalent(new[] { "dallas cowboys" }, results);
        }

        [Test]
        public void ExtractAllAsync_WhenCancelled_ThrowsOperationCanceledException()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            var choices = CancellableAsyncEnumerable(new[] { "a", "b", "c" }, cts.Token);

            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await foreach (var _ in Process.ExtractAllAsync("a", choices, cancellationToken: cts.Token))
                {
                }
            });
        }

        [Test]
        public void TokenAbbreviationRatio_WhenNoTokensOnShorterSide_ReturnsZero()
        {
            var score = Fuzz.TokenAbbreviationRatio("!!!", "baseline section 420");
            ClassicAssert.AreEqual(0, score);
        }

        private static async IAsyncEnumerable<string> ToAsync(IEnumerable<string> items, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
                await Task.Yield();
            }
        }

        private static async IAsyncEnumerable<string> CancellableAsyncEnumerable(IEnumerable<string> items, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
                await Task.Delay(5, cancellationToken);
            }
        }
    }
}
