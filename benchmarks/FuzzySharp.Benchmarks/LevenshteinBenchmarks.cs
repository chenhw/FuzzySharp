using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace FuzzySharp.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(launchCount: 1, warmupCount: 5, iterationCount: 8)]
public class LevenshteinBenchmarks
{
    private string _left = string.Empty;
    private string _right = string.Empty;

    [Params(8, 16, 32, 64)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(20260213 + Size);
        _left = RandomText(random, Size);
        _right = RandomText(random, Size + 5);
    }

    [Benchmark(Baseline = true)]
    public int Legacy_EditDistance_XCost() => LegacyLevenshtein.EditDistance(_left, _right, 1);

    [Benchmark]
    public int New_EditDistance_XCost() => Levenshtein.EditDistance(_left, _right, 1);

    [Benchmark]
    public double Legacy_GetRatio() => LegacyLevenshtein.GetRatio(_left, _right);

    [Benchmark]
    public double New_GetRatio() => Levenshtein.GetRatio(_left, _right);

    private static string RandomText(Random random, int length)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789     ";
        var chars = new char[length];

        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = alphabet[random.Next(alphabet.Length)];
        }

        return new string(chars);
    }
}
