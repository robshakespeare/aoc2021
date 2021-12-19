using MoreLinq;
using static System.Environment;

namespace AoC.Day19;

public class Day19Solver : SolverBase
{
    public override string DayName => "Beacon Scanner";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (beacons, _) = AssembleMapOfBeacons(input);

        return beacons.Count;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var (_, scannerPositions) = AssembleMapOfBeacons(input);

        return scannerPositions.SelectMany(a => scannerPositions.Select(b => MathUtils.ManhattanDistance(a, b))).Max();
    }

    public static (IReadOnlyCollection<Vector3> beacons, IReadOnlyCollection<Vector3> scannerPositions) AssembleMapOfBeacons(PuzzleInput input)
    {
        // Challenge is basically given to sets of positions in local/relative space, which ones overlap, without knowing their positions in world space
        // Min/mid bounds approach isn't going to work, that relies of the set of positions being in the same grid
        // Brute force way is to work out the differences, and then any where 12 or more align, we have a matching overlap

        var scanners = Scanner.ParseInputToScanners(input);

        var knownPositions = new Dictionary<int, Vector3> {{0, Vector3.Zero}};
        var knownScannerOrientations = new Dictionary<int, Scanner> {{0, scanners[0]}};

        var scannersToCheck = new List<Scanner> {scanners[0]};

        while (scannersToCheck.Any() && knownPositions.Count != scanners.Count)
        {
            var scannersToCheckNext = new List<Scanner>();

            foreach (var scanner in scannersToCheck)
            {
                foreach (var otherScanner in scanners.Where(x => !knownPositions.ContainsKey(x.ScannerId)))
                {
                    var overlappingDetails = scanner.GetOverlappingDetailsOrNull(otherScanner);

                    if (overlappingDetails != null)
                    {
                        var scannerFound = overlappingDetails.ScannerFound;
                        var relativeToScanner = overlappingDetails.RelativeToScanner;
                        Console.WriteLine($"{scannerFound} overlaps with {relativeToScanner}.");

                        knownPositions[scannerFound.ScannerId] = knownPositions[relativeToScanner.ScannerId] + overlappingDetails.RelativePosition;
                        knownScannerOrientations[scannerFound.ScannerId] = scannerFound;

                        scannersToCheckNext.Add(scannerFound);
                    }
                }
            }

            scannersToCheck = scannersToCheckNext;
        }

        Console.WriteLine("");
        Console.WriteLine("Scanner positions are:");
        foreach (var (scannerId, position) in knownPositions.OrderBy(x => x.Key))
        {
            Console.WriteLine($"Scanner {scannerId}: {position}");
        }

        var beacons = knownPositions.OrderBy(x => x.Key).SelectMany(x => knownScannerOrientations[x.Key].Beacons.Select(b => b + x.Value)).Distinct();

        return (beacons.ToArray(), knownPositions.Values);
    }

    public class Scanner
    {
        public int ScannerId { get; }

        public IReadOnlyList<Vector3> Beacons { get; }

        public int Orientation { get; }

        public Lazy<IReadOnlyList<Scanner>> AllOrientations { get; }

        public Scanner(int scannerId, IReadOnlyList<Vector3> beacons, bool baseOrientation, int orientation = 0)
        {
            ScannerId = scannerId;
            Beacons = beacons;
            Orientation = orientation;
            AllOrientations = new Lazy<IReadOnlyList<Scanner>>(
                () => baseOrientation ? GetOrientations() : throw new InvalidOperationException("Should only get orientations of base orientation"));
        }

        public override string ToString() => $"Scanner {ScannerId}";

        public OverlappingDetails? GetOverlappingDetailsOrNull(Scanner otherScanner)
        {
            // Brute force way is to work out the differences, and then any where 12 or more align, we have a matching overlap
            // For each orientation in the other scanner
            // For each point in source, get the differences to all the other points in source
            // For each point in otherOrientation, get the differences to all the other points in otherOrientation
            // Get the intersection of those differences, and if we have 12 or more, we have our match!

            return (from sourceBeacon in Beacons
                    select GetDeltasToBeacon(sourceBeacon).ToArray()
                    into sourceDeltas
                    from otherOrientation in otherScanner.AllOrientations.Value
                    from otherBeacon in otherOrientation.Beacons
                    let otherDeltas = otherOrientation.GetDeltasToBeacon(otherBeacon)
                    let overlaps = sourceDeltas
                        .Join(otherDeltas, a => a.Delta, b => b.Delta, (source, other) => new Overlap(source.Beacon, other.Beacon))
                        .ToArray()
                    select new OverlappingDetails(otherOrientation, this, overlaps))
                .FirstOrDefault(x => x.Overlaps.Length >= 12);
        }

        public readonly record struct Overlap(Vector3 SourceBeacon, Vector3 OtherBeacon);

        public record OverlappingDetails(Scanner ScannerFound, Scanner RelativeToScanner, Overlap[] Overlaps)
        {
            private Lazy<Vector3> LazyRelativePosition { get; } = new(() => Overlaps.First().SourceBeacon - Overlaps.First().OtherBeacon);

            public Vector3 RelativePosition => LazyRelativePosition.Value;
        }

        public IEnumerable<(Vector3 Beacon, Vector3 Delta)> GetDeltasToBeacon(Vector3 endBeacon) =>
            Beacons.Select(startBeacon => (startBeacon, endBeacon - startBeacon));

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
            static float Fix(float f) => Math.Abs(f - (-0f)) < 0.0001 ? 0 : f;
            return new[]
                {
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
