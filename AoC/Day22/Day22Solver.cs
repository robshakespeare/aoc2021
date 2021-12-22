namespace AoC.Day22;

public class Day22Solver : SolverBase
{
    public override string DayName => "Reactor Reboot";

    public override long? SolvePart1(PuzzleInput input)
    {
        var activeCuboids = new List<Cube>();
        var rebootSteps = ParseInput(input);
        var outerBounds = new Cube(new Vector3(-50, -50, -50), new Vector3(50, 50, 50));

        foreach (var (isSet, candidateCube) in rebootSteps)
        {
            var cubeBoundsIntersection = Cube.GetIntersection(candidateCube, outerBounds);

            // We just deal with the part of the cube that is within our bounds
            // If this is the whole cube, that's fine
            // If this is the part of the cube within the bounds, that is fine
            // If the cube totally lies out of the bounds, there is no intersection and we can ignore the cube
            if (cubeBoundsIntersection.intersectionArea > 0)
            {
                var cubeWithinBounds = cubeBoundsIntersection.intersection;

                // Next, for each active cube, we get get the intersection and exceptions, and deal with adding/removing as necessary
                // But if we have no active cubes, we need to add the current cube if we are turning on
                if (activeCuboids.Count == 0)
                {
                    if (isSet)
                    {
                        activeCuboids.Add(cubeWithinBounds);
                    }
                }
                else
                {
                    foreach (var activeCuboid in activeCuboids.ToArray())
                    {
                        var (_, exceptionBoxes) = Cube.GetIntersectionAndExceptionCubes(activeCuboid, cubeWithinBounds);
                        if (isSet)
                        {
                            // i.e. we are turning ON
                            // Only turn on the exceptions (i.e. the additional cubes)
                            activeCuboids.AddRange(exceptionBoxes);
                        }
                        else
                        {
                            // i.e. we are turning OFF
                            // Remove the whole old active cuboid
                            // And only re-add the exceptions that are within the current active cuboid
                            // i.e. the end state is that only the active cubes that were not inside the area to turn off are kept
                            activeCuboids.Remove(activeCuboid);
                            activeCuboids.AddRange(exceptionBoxes.Where(exceptionBox => activeCuboid.Contains(exceptionBox)));
                        }
                    }
                }
            }
        }

        return activeCuboids.Sum(x => x.Area);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

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
            new Cube(
                Lower: new Vector3(Math.Min(x1, x2), Math.Min(y1, y2), Math.Min(z1, z2)),
                Upper: new Vector3(Math.Max(x1, x2), Math.Max(y1, y2), Math.Max(z1, z2))));
    }).ToArray();

    public record RebootStep(bool IsSet, Cube Cube);

    public record Cube(Vector3 Lower, Vector3 Upper)
    {
        ////public Vector3 Size { get; } = Upper - Lower;

        public int Area { get; } = GetArea(Lower, Upper);

        public static (int intersectionArea, Cube intersection) GetIntersection(Cube cubeA, Cube cubeB)
        {
            var xA = Math.Max((int) cubeA.Lower.X, (int) cubeB.Lower.X);
            var yA = Math.Max((int) cubeA.Lower.Y, (int) cubeB.Lower.Y);
            var zA = Math.Max((int) cubeA.Lower.Z, (int) cubeB.Lower.Z);

            var xB = Math.Min((int) cubeA.Upper.X, (int) cubeB.Upper.X);
            var yB = Math.Min((int) cubeA.Upper.Y, (int) cubeB.Upper.Y);
            var zB = Math.Min((int) cubeA.Upper.Z, (int) cubeB.Upper.Z);

            var intersectionArea = Math.Abs(Math.Max(xB - xA, 0) * Math.Max(yB - yA, 0) * Math.Max(zB - zA, 0));

            var intersection = new Cube(new Vector3(xA, yA, zA), new Vector3(xB, yB, zB));

            return (intersectionArea, intersection);
        }

        public static (Cube? intersection, IReadOnlyList<Cube> exceptionBoxes) GetIntersectionAndExceptionCubes(Cube cubeA, Cube cubeB)
        {
            // When the 2 input boxes match exactly, either will be the intersection box, and there will be no exception boxes.
            if (cubeA == cubeB)
            {
                return (cubeA, Array.Empty<Cube>());
            }

            // If there is no intersection, that means the boxes don't overlap, so just return the 2 input boxes as the exception boxes.
            var (intersectionArea, intersection) = GetIntersection(cubeA, cubeB);
            if (intersectionArea == 0)
            {
                return (null, new[] {cubeA, cubeB});
            }

            // Otherwise, there will be the single intersection box, and some exception boxes
            // Work out all of the boxes, and any whose areas are 0, exclude them

            var exceptionCubes = GetExceptionCubes(cubeA, intersection)
                .Concat(GetExceptionCubes(cubeB, intersection))
                .ToArray();

            return (intersection, exceptionCubes);

            //// rs-todo: work out the exception boxes

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the cubes that form part of the specified `cube` except the `intersection`.
        /// The `intersection` must be a true intersection of `cube`, otherwise this method will return unexpected/undefined results.
        /// </summary>
        public static IReadOnlyList<Cube> GetExceptionCubes(Cube cube, Cube intersection)
        {
            var exceptions = new Cube[]
            {
                new(cube.Lower, new Vector3(intersection.Lower.X, cube.Upper.Y, cube.Upper.Z)), // top slab
                new(new Vector3(intersection.Upper.X, cube.Lower.Y, cube.Lower.Z), cube.Upper), // bottom slab

                new(new Vector3(intersection.Lower.X, cube.Lower.Y, cube.Lower.Z),
                    new Vector3(intersection.Upper.X, cube.Upper.Y, intersection.Lower.Z)), // front slab

                new(new Vector3(intersection.Lower.X, cube.Lower.Y, intersection.Upper.Z),
                    new Vector3(intersection.Upper.X, cube.Upper.Y, cube.Upper.Z)), // back slab

                new(new Vector3(intersection.Lower.X, cube.Lower.Y, intersection.Lower.Z),
                    new Vector3(intersection.Upper.X, intersection.Lower.Y, intersection.Upper.Z)), // left slab

                new(new Vector3(intersection.Lower.X, intersection.Upper.Y, intersection.Lower.Z),
                    new Vector3(intersection.Upper.X, cube.Upper.Y, intersection.Upper.Z)) // right slab
            };

            // filer out zero areas
            return exceptions.Where(x => x.Area > 0).ToArray();
        }

        /// <summary>
        /// Returns true if this box totally contains the other box.
        /// </summary>
        public bool Contains(Cube otherCube) => Contains(otherCube.Lower) && Contains(otherCube.Upper);

        /// <summary>
        /// Returns true if this box contains the specified position.
        /// </summary>
        public bool Contains(Vector3 position) =>
            Lower.X >= position.X && Lower.Y >= position.Y && Lower.Z >= position.Z &&
            Upper.X <= position.X && Upper.Y <= position.Y && Upper.Z <= position.Z;

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

        //public IEnumerable<Vector3> GetPositionsWithinBounds()
        //{
        //    for (var z = (int) LowerBounds.Z; z <= (int) UpperBounds.Z; z++)
        //    {
        //        for (var y = (int) LowerBounds.Y; y <= (int) UpperBounds.Y; y++)
        //        {
        //            for (var x = (int) LowerBounds.X; x <= (int) UpperBounds.X; x++)
        //            {
        //                yield return new Vector3(x, y, z);
        //            }
        //        }
        //    }
        //}

        //public IEnumerable<Vector3> GetPositionsWithinBoundsAndWithinOuterBounds(Bounds outerBounds) =>
        //    GetPositionsWithinBounds().Where(outerBounds.Contains);
    }

    public static int GetArea(Vector3 lowerBounds, Vector3 upperBounds)
    {
        var size = upperBounds - lowerBounds;
        return Math.Abs((int) size.X * (int) size.Y * (int) size.Z);
    }
}
