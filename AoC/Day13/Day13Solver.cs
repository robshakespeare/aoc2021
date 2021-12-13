using System.ComponentModel.DataAnnotations;
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
        var (dots, foldInstructions) = Parse2(input);

        //Console.WriteLine("Width: " + grid[0].Length);
        //Console.WriteLine("Height: " + grid.Count);

        return foldInstructions.First().Fold(dots).Count;
    }

    /// <summary>
    /// Finish folding the transparent paper according to the instructions. The manual says the code is always eight capital letters.
    /// What code do you use to activate the infrared thermal imaging camera system?
    /// </summary>
    public override long? SolvePart2(PuzzleInput input)
    {
        var (dots, foldInstructions) = Parse2(input);

        dots = foldInstructions.Aggregate(dots, (agg, foldInstruction) => foldInstruction.Fold(agg));

        foreach (var line in dots.ToStringGrid(dot => new Vector2(dot.X, dot.Y), _ => '#', '.'))
        {
            Console.WriteLine(line);
        }

        return null;

        ////grid = foldInstructions.Aggregate(grid, Fold);

        //using var sw = new StreamWriter(@"C:\Users\Rob.Shakespeare\OneDrive - BJSS Ltd\Desktop\test.txt");

        //foreach (var foldInstruction in foldInstructions)
        //{
        //    var oldWidth = grid[0].Length;
        //    var oldHeight = grid.Count;

        //    Console.WriteLine($"{foldInstruction} // {oldWidth} x {oldHeight}");



        //    grid = Fold(grid, foldInstruction);



        //    sw.WriteLine(foldInstruction);
        //    foreach (var line in grid)
        //    {
        //        sw.WriteLine(line);
        //    }
        //    sw.WriteLine("");

        //    if (foldInstruction.Axis == 'x')
        //    {
        //        var newWidth = foldInstruction.Position;
        //        var actualWidth = grid[0].Length;
        //        if (newWidth != actualWidth)
        //        {
        //            throw new InvalidOperationException($"Expected width to be {newWidth} but got {actualWidth}");
        //        }

        //        var count = grid.Select(x => x.Length).Distinct().Count();
        //        if (count != 1)
        //        {
        //            throw new InvalidOperationException("huh, widths aren't all same");
        //        }
        //    }

        //    if (foldInstruction.Axis == 'y')
        //    {
        //        var newHeight = foldInstruction.Position;
        //        var actualHeight = grid.Count;
        //        if (newHeight != actualHeight)
        //        {
        //            throw new InvalidOperationException($"Expected height to be {newHeight} but got {actualHeight}");
        //        }
        //    }


        //    //if (grid.Count % 2 == 0)
        //    //{
        //    //    throw new InvalidOperationException("even height");
        //    //}

        //    //if (grid[0].Length % 2 == 0)
        //    //{
        //    //    throw new InvalidOperationException("even width");
        //    //}
        //}

        //foreach (var line in grid)
        //{
        //    Console.WriteLine(line);
        //}

        //return null;
    }

    public record FoldInstruction(char Axis, int Position)
    {
        public bool FoldX { get; } = Axis == 'x';

        public IReadOnlyList<Dot2> Fold(IReadOnlyList<Dot2> dots) => dots.Select(Fold).Distinct().ToArray();

        public Dot2 Fold(Dot2 dot)
        {
            // If the dot is in the section that should not be folded, just return it
            if ((FoldX ? dot.X : dot.Y) < Position)
            {
                return dot;
            }

            int Transpose(int v) => Position - (v - Position);

            // Fold the dot then return the new location
            return FoldX
                ? new Dot2(Transpose(dot.X), dot.Y)
                : new Dot2(dot.X, Transpose(dot.Y));
        }
    }

    public readonly record struct Dot2(int X, int Y);

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

    public static (IReadOnlyList<Dot2> dots, FoldInstruction[] foldInstructions) Parse2(PuzzleInput input)
    {
        var dots = ParseDotsRegex.Matches(input.ToString())
            .Select(m => new Dot2(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value)))
            .ToArray();

        var foldInstructions = ParseFoldsRegex.Matches(input.ToString())
            .Select(m => new FoldInstruction(m.Groups["foldAxis"].Value[0], int.Parse(m.Groups["foldPosition"].Value)))
            .ToArray();

        return (dots, foldInstructions);
    }

    //public static IReadOnlyList<string> Fold(IReadOnlyList<string> grid, FoldInstruction instruction) => Overlay(Split(grid, instruction));

    public static IReadOnlyList<string> Fold(IReadOnlyList<string> grid, FoldInstruction instruction)
    {
        var (axis, position) = instruction;
        var foldX = axis == 'x';

        if (position == 163)
        {
            //position = 110;
        }

        if (foldX)
        {
            grid = GridUtils.RotateGrid(grid, 90);
        }

        // validate fold line
        //if (grid[position].Any(c => c != '.'))
        //{
        //    throw new InvalidOperationException($"Invalid fold {instruction}: {grid[position]}");
        //}

        StringBuilder[] newGrid = grid.Take(position).Select(line => new StringBuilder(line)).ToArray();

        var newGridHeight = newGrid.Length;
        var second = grid.Skip(position + 1).ToArray();

        var blankLine = string.Join("", Enumerable.Repeat(Empty, newGrid[0].Length));

        var blankLinesNeeded = newGridHeight - second.Length;

        var overlay = Enumerable.Repeat(blankLine, blankLinesNeeded).Concat(second.Reverse()).ToArray();

        var readOnlyList = Overlay((newGrid.Select(x => x.ToString()).ToArray(), overlay));

        if (foldX)
        {
            readOnlyList = GridUtils.RotateGrid(readOnlyList, -90);
        }

        return readOnlyList;

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

        grid = newGrid.Select(line => line.ToString()).ToArray();

        if (foldX)
        {
            grid = GridUtils.RotateGrid(grid, -90);
        }

        return grid;

        //StringBuilder[] newGrid;
        //switch (axis)
        //{
        //    case 'y':
        //    {
        //        // validate fold line
        //        if (grid[position].Any(c => c != '.'))
        //        {
        //            throw new InvalidOperationException($"Invalid fold {instruction}: {grid[position]}");
        //        }

        //        newGrid = grid.Take(position).Select(line => new StringBuilder(line)).ToArray();

        //        var newGridHeight = newGrid.Length;
        //        var second = grid.Skip(position + 1);

        //        // n - length - 1
        //        foreach (var (secondLine, secondY) in second.Select((line, y) => (line, y)))
        //        {
        //            var newY = newGridHeight - secondY - 1; //secondY - newGridHeight - 1;
        //            foreach (var (chr, x) in secondLine.Select((chr, x) => (chr, x)))
        //            {
        //                Overlay(newGrid, x, newY, chr);
        //                //newGrid[newY][x] = Overlay(newGrid[newY][x], chr);
        //            }
        //        }

        //        break;
        //    }
        //    case 'x':
        //    {
        //        // validate fold line
        //        if (grid.Select(line => line[position]).Any(c => c != '.'))
        //        {
        //            throw new InvalidOperationException($"Invalid fold {instruction}: {grid[position]}");
        //        }

        //        var first = new List<StringBuilder>();
        //        var second = new List<string>();

        //        foreach (var line in grid)
        //        {
        //            first.Add(new StringBuilder(new string(line.Take(position).ToArray())));
        //            second.Add(new string(line.Skip(position + 1).ToArray()));
        //        }

        //        newGrid = first.ToArray();
        //        var newGridWidth = newGrid.First().Length;

        //        foreach (var (secondLine, y) in second.Select((line, y) => (line, y)))
        //        {
        //            foreach (var (chr, secondX) in secondLine.Select((chr, x) => (chr, x)))
        //            {
        //                var newX = newGridWidth - secondX - 1;
        //                Overlay(newGrid, newX, y, chr);
        //                //newGrid[y][x] = Overlay(newGrid[y][x], chr);
        //            }
        //        }

        //        break;
        //    }
        //    default:
        //        throw new InvalidOperationException("Invalid axis: " + axis);
        //}

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
