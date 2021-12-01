using AoC.Day01;

namespace AoC.Tests.Day01;

public class Day1SolverTests
{
    private readonly Day1Solver _sut = new();

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1Result = _sut.SolvePart1(@"199
200
208
210
200
207
240
269
260
263");

        // ASSERT
        part1Result.Should().Be(7);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1715);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2Result = _sut.SolvePart2(@"199
200
208
210
200
207
240
269
260
263");

        // ASSERT
        part2Result.Should().Be(5);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(1739);
    }
}
