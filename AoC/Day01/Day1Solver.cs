namespace AoC.Day01;

public class Day1Solver : SolverBase
{
    public override string DayName => "";

    public override long? SolvePart1(PuzzleInput input)
    {
        var i = 0L;
        while (i < 19999999) i += 1;

        return i;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var i = 0L;
        while (i < 29999999) i += 1;

        return i + 364682626;
    }
}
