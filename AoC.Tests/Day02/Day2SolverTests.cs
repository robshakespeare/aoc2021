using AoC.Day02;

namespace AoC.Tests.Day02;

public class Day2SolverTests
{
    private const string ExampleInput = @"forward 5
down 5
forward 8
up 3
down 8
forward 2";

    private static SolverBase[] Day2Solvers => new SolverBase[]
    {
        new Day2Solver(),
        new Day2SolverV2()
    };

    [TestCaseSource(nameof(Day2Solvers))]
    public void Part1Example(SolverBase sut)
    {
        // ACT
        var part1ExampleResult = sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(150);
    }

    [TestCaseSource(nameof(Day2Solvers))]
    public void Part1ReTest(SolverBase sut)
    {
        // ACT
        var part1Result = sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1893605);
    }

    [TestCaseSource(nameof(Day2Solvers))]
    public void Part2Example(SolverBase sut)
    {
        // ACT
        var part2ExampleResult = sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(900);
    }

    [TestCaseSource(nameof(Day2Solvers))]
    public void Part2ReTest(SolverBase sut)
    {
        // ACT
        var part2Result = sut.SolvePart2();

        // ASSERT
        part2Result.Should().NotBe(2120734336);
        part2Result.Should().Be(2120734350);
    }
}
