using AoC.Day21;

namespace AoC.Tests.Day21;

public class Day21SolverTests
{
    private readonly Day21Solver _sut = new();

    private const string ExampleInput = @"Player 1 starting position: 4
Player 2 starting position: 8";

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(739785);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(432450);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().BeLessThan(198806050796934240);
        part2ExampleResult.Should().BeGreaterThan(39896360503505);
        part2ExampleResult.Should().Be(444356092776315);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().BeLessThan(67529729733870744);
        part2Result.Should().BeGreaterThan(13354751908734);
        part2Result.Should().Be(138508043837521);
    }
}
