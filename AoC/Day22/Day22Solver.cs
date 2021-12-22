namespace AoC.Day22;

public class Day22Solver : SolverBase
{
    public override string DayName => "Reactor Reboot";

    public override long? SolvePart1(PuzzleInput input)
    {
        var rebootSteps = ParseInput(input);
        var initializationProcedureBounds = new Cube(new Vector3(-50, -50, -50), new Vector3(50, 50, 50));

        var size = initializationProcedureBounds.Size + Vector3.One; // Note plus 1 in all directions because bounds are inclusive

        var orgSizeHalf = initializationProcedureBounds.Size / 2;
        Vector3 ShiftPosToIndex(Vector3 pos) => pos + orgSizeHalf;

        var grid3D =
            Enumerable.Range(0, (int) size.Z).Select(
                _ => Enumerable.Range(0, (int) size.Y).Select(
                    _ => Enumerable.Range(0, (int) size.X).Select(_ => false).ToArray()).ToArray()).ToArray();

        foreach (var (isSet, bounds) in rebootSteps)
        {
            if (initializationProcedureBounds.Contains(bounds))
            {
                foreach (var position in bounds.GetPositionsWithinBounds().Select(ShiftPosToIndex))
                {
                    grid3D[(int) position.Z][(int) position.Y][(int) position.X] = isSet;
                }
            }
        }

        var count = grid3D.Sum(z => z.Sum(y => y.Count(x => x)));

        return count;
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
                Min: new Vector3(Math.Min(x1, x2), Math.Min(y1, y2), Math.Min(z1, z2)),
                Max: new Vector3(Math.Max(x1, x2), Math.Max(y1, y2), Math.Max(z1, z2))));
    }).ToArray();

    public record RebootStep(bool IsSet, Cube Bounds);

    public record Cube(Vector3 Min, Vector3 Max)
    {
        public Vector3 Size { get; } = Max - Min;

        public long Area { get; } = GetArea(Min, Max);

        public static (long intersectionArea, Cube intersection) GetIntersection(Cube boxA, Cube boxB)
        {
            var xA = Math.Max((int) boxA.Min.X, (int) boxB.Min.X);
            var yA = Math.Max((int) boxA.Min.Y, (int) boxB.Min.Y);
            var zA = Math.Max((int) boxA.Min.Z, (int) boxB.Min.Z);

            var xB = Math.Min((int) boxA.Max.X, (int) boxB.Max.X);
            var yB = Math.Min((int) boxA.Max.Y, (int) boxB.Max.Y);
            var zB = Math.Min((int) boxA.Max.Z, (int) boxB.Max.Z);

            var intersectionArea = Math.Abs(Math.Max(xB - xA, 0) * Math.Max(yB - yA, 0) * Math.Max(zB - zA, 0));

            var intersection = new Cube(new Vector3(xA, yA, zA), new Vector3(xB, yB, zB));

            return (intersectionArea, intersection);
        }

        /// <summary>
        /// Returns true if this box totally contains the other box.
        /// </summary>
        public bool Contains(Cube otherCube) => Contains(otherCube.Min) && Contains(otherCube.Max);

        /// <summary>
        /// Returns true if this box contains the specified position.
        /// </summary>
        public bool Contains(Vector3 position) =>
            position.X >= Min.X && position.Y >= Min.Y && position.Z >= Min.Z &&
            position.X <= Max.X && position.Y <= Max.Y && position.Z <= Max.Z;

        public IEnumerable<Vector3> GetPositionsWithinBounds()
        {
            for (var z = (int) Min.Z; z <= (int) Max.Z; z++)
            {
                for (var y = (int) Min.Y; y <= (int) Max.Y; y++)
                {
                    for (var x = (int) Min.X; x <= (int) Max.X; x++)
                    {
                        yield return new Vector3(x, y, z);
                    }
                }
            }
        }

        public static long GetArea(Vector3 lowerBounds, Vector3 upperBounds)
        {
            var size = upperBounds - lowerBounds;
            return Math.Abs((long) size.X * (long) size.Y * (long) size.Z);
        }
    }
}
