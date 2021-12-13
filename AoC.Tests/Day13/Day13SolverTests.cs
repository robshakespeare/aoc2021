using AoC.Day13;

namespace AoC.Tests.Day13;

public class Day13SolverTests
{
    private readonly Day13Solver _sut = new();

    private const string ExampleInput = @"6,10
0,14
9,10
0,3
10,4
4,11
6,0
6,12
4,1
0,13
10,12
3,4
3,0
8,4
1,10
2,14
8,10
9,0

fold along y=7
fold along x=5";

    [Test]
    public void Part1Example1()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(17);
    }

    [Test]
    public void Part1Example2()
    {
        var (grid, foldInstructions) = Day13Solver.Parse(ExampleInput);

        // ACT
        grid = Day13Solver.Fold(grid, foldInstructions.ElementAt(0));
        grid = Day13Solver.Fold(grid, foldInstructions.ElementAt(1));

        // ASSERT
        grid.SelectMany(line => line).Count(chr => chr == Day13Solver.Dot).Should().Be(16);

        grid.Should().BeEquivalentTo(new[]
        {
            "#####",
            "#...#",
            "#...#",
            "#...#",
            "#####",
            ".....",
            "....."
        }, opts => opts.WithStrictOrdering());
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().NotBe(749);
        part1Result.Should().Be(647);
    }

    [Test]
    public void FoldXTest()
    {
        var input = new[]
        {
            ".....##",
            "..#...#",
            "##.....",
            "#.....#",
            "..##...",
            ".#.#.##",
            "#......"
        };

        // ACT
        var result = Day13Solver.Fold(input, new Day13Solver.FoldInstruction('x', 4));

        // ASSERT
        result.Should().BeEquivalentTo(new[]
        {
            "..##",
            "..#.",
            "##..",
            "#.#.",
            "..##",
            ".###",
            "#..."
        }, opts => opts.WithStrictOrdering());
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
