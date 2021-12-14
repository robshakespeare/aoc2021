using System.Text;
using static System.Environment;

namespace AoC.Day14;

public class Day14Solver : SolverBase
{
    public override string DayName => "Extended Polymerization";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (polymerTemplate, pairInsertionRules) = Parse(input);

        for (var step = 1; step <= 10; step++)
        {
            polymerTemplate = Step(step, polymerTemplate, pairInsertionRules);
        }

        var groups = polymerTemplate.GroupBy(c => c).ToArray();

        var mostCommonElement = groups.MaxBy(x => x.Count()) ?? throw new InvalidOperationException("max not possible, no elements");
        var leastCommonElement = groups.MinBy(x => x.Count()) ?? throw new InvalidOperationException("min not possible, no elements");

        return mostCommonElement.LongCount() - leastCommonElement.LongCount();
    }

    private static string Step(int step, string polymerTemplate, Dictionary<MatchTuple, PairInsertionRule> pairInsertionRules)
    {
        var result = new StringBuilder();

        for (var i = 1; i < polymerTemplate.Length; i++)
        {
            var charA = polymerTemplate[i - 1];
            var charB = polymerTemplate[i];

            var insertionRule = pairInsertionRules[new MatchTuple(charA, charB)];

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

        for (var step = 1; step <= 15; step++)
        {
            polymerTemplate = Step(step, polymerTemplate, pairInsertionRules);
        }

        var groups = polymerTemplate.GroupBy(c => c).ToArray();

        var mostCommonElement = groups.MaxBy(x => x.Count()) ?? throw new InvalidOperationException("max not possible, no elements");
        var leastCommonElement = groups.MinBy(x => x.Count()) ?? throw new InvalidOperationException("min not possible, no elements");

        return mostCommonElement.LongCount() - leastCommonElement.LongCount();
    }

    private static readonly Regex ParseRuleRegex = new(@"(?<matchA>.)(?<matchB>.) -> (?<insertionChar>.+)");

    public static (string polymerTemplate, Dictionary<MatchTuple, PairInsertionRule> pairInsertionRules) Parse(PuzzleInput input)
    {
        var parts = input.ToString().Split($"{NewLine}{NewLine}");
        var polymerTemplate = parts[0];

        var pairInsertionRules = ParseRuleRegex.Matches(parts[1]).Select(m => new PairInsertionRule(
            m.Groups["matchA"].Value[0],
            m.Groups["matchB"].Value[0],
            m.Groups["insertionChar"].Value[0])).ToDictionary(x => x.MatchTuple);

        return (polymerTemplate, pairInsertionRules);
    }

    public record PairInsertionRule(char MatchA, char MatchB, char InsertionChar)
    {
        public MatchTuple MatchTuple { get; } = new(MatchA, MatchB);
    }

    public readonly record struct MatchTuple(char MatchA, char MatchB);
}
