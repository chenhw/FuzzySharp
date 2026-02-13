# FuzzySharp Modernization Status (.NET 10)

- Last updated: 2026-02-13
- Scope: `FuzzySharp` library + tests + CI

## Completed

### Platform and packaging
- Migrated library and tests to `net10.0`
- Upgraded key dependencies to latest stable versions
- Upgraded `development_package.yml` and `master_package_and_publish.yml` to `actions/*@v4` + `.NET 10.0.x`
- Added AOT smoke publish step in CI workflows
- Added sample AOT host: `samples/FuzzySharp.AotSmoke`

### AOT and compatibility hardening
- Enabled `<IsAotCompatible>`, `<IsTrimmable>`, `<EnableTrimAnalyzer>`, `<EnableAotAnalyzer>` in `FuzzySharp.csproj`
- Added null guards (`ArgumentNullException.ThrowIfNull`) across public matching entry points
- Added runtime invariant checks in `Levenshtein` where previous behavior relied on `Debug.Assert`
- Added overflow guard for large matrix allocation in `Levenshtein` edit-op cost matrix

### Performance-oriented refactors
- Replaced regex-heavy preprocessing with `String.Create` + `SearchValues<char>`
- Added `StringTokenization` for whitespace and ASCII-letter token scanning
- Migrated token-based scorers away from runtime regex splitting/matching
- Optimized scorer cache creation (`GetOrAdd` with lazy factory)
- Removed temporary list allocations in partial-ratio strategies by tracking `max` directly
- Rewrote `Levenshtein` hot paths (`EditDistance` / `GetRatio`) to use `ReadOnlySpan<T>` with `stackalloc` + `ArrayPool<int>` row buffers
- Added benchmark suite (`BenchmarkDotNet`) comparing legacy and rewritten Levenshtein paths

### API modernization
- Added async extraction APIs (`ExtractAllAsync`, `ExtractOneAsync`)
- Unified sync/async `ExtractOne*` behavior: both throw `InvalidOperationException` when no match satisfies cutoff

### Tests
- NUnit 4 migration with `ClassicAssert` compatibility
- Added async API tests
- Added null-guard tests for `Fuzz`, `Process`, and `Levenshtein` string APIs
- Added direct `StringTokenization` unit tests

## Remaining (optional / next phase)

### Code quality follow-up
- Improve documentation depth for all XML comments (currently materially better than baseline, but still not exhaustive)
- Consider converting tiny mutation-heavy DTO-like types (`EditOp`, `MatchingBlock`, `OpCode`) to immutable `readonly record struct` in a future major cycle

## Release recommendation
- Version line should be `3.0.0` because target framework contraction to `net10.0` is a breaking change.
