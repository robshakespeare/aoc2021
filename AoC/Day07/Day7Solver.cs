namespace AoC.Day07;

public class Day7Solver : SolverBase
{
    public override string DayName => "The Treachery of Whales";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (positions, positionFrequencies) = Parse(input);

        // Console.WriteLine(horizontalPositionFrequencies.Dump());

        var largestFrequency = positionFrequencies.Max(x => x.freq);

        var candidateAlignmentPositions = positionFrequencies.Where(x => x.freq == largestFrequency).Select(x => x.pos).ToArray();

        return candidateAlignmentPositions.Select(pos => GetFuelCostToAlignTo(pos, positions)).Min();
    }

    private static long GetFuelCostToAlignTo(long targetPosition, long[] positions) =>
        positions.Select(pos => Math.Abs(targetPosition - pos)).Sum();

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    private static (long[] positions, (long pos, long freq)[] positionFrequencies) Parse(PuzzleInput input)
    {
        var horizontalPositions = input.ToString().Split(',').Select(long.Parse).ToArray();

        return (
            horizontalPositions,
            horizontalPositions.ToLookup(x => x).Select(l => (l.Key, l.LongCount())).ToArray());
    }
}
