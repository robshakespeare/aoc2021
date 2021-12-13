using System.Text;

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
        var (grid, foldInstructions) = Parse(input);

        grid = foldInstructions.Aggregate(grid, Fold);

        foreach (var line in grid)
        {
            Console.WriteLine(line);
        }

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

    //public static IReadOnlyList<string> Fold(IReadOnlyList<string> grid, FoldInstruction instruction) => Overlay(Split(grid, instruction));

    public static IReadOnlyList<string> Fold(IReadOnlyList<string> grid, FoldInstruction instruction)
    {
        var (axis, position) = instruction;
        StringBuilder[] newGrid;
        switch (axis)
        {
            case 'y':
            {
                newGrid = grid.Take(position).Select(line => new StringBuilder(line)).ToArray();

                var newGridHeight = newGrid.Length;
                var second = grid.Skip(position + 1);

                // n - length - 1
                foreach (var (secondLine, secondY) in second.Select((line, y) => (line, y)))
                {
                    var newY = newGridHeight - secondY - 1; //secondY - newGridHeight - 1;
                    foreach (var (chr, x) in secondLine.Select((chr, x) => (chr, x)))
                    {
                        Overlay(newGrid, x, newY, chr);
                        //newGrid[newY][x] = Overlay(newGrid[newY][x], chr);
                    }
                }

                break;
            }
            case 'x':
            {
                var first = new List<StringBuilder>();
                var second = new List<string>();

                foreach (var line in grid)
                {
                    first.Add(new StringBuilder(new string(line.Take(position).ToArray())));
                    second.Add(new string(line.Skip(position + 1).ToArray()));
                }

                newGrid = first.ToArray();
                var newGridWidth = newGrid.First().Length;

                foreach (var (secondLine, y) in second.Select((line, y) => (line, y)))
                {
                    foreach (var (chr, secondX) in secondLine.Select((chr, x) => (chr, x)))
                    {
                        var newX = newGridWidth - secondX - 1;
                        Overlay(newGrid, newX, y, chr);
                        //newGrid[y][x] = Overlay(newGrid[y][x], chr);
                    }
                }

                break;
            }
            default:
                throw new InvalidOperationException("Invalid axis: " + axis);
        }

        return newGrid.Select(line => line.ToString()).ToArray();
    }

    public static void Overlay(StringBuilder[] grid, int x, int y, char chr)
    {
        var currentChar = grid[y][x];
        grid[y][x] = Overlay(currentChar, chr);
    }

    public static char Overlay(char chr1, char chr2) => chr1 == Dot || chr2 == Dot ? Dot : Empty;

    //public static IReadOnlyList<string> Overlay((IReadOnlyList<string> first, IReadOnlyList<string> second) parts)
    //{
    //    var (first, second) = parts;
    //    return first.Select((line, y) => string.Join("", line.Select((chr1, x) =>
    //    {
    //        var chr2 = second[y][x];
    //        return chr1 == Dot || chr2 == Dot ? Dot : Empty;
    //    }))).ToArray();
    //}

    //public static IReadOnlyList<string> Overlay((IReadOnlyList<string> first, IReadOnlyList<string> second) parts)
    //{
    //    var (first, second) = parts;
    //    return first.Select((line, y) => string.Join("", line.Select((chr1, x) =>
    //    {
    //        var chr2 = second[y][x];
    //        return chr1 == Dot || chr2 == Dot ? Dot : Empty;
    //    }))).ToArray();
    //}
}
