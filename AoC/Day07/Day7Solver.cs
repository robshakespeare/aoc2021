namespace AoC.Day07;

public class Day7Solver : SolverBase
{
    public override string DayName => "The Treachery of Whales";

    public override long? SolvePart1(PuzzleInput input) => GetCheapestFuelCost(CrabData.Parse(input), fuel => fuel);

    public override long? SolvePart2(PuzzleInput input)
    {
        var crabData = CrabData.Parse(input);

        var prevCost = 0L;
        var fuelToCostLookup = new SortedList<int, long>((..(crabData.MaxPosition + 1)).ToEnumerable().Select(fuel =>
        {
            prevCost += fuel;
            return new {fuel, cost = prevCost};
        }).ToDictionary(x => x.fuel, x => x.cost));

        return GetCheapestFuelCost(crabData, fuel => fuelToCostLookup[fuel]);
    }

    private static long GetCheapestFuelCost(CrabData crabData, Func<int, long> fuelToCost) =>
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
