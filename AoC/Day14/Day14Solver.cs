using System.Text;
using static System.Environment;

namespace AoC.Day14;

public class Day14Solver : SolverBase
{
    public override string DayName => "Extended Polymerization";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (polymerTemplate, pairInsertionRules) = Parse(input);

        var pairs = ToInitialPairs(polymerTemplate).ToArray();

        IReadOnlyDictionary<string, long> pairCounts = pairs.GroupBy(pair => pair).ToDictionary(grp => grp.Key, grp =>  grp.LongCount());
        //.ToDictionary(pair => pair, _ => 1L);

        for (var step = 1; step <= 10; step++)
        {
            pairCounts = Step(pairCounts, pairInsertionRules);
        }

        // As a whole string, each second char of a pair is always the first char in the next pair, so we only need to count the first character in each pair
        // And then add the last character in, because that never has a pair

        var resolvedPairCounts = pairCounts
            .GroupBy(x => x.Key[0])
            .Select(x => new
            {
                chr = x.Key,
                count = x.Sum(y => y.Value)
            }).ToDictionary(x => x.chr, x => x.count);

        resolvedPairCounts[polymerTemplate.Last()] += 1;

        var mostCommonElement = resolvedPairCounts.MaxBy(x => x.Value); //?? throw new InvalidOperationException("max not possible, no elements");
        var leastCommonElement = resolvedPairCounts.MinBy(x => x.Value); //?? throw new InvalidOperationException("min not possible, no elements");

        return mostCommonElement.Value - leastCommonElement.Value;

        //var mostCommonElement = pairCounts.MaxBy(x => x.Value);
        //var leastCommonElement = pairCounts.MinBy(x => x.Value);

        //return mostCommonElement.Value - leastCommonElement.Value;

        //var emptyPairCountDictionary = ToEmptyPairCountDictionary(pairInsertionRules.Keys);

        //for (var step = 1; step <= 10; step++)
        //{
        //    polymerTemplate = Step(step, polymerTemplate, pairInsertionRules);
        //}

        //var groups = polymerTemplate.GroupBy(c => c).ToArray();

        //var mostCommonElement = groups.MaxBy(x => x.Count()) ?? throw new InvalidOperationException("max not possible, no elements");
        //var leastCommonElement = groups.MinBy(x => x.Count()) ?? throw new InvalidOperationException("min not possible, no elements");

