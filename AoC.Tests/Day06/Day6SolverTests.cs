using AoC.Day06;

namespace AoC.Tests.Day06;

public class Day6SolverTests
{
    private readonly Day6Solver _sut = new();

    private const string ExampleInput = @"3,4,3,1,2";

    [Test]
    public void Part1Example1()
    {
        // ACT
        var part1ExampleResult = Day6Solver.Simulate(ExampleInput, 18);

        // ASSERT
        part1ExampleResult.Count.Should().Be(26);
    }

    [Test]
    public void Part1Example2()
    {
        // ACT
        var part1ExampleResult = Day6Solver.Simulate(ExampleInput, 80);

        // ASSERT
        part1ExampleResult.Count.Should().Be(5934);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(362740);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(null);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(null);
    }
}
