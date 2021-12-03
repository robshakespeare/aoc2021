using AoC.Day01;

namespace AoC.Tests.Day01;

public class Day1SolverTests
{
    private static SolverBase[] Day1Solvers => new SolverBase[]
    {
        new Day1Solver(),
        new Day1SolverV2()
    };

    private const string ExampleInput = @"199
200
208
210
200
207
240
269
260
263";

    [TestCaseSource(nameof(Day1Solvers))]
    public void Part1Example(SolverBase sut)
    {
        // ACT
        var part1ExampleResult = sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(7);
    }

    [TestCaseSource(nameof(Day1Solvers))]
    public void Part1ReTest(SolverBase sut)
    {
        // ACT
        var part1Result = sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1715);
    }

    [TestCaseSource(nameof(Day1Solvers))]
    public void Part2Example(SolverBase sut)
    {
        // ACT
        var part2ExampleResult = sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(5);
    }

    [TestCaseSource(nameof(Day1Solvers))]
    public void Part2ReTest(SolverBase sut)
    {
        // ACT
        var part2Result = sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(1739);
    }
}
