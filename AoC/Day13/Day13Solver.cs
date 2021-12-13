namespace AoC.Day13;

public class Day13Solver : SolverBase
{
    public override string DayName => "Transparent Origami";

    public const char Dot = '#';
    public const char Empty = '.';

    private static readonly Regex ParseDotsRegex = new(@"(?<x>\d+),(?<y>\d+)", RegexOptions.Compiled);
    private static readonly Regex ParseFoldsRegex = new(@"fold along (?<foldAxis>.)=(?<foldPosition>\d+)", RegexOptions.Compiled);

    /// <summary>
    /// How many dots are visible after completing just the first fold instruction on your transparent paper?
    /// </summary>
    public override long? SolvePart1(PuzzleInput input)
    {
        var (grid, foldInstructions) = Parse(input);

        grid = Fold(grid, foldInstructions.First());

        return grid.SelectMany(line => line).Count(chr => chr == Dot);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public readonly record struct FoldInstruction(char Axis, int Position);

    public static (IReadOnlyList<string> grid, FoldInstruction[] foldInstructions) Parse(PuzzleInput input)
    {
        var grid = ParseDotsRegex.Matches(input.ToString())
            .Select(m => new { pos = new Vector2(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value)), chr = Dot })
            .ToStringGrid(i => i.pos, i => i.chr, Empty);

        var foldInstructions = ParseFoldsRegex.Matches(input.ToString())
            .Select(m => new FoldInstruction(m.Groups["foldAxis"].Value[0], int.Parse(m.Groups["foldPosition"].Value)))
            .ToArray();

        return (grid, foldInstructions);
    }

    public static IReadOnlyList<string> Fold(IReadOnlyList<string> grid, FoldInstruction instruction) => Overlay(Split(grid, instruction));

    public static (IReadOnlyList<string> first, IReadOnlyList<string> second) Split(IReadOnlyList<string> grid, FoldInstruction instruction)
    {
        var (axis, position) = instruction;
        switch (axis)
        {
            case 'y':
            {
                var first = grid.Take(position).ToArray();
                var second = grid.Skip(position + 1).Reverse().ToArray();
                return (first, second);
            }
            case 'x':
            {
                var first = new List<string>();
                var second = new List<string>();

                foreach (var line in grid)
                {
                    first.Add(new string(line.Take(position).ToArray()));
                    second.Add(new string(line.Skip(position + 1).Reverse().ToArray()));
                }

                return (first, second);
            }
            default:
                throw new InvalidOperationException("Invalid axis: " + axis);
        }
    }

    public static IReadOnlyList<string> Overlay((IReadOnlyList<string> first, IReadOnlyList<string> second) parts)
    {
        var (first, second) = parts;
        return first.Select((line, y) => string.Join("", line.Select((chr1, x) =>
        {
            var chr2 = second[y][x];
            return chr1 == Dot || chr2 == Dot ? Dot : Empty;
        }))).ToArray();
    }

}
