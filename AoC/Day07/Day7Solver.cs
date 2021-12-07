using System.Security.AccessControl;

namespace AoC.Day07;

public class Day7Solver : SolverBase
{
    public override string DayName => "The Treachery of Whales";

    public override long? SolvePart1(PuzzleInput input) => GetCheapestFuelCost(input, CalcFuelCostToAlignTo);

    public override long? SolvePart2(PuzzleInput input)
    {
        var positions = input.ToString().Split(',').Select(int.Parse).ToArray();

        var minPosition = positions.Min();
        var maxPosition = positions.Max();

        var prevCost = 0L;
        var fuelToCostLookup = new SortedList<int, long>((..(maxPosition + 1)).ToEnumerable().Select(fuel =>
        {
            prevCost += fuel;
            return new {fuel, cost = prevCost};
        }).ToDictionary(x => x.fuel, x => x.cost));

        //Console.WriteLine(string.Join(Environment.NewLine,
        //    (minPosition..(maxPosition + 1)).ToEnumerable()
        //    .Select(targetPosition => CalcFuelCostToAlignTo(targetPosition, positions))));

        return (minPosition..(maxPosition + 1)).ToEnumerable()
            .Select(targetPosition => positions.Select(pos => fuelToCostLookup[Math.Abs(targetPosition - pos)]).Sum())
            //.Select(fuel => fuelToCostLookup[fuel])
            .Min();
    }

    private static long GetCheapestFuelCost(PuzzleInput input, Func<int, int[], int> calcFuelCostToAlignTo)
    {
        var positions = input.ToString().Split(',').Select(int.Parse).ToArray();

        var minPosition = positions.Min();
        var maxPosition = positions.Max();

        return (minPosition..(maxPosition + 1)).ToEnumerable()
            .Select(targetPosition => calcFuelCostToAlignTo(targetPosition, positions))
            .Min();
    }

    private static int CalcFuelCostToAlignTo(int targetPosition, int[] positions) => positions.Select(pos => Math.Abs(targetPosition - pos)).Sum();

    public record CrabData(int[] Positions)
    {
        public int MinPosition { get; } = Positions.Min();

        public int MaxPosition { get; } = Positions.Max();
    }
}
