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

    public static List<Lanternfish> Simulate(PuzzleInput input, int numOfDays)
    {
        var fishes = Lanternfish.ParseInput(input);

        for (var day = 0; day < numOfDays; day++)
        {
            var spawned = fishes.Select(fish => fish.Update()).Where(shouldSpawn => shouldSpawn).Select(_ => new Lanternfish()).ToArray();
            fishes.AddRange(spawned);
        }

        return fishes;
    }

    public class Lanternfish
    {
        private int _timer;

        public Lanternfish()
        {
            _timer = 8;
        }

        public Lanternfish(int timer)
        {
            _timer = timer;
        }

        public bool Update()
        {
            if (_timer == 0)
            {
                _timer = 6;
                return true;
            }

            _timer--;
            return false;
        }

        public static List<Lanternfish> ParseInput(PuzzleInput input) => input.ToString().Split(',').Select(n => new Lanternfish(int.Parse(n))).ToList();
    }
}
