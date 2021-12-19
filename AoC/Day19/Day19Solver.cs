using System.Diagnostics;
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
        // Min/mid bounds approach isn't going to work, that relies of the set of positions being in the same grid
        // Brute force way is to work out the differences, and then any where 12 or more align, we have a matching "intersection"

        // Each scanner must have at least one overlapping scanner

        var scanners = Scanner.ParseInputToScanners(input);

        //var scannerRelations = new Dictionary<int, (Vector3 RelativePosition, int ScannerId)>();

        //var unknownScanners = new HashSet<int>(scanners.Select(x => x.ScannerId));

        //var nextScanner = scanners[0];
        //unknownScanners.Remove(nextScanner.ScannerId);

        ////bool end = false;

        //var stopwatch = Stopwatch.StartNew();

        //while (unknownScanners.Any() /*&& stopwatch.Elapsed < TimeSpan.FromSeconds(10)*/)
        //{
        //    //var (_, otherScanner) = GetFirstScannerRelativeTo(scanner2, scanners, knownScanners);
        //    ////    knownScanners.Add(otherScanner);
        //    ////    scanner2 = otherScanner;

        //    foreach (var otherScanner in scanners.Where(x => unknownScanners.Contains(x.ScannerId)))
        //    {
        //        var overlappingDetails = nextScanner.GetOverlappingDetailsOrNull(otherScanner);

        //        if (overlappingDetails != null)
        //        {
        //            Console.WriteLine($"{overlappingDetails.OverlappingScannerOriented} overlaps with {overlappingDetails.BaseScanner}.");
        //            nextScanner = overlappingDetails.OverlappingScannerOriented;
        //            unknownScanners.Remove(nextScanner.ScannerId);
        //            break;
        //        }
        //        else
        //        {
        //            //Console.WriteLine($"{otherScanner} DOES NOT overlap with {nextScanner}, breaking out.");
        //            //break;
        //        }
        //    }
        //}

        //var checkNext = new List<Scanner>();

        //{
        //    var scanner0 = scanners[0];

        //    foreach (var otherScanner in scanners.Where(x => x != scanner0))
        //    {
        //        var overlappingDetails = scanner0.GetOverlappingDetailsOrNull(otherScanner);

        //        if (overlappingDetails != null)
        //        {
        //            Console.WriteLine($"{overlappingDetails.OverlappingScannerOriented} overlaps with {overlappingDetails.BaseScanner}.");
        //            checkNext.Add(overlappingDetails.OverlappingScannerOriented);
        //        }
        //    }
        //}

        var knownPositions = new Dictionary<int, Vector3> {{0, Vector3.Zero}};
        var knownScannerOrientations = new Dictionary<int, Scanner> {{0, scanners[0]}};

        var scannersToCheck = new List<Scanner> {scanners[0]};

        while (scannersToCheck.Any() && knownPositions.Count != scanners.Count)
        {
            var checkNext2 = new List<Scanner>();

            foreach (var scanner in scannersToCheck)
            {
                foreach (var otherScanner in scanners.Where(x => !knownPositions.ContainsKey(x.ScannerId)))
                {
                    var overlappingDetails = scanner.GetOverlappingDetailsOrNull(otherScanner);

                    if (overlappingDetails != null)
                    {
                        var scannerFound = overlappingDetails.OverlappingScannerOriented;
                        var relativeToScanner = overlappingDetails.BaseScanner;
                        Console.WriteLine($"{scannerFound} overlaps with {relativeToScanner}.");

                        knownPositions[scannerFound.ScannerId] = knownPositions[relativeToScanner.ScannerId] + overlappingDetails.RelativePosition;
                        knownScannerOrientations[scannerFound.ScannerId] = scannerFound;

                        checkNext2.Add(scannerFound);
                    }
                }
            }

            scannersToCheck = checkNext2;
        }

        foreach (var knownPosition in knownPositions.OrderBy(x => x.Key))
        {
            Console.WriteLine($"{knownPosition.Key}: {knownPosition.Value}.");
        }

        var beacons = knownPositions.OrderBy(x => x.Key).SelectMany(x => knownScannerOrientations[x.Key].Beacons.Select(b => b + x.Value)).Distinct();

        return beacons.Count();

        //foreach (var scanner in scanners)
        //{
        //    foreach (var otherScanner in scanners.Where(x => x != scanner))
        //    {
        //        //if (!scannerRelations.ContainsKey(otherScanner.ScannerId))
        //        //{
        //        var relativePosition = scanner.GetRelativePositionOfOtherScanner(otherScanner);

        //        if (relativePosition != null)
        //        {
        //            //Console.WriteLine($"Scanner {otherScanner.ScannerId} is at {relativePosition} (relative to scanner {scanner.ScannerId}).");

        //            Console.WriteLine($"{scanner} overlaps with {otherScanner}.");

        //            knownOverlappingPairs.Add((scanner, otherScanner));
        //            //scannerRelations[otherScanner.ScannerId] = (relativePosition.Value, scanner.ScannerId);
        //        }
        //        //}

        //        //break;
        //    }
        //}



        ////var knownOverlappingPairs = new List<(Scanner a, Scanner b)>();

        ////foreach (var scanner in scanners)
        ////{
        ////    foreach (var otherScanner in scanners
        ////                 .Where(x => x != scanner)
        ////                 .Where(x => !knownOverlappingPairs.Contains((scanner, x)) &&
        ////                             !knownOverlappingPairs.Contains((x, scanner))))
        ////    {
        ////        //if (!scannerRelations.ContainsKey(otherScanner.ScannerId))
        ////        //{
        ////        var relativePosition = scanner.GetRelativePositionOfOtherScanner(otherScanner);

        ////        if (relativePosition != null)
        ////        {
        ////            //Console.WriteLine($"Scanner {otherScanner.ScannerId} is at {relativePosition} (relative to scanner {scanner.ScannerId}).");

        ////            Console.WriteLine($"{scanner} overlaps with {otherScanner}.");

        ////            knownOverlappingPairs.Add((scanner, otherScanner));
        ////            //scannerRelations[otherScanner.ScannerId] = (relativePosition.Value, scanner.ScannerId);
        ////        }
        ////        //}

        ////        //break;
        ////    }
        ////}




        //var scanner2 = scanners[0];
        //var knownScanners = new List<Scanner>() { scanner2 };

        //while (knownScanners.Count != scanners.Count)
        //{
        //    var (_, otherScanner) = GetFirstScannerRelativeTo(scanner2, scanners, knownScanners);
        //    knownScanners.Add(otherScanner);
        //    scanner2 = otherScanner;
        //}

        //foreach (var (otherScannerId, (relativePosition, scannerId)) in scannerRelations)
        //{
        //    Console.WriteLine($"Scanner {otherScannerId} is at {relativePosition} (relative to scanner {scannerId}).");
        //}

        return null;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    ////public static (Vector3? relativePosition, Scanner otherScanner) GetFirstScannerRelativeTo(
    ////    Scanner scanner,
    ////    IReadOnlyList<Scanner> allScanners,
    ////    IReadOnlyList<Scanner> knownScanners)
    ////{
    ////    foreach (var otherScanner in allScanners.Where(x => x != scanner && !knownScanners.Contains(x)))
    ////    {
    ////        var relativePosition = scanner.GetRelativePositionOfOtherScanner(otherScanner);

    ////        if (relativePosition != null)
    ////        {
    ////            //Console.WriteLine($"Scanner {otherScanner.ScannerId} is at {relativePosition} (relative to scanner {scanner.ScannerId}).");
    ////            //scannerRelations[otherScanner.ScannerId] = (relativePosition.Value, scanner.ScannerId);

    ////            return (relativePosition, otherScanner);
    ////        }
    ////    }
    ////    //    {
    ////    //        if (!scannerRelations.ContainsKey(otherScanner.ScannerId))
    ////    //        {
    ////    //            var relativePosition = scanner.GetRelativePositionOfOtherScanner(otherScanner);

    ////    //            if (relativePosition != null)
    ////    //            {
    ////    //                //Console.WriteLine($"Scanner {otherScanner.ScannerId} is at {relativePosition} (relative to scanner {scanner.ScannerId}).");
    ////    //                scannerRelations[otherScanner.ScannerId] = (relativePosition.Value, scanner.ScannerId);
    ////    //            }
    ////    //        }

    ////    //        //break;
    ////    //    }

    ////    throw new InvalidOperationException($"Failed to find scanner relative to {scanner}");
    ////}

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
                    let overlaps = sourceDeltas
                        .Join(otherDeltas, a => a.Delta, b => b.Delta, (source, other) => new Overlap(source.Beacon, other.Beacon))
                        .ToArray()
                    select new OverlappingDetails(otherOrientation, this, overlaps))
                .FirstOrDefault(x => x.Overlaps.Length >= 12);
        }

        public readonly record struct Overlap(Vector3 SourceBeacon, Vector3 OtherBeacon);

        public record OverlappingDetails(
            Scanner OverlappingScannerOriented,
            Scanner BaseScanner,
            Overlap[] Overlaps)
        {
            private Lazy<Vector3> LazyRelativePosition { get; } = new(() => Overlaps.First().SourceBeacon - Overlaps.First().OtherBeacon);

            public Vector3 RelativePosition => LazyRelativePosition.Value;
        }

        //public IEnumerable<(Vector3 SourceBeacon, Vector3 OtherBeacon)[]> GetOverlappingBeacons2(Scanner otherScanner)
        //{
        //    // Brute force way is to work out the differences, and then any where 12 or more align, we have a matching "intersection"
        //    // For each orientation in the other scanner
        //    // For each point in source, get the differences to all the other points in source
        //    // For each point in otherOrientation, get the differences to all the other points in otherOrientation
        //    // Get the intersection of those differences, and if we have get more than 12, we have our match!

        //    return (from sourceBeacon in Beacons
        //            select GetDeltasToBeacon(sourceBeacon).ToArray()
        //            into sourceDeltas
        //            from otherOrientation in otherScanner.AllOrientations.Value
        //            from otherBeacon in otherOrientation.Beacons
        //            let otherDeltas = otherOrientation.GetDeltasToBeacon(otherBeacon)
        //            select sourceDeltas.Join(otherDeltas, a => a.Delta, b => b.Delta, (source, other) => (source.Beacon, other.Beacon)).ToArray())
        //        .Where(intersections => intersections.Length >= 12);
        //}

        //public Vector3? GetRelativePositionOfOtherScanner(Scanner otherScanner)
        //{
        //    var overlappingBeacons = GetOverlappingBeacons(otherScanner);

        //    if (overlappingBeacons != null)
        //    {
        //        var (sourceBeacon, otherBeacon) = overlappingBeacons.First();
        //        return sourceBeacon - otherBeacon;
        //    }

        //    return null;
        //}

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
