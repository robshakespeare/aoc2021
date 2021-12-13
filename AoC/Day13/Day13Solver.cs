namespace AoC.Day13;

public class Day13Solver : SolverBase<long?, string>
{
    public override string DayName => "Transparent Origami";

    /// <summary>
    /// How many dots are visible after completing just the first fold instruction on your transparent paper?
    /// </summary>
    public override long? SolvePart1(PuzzleInput input)
    {
        var (dots, foldInstructions) = Parse(input);

        return foldInstructions.First().Fold(dots).Count;
    }

    /// <summary>
    /// Finish folding the transparent paper according to the instructions. The manual says the code is always eight capital letters.
    /// What code do you use to activate the infrared thermal imaging camera system?
    /// </summary>
    public override string SolvePart2(PuzzleInput input)
    {
        var (dots, foldInstructions) = Parse(input);

        dots = foldInstructions.Aggregate(dots, (agg, foldInstruction) => foldInstruction.Fold(agg));

        return DotsToOutputString(dots);
    }

    public static string DotsToOutputString(IReadOnlyList<Vector2> dots) =>
        string.Join(Environment.NewLine, dots.ToStringGrid(dot => dot, _ => '#', ' '));

    public record FoldInstruction(char Axis, int Position)
    {
        private bool FoldX { get; } = Axis == 'x';

        public IReadOnlyList<Vector2> Fold(IReadOnlyList<Vector2> dots) => dots.Select(Fold).Distinct().ToArray();

        private Vector2 Fold(Vector2 dot)
        {
            // If the dot is in the section that should not be folded, just return it
            if ((FoldX ? dot.X : dot.Y) < Position)
            {
                return dot;
            }

            // Fold the dot then return the new location
            float Transpose(float coordinate) => Position - (coordinate - Position);
            return FoldX
                ? new Vector2(Transpose(dot.X), dot.Y)
                : new Vector2(dot.X, Transpose(dot.Y));
        }
    }

    private static readonly Regex ParseDotsRegex = new(@"(?<x>\d+),(?<y>\d+)", RegexOptions.Compiled);
    private static readonly Regex ParseFoldsRegex = new(@"fold along (?<foldAxis>.)=(?<foldPosition>\d+)", RegexOptions.Compiled);

    public static (IReadOnlyList<Vector2> dots, FoldInstruction[] foldInstructions) Parse(PuzzleInput input)
    {
        var dots = ParseDotsRegex.Matches(input.ToString())
            .Select(m => new Vector2(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value)))
            .ToArray();

        var foldInstructions = ParseFoldsRegex.Matches(input.ToString())
            .Select(m => new FoldInstruction(m.Groups["foldAxis"].Value[0], int.Parse(m.Groups["foldPosition"].Value)))
            .ToArray();

        return (dots, foldInstructions);
    }
}
