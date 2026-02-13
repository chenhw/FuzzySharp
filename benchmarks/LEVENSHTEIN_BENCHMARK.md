# Levenshtein Rewrite Benchmark (.NET 10)

Date: 2026-02-13  
Machine: Intel Core i9-14900HX, Windows 11  
Runtime: .NET 10.0.2  
Tool: BenchmarkDotNet 0.14.0

## Run command

```bash
dotnet run -c Release --project benchmarks/FuzzySharp.Benchmarks/FuzzySharp.Benchmarks.csproj -- --filter "*LevenshteinBenchmarks*"
```

## Key results (legacy vs rewritten)

`Legacy_*` is the pre-rewrite implementation copied into benchmark baseline.  
`New_*` is the current `FuzzySharp.Levenshtein`.

| Method | Size | Legacy Mean | New Mean | Relative | Legacy Alloc | New Alloc |
|---|---:|---:|---:|---:|---:|---:|
| EditDistance(xcost=1) | 16 | 215.5 ns | 247.8 ns | +15% | 240 B | 0 B |
| EditDistance(xcost=1) | 64 | 2,870.5 ns | 3,036.2 ns | +6% | 624 B | 0 B |
| EditDistance(xcost=1) | 256 | 75,128.5 ns | 74,699.3 ns | -1% | 2,160 B | 0 B |
| EditDistance(xcost=1) | 512 | 333,809.1 ns | 345,386.5 ns | +3% | 4,208 B | 0 B |
| GetRatio | 16 | 270.5 ns | 246.4 ns | -9% | 240 B | 0 B |
| GetRatio | 64 | 2,427.5 ns | 3,280.1 ns | +35% | 624 B | 0 B |
| GetRatio | 256 | 71,452.0 ns | 72,540.7 ns | +2% | 2,160 B | 0 B |
| GetRatio | 512 | 334,965.0 ns | 338,847.6 ns | +1% | 4,208 B | 0 B |

## Interpretation

- The rewrite consistently removes per-call managed allocations (`string.ToCharArray()` and row array allocations).
- Throughput is mixed on this machine/profile:
  - Similar performance for medium/large inputs (`256+`), with small wins/losses depending on method.
  - Some small-input scenarios regress due to different loop shape and span-based path overhead.
- The rewrite primarily targets **AOT safety + allocation elimination**; if required, a second pass can specialize short-string paths for raw latency.

## Full report location

- `BenchmarkDotNet.Artifacts/results/FuzzySharp.Benchmarks.LevenshteinBenchmarks-report-github.md`
- `BenchmarkDotNet.Artifacts/FuzzySharp.Benchmarks.LevenshteinBenchmarks-*.log`
