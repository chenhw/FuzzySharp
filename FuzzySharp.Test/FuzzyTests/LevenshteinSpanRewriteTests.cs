using System;
using NUnit.Framework;

namespace FuzzySharp.Test.FuzzyTests
{
    [TestFixture]
    public class LevenshteinSpanRewriteTests
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz0123456789 ";

        [Test]
        public void EditDistance_String_MatchesReference_ForRandomInputs()
        {
            var random = new Random(20260213);

            for (int i = 0; i < 250; i++)
            {
                string left = NextRandomString(random, random.Next(0, 80));
                string right = NextRandomString(random, random.Next(0, 80));

                int expectedZeroCost = ReferenceEditDistance(left.AsSpan(), right.AsSpan(), 0);
                int expectedXCost = ReferenceEditDistance(left.AsSpan(), right.AsSpan(), 1);

                ClassicAssert.AreEqual(expectedZeroCost, Levenshtein.EditDistance(left, right, 0), $"Case #{i}, xcost=0");
                ClassicAssert.AreEqual(expectedXCost, Levenshtein.EditDistance(left, right, 1), $"Case #{i}, xcost=1");

                double expectedRatio = ReferenceRatio(left.AsSpan(), right.AsSpan());
                double actualRatio = Levenshtein.GetRatio(left, right);
                Assert.That(actualRatio, Is.EqualTo(expectedRatio).Within(1e-12), $"Case #{i}, ratio");
            }
        }

        [Test]
        public void EditDistance_String_MatchesReference_ForLargeInputs()
        {
            var random = new Random(707);

            for (int i = 0; i < 20; i++)
            {
                string left = NextRandomString(random, random.Next(280, 420));
                string right = NextRandomString(random, random.Next(280, 420));

                int expected = ReferenceEditDistance(left.AsSpan(), right.AsSpan(), 1);
                int actual = Levenshtein.EditDistance(left, right, 1);
                ClassicAssert.AreEqual(expected, actual, $"Large case #{i}");
            }
        }

        [Test]
        public void EditDistance_GenericArray_MatchesReference_ForRandomInputs()
        {
            var random = new Random(909);

            for (int i = 0; i < 120; i++)
            {
                int[] left = NextRandomIntArray(random, random.Next(0, 64));
                int[] right = NextRandomIntArray(random, random.Next(0, 64));

                int expectedZeroCost = ReferenceEditDistance(left.AsSpan(), right.AsSpan(), 0);
                int expectedXCost = ReferenceEditDistance(left.AsSpan(), right.AsSpan(), 1);

                ClassicAssert.AreEqual(expectedZeroCost, Levenshtein.EditDistance(left, right, 0), $"Generic case #{i}, xcost=0");
                ClassicAssert.AreEqual(expectedXCost, Levenshtein.EditDistance(left, right, 1), $"Generic case #{i}, xcost=1");
            }
        }

        private static string NextRandomString(Random random, int length)
        {
            var chars = new char[length];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = Alphabet[random.Next(Alphabet.Length)];
            }

            return new string(chars);
        }

        private static int[] NextRandomIntArray(Random random, int length)
        {
            var values = new int[length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = random.Next(0, 16);
            }

            return values;
        }

        private static int ReferenceEditDistance<T>(ReadOnlySpan<T> left, ReadOnlySpan<T> right, int xcost)
            where T : IEquatable<T>
        {
            int substitutionCost = xcost != 0 ? 2 : 1;

            if (left.Length == 0)
            {
                return right.Length;
            }

            if (right.Length == 0)
            {
                return left.Length;
            }

            var previous = new int[right.Length + 1];
            var current = new int[right.Length + 1];

            for (int j = 0; j <= right.Length; j++)
            {
                previous[j] = j;
            }

            for (int i = 1; i <= left.Length; i++)
            {
                current[0] = i;
                T leftValue = left[i - 1];

                for (int j = 1; j <= right.Length; j++)
                {
                    int insertion = current[j - 1] + 1;
                    int deletion = previous[j] + 1;
                    int substitution = previous[j - 1] + (leftValue.Equals(right[j - 1]) ? 0 : substitutionCost);

                    int best = insertion < deletion ? insertion : deletion;
                    if (substitution < best)
                    {
                        best = substitution;
                    }

                    current[j] = best;
                }

                (previous, current) = (current, previous);
            }

            return previous[right.Length];
        }

        private static double ReferenceRatio(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            int lensum = left.Length + right.Length;
            int editDistance = ReferenceEditDistance(left, right, 1);
            return editDistance == 0 ? 1 : (lensum - editDistance) / (double)lensum;
        }
    }
}
