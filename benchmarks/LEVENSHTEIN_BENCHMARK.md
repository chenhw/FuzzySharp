# Levenshtein Rewrite Benchmark (.NET 10)

Date: 2026-02-13  
Machine: Intel Core i9-14900HX, Windows 11  
Runtime: .NET 10.0.2  
Tool: BenchmarkDotNet 0.14.0

## Run command

```bash
dotnet run -c Release --project benchmarks/FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj -- --filter "*LevenshteinBenchmarks*"
```

## Key results (legacy vs rewritten, short strings <= 64)

`Legacy_*` is the pre-rewrite implementation copied into benchmark baseline.  
`New_*` is the current `FuzzySharp.Levenshtein`.

| Method | Size | Legacy Mean | New Mean | Relative | Legacy Alloc | New Alloc |
|---|---:|---:|---:|---:|---:|---:|
| EditDistance(xcost=1) | 8 | 82.11 ns | 70.60 ns | -14% | 176 B | 0 B |
| EditDistance(xcost=1) | 16 | 217.26 ns | 215.45 ns | -1% | 240 B | 0 B |
| EditDistance(xcost=1) | 32 | 764.73 ns | 679.12 ns | -11% | 368 B | 0 B |
| EditDistance(xcost=1) | 64 | 2,446.71 ns | 2,381.84 ns | -3% | 624 B | 0 B |
| GetRatio | 8 | 84.14 ns | 71.44 ns | -15% | 176 B | 0 B |
| GetRatio | 16 | 239.30 ns | 215.15 ns | -10% | 240 B | 0 B |
| GetRatio | 32 | 709.89 ns | 804.74 ns | +13% | 368 B | 0 B |
| GetRatio | 64 | 2,336.71 ns | 2,242.99 ns | -4% | 624 B | 0 B |

## Interpretation

- The rewrite consistently removes per-call managed allocations (`string.ToCharArray()` and row array allocations).
- With the new short-string fast path (`<=64`), most short-input scenarios improved latency while keeping 0 allocations.
- One case (`GetRatio`, size 32) remains slightly slower in this run; this is a candidate for another micro-optimization pass if needed.

## Full report location

- `BenchmarkDotNet.Artifacts/results/FuzzySharp.Benchmarks.LevenshteinBenchmarks-report-github.md`
- `BenchmarkDotNet.Artifacts/FuzzySharp.Benchmarks.LevenshteinBenchmarks-*.log`
