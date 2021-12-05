using System.Numerics;
using System.Text.RegularExpressions;

namespace AoC.Day05;

public class Day5Solver : SolverBase
{
    public override string DayName => "Hydrothermal Venture";

    public override long? SolvePart1(PuzzleInput input)
    {
        var noneDiagonalLines = Line.ParseInputToLines(input).Where(line => line.IsNoneDiagonal);
        return NumberOfPointsWhereLinesOverlap(noneDiagonalLines);
    }

    public override long? SolvePart2(PuzzleInput input) => NumberOfPointsWhereLinesOverlap(Line.ParseInputToLines(input));

    /// <summary>
    /// Returns the number of points where at least two of the specified lines overlap.
    /// </summary>
    private static long NumberOfPointsWhereLinesOverlap(IEnumerable<Line> lines)
    {
        lines = lines.ToArray();
        //var points = lines.Select(l => l.Start).Concat(lines.Select(l => l.End)).ToArray();
        //var minBounds = new Vector2(points.Min(p => p.X), points.Min(p => p.Y));
        //var maxBounds = new Vector2(points.Max(p => p.X), points.Max(p => p.Y));
        //var size = maxBounds - minBounds;
        //var map = new int[size.X.Round() + 1, size.Y.Round() + 1];

        var intersections = new HashSet<Vector2>();

        foreach (var line in lines)
        {
            foreach (var otherLine in lines.Where(l => l != line))
            {
                var intersection = Line.Intersection(line, otherLine);
                if (intersection != null)
                {
                    intersections.Add(intersection.Value.Round());
                }
            }
        }

        return intersections.Count;
    }

    private static readonly Regex ParseLinesRegex = new(@"(?<x1>[\d]+),(?<y1>[\d]+) -> (?<x2>[\d]+),(?<y2>[\d]+)", RegexOptions.Compiled);

    public record Line(Vector2 Start, Vector2 End, float A, float B, float C)
    {
        public bool IsNoneDiagonal { get; } = Start.X.Round() == End.X.Round() || Start.Y.Round() == End.Y.Round();

        public static Line[] ParseInputToLines(PuzzleInput input) =>
            ParseLinesRegex.Matches(input.ToString())
                .Select(match => From(
                    new Vector2(int.Parse(match.Groups["x1"].Value), int.Parse(match.Groups["y1"].Value)),
                    new Vector2(int.Parse(match.Groups["x2"].Value), int.Parse(match.Groups["y2"].Value))))
                .ToArray();

        public static Line From(Vector2 start, Vector2 end)
        {
            var x1 = start.X;
            var y1 = start.Y;

            var x2 = end.X;
            var y2 = end.Y;

            var a = y2 - y1;
            var b = x1 - x2;
            var c = a * x1 + b * y1;

            return new Line(start, end, a, b, c);
        }

        /// <summary>
        /// From: https://www.topcoder.com/thrive/articles/Geometry%20Concepts%20part%202:%20%20Line%20Intersection%20and%20its%20Applications#LineLineIntersection
        /// </summary>
        public static Vector2? Intersection(Line line1, Line line2)
        {
            var delta = line1.A * line2.B - line2.A * line1.B;
            if (delta == 0)
            {
                return null; // Lines are parallel
            }

            var x = (line2.B * line1.C - line1.B * line2.C) / delta;
            var y = (line1.A * line2.C - line2.A * line1.C) / delta;

            var point = new Vector2(x, y);

            // Determine if the intersection point is actually on both of the line, rather than the extrapolation of the lines
            // From: https://stackoverflow.com/a/17693146
            bool IsPointOn(Line line)
            {
                var distAC = Vector2.Distance(line.Start, point);
                var distBC = Vector2.Distance(line.End, point);
                var distAB = Vector2.Distance(line.Start, line.End);
                return Math.Abs(distAC + distBC - distAB) < 0.00001;
            }

            return IsPointOn(line1) && IsPointOn(line2) ? point : null;
        }
    }
}
