using MoreLinq;
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

        var scanners = Scanner.ParseInputToScanners(input);

        var scanner0 = scanners.First();
        ////var scanner0LocalSpaceBeacons = new HashSet<Vector3>(scanner0.LocalSpaceBeacons);

        foreach (var otherScanner in scanners.Skip(1))
        {
            Console.WriteLine($"--- Scanner {otherScanner.ScannerId} ---");
            foreach (var scannerOrientation in otherScanner.GetOrientations())
            {
                var intersections = scanner0.LocalSpaceBeacons.Intersect(scannerOrientation.LocalSpaceBeacons).ToArray();
                Console.WriteLine($"Intersections: {intersections.Length}");
                if (intersections.Length > 0) // >= 12) // rs-todo: should be 12!!!
                {
                    Console.WriteLine(
                        $"Scanner {scanner0.ScannerId} has {scanner0.LocalSpaceBeacons.Count} {scannerOrientation.LocalSpaceBeacons.Count} {intersections.Length} matches other scanner {scannerOrientation.ScannerId} orientation {scannerOrientation.Orientation}");
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        ////var points = new Vector3(404, -588, -901);

        ////var perms = GetAllPermutations(points).Select(x => string.Join(", ", x)).Distinct().ToArray();

        ////Console.WriteLine($"There are {perms.Length} perms:");

        ////foreach (var perm in perms)
        ////{
        ////    Console.WriteLine(perm);
        ////}

        return null;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class Scanner
    {
        public int ScannerId { get; }

        public IReadOnlyList<Vector3> Beacons { get; }

        /// <summary>
        /// rs-todo: is this comment going to be correct?
        /// The orientation of the scanner. In total, each scanner could be in any of 24 different orientations.
        /// Orientation 0 is the original orientation.
        /// </summary>
        public int Orientation { get; }

        public Vector3 MinBounds { get; }

        // rs-todo: local space isn't the right name, that is confusing
        public IReadOnlyList<Vector3> LocalSpaceBeacons { get; set; }

        ////public Vector3? Position { get; set; } = null;

        public Scanner(int scannerId, IReadOnlyList<Vector3> beacons, int orientation = 0)
        {
            ScannerId = scannerId;
            Beacons = beacons;
            Orientation = orientation;
            MinBounds = beacons.Aggregate(Vector3.Min);
            LocalSpaceBeacons = beacons.Select(beacon => beacon - MinBounds).ToArray();
        }

        /// <summary>
        /// In total, each scanner could be in any of 24 different orientations.
        /// </summary>
        public IReadOnlyList<Scanner> GetOrientations()
        {
            // Top level array is the length of the original number of Beacons
            // Each element in the array is another array which represents the possible orientations of that Beacon

            var beaconsByOrientation = new SortedList<int, List<Vector3>>();

            foreach (var beacon in Beacons)
            {
                bool firstRun = beaconsByOrientation.Count == 0;
                foreach (var (orientation, position) in GetAllPermutations(beacon).Index())
                {
                    if (firstRun)
                    {
                        beaconsByOrientation[orientation] = new List<Vector3>();
                    }

                    beaconsByOrientation[orientation].Add(position);
                }
            }

            return beaconsByOrientation.Select(x => new Scanner(ScannerId, x.Value, x.Key)).ToArray();



            //    var scanners = new List<Scanner>();
            //    var orientationsOfEachBeacon = Beacons.Select(beacon => GetAllPermutations(beacon).ToArray()).ToArray();

            //    for (var orientation = 0; orientation < 24; orientation++)
            //    {
            //        var beaconsForThisOrientation = new List<Vector3>();

            //        for (var beaconIndex = 0; beaconIndex < Beacons.Count; beaconIndex++)
            //        {
            //            beaconsForThisOrientation.Add(orientationsOfEachBeacon[beaconIndex][orientation]);
            //        }

            //        scanners.Add(new Scanner(ScannerId, beaconsForThisOrientation, orientation));
            //    }

            //    return scanners;
            //}
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

        public static Vector3 LineToVector3(string line)
        {
            var values = line.Split(',').Select(int.Parse).ToArray();
            return new Vector3(values[0], values[1], values[2]);
        }
    }

    public static Func<Vector3, Vector3>[] BuildPermutors()
    {
        //float Fix(float f) => f == -0f ? 0 : f;

        return new[]
            {
                new Func<Vector3, float>[] {v => v.X, v => v.Y, v => v.Z},
                new Func<Vector3, float>[] {v => -v.X, v => -v.Y, v => -v.Z},

                new Func<Vector3, float>[] {v => -v.X, v => v.Y, v => v.Z},
                new Func<Vector3, float>[] {v => v.X, v => -v.Y, v => v.Z},
                new Func<Vector3, float>[] {v => v.X, v => v.Y, v => -v.Z},

                new Func<Vector3, float>[] {v => -v.X, v => -v.Y, v => v.Z},
                new Func<Vector3, float>[] {v => v.X, v => -v.Y, v => -v.Z},

                new Func<Vector3, float>[] {v => -v.X, v => v.Y, v => -v.Z}

                //new Func<Vector3, float>[] {v => v.X, v => v.Y, v => v.Z},
                //new Func<Vector3, float>[] {v => Fix(-v.X), v => Fix(-v.Y), v => Fix(-v.Z)},

                //new Func<Vector3, float>[] {v => Fix(-v.X), v => Fix(v.Y), v => v.Z},
                //new Func<Vector3, float>[] {v => v.X, v => Fix(-v.Y), v => v.Z},
                //new Func<Vector3, float>[] {v => v.X, v => v.Y, v => Fix(-v.Z)},

                //new Func<Vector3, float>[] {v => Fix(-v.X), v => Fix(-v.Y), v => v.Z},
                //new Func<Vector3, float>[] {v => v.X, v => Fix(-v.Y), v => Fix(-v.Z)},

                //new Func<Vector3, float>[] {v => Fix(-v.X), v => v.Y, v => Fix(-v.Z)}
            }
            .SelectMany(x => x.Permutations())
            .Select(x => (Func<Vector3, Vector3>) (input => new Vector3(x[0](input), x[1](input), x[2](input))))
            .ToArray();

        //var test1 = new[] { "x", "y", "z" };
        //var test2 = new[] { "-x", "-y", "-z" };

        //var test3 = new[] { "-x", "y", "z" };
        //var test4 = new[] { "x", "-y", "z" };
        //var test5 = new[] { "x", "y", "-z" };

        //var test6 = new[] { "-x", "-y", "z" };
        //var test7 = new[] { "x", "-y", "-z" };
    }

    private static readonly Func<Vector3, Vector3>[] Permutors = BuildPermutors();

    public static IEnumerable<Vector3> GetAllPermutations(Vector3 vector)
    {
        static float Fix(float f) => f == -0f ? 0 : f;

        return Permutors.Select(permutor => permutor(vector)).Select(x => new Vector3(Fix(x.X), Fix(x.Y), Fix(x.Z)));

        //static IEnumerable<Vector3> GetAllPermutationsRaw(Vector3 vector)
        //{
        //    ////vector = Vector3.Abs(vector); // rs-todo: I think this is wrong actually

        //    yield return new Vector3(vector.X, vector.Y, vector.Z);
        //    yield return new Vector3(-vector.X, vector.Y, vector.Z);
        //    yield return new Vector3(-vector.X, -vector.Y, vector.Z);
        //    yield return new Vector3(vector.X, -vector.Y, vector.Z);
        //    yield return new Vector3(vector.X, -vector.Y, -vector.Z);
        //    yield return new Vector3(vector.X, vector.Y, -vector.Z);
        //    yield return new Vector3(-vector.X, vector.Y, -vector.Z);
        //    yield return new Vector3(-vector.X, -vector.Y, -vector.Z);

        //    yield return new Vector3(vector.Z, vector.X, vector.Y);
        //    yield return new Vector3(-vector.Z, vector.X, vector.Y);
        //    yield return new Vector3(-vector.Z, -vector.X, vector.Y);
        //    yield return new Vector3(vector.Z, -vector.X, vector.Y);
        //    yield return new Vector3(vector.Z, -vector.X, -vector.Y);
        //    yield return new Vector3(vector.Z, vector.X, -vector.Y);
        //    yield return new Vector3(-vector.Z, vector.X, -vector.Y);
        //    yield return new Vector3(-vector.Z, -vector.X, -vector.Y);

        //    yield return new Vector3(vector.Y, vector.Z, vector.X);
        //    yield return new Vector3(-vector.Y, vector.Z, vector.X);
        //    yield return new Vector3(-vector.Y, -vector.Z, vector.X);
        //    yield return new Vector3(vector.Y, -vector.Z, vector.X);
        //    yield return new Vector3(vector.Y, -vector.Z, -vector.X);
        //    yield return new Vector3(vector.Y, vector.Z, -vector.X);
        //    yield return new Vector3(-vector.Y, vector.Z, -vector.X);
        //    yield return new Vector3(-vector.Y, -vector.Z, -vector.X);
        //}

        //float Fix(float f) => f == -0f ? 0 : f;

        //return GetAllPermutationsRaw(vector).Select(x => new Vector3(Fix(x.X), Fix(x.Y), Fix(x.Z)));
    }
}
