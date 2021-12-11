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
        var map = new Dictionary<Vector2, long>();

        foreach (var line in lines)
        {
            var position = line.Start;
            var end = line.End + line.ManhattanNormal;

            while (position != end)
            {
                if (!map.ContainsKey(position))
                    map[position] = 1;
                else
                    map[position] += 1;

                position += line.ManhattanNormal;
            }
        }

        return map.Values.Count(n => n > 1);
    }

    private static readonly Regex ParseLinesRegex = new(@"(?<x1>[\d]+),(?<y1>[\d]+) -> (?<x2>[\d]+),(?<y2>[\d]+)", RegexOptions.Compiled);

    public record Line(Vector2 Start, Vector2 End)
    {
        public bool IsNoneDiagonal { get; } = Start.X.Round() == End.X.Round() || Start.Y.Round() == End.Y.Round();

        public Vector2 ManhattanNormal { get; } = Vector2.Normalize(End - Start).Round();

        public static Line[] ParseInputToLines(PuzzleInput input) =>
            ParseLinesRegex.Matches(input.ToString())
                .Select(match => new Line(
                    new Vector2(int.Parse(match.Groups["x1"].Value), int.Parse(match.Groups["y1"].Value)),
                    new Vector2(int.Parse(match.Groups["x2"].Value), int.Parse(match.Groups["y2"].Value))))
                .ToArray();
    }
}
