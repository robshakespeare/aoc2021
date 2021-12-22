namespace AoC.Day22;

public class Day22Solver : SolverBase
{
    public override string DayName => "Reactor Reboot";

    private static readonly Region InitializationProcedureRegion = new(new Vector3(-50, -50, -50), new Vector3(50, 50, 50));

    /// <summary>
    /// Considering only cubes in the region x=-50..50,y=-50..50,z=-50..50, how many cubes are on?
    /// </summary>
    public override long? SolvePart1(PuzzleInput input)
    {
        var rebootSteps = ParseInput(input);

        return CountOfCubesOnInInitializationProcedureRegion(rebootSteps);

        //var activeCuboids = new List<Cube>();
        //var cubeBoundsIntersection = Cube.GetIntersection(candidateCube, outerBounds);

        //foreach (var (isSet, candidateCube) in rebootSteps)
        //{
        //    var cubeBoundsIntersection = Cube.GetIntersection(candidateCube, outerBounds);

        //    // We just deal with the part of the cube that is within our bounds
        //    // If this is the whole cube, that's fine
        //    // If this is the part of the cube within the bounds, that is fine
        //    // If the cube totally lies out of the bounds, there is no intersection and we can ignore the cube
        //    if (cubeBoundsIntersection.intersectionArea > 0)
        //    {
        //        var cubeWithinBounds = cubeBoundsIntersection.intersection;

        //        // Next, for each active cube, we get get the intersection and exceptions, and deal with adding/removing as necessary
        //        // But if we have no active cubes, we need to add the current cube if we are turning on
        //        if (activeCuboids.Count == 0)
        //        {
        //            if (isSet)
        //            {
        //                activeCuboids.Add(cubeWithinBounds);
        //            }
        //        }
        //        else
        //        {
        //            foreach (var activeCuboid in activeCuboids.ToArray())
        //            {
        //                var (_, exceptionBoxes) = Cube.GetIntersectionAndExceptionCubes(activeCuboid, cubeWithinBounds);
        //                if (isSet)
        //                {
        //                    // i.e. we are turning ON
        //                    // Only turn on the exceptions (i.e. the additional cubes)
        //                    activeCuboids.AddRange(exceptionBoxes);
        //                }
        //                else
        //                {
        //                    // i.e. we are turning OFF
        //                    // Remove the whole old active cuboid
        //                    // And only re-add the exceptions that are within the current active cuboid
        //                    // i.e. the end state is that only the active cubes that were not inside the area to turn off are kept
        //                    activeCuboids.Remove(activeCuboid);
        //                    activeCuboids.AddRange(exceptionBoxes.Where(exceptionBox => activeCuboid.Contains(exceptionBox)));
        //                }
        //            }
        //        }
        //    }
        //}

        //return activeCuboids.Sum(x => x.Area);
    }

    public static long CountOfCubesOnInInitializationProcedureRegion(IReadOnlyList<RebootStep> rebootSteps)
    {
        var size = InitializationProcedureRegion.SizeInclusive; // note: bounds are inclusive

        var sizeHalved = InitializationProcedureRegion.Size / 2;
        Vector3 ShiftPositionToIndex(Vector3 position) => position + sizeHalved;

        var grid3D =
            Enumerable.Range(0, (int)size.Z).Select(
                _ => Enumerable.Range(0, (int)size.Y).Select(
                    _ => Enumerable.Range(0, (int)size.X).Select(_ => false).ToArray()).ToArray()).ToArray();

        var initializationProcedureSteps = GetInitializationProcedureSteps(rebootSteps);

        foreach (var (turnOn, bounds) in initializationProcedureSteps)
        {
            foreach (var position in bounds.GetPositionsWithinBounds().Select(ShiftPositionToIndex))
            {
                grid3D[(int)position.Z][(int)position.Y][(int)position.X] = turnOn;
            }

            Console.WriteLine("[ORG] countOfCubesOnAfterStep: " + grid3D.Sum(z => z.Sum(y => y.Count(x => x))));
        }

        var countOfCubesOn = grid3D.Sum(z => z.Sum(y => y.Count(x => x)));
        return countOfCubesOn;
    }

