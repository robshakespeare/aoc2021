using AoC.Day08;

namespace AoC.Tests.Day08;

public class Day8SolverTests
{
    private readonly Day8Solver _sut = new();

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1Result = _sut.SolvePart1(@"");

        // ASSERT
        part1Result.Should().Be(null);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(null);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2Result = _sut.SolvePart2(@"");

        // ASSERT
        part2Result.Should().Be(null);
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
