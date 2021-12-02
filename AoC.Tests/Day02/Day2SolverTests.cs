using AoC.Day02;

namespace AoC.Tests.Day02;

public class Day2SolverTests
{
    private readonly Day2Solver _sut = new();

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1Result = _sut.SolvePart1(@"forward 5
down 5
forward 8
up 3
down 8
forward 2");

        // ASSERT
        part1Result.Should().Be(150);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(1893605);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2Result = _sut.SolvePart2(@"forward 5
down 5
forward 8
up 3
down 8
forward 2");

        // ASSERT
        part2Result.Should().Be(900);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().NotBe(2120734336);
        part2Result.Should().Be(2120734350);
    }
}
