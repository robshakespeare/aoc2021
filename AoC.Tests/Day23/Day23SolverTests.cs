using AoC.Day23;

namespace AoC.Tests.Day23;

public class Day23SolverTests
{
    private readonly Day23Solver _sut = new();

    private const string ExampleInput = @"#############
#...........#
###B#C#B#D###
  #A#D#C#A#
  #########";

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(11536);
    }

    [Test]
    public void Grid_AdditionalLinesGetInsertedAsExpected()
    {
        var sut = Grid.Parse(ExampleInput, insertAdditionalLines: true);
        sut.GridToString().Should().Be(@"
#############
#...........#
###B#C#B#D###
  #D#C#B#A#
  #D#B#A#C#
  #A#D#C#A#
  #########".TrimStart().NormalizeLineEndings());
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
