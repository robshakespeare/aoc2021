using System.Runtime.CompilerServices;
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

        // Challenge is basically given to sets of positions in local/relative space, which ones overlap, without knowing their positions in world space
        // Min/mid bounds approach isn;t going to work, that relies of the set of positions being in the same grid
        // Brute force way is to work out the differences, and then any where 12 or more align, we have a matching "intersection"

        var scanners = Scanner.ParseInputToScanners(input);

        //var scanner = scanners.First();
        ////var scanner0LocalSpaceBeacons = new HashSet<Vector3>(scanner0.RelativeToBinBoundsBeaconPositions);

        //foreach (var scanner2 in scanners)
        //{
        //    foreach (var otherScanner in scanners.Where(x => x != scanner2))
        //    {
        //        //foreach (var scannerOrientation in scanner2.GetOrientations())
        //        //{
        //            //Console.WriteLine($"--- Scanner {otherScanner.ScannerId} ---");
        //            foreach (var otherScannerOrientation in otherScanner.GetOrientations())
        //            {
        //                var intersections = scanner2.RelativeToBinBoundsBeaconPositions.Intersect(otherScannerOrientation.RelativeToBinBoundsBeaconPositions).ToArray();
        //                //Console.WriteLine($"Intersections: {intersections.Length}");
        //                if (intersections.Length > 0) // >= 12) // rs-todo: should be 12!!!
        //                {
        //                    Console.WriteLine(
        //                        $"Scanner {scanner2.ScannerId} has ({scanner2.RelativeToBinBoundsBeaconPositions.Count} {otherScannerOrientation.RelativeToBinBoundsBeaconPositions.Count}) {intersections.Length} matches other scanner {otherScannerOrientation.ScannerId} orientation {otherScannerOrientation.Orientation}");
        //                }
        //            }

        //            //Console.WriteLine();
        //            //Console.WriteLine();
        //        //}
        //    }
        //}

        

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

    //public readonly record struct Vector3(int X, int Y, int Z)
    //{
    //    public static Vector3 operator -(Vector3 left, Vector3 right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    //}

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

        public Lazy<IReadOnlyList<Scanner>> AllOrientations { get; }

        ////public Vector3 MinBounds { get; }

        ////public IReadOnlyList<Vector3> RelativeToBinBoundsBeaconPositions { get; set; }

        ////public Vector3? Position { get; set; } = null;

        public Scanner(int scannerId, IReadOnlyList<Vector3> beacons, bool baseOrientation, int orientation = 0)
        {
            ScannerId = scannerId;
            Beacons = beacons;
            Orientation = orientation;
            ////MinBounds = new Vector3(beacons.Min(b => b.X), beacons.Min(b => b.Y), beacons.Min(b => b.Z));  //beacons.Aggregate(Vector3.Min);
            ////RelativeToBinBoundsBeaconPositions = beacons.Select(beacon => beacon - MinBounds).ToArray();

            AllOrientations = new Lazy<IReadOnlyList<Scanner>>(
                () => baseOrientation ? GetOrientations() : throw new InvalidOperationException("Should only get orientations of base orientation"));
        }

        public (Vector3 SourceBeacon, Vector3 OtherBeacon)[]? GetOverlappingBeacons(Scanner otherScanner)
        {
            // Brute force way is to work out the differences, and then any where 12 or more align, we have a matching "intersection"
            // For each orientation in the other scanner
            // For each point in source, get the differences to all the other points in source
            // For each point in otherOrientation, get the differences to all the other points in otherOrientation
            // Get the intersection of those differences, and if we have get more than 12, we have our match!

            return (from sourceBeacon in Beacons
                    select GetDeltasToBeacon(sourceBeacon).ToArray()
                    into sourceDeltas
                    from otherOrientation in otherScanner.AllOrientations.Value
                    from otherBeacon in otherOrientation.Beacons
                    let otherDeltas = otherOrientation.GetDeltasToBeacon(otherBeacon)
                    select sourceDeltas.Join(otherDeltas, a => a.Delta, b => b.Delta, (source, other) => (source.Beacon, other.Beacon)).ToArray())
                .FirstOrDefault(intersections => intersections.Length >= 12);
        }

        public Vector3? GetRelativePositionOfOtherScanner(Scanner otherScanner)
        {
            var overlappingBeacons = GetOverlappingBeacons(otherScanner);

            if (overlappingBeacons != null)
            {
                var (sourceBeacon, otherBeacon) = overlappingBeacons.First();
                return sourceBeacon - otherBeacon;
            }

            return null;
        }

        public IEnumerable<(Vector3 Beacon, Vector3 Delta)> GetDeltasToBeacon(Vector3 endBeacon) =>
            Beacons.Select(startBeacon => (startBeacon, endBeacon - startBeacon));

        /// <summary>
        /// In total, each scanner could be in any of 24 different orientations. // rs-todo: is comment correct?
        /// </summary>
        private IReadOnlyList<Scanner> GetOrientations()
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

            return beaconsByOrientation.Select(x => new Scanner(ScannerId, x.Value, false, x.Key)).ToArray();
        }

        public static IEnumerable<Vector3> GetAllPermutations(Vector3 vector) => Permutors.Select(permutor => permutor(vector));

        private static Func<Vector3, Vector3>[] BuildPermutors()
        {
            // rs-todo: use a delta?
            float Fix(float f) => f == -0f ? 0 : f;

            return new[]
                {
                    //new Func<Vector3, int>[] {v => v.X, v => v.Y, v => v.Z},
                    //new Func<Vector3, int>[] {v => -v.X, v => -v.Y, v => -v.Z},

                    //new Func<Vector3, int>[] {v => -v.X, v => v.Y, v => v.Z},
                    //new Func<Vector3, int>[] {v => v.X, v => -v.Y, v => v.Z},
                    //new Func<Vector3, int>[] {v => v.X, v => v.Y, v => -v.Z},

                    //new Func<Vector3, int>[] {v => -v.X, v => -v.Y, v => v.Z},
                    //new Func<Vector3, int>[] {v => v.X, v => -v.Y, v => -v.Z},

                    //new Func<Vector3, int>[] {v => -v.X, v => v.Y, v => -v.Z}

                    new Func<Vector3, float>[] {v => v.X, v => v.Y, v => v.Z},
                    new Func<Vector3, float>[] {v => Fix(-v.X), v => Fix(-v.Y), v => Fix(-v.Z)},

                    new Func<Vector3, float>[] {v => Fix(-v.X), v => Fix(v.Y), v => v.Z},
                    new Func<Vector3, float>[] {v => v.X, v => Fix(-v.Y), v => v.Z},
                    new Func<Vector3, float>[] {v => v.X, v => v.Y, v => Fix(-v.Z)},

                    new Func<Vector3, float>[] {v => Fix(-v.X), v => Fix(-v.Y), v => v.Z},
                    new Func<Vector3, float>[] {v => v.X, v => Fix(-v.Y), v => Fix(-v.Z)},

                    new Func<Vector3, float>[] {v => Fix(-v.X), v => v.Y, v => Fix(-v.Z)}
                }
                .SelectMany(x => x.Permutations())
                .Select(x => (Func<Vector3, Vector3>) (input => new Vector3(x[0](input), x[1](input), x[2](input))))
                .ToArray();
        }

        private static readonly Func<Vector3, Vector3>[] Permutors = BuildPermutors();

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

            return new Scanner(scannerId, beacons, true);
        }).ToArray();

        public static Vector3 LineToVector3(string line)
        {
            var values = line.Split(',').Select(int.Parse).ToArray();
            return new Vector3(values[0], values[1], values[2]);
        }
    }
}
