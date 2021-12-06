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
        // The count of lanternfish in index zero get moved to index 6 (A lanternfish that creates a new fish resets its timer to 6)
        // But this same count gets put in index 8, because each lanternfish creates a new one at day 8 after each fish reaches zero
        for (var day = 1; day <= numOfDays; day++)
        {
            var spawnCount = lanternfishCounts[0];
            var newLanternfishCounts = new long[lanternfishCounts.Length];

            for (var shift = 1; shift < lanternfishCounts.Length; shift++)
            {
                newLanternfishCounts[shift - 1] = lanternfishCounts[shift];
            }

            newLanternfishCounts[6] += spawnCount;
            newLanternfishCounts[8] = spawnCount;

            lanternfishCounts = newLanternfishCounts;
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
