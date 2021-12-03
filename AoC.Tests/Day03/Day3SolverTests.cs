using AoC.Day03;

namespace AoC.Tests.Day03;

public class Day3SolverTests
{
    private static SolverBase[] Day3Solvers => new SolverBase[]
    {
        new Day3SolverOriginal(),
        new Day3Solver()
    };

    private const string ExampleInput = @"00100
11110
10110
10111
10101
01111
00111
11100
10000
11001
00010
01010";

    [TestCaseSource(nameof(Day3Solvers))]
    public void Part1Example(SolverBase sut)
    {
        // ACT
        var part1ExampleResult = sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(198);
    }

    [TestCaseSource(nameof(Day3Solvers))]
    public void Part1ReTest(SolverBase sut)
    {
        // ACT
        var part1Result = sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(3009600);
    }

    [TestCaseSource(nameof(Day3Solvers))]
    public void Part2Example(SolverBase sut)
    {
        // ACT
        var part2ExampleResult = sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(230);
    }

    [TestCaseSource(nameof(Day3Solvers))]
    public void Part2ReTest(SolverBase sut)
    {
        // ACT
        var part2Result = sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(6940518);
    }
}
