namespace AoC.Day23;

public class Day23Solver : SolverBase
{
    public override string DayName => "Amphipod";

    public override long? SolvePart1(PuzzleInput input) => new Day23Part1Solver(input).SolvePart1();

    public override long? SolvePart2(PuzzleInput input) => new Day23Part2Solver(input).SolvePart2();
}
