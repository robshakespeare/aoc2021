namespace AoC.Day14;

public class Day14Solver : SolverBase
{
    public override string DayName => "Extended Polymerization";

    public override long? SolvePart1(PuzzleInput input) => Solve(input, 10);

    public override long? SolvePart2(PuzzleInput input) => Solve(input, 40);

    private static long? Solve(PuzzleInput input, int numOfSteps)
    {
        var (polymerTemplate, pairInsertionRules) = Parse(input);

        IReadOnlyDictionary<string, long> pairCounts =
            ToInitialPairs(polymerTemplate).GroupBy(pair => pair).ToDictionary(grp => grp.Key, grp => grp.LongCount());

        for (var step = 1; step <= numOfSteps; step++)
        {
            pairCounts = Step(pairCounts, pairInsertionRules);
        }

        // Resolve the final pair counts in to count of characters
        // As a whole string, each second char of a pair is always the first char in the next pair, so we only need to count the first character in each pair
        // And then add the last character in, because that never has a pair

        var charCounts = pairCounts
            .GroupBy(pairCount => pairCount.Key[0])
            .Select(grp => new
            {
                chr = grp.Key,
                count = grp.Sum(pairCount => pairCount.Value)
            })
            .ToDictionary(x => x.chr, x => x.count);

        charCounts.AddOrIncrement(polymerTemplate.Last(), 1);

        var mostCommonElement = charCounts.MaxBy(x => x.Value);
        var leastCommonElement = charCounts.MinBy(x => x.Value);

        return mostCommonElement.Value - leastCommonElement.Value;
    }

    private static IEnumerable<string> ToInitialPairs(string polymerTemplate)
    {
        for (var i = 1; i < polymerTemplate.Length; i++)
        {
            var charA = polymerTemplate[i - 1];
            var charB = polymerTemplate[i];

            yield return $"{charA}{charB}";
        }
    }

    private static IReadOnlyDictionary<string, long> Step(
        IReadOnlyDictionary<string, long> pairCounts,
        IReadOnlyDictionary<string, PairInsertionRule> pairInsertionRules)
    {
        var newPairCounts = new Dictionary<string, long>();

        var resultPairCounts = pairCounts.Select(pairCount => new
        {
            pairInsertionRules[pairCount.Key].ResultPairs,
            Count = pairCount.Value
        });

        foreach (var resultPairCount in resultPairCounts)
        {
            newPairCounts.AddOrIncrement(resultPairCount.ResultPairs.Pair1, resultPairCount.Count);
            newPairCounts.AddOrIncrement(resultPairCount.ResultPairs.Pair2, resultPairCount.Count);
        }

        return newPairCounts;
    }

    private static readonly Regex ParseRuleRegex = new(@"(?<pair>.+) -> (?<insertionChar>.+)");

    public static (string polymerTemplate, IReadOnlyDictionary<string, PairInsertionRule> pairInsertionRules) Parse(PuzzleInput input)
    {
        var polymerTemplate = input.ReadLines().First();

        var pairInsertionRules = ParseRuleRegex.Matches(input.ToString()).Select(m => new PairInsertionRule(
            m.Groups["pair"].Value,
            m.Groups["insertionChar"].Value[0])).ToDictionary(x => x.Pair);

        return (polymerTemplate, pairInsertionRules);
    }

    public record PairInsertionRule(string Pair, char InsertionChar)
    {
        public (string Pair1, string Pair2) ResultPairs { get; } = ($"{Pair[0]}{InsertionChar}", $"{InsertionChar}{Pair[1]}");
    }
}
