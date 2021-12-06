namespace AoC.Day06;

public class Day6Solver : SolverBase
{
    public override string DayName => "Lanternfish";

    public override long? SolvePart1(PuzzleInput input) => LanternfishSimulator(input, 80);

    public override long? SolvePart2(PuzzleInput input) => LanternfishSimulator(input, 256);

    private static long LanternfishSimulator(PuzzleInput input, int numOfDays)
    {
        var lanternfishCounts = GetInitialLanternfishCounts(input);

        // On each day, shift the counts to the left (simulates each fish's timer reducing by a day)
        // The count of lanternfish in index zero get added to index 6 (A lanternfish that creates a new fish resets its timer to 6)
        // But this same count gets put in index 8, because each lanternfish creates a new one (at day 8) after each fish resets its timer
        for (var day = 1; day <= numOfDays; day++)
        {
            var spawnCount = lanternfishCounts[0];

            Array.Copy(lanternfishCounts, 1, lanternfishCounts, 0, 8); // Shift fish 1 day earlier for the fish in days 1 to 8, making them in days 0 to 7

            lanternfishCounts[6] += spawnCount;
            lanternfishCounts[8] = spawnCount;
        }

        return lanternfishCounts.Sum();
    }

    /// <summary>
    /// Returns an array that holds the lanternfish counts for each day of the cycle.
    /// 
    /// Each index of the array is the timer, the number of days until each lanternfish creates a new lanternfish.
    /// i.e. the indexes 0, 1, 2, 3, 4, 5, 6, 7, 8 are the days
    /// 
    /// The values of the array are the count of lanternfish for that day in the cycle.
    /// </summary>
    private static long[] GetInitialLanternfishCounts(PuzzleInput input)
    {
        var lanternfishCounts = new long[9];

        foreach (var lanternfishTimer in input.ToString().Split(',').Select(int.Parse))
        {
            lanternfishCounts[lanternfishTimer]++;
        }

        return lanternfishCounts;
    }
}
