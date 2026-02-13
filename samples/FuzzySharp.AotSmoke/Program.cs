using FuzzySharp;

var ratio = Fuzz.WeightedRatio("fuzzy sharp", "fuzzysharp");
var best = Process.ExtractOne("cowboys", "Dallas Cowboys", "New York Jets");

Console.WriteLine($"ratio={ratio}");
Console.WriteLine($"best={best.Value}:{best.Score}");
