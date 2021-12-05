using System.Numerics;
using System.Text.RegularExpressions;

namespace AoC.Day05;

public class Day5Solver : SolverBase
{
    public override string DayName => "Hydrothermal Venture";

    public override long? SolvePart1(PuzzleInput input)
    {
        var noneDiagonalLines = Line.ParseInputToLines(input).Where(line => line.IsNoneDiagonal).ToArray();

        var map = new Dictionary<(long X, long Y), long>();

        foreach (var line in noneDiagonalLines)
        {
            var position = line.Start;
            var end = line.End + line.Normal;

            while (position != end)
            {
                var pos = (position.X.Round(), position.Y.Round());
                if (!map.ContainsKey(pos))
                {
                    map[pos] = 1;
                }
                else
                {
                    map[pos] += 1;
                }
                position += line.Normal;
            }

            //do
            //{
            //    var pos = (position.X.Round(), position.Y.Round());
            //    if (!map.ContainsKey(pos))
            //    {
            //        map[pos] = 1;
            //    }
            //    else
            //    {
            //        map[pos] += 1;
            //    }
            //    position += line.Normal;
            //} while (position != line.End);
        }

        Console.WriteLine(map.Select(x => $"<{x.Key.X},{x.Key.Y}>: {x.Value}").Dump());

        return map.Values.Count(n => n > 1);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    private static readonly Regex ParseLinesRegex = new(@"(?<x1>[\d]+),(?<y1>[\d]+) -> (?<x2>[\d]+),(?<y2>[\d]+)", RegexOptions.Compiled);

    public record Line(Vector2 Start, Vector2 End)
    {
        public bool IsNoneDiagonal { get; } = Start.X.Round() == End.X.Round() || Start.Y.Round() == End.Y.Round();

        public float Length { get; } = Vector2.Distance(End, Start);

        public Vector2 Normal { get; } = Vector2.Normalize(End - Start);

        public static Line[] ParseInputToLines(PuzzleInput input) =>
            ParseLinesRegex.Matches(input.ToString())
                .Select(match => new Line(
                    new Vector2(int.Parse(match.Groups["x1"].Value), int.Parse(match.Groups["y1"].Value)),
                    new Vector2(int.Parse(match.Groups["x2"].Value), int.Parse(match.Groups["y2"].Value))))
                .ToArray();
    }
}
