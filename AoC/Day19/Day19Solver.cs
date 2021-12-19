using static System.Environment;

namespace AoC.Day19;

public class Day19Solver : SolverBase
{
    public override string DayName => "Beacon Scanner";

    public override long? SolvePart1(PuzzleInput input)
    {
        // Given Scanner 0, try all the orientations of all of the other Scanners
        // For Each orientation, get the deltas between the orientation and scanner 0
        // Any where there are 12 or more deltas that match, we should then be able to say they overlap and get the scanner's position

        var points = new Vector3(404, -588, -901);

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

        public IReadOnlyList<Vector3> Beacons { get; }

        ////public Vector3? Position { get; set; } = null;

        public Scanner(int id, IReadOnlyList<Vector3> beacons)
        {
            Id = id;
            Beacons = beacons;
        }

        /// <summary>
        /// In total, each scanner could be in any of 24 different orientations.
        /// </summary>
        public IReadOnlyList<Scanner> GetOrientations()
        {
            // Top level array is the length of the original number of Beacons
            // Each element in the array is another array which represents the possible orientations of that Beacon

            var scanners = new List<Scanner>();
            var orientationsOfEachBeacon = Beacons.Select(beacon => GetAllPermutations(beacon).ToArray()).ToArray();

            for (var orientation = 0; orientation < 24; orientation++)
            {
                var beaconsForThisOrientation = new List<Vector3>();

                for (var beaconIndex = 0; beaconIndex < Beacons.Count; beaconIndex++)
                {
                    beaconsForThisOrientation.Add(orientationsOfEachBeacon[beaconIndex][orientation]);
                }

                scanners.Add(new Scanner(Id, beaconsForThisOrientation));
            }

            return scanners;
        }

        private static readonly Regex ParseScannerIdRegex = new(@"--- scanner (?<scannerId>\d+) ---", RegexOptions.Compiled);

        public static IReadOnlyList<Scanner> ParseInputToScanners(PuzzleInput input) => input.ToString().Split($"{NewLine}{NewLine}").Select(chunk =>
        {
            var lines = chunk.Split(NewLine).ToArray();

            var scannerIdLine = lines.First();
            var scannerIdMatch = ParseScannerIdRegex.Match(scannerIdLine);
            if (!scannerIdMatch.Success)
                throw new InvalidOperationException("Invalid scanner ID line: " + scannerIdLine);
            var scannerId = int.Parse(scannerIdMatch.Groups["scannerId"].Value);

            var beacons = lines.Skip(1).Select(LineToVector3).ToArray();

            return new Scanner(scannerId, beacons);
        }).ToArray();
    }

    public static Vector3 LineToVector3(string line)
    {
        var values = line.Split(',').Select(int.Parse).ToArray();
        return new Vector3(values[0], values[1], values[2]);
    }

    public static IEnumerable<Vector3> GetAllPermutations(Vector3 vector)
    {
        ////vector = Vector3.Abs(vector); // rs-todo: I think this is wrong actually

        yield return new Vector3(vector.X, vector.Y, vector.Z);
        yield return new Vector3(-vector.X, vector.Y, vector.Z);
        yield return new Vector3(-vector.X, -vector.Y, vector.Z);
        yield return new Vector3(vector.X, -vector.Y, vector.Z);
        yield return new Vector3(vector.X, -vector.Y, -vector.Z);
        yield return new Vector3(vector.X, vector.Y, -vector.Z);
        yield return new Vector3(-vector.X, vector.Y, -vector.Z);
        yield return new Vector3(-vector.X, -vector.Y, -vector.Z);

        yield return new Vector3(vector.Z, vector.X, vector.Y);
        yield return new Vector3(-vector.Z, vector.X, vector.Y);
        yield return new Vector3(-vector.Z, -vector.X, vector.Y);
        yield return new Vector3(vector.Z, -vector.X, vector.Y);
        yield return new Vector3(vector.Z, -vector.X, -vector.Y);
        yield return new Vector3(vector.Z, vector.X, -vector.Y);
        yield return new Vector3(-vector.Z, vector.X, -vector.Y);
        yield return new Vector3(-vector.Z, -vector.X, -vector.Y);

        yield return new Vector3(vector.Y, vector.Z, vector.X);
        yield return new Vector3(-vector.Y, vector.Z, vector.X);
        yield return new Vector3(-vector.Y, -vector.Z, vector.X);
        yield return new Vector3(vector.Y, -vector.Z, vector.X);
        yield return new Vector3(vector.Y, -vector.Z, -vector.X);
        yield return new Vector3(vector.Y, vector.Z, -vector.X);
        yield return new Vector3(-vector.Y, vector.Z, -vector.X);
        yield return new Vector3(-vector.Y, -vector.Z, -vector.X);
    }
}
