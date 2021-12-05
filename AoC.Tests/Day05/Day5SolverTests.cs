using AoC.Day05;

namespace AoC.Tests.Day05;

public class Day5SolverTests
{
    private static SolverBase[] Day5Solvers => new SolverBase[]
    {
        new Day5SolverOriginal(),
        new Day5Solver()
    };

    private const string ExampleInput = @"0,9 -> 5,9
8,0 -> 0,8
9,4 -> 3,4
2,2 -> 2,1
7,0 -> 7,4
6,4 -> 2,0
0,9 -> 2,9
3,4 -> 1,4
0,0 -> 8,8
5,5 -> 8,2";

    [TestCaseSource(nameof(Day5Solvers))]
    public void Part1Example(SolverBase sut)
    {
        // ACT
        var part1ExampleResult = sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(5);
    }

    [TestCaseSource(nameof(Day5Solvers))]
    public void Part1ReTest(SolverBase sut)
    {
        // ACT
        var part1Result = sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(4745);
    }

    [TestCaseSource(nameof(Day5Solvers))]
    public void Part2Example(SolverBase sut)
    {
        // ACT
        var part2ExampleResult = sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(12);
    }

    [TestCaseSource(nameof(Day5Solvers))]
    public void Part2ReTest(SolverBase sut)
    {
        // ACT
        var part2Result = sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(18442);
    }
}
