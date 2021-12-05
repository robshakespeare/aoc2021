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

        var points = lines.Select(l => l.Start).Concat(lines.Select(l => l.End)).ToArray();

        var minBounds = new Vector2(points.Min(p => p.X), points.Min(p => p.Y));
        var maxBounds = new Vector2(points.Max(p => p.X), points.Max(p => p.Y));
        var size = maxBounds - minBounds;

        var map = new int[size.X.Round() + 1, size.Y.Round() + 1];

        foreach (var line in lines)
        {
            var position = line.Start;
            var end = line.End + line.NormalRounded;

            while (position != end)
            {
                var localPos = position - minBounds;
                map[localPos.X.Round(), localPos.Y.Round()] += 1;
                position += line.NormalRounded;
            }
        }

        long count = 0;

        for (var x = 0; x < map.GetLength(0); x++)
            for (var y = 0; y < map.GetLength(1); y++)
                if (map[x, y] > 1)
                    count++;

        return count;
    }

    private static readonly Regex ParseLinesRegex = new(@"(?<x1>[\d]+),(?<y1>[\d]+) -> (?<x2>[\d]+),(?<y2>[\d]+)", RegexOptions.Compiled);

    public record Line(Vector2 Start, Vector2 End)
    {
        public bool IsNoneDiagonal { get; } = Start.X.Round() == End.X.Round() || Start.Y.Round() == End.Y.Round();

        public Vector2 NormalRounded { get; } = Vector2.Normalize(End - Start).Round();

        public static Line[] ParseInputToLines(PuzzleInput input) =>
            ParseLinesRegex.Matches(input.ToString())
                .Select(match => new Line(
                    new Vector2(int.Parse(match.Groups["x1"].Value), int.Parse(match.Groups["y1"].Value)),
                    new Vector2(int.Parse(match.Groups["x2"].Value), int.Parse(match.Groups["y2"].Value))))
                .ToArray();
    }
}
