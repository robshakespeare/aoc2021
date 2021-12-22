namespace AoC.Day22;

public class Day22Solver : SolverBase
{
    public override string DayName => "Reactor Reboot";

    public override long? SolvePart1(PuzzleInput input)
    {
        var activeCubes = new HashSet<Vector3>();
        var rebootSteps = ParseInput(input);
        var outerBounds = new Bounds(new Vector3(-50, -50, -50), new Vector3(50, 50, 50));

        //foreach (var (isSet, bounds) in rebootSteps)
        //{
        //    foreach (var position in bounds.GetPositionsWithinBoundsAndWithinOuterBounds(outerBounds))
        //    {
        //        if (isSet)
        //        {
        //            activeCubes.Add(position);
        //        }
        //        else
        //        {
        //            activeCubes.Remove(position);
        //        }
        //    }
        //}

        return activeCubes.Count;
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
            new Bounds(
                Lower: new Vector3(Math.Min(x1, x2), Math.Min(y1, y2), Math.Min(z1, z2)),
                Upper: new Vector3(Math.Max(x1, x2), Math.Max(y1, y2), Math.Max(z1, z2))));
    }).ToArray();

    public record RebootStep(bool IsSet, Bounds Bounds)
    {

    }

    public record Bounds(Vector3 Lower, Vector3 Upper)
    {
        public Vector3 Size { get; } = Upper - Lower;

        public int Area { get; } = GetArea(Lower, Upper);

        public (int intersectionArea, Bounds intersection) GetIntersectionArea(Bounds boxB)
        {
            var boxA = this;

            var xA = Math.Max((int) boxA.Lower.X, (int) boxB.Lower.X);
            var yA = Math.Max((int) boxA.Lower.Y, (int) boxB.Lower.Y);
            var zA = Math.Max((int) boxA.Lower.Z, (int) boxB.Lower.Z);

            var xB = Math.Min((int) boxA.Upper.X, (int) boxB.Upper.X);
            var yB = Math.Min((int) boxA.Upper.Y, (int) boxB.Upper.Y);
            var zB = Math.Min((int) boxA.Upper.Z, (int) boxB.Upper.Z);

            var intersectionArea = Math.Abs(Math.Max(xB - xA, 0) * Math.Max(yB - yA, 0) * Math.Max(zB - zA, 0));

            var intersection = new Bounds(new Vector3(xA, yA, zA), new Vector3(xB, yB, zB));

            return (intersectionArea, intersection);
        }

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
