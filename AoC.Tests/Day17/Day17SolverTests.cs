using AoC.Day17;

namespace AoC.Tests.Day17;

public class Day17SolverTests
{
    private readonly Day17Solver _sut = new();

    private const string ExampleInput = @"target area: x=20..30, y=-10..-5";

    [TestCase(7, 2, true, 3)]
    [TestCase(6, 3, true, 6)]
    [TestCase(6, 9, true, 45)]
    [TestCase(9, 0, true, 0)]
    [TestCase(17, -4, false, 0)]
    [TestCase(3, 3, false, 6)]
    [TestCase(1, 1, false, 1)]
    [TestCase(0, 0, false, 0)]
    public void TryVelocity_Tests(int initialVelocityX, int initialVelocityY, bool expectedSuccess, long expectedMaxHeight)
    {
        var target = Day17Solver.InputToTargetBounds(ExampleInput);
        var velocity = new Vector2(initialVelocityX, initialVelocityY);

        // ACT
        var result = Day17Solver.TryVelocity(target, velocity);

        // ASSERT
        using (new AssertionScope())
        {
            result.success.Should().Be(expectedSuccess);
            result.maxHeight.Should().Be(expectedMaxHeight);
        }
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(45);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(2850);
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
