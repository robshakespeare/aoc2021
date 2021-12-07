namespace AoC.Day07;

public class Day7Solver : SolverBase
{
    public override string DayName => "The Treachery of Whales";

    public override long? SolvePart1(PuzzleInput input) => GetCheapestFuelCost(CrabData.Parse(input), fuel => fuel);

    public override long? SolvePart2(PuzzleInput input)
    {
        static int NthTriangularNumber(int n) => n * (n + 1) / 2; // From: https://math.stackexchange.com/a/60581

        return GetCheapestFuelCost(CrabData.Parse(input), NthTriangularNumber);
    }

    private static int GetCheapestFuelCost(CrabData crabData, Func<int, int> fuelToCost) =>
        (crabData.MinPosition..(crabData.MaxPosition + 1)).ToEnumerable()
        .Select(targetPosition => crabData.Positions.Select(pos => fuelToCost(Math.Abs(targetPosition - pos))).Sum())
        .Min();

    private record CrabData(int[] Positions)
    {
        public int MinPosition { get; } = Positions.Min();

        public int MaxPosition { get; } = Positions.Max();

        public static CrabData Parse(PuzzleInput input) => new(input.ToString().Split(',').Select(int.Parse).ToArray());
    }
}
