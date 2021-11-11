namespace AoC.Day00;

public class Day0Solver : SolverBase
{
    public override string DayName => "Test Day";

    public override long? SolvePart1(PuzzleInput input)
    {
        var numbers = input.ReadLinesAsLongs().ToArray();

        return MathUtils.LeastCommonMultiple(numbers[0], numbers[1]);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var numbers = input.ToString().Split(',').Select(long.Parse).ToArray();

        return MathUtils.LeastCommonMultiple(numbers[0], numbers[1], numbers[2]);
    }
}
