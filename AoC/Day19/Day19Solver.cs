using static System.Environment;

namespace AoC.Day19;

public class Day19Solver : SolverBase
{
    public override string DayName => "Beacon Scanner";

    public override long? SolvePart1(PuzzleInput input)
    {
        // HM, PROBABLY NOT:
        // Every scanner should have its Beacons shifted to local space, as well as remembering original space too
        // Given Scanner 0, try all the orientations of all of the other Scanners
        // For Each orientation, get the deltas between the orientation and scanner 0, IN LOCAL SPACE
        // Any where there are 12 or more overlapping deltas, we should then be able to say they overlap and get the scanner's position



        var points = new[] {404, -588, -901};

        var perms = GetAllPermutations(points).Select(x => string.Join(", ", x)).Distinct().ToArray();

        Console.WriteLine($"There are {perms.Length} perms:");

        foreach (var perm in perms)
        {
            Console.WriteLine(perm);
        }

        return null;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class Scanner
    {
        public int Id { get; }

        public Vector3[] Beacons { get; }

        /// <summary>
        /// The orientation of the scanner. In total, each scanner could be in any of 24 different orientations.
        /// Orientation 0 is the original orientation.
        /// </summary>
        public int Orientation { get; }

        public Scanner(int id, Vector3[] beacons, int orientation = 0)
        {
            Id = id;
            Beacons = beacons;
            Orientation = orientation;
        }

        private static readonly Regex ParseScannerIdRegex = new(@"--- scanner (?<scannerId>\d+) ---", RegexOptions.Compiled);

        public static Scanner[] ParseInputToScanners(PuzzleInput input) => input.ToString().Split($"{NewLine}{NewLine}").Select(chunk =>
        {
            var lines = chunk.Split(NewLine).ToArray();

            var scannerIdLine = lines.First();
            var scannerIdMatch = ParseScannerIdRegex.Match(scannerIdLine);
            if (!scannerIdMatch.Success)
                throw new InvalidOperationException("Invalid scanner ID line: " + scannerIdLine);
            var scannerId = int.Parse(scannerIdMatch.Groups["scannerId"].Value);

            Vector3 ValuesToVector3(int[] values) => new(values[0], values[1], values[2]);
            var beacons = lines.Skip(1).Select(line => line.Split(',').Select(int.Parse).ToArray()).Select(ValuesToVector3).ToArray();

            return new Scanner(scannerId, beacons);
        }).ToArray();
    }

    public static IEnumerable<int[]> GetAllPermutations(IEnumerable<int> vectorCoords)
    {
        var vector = vectorCoords.Select(Math.Abs).ToArray();

        yield return new[] {vector[0], vector[1], vector[2]};
        yield return new[] {-vector[0], vector[1], vector[2]};
        yield return new[] {-vector[0], -vector[1], vector[2]};
        yield return new[] {vector[0], -vector[1], vector[2]};
        yield return new[] {vector[0], -vector[1], -vector[2]};
        yield return new[] {vector[0], vector[1], -vector[2]};
        yield return new[] {-vector[0], vector[1], -vector[2]};
        yield return new[] {-vector[0], -vector[1], -vector[2]};

        yield return new[] {vector[2], vector[0], vector[1]};
        yield return new[] {-vector[2], vector[0], vector[1]};
        yield return new[] {-vector[2], -vector[0], vector[1]};
        yield return new[] {vector[2], -vector[0], vector[1]};
        yield return new[] {vector[2], -vector[0], -vector[1]};
        yield return new[] {vector[2], vector[0], -vector[1]};
        yield return new[] {-vector[2], vector[0], -vector[1]};
        yield return new[] {-vector[2], -vector[0], -vector[1]};

        yield return new[] {vector[1], vector[2], vector[0]};
        yield return new[] {-vector[1], vector[2], vector[0]};
        yield return new[] {-vector[1], -vector[2], vector[0]};
        yield return new[] {vector[1], -vector[2], vector[0]};
        yield return new[] {vector[1], -vector[2], -vector[0]};
        yield return new[] {vector[1], vector[2], -vector[0]};
        yield return new[] {-vector[1], vector[2], -vector[0]};
        yield return new[] {-vector[1], -vector[2], -vector[0]};
    }
}
