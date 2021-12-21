namespace AoC.Day21;

public static class Day21Part2Solver
{
    public static long SolvePart2(int p1Start, int p2Start)
    {
        const int goal = 21;
        var p1Wins = 0L;
        var p2Wins = 0L;

        var universeAggregates = new[] {new UniverseAggregate(p1Start, 0, p2Start, 0, 1)};

        while (universeAggregates.Any())
        {
            var newUniverseAggregates = GetResultUniverses(universeAggregates);

            // Filter out winners
            p1Wins += newUniverseAggregates.Where(x => x.P1Score >= goal).Sum(x => x.NumOfUniverses);
            p1Wins += newUniverseAggregates.Where(x => x.P2Score >= goal).Sum(x => x.NumOfUniverses);

            universeAggregates = newUniverseAggregates.Where(x => x.P1Score < goal && x.P2Score < goal).ToArray();
        }

        return Math.Max(p1Wins, p2Wins);
    }

    public record UniverseAggregate(int P1Position, int P1Score, int P2Position, int P2Score, long NumOfUniverses);

    public record Result(int p1EndPos, int p1EndScore, int p2EndPos, int p2EndScore, long startNumUniverses);

    private static readonly int[] nums = new[] { 1, 2, 3 };

    public static IReadOnlyCollection<UniverseAggregate> GetResultUniverses(IReadOnlyCollection<UniverseAggregate> universeAggregates)
    {
        return universeAggregates
            .SelectMany(a => GetResults(a.P1Position, a.P1Score, a.P2Position, a.P2Score, a.NumOfUniverses))
            .GroupBy(x => new { x.p1EndPos, x.p1EndScore, x.p2EndPos, x.p2EndScore, x.startNumUniverses })
            .Select(grp => new UniverseAggregate(grp.Key.p1EndPos, grp.Key.p1EndScore, grp.Key.p2EndPos, grp.Key.p2EndScore, grp.Key.startNumUniverses * grp.Count()))
            .ToArray();
    }

    public static IEnumerable<Result> GetResults(int p1StartPos, int p1StartScore, int p2StartPos, int p2StartScore, long startNumUniverses)
    {
        return nums.SelectMany(
            n1 => nums.SelectMany(
                n2 => nums.Select(n3 =>
                {
                    var nsum = n1 + n2 + n3;

                    var p1EndPos = (p1StartPos + nsum) % 10;
                    p1EndPos = p1EndPos == 0 ? 10 : p1EndPos;

                    var p2EndPos = (p2StartPos + nsum) % 10;
                    p2EndPos = p2EndPos == 0 ? 10 : p2EndPos;

                    return new Result
                    (
                        p1EndPos,
                        p1StartScore + p1EndPos,
                        p2EndPos,
                        p2StartScore + p2EndPos,
                        startNumUniverses * 2
                    );
                })));
    }
}