    /// <summary>
    /// Starting again with all cubes off, execute all reboot steps.
    /// Afterward, considering all cubes, how many cubes are on?
    /// </summary>
    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    /// <summary>
    /// Splits the specified reboot steps in to steps inside the Initialization Procedure Bounds, and steps outside those bounds.
    /// Note that steps never intersect those bounds.
    /// </summary>
    public static SplitRebootSteps SplitSteps(IReadOnlyList<RebootStep> rebootSteps)
    {
        var initializationProcedureSteps = new List<RebootStep>();
        var nonInitializationProcedureSteps = new List<RebootStep>();

        foreach (var rebootStep in rebootSteps)
        {
            if (InitializationProcedureRegion.Contains(rebootStep.Region))
            {
                initializationProcedureSteps.Add(rebootStep);
            }
            else
            {
                nonInitializationProcedureSteps.Add(rebootStep);
            }
        }

        return new SplitRebootSteps(initializationProcedureSteps, nonInitializationProcedureSteps);
    }

    public static IReadOnlyList<RebootStep> GetInitializationProcedureSteps(IReadOnlyList<RebootStep> rebootSteps) =>
        SplitSteps(rebootSteps).InitializationProcedureSteps;

    private static readonly Regex ParseInputRegex = new(
        @"(?<onOrOff>on|off) x=(?<x1>-?\d+)\.\.(?<x2>-?\d+),y=(?<y1>-?\d+)\.\.(?<y2>-?\d+),z=(?<z1>-?\d+)\.\.(?<z2>-?\d+)", RegexOptions.Compiled);

    public static IReadOnlyList<RebootStep> ParseInput(PuzzleInput input) => ParseInputRegex.Matches(input.ToString()).Select(match =>
    {
        var on = match.Groups["onOrOff"].Value == "on";
        var x1 = int.Parse(match.Groups["x1"].Value);
        var x2 = int.Parse(match.Groups["x2"].Value);
        var y1 = int.Parse(match.Groups["y1"].Value);
        var y2 = int.Parse(match.Groups["y2"].Value);
        var z1 = int.Parse(match.Groups["z1"].Value);
        var z2 = int.Parse(match.Groups["z2"].Value);

        // Coordinate space shouldn't matter, but for the sake of ease mental visualization, +ve X RIGHT, +ve Y DOWN, +ve Z BACK
        return new RebootStep(
            on,
            new Region(
                Lower: new Vector3(Math.Min(x1, x2), Math.Min(y1, y2), Math.Min(z1, z2)),
                Upper: new Vector3(Math.Max(x1, x2), Math.Max(y1, y2), Math.Max(z1, z2))));
    }).ToArray();

    public record RebootStep(bool TurnOn, Region Region);

    public record SplitRebootSteps(IReadOnlyList<RebootStep> InitializationProcedureSteps, IReadOnlyList<RebootStep> NonInitializationProcedureSteps);

    public record Region(Vector3 Lower, Vector3 Upper)
    {
        public Vector3 Size { get; } = Upper - Lower;
        public Vector3 SizeInclusive { get; } = Upper - Lower + Vector3.One;
        public long Area { get; } = GetArea(Lower, Upper); // rs-todo: probably remove
        public long AreaInclusive { get; } = GetAreaInclusive(Lower, Upper);

        public static long GetAreaInclusive(Vector3 lowerBounds, Vector3 upperBounds)
        {
            var size = upperBounds - lowerBounds + Vector3.One;
            return Math.Abs((long)size.X * (long)size.Y * (long)size.Z);
        }

        public static (long intersectionArea, Region intersection) GetIntersection(Region boxA, Region boxB)
        {
            var xA = Math.Max((long) boxA.Lower.X, (long) boxB.Lower.X);
            var yA = Math.Max((long) boxA.Lower.Y, (long) boxB.Lower.Y);
            var zA = Math.Max((long) boxA.Lower.Z, (long) boxB.Lower.Z);

            var xB = Math.Min((long) boxA.Upper.X, (long) boxB.Upper.X);
            var yB = Math.Min((long) boxA.Upper.Y, (long) boxB.Upper.Y);
            var zB = Math.Min((long) boxA.Upper.Z, (long) boxB.Upper.Z);

            var intersectionArea = Math.Abs(Math.Max(xB - xA, 0) * Math.Max(yB - yA, 0) * Math.Max(zB - zA, 0));

            var intersection = new Region(new Vector3(xA, yA, zA), new Vector3(xB, yB, zB));

            return (intersectionArea, intersection);
        }

