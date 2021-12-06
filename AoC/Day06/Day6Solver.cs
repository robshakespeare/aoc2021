namespace AoC.Day06;

public class Day6Solver : SolverBase
{
    public override string DayName => "";

    public override long? SolvePart1(PuzzleInput input) => Simulate(input, 80).Count;

    public override long? SolvePart2(PuzzleInput input)
    {
        // Simulate(input, 256).Count // hmm, takes too long! Must be an optimal way to do it!
        return null;
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
