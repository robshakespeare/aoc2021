using AoC.Day25;
using static AoC.Day25.Day25Solver;

namespace AoC.Tests.Day25;

public class Day25SolverTests
{
    private readonly Day25Solver _sut = new();

    [Test]
    public void EastFacingOnlyExampleTest()
    {
        var grid = new Grid("...>>>>>...");

        // ACT & ASSERT #1
        grid.Step().Should().Be(1);
        grid.ToString().Should().Be("...>>>>.>..");

        // ACT & ASSERT #2
        grid.Step().Should().Be(2);
        grid.ToString().Should().Be("...>>>.>.>.");
    }

    [Test]
    public void EastFirstThenSouthNext_InASingleStep_Example_Test()
    {
        var grid = new Grid(@"..........
.>v....v..
.......>..
..........");

        // ACT & ASSERT
        grid.Step();
        grid.ToString().Should().Be(@"..........
.>........
..v....v>.
..........".NormalizeLineEndings());
    }

    [Test]
    public void StringCurrentCauseOverlap_Example_Test()
    {
        var grid = new Grid(@"...>...
.......
......>
v.....>
......>
.......
..vvv..");

        // ACT & ASSERT #1
        grid.Step();
        grid.ToString().Should().Be(@"..vv>..
.......
>......
v.....>
>......
.......
....v..".NormalizeLineEndings());

        // ACT & ASSERT #2
        grid.Step();
        grid.ToString().Should().Be(@"....v>.
..vv...
.>.....
......>
v>.....
.......
.......".NormalizeLineEndings());

        // ACT & ASSERT #3
        grid.Step();
        grid.ToString().Should().Be(@"......>
..v.v..
..>v...
>......
..>....
v......
.......".NormalizeLineEndings());

        // ACT & ASSERT #4
        grid.Step();
        grid.ToString().Should().Be(@">......
..v....
..>.v..
.>.v...
...>...
.......
v......".NormalizeLineEndings());
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(@"v...>>.vv>
.vv>>.vv..
>>.>v>...v
>>v>>.>.v.
v>v.vv.v..
>.>>..v...
.vv..>.>v.
v.v..>>v.v
....v..v.>");

        // ASSERT
        part1ExampleResult.Should().Be(58);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(308);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Contain("Sleigh keys detected!");
        part2Result.Should().Contain("Advent of Code 2021 is complete.");
        part2Result.Should().Contain("Merry Christmas & Happy New Year!");
    }
}
