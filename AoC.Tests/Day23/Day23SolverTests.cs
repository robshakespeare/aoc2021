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
    public void Part1Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(12521);
    }

    private const int ExpectedPart1ReTestResult = 11536;

    [Test]
    [LongRunningTest("1.4 seconds")]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(ExpectedPart1ReTestResult);
    }

    [Test]
    public void Part1ReTest_OriginalAlmostPaperBasedSolver()
    {
        var originalAlmostPaperBasedSolver = new Day23MyPuzzlePart1Solver(new InputLoader(_sut).PuzzleInputPart1);

        // ACT
        var part1ResultOriginalImpl = originalAlmostPaperBasedSolver.SolvePart1();

        // ASSERT
        part1ResultOriginalImpl.Should().Be(ExpectedPart1ReTestResult);
    }

    [Test]
    public void Grid_AdditionalLinesGetInsertedAsExpected()
    {
        var sut = Grid.Parse(ExampleInput, insertAdditionalLines: true);
        sut.GridAsString.Should().Be(@"
#############
#...........#
###B#C#B#D###
  #D#C#B#A#
  #D#B#A#C#
  #A#D#C#A#
  #########".TrimStart().NormalizeLineEndings());
    }

    [Test]
    public void Grid_Equality_Test()
    {
        var hash = new HashSet<Grid>();

        var grid1 = Grid.Parse(ExampleInput, insertAdditionalLines: true);
        var grid2 = Grid.Parse(ExampleInput, insertAdditionalLines: true);
        var grid3 = grid1.GetSuccessors().First().Grid;
        var grid4 = Grid.Parse(ExampleInput, insertAdditionalLines: false);

        // ACT
        hash.Add(grid1).Should().BeTrue();
        hash.Add(grid3).Should().BeTrue();

        // ASSERT
        hash.Contains(grid1).Should().BeTrue();
        hash.Contains(grid2).Should().BeTrue();
        hash.Contains(grid3).Should().BeTrue();
        hash.Contains(grid2.GetSuccessors().First().Grid).Should().BeTrue();

        hash.Contains(grid4).Should().BeFalse();

        grid1.Equals(grid2).Should().BeTrue();
        grid1.Equals(grid3).Should().BeFalse();
        grid1.Equals(grid4).Should().BeFalse();

        grid1.GetSuccessors().First().Grid.Equals(grid2.GetSuccessors().First().Grid).Should().BeTrue();
    }

    [Test]
    [LongRunningTest("2.1 seconds")]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(44169);
    }

    [Test]
    [LongRunningTest("2.5 seconds")]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(55136);
    }
}