        public static (Region? intersection, IReadOnlyList<Region> exceptionBoxes) GetIntersectionAndExceptionCubes(Region regionA, Region regionB)
        {
            // When the 2 input boxes match exactly, either will be the intersection box, and there will be no exception boxes.
            if (regionA == regionB)
            {
                return (regionA, Array.Empty<Region>());
            }

            // If there is no intersection, that means the boxes don't overlap, so just return the 2 input boxes as the exception boxes.
            var (intersectionArea, intersection) = GetIntersection(regionA, regionB);
            if (intersectionArea == 0)
            {
                return (null, new[] {regionA, regionB});
            }

            // Otherwise, there will be the single intersection box, and some exception boxes
            // Work out all of the boxes, and any whose areas are 0, exclude them

            var exceptionCubes = GetExceptionCubes(regionA, intersection)
                .Concat(GetExceptionCubes(regionB, intersection))
                .ToArray();

            return (intersection, exceptionCubes);

            //// rs-todo: work out the exception boxes

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the cubes that form part of the specified `cube` except the `intersection`.
        /// The `intersection` must be a true intersection of `cube`, otherwise this method will return unexpected/undefined results.
        /// </summary>
        public static IReadOnlyList<Region> GetExceptionCubes(Region region, Region intersection)
        {
            var exceptions = new Region[]
            {
                new(region.Lower,
                    new Vector3(intersection.Lower.X, region.Upper.Y, region.Upper.Z)), // top slab

                new(new Vector3(intersection.Upper.X, region.Lower.Y, region.Lower.Z),
                    region.Upper), // bottom slab

                new(new Vector3(intersection.Lower.X, region.Lower.Y, region.Lower.Z),
                    new Vector3(intersection.Upper.X, region.Upper.Y, intersection.Lower.Z)), // front slab

                new(new Vector3(intersection.Lower.X, region.Lower.Y, intersection.Upper.Z),
                    new Vector3(intersection.Upper.X, region.Upper.Y, region.Upper.Z)), // back slab

                new(new Vector3(intersection.Lower.X, region.Lower.Y, intersection.Lower.Z),
                    new Vector3(intersection.Upper.X, intersection.Lower.Y, intersection.Upper.Z)), // left slab

                new(new Vector3(intersection.Lower.X, intersection.Upper.Y, intersection.Lower.Z),
                    new Vector3(intersection.Upper.X, region.Upper.Y, intersection.Upper.Z)) // right slab
            };

            // filer out zero areas
            return exceptions.Where(x => x.Area > 0).ToArray();
        }

        /// <summary>
        /// Returns true if this box totally contains the other box.
        /// </summary>
        public bool Contains(Region otherRegion) => Contains(otherRegion.Lower) && Contains(otherRegion.Upper);

        /// <summary>
        /// Returns true if this box contains the specified position.
        /// </summary>
        public bool Contains(Vector3 position) =>
            position.X >= Lower.X && position.Y >= Lower.Y && position.Z >= Lower.Z &&
            position.X <= Upper.X && position.Y <= Upper.Y && position.Z <= Upper.Z;

        //private int GetIntersectionAreaAndBounds(Bounds boxB)
        //{
        //    var boxA = this;

        //    var xA = Math.Max((int)boxA.Lower.X, (int)boxB.Lower.X);
        //    var yA = Math.Max((int)boxA.Lower.Y, (int)boxB.Lower.Y);
        //    var zA = Math.Max((int)boxA.Lower.Z, (int)boxB.Lower.Z);

        //    var xB = Math.Min((int)boxA.Upper.X, (int)boxB.Upper.X);
        //    var yB = Math.Min((int)boxA.Upper.Y, (int)boxB.Upper.Y);
        //    var zB = Math.Min((int)boxA.Upper.Z, (int)boxB.Upper.Z);

        //    return Math.Abs(Math.Max(xB - xA, 0) * Math.Max(yB - yA, 0) * Math.Max(zB - zA, 0));
        //}

        //public bool Contains(Vector3 position) =>
        //    LowerBounds.X >= position.X && LowerBounds.Y >= position.Y && LowerBounds.Z >= position.Z &&
        //    UpperBounds.X <= position.X && UpperBounds.Y <= position.Y && UpperBounds.Z <= position.Z;

        public IEnumerable<Vector3> GetPositionsWithinBounds()
        {
            for (var z = (int) Lower.Z; z <= (int) Upper.Z; z++)
            {
                for (var y = (int) Lower.Y; y <= (int) Upper.Y; y++)
                {
                    for (var x = (int) Lower.X; x <= (int) Upper.X; x++)
                    {
                        yield return new Vector3(x, y, z);
                    }
                }
            }
        }

        //public IEnumerable<Vector3> GetPositionsWithinBoundsAndWithinOuterBounds(Bounds outerBounds) =>
        //    GetPositionsWithinBounds().Where(outerBounds.Contains);
    }

    public static long GetArea(Vector3 lowerBounds, Vector3 upperBounds)
    {
        var size = upperBounds - lowerBounds;
        return Math.Abs((long) size.X * (long) size.Y * (long) size.Z);
    }
}
