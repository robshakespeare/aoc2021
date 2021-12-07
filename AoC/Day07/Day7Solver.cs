namespace AoC.Day07;

public class Day7Solver : SolverBase
{
    public override string DayName => "The Treachery of Whales";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (positions, positionFrequencies) = Parse(input);

        var minPosition = positions.Min();
        var maxPosition = positions.Max();

        ////var cheapestFuelRequirement = long.MaxValue;

        return (minPosition..(maxPosition + 1)).ToEnumerable()
            .Select(targetPosition => GetFuelCostToAlignTo(targetPosition, positions))
            .Min();

        //var largestFrequency = positionFrequencies.Max(x => x.freq);

        //var candidateAlignmentPositions = positionFrequencies.Where(x => x.freq == largestFrequency).Select(x => x.pos).ToArray();

        //return candidateAlignmentPositions.Select(pos => GetFuelCostToAlignTo(pos, positions)).Min();
    }

    private static long GetFuelCostToAlignTo(int targetPosition, int[] positions) =>
        positions.Select(pos => Math.Abs(targetPosition - pos)).Sum();

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    private static (int[] positions, (int pos, int freq)[] positionFrequencies) Parse(PuzzleInput input)
    {
        var horizontalPositions = input.ToString().Split(',').Select(int.Parse).ToArray();

        return (
            horizontalPositions,
            horizontalPositions.ToLookup(x => x).Select(l => (l.Key, l.Count())).ToArray());
    }
}
