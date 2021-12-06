namespace AoC.Day06;

public class Day6Solver : SolverBase
{
    public override string DayName => "Lanternfish";

    public override long? SolvePart1(PuzzleInput input) => LanternfishSimulator.Run(input, 80);

    public override long? SolvePart2(PuzzleInput input) => LanternfishSimulator.Run(input, 256);

    public static class LanternfishSimulator
    {
        public static long Run(PuzzleInput input, int numOfDays)
        {
            var lanternfishCounts = GetInitialLanternfishCounts(input);

            // On each day, shift the counts to the left
            // The count of lanternfish in index zero get moved to index 6 (A lanternfish that creates a new fish resets its timer to 6)
            // But this same count gets put in index 8, because each lanternfish creates a new one when it spawns and resets to 6

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
        /// The index of the array is the timer, the number of days until each lanternfish creates a new lanternfish.
        /// i.e. the indexes 0, 1, 2, 3, 4, 5, 6, 7, 8 are the days
        /// 
        /// The value of the array is the count of lanternfish for that day in the cycle.
        /// </summary>
        private static long[] GetInitialLanternfishCounts(PuzzleInput input)
        {
            var lanternfishTimers = input.ToString().Split(',').Select(int.Parse);
            var lanternfishCounts = new long[9];

            foreach (var timer in lanternfishTimers)
            {
                lanternfishCounts[timer]++;
            }

            return lanternfishCounts;
        }
    }
}
