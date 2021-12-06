namespace AoC.Day06;

public class Day6Solver : SolverBase
{
    public override string DayName => "Lanternfish";

    public override long? SolvePart1(PuzzleInput input) // => Simulate(input, 80).Count;
    {
        return LanternfishSimulator.ParseInput(input).Run(80);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return LanternfishSimulator.ParseInput(input).Run(256);
        // Simulate(input, 256).Count // hmm, takes too long! Must be an optimal way to do it!
        return null;
    }

    public class LanternfishSimulator
    {
        /// <summary>
        /// Array that holds the lanternfish counts for each day of the cycle.
        /// 
        /// The index is the timer, the number of days until each lanternfish creates a new lanternfish.
        /// i.e. the indexes 0, 1, 2, 3, 4, 5, 6, 7, 8 are the days
        /// 
        /// The value is the count of lanternfish for that day in the cycle. 
        /// </summary>
        private long[] _lanternfishCounts = new long[9];

        public long Run(int numOfDays)
        {
            // On each day, shift the counts to the left
            // The count of lanternfish in index zero get moved to index 6
            // But this same count gets put in index 8, because each lanternfish creates a new one when it spawns and resets to 6

            for (var day = 1; day <= numOfDays; day++)
            {
                var spawnCount = _lanternfishCounts[0];

                var newLanternfishCounts = new long[9];

                for (var shift = 1; shift < 9; shift++)
                {
                    newLanternfishCounts[shift - 1] = _lanternfishCounts[shift];
                }

                newLanternfishCounts[6] += spawnCount;
                newLanternfishCounts[8] = spawnCount;

                _lanternfishCounts = newLanternfishCounts;
            }

            return _lanternfishCounts.Sum();
        }

        public static LanternfishSimulator ParseInput(PuzzleInput input)
        {
            var lanternfishTimers = input.ToString().Split(',').Select(int.Parse);
            var simulator = new LanternfishSimulator();

            foreach (var timer in lanternfishTimers)
            {
                simulator._lanternfishCounts[timer]++;
            }

            return simulator;
        }
    }

    public static List<Lanternfish> Simulate(PuzzleInput input, int numOfDays, Action<int>? onNextDay = null)
    {
        var fishes = Lanternfish.ParseInput(input);

        //void DisplayInfo(int day, int countBefore) => Console.WriteLine($"{day,3}: fishes {fishes.Count} (+{fishes.Count - countBefore} fishes) sum: {fishes.Sum(f => f.Timer)}");

        //DisplayInfo(0, fishes.Count);

        for (var day = 1; day <= numOfDays; day++)
        {
            onNextDay?.Invoke(day);

            var countBefore = fishes.Count;
            var spawned = fishes.Select(fish => fish.Update()).Where(shouldSpawn => shouldSpawn).Select(_ => new Lanternfish()).ToArray();
            fishes.AddRange(spawned);
            //DisplayInfo(day, countBefore);
        }

        return fishes;
    }

    public class Lanternfish
    {
        public int Timer { get; private set; }

        public Lanternfish()
        {
            Timer = 8;
        }

        public Lanternfish(int timer)
        {
            Timer = timer;
        }

        public bool Update()
        {
            if (Timer == 0)
            {
                Timer = 6;
                return true;
            }

            Timer--;
            return false;
        }

        public static List<Lanternfish> ParseInput(PuzzleInput input) => input.ToString().Split(',').Select(n => new Lanternfish(int.Parse(n))).ToList();
    }
}