        //return mostCommonElement.LongCount() - leastCommonElement.LongCount();
    }

    private static IEnumerable<string> ToInitialPairs(string polymerTemplate)
    {
        for (var i = 1; i < polymerTemplate.Length; i++)
        {
            var charA = polymerTemplate[i - 1];
            var charB = polymerTemplate[i];

            yield return new string(new [] { charA, charB }); // rs-todo: how to create a span of the 2 chars??
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
            var pair1 = resultPairCount.ResultPairs.Pair1;
            var pair2 = resultPairCount.ResultPairs.Pair2;

            if (!newPairCounts.ContainsKey(pair1))
            {
                newPairCounts[pair1] = 0;
            }

            if (!newPairCounts.ContainsKey(pair2))
            {
                newPairCounts[pair2] = 0;
            }

            newPairCounts[pair1] += resultPairCount.Count;
            newPairCounts[pair2] += resultPairCount.Count;
        }

        return newPairCounts;
    }

    //private static IReadOnlyDictionary<MatchTuple, long> ToEmptyPairCountDictionary(IEnumerable<MatchTuple> pairs) => pairs.ToDictionary(pair => pair, _ => 0L);

    //private static string Step2(int step, string polymerTemplate, Dictionary<MatchTuple, PairInsertionRule> pairInsertionRules)
    //{
    //    return "rs-todo";
    //}

    private static string Step(int step, string polymerTemplate, IReadOnlyDictionary<string, PairInsertionRule> pairInsertionRules)
    {
        var result = new StringBuilder();

        for (var i = 1; i < polymerTemplate.Length; i++)
        {
            var charA = polymerTemplate[i - 1];
            var charB = polymerTemplate[i];

            var insertionRule = pairInsertionRules[$"{charA}{charB}"];

            if (result.Length == 0)
            {
                result.Append(charA);
            }

            result.Append(insertionRule.InsertionChar);
            result.Append(charB);

            //if (pairInsertionRules.TryGetValue(new MatchTuple(charA, charB), out var insertionRule))
            //{
            //}
        }

        var resultStr = result.ToString();

        Console.WriteLine($"Step {step,2} complete: length: {result.Length,6}: {resultStr}");

        var groups = polymerTemplate.GroupBy(c => c).ToArray();

        var mostCommonElement = groups.MaxBy(x => x.Count()) ?? throw new InvalidOperationException("max not possible, no elements");
        var leastCommonElement = groups.MinBy(x => x.Count()) ?? throw new InvalidOperationException("min not possible, no elements");

        Console.WriteLine($"mostCommonElement: {mostCommonElement.Key}: {mostCommonElement.Count()}");
        Console.WriteLine($"leastCommonElement: {leastCommonElement.Key}: {leastCommonElement.Count()}");

        Console.WriteLine();

        return resultStr;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var (polymerTemplate, pairInsertionRules) = Parse(input);

        var pairs = ToInitialPairs(polymerTemplate).ToArray();

        IReadOnlyDictionary<string, long> pairCounts = pairs.GroupBy(pair => pair).ToDictionary(grp => grp.Key, grp => grp.LongCount());
        //.ToDictionary(pair => pair, _ => 1L);

        for (var step = 1; step <= 40; step++)
        {
            pairCounts = Step(pairCounts, pairInsertionRules);
        }

        // As a whole string, each second char of a pair is always the first char in the next pair, so we only need to count the first character in each pair
        // And then add the last character in, because that never has a pair

        var resolvedPairCounts = pairCounts
            .GroupBy(x => x.Key[0])
            .Select(x => new
            {
                chr = x.Key,
                count = x.Sum(y => y.Value)
            }).ToDictionary(x => x.chr, x => x.count);

        resolvedPairCounts[polymerTemplate.Last()] += 1;

        var mostCommonElement = resolvedPairCounts.MaxBy(x => x.Value); //?? throw new InvalidOperationException("max not possible, no elements");
        var leastCommonElement = resolvedPairCounts.MinBy(x => x.Value); //?? throw new InvalidOperationException("min not possible, no elements");

        return mostCommonElement.Value - leastCommonElement.Value;

        //throw new NotImplementedException();

        //var (polymerTemplate, pairInsertionRules) = Parse(input);

        //for (var step = 1; step <= 15; step++)
        //{
        //    polymerTemplate = Step(step, polymerTemplate, pairInsertionRules);
        //}

        //var groups = polymerTemplate.GroupBy(c => c).ToArray();

        //var mostCommonElement = groups.MaxBy(x => x.Count()) ?? throw new InvalidOperationException("max not possible, no elements");
        //var leastCommonElement = groups.MinBy(x => x.Count()) ?? throw new InvalidOperationException("min not possible, no elements");

        //return mostCommonElement.LongCount() - leastCommonElement.LongCount();
    }

    private static readonly Regex ParseRuleRegex = new(@"(?<pair>.+) -> (?<insertionChar>.+)");

    public static (string polymerTemplate, IReadOnlyDictionary<string, PairInsertionRule> pairInsertionRules) Parse(PuzzleInput input)
    {
        var parts = input.ToString().Split($"{NewLine}{NewLine}");
        var polymerTemplate = parts[0];

        var pairInsertionRules = ParseRuleRegex.Matches(parts[1]).Select(m => new PairInsertionRule(
            m.Groups["pair"].Value,
            m.Groups["insertionChar"].Value[0])).ToDictionary(x => x.Pair);

        return (polymerTemplate, pairInsertionRules);
    }

    public record PairInsertionRule(string Pair, char InsertionChar)
    {
        //public MatchTuple MatchTuple { get; } = new(MatchA, MatchB);

        public (string Pair1, string Pair2) ResultPairs { get; } = ($"{Pair[0]}{InsertionChar}", $"{InsertionChar}{Pair[1]}");
    }

    //public readonly record struct MatchTuple(char MatchA, char MatchB);
}
