namespace AoC.Day23;

public class Day23Part2Solver : Day23Base
{
    public Day23Part2Solver(PuzzleInput input)
        : base(InsertAdditionalLines(input))
    {
    }

    public static PuzzleInput InsertAdditionalLines(PuzzleInput originalInput)
    {
        var lines = originalInput.ReadLines().ToList();

        lines.InsertRange(3, new[]
        {
            "  #D#C#B#A#",
            "  #D#B#A#C#"
        });

        return new PuzzleInput(string.Join(Environment.NewLine, lines));
    }

    public long? SolvePart2()
    {
        return null;
    }
}
