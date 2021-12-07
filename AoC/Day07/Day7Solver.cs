namespace AoC.Day07;

public class Day7Solver : SolverBase
{
    public override string DayName => "The Treachery of Whales";

    public override long? SolvePart1(PuzzleInput input)
    {
        var positions = input.ToString().Split(',').Select(long.Parse).ToArray();
        var targetPosition = positions.Median();

        return positions.Select(pos => Math.Abs(targetPosition - pos)).Sum();
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        static int NthTriangularNumber(int n) => n * (n + 1) / 2; // From: https://math.stackexchange.com/a/60581
        var positions = input.ToString().Split(',').Select(int.Parse).ToArray();

        return FindCheapestFuelCost(positions, NthTriangularNumber);
    }

    private static int FindCheapestFuelCost(int[] positions, Func<int, int> fuelToCost) =>
        (positions.Min()..(positions.Max() + 1)).ToEnumerable()
        .Select(targetPosition => positions.Select(pos => fuelToCost(Math.Abs(targetPosition - pos))).Sum())
        .Min();
}
