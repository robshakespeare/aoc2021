using AoC.Day20;
using static AoC.Day20.Day20Solver;

namespace AoC.Tests.Day20;

public class Day20SolverTests
{
    private readonly Day20Solver _sut = new();

    private const string ExampleInput = @"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###";

    [Test]
    public void GetImageEnhancementIndex_Test()
    {
        var (imageEnhancer, image) = ParseInput(ExampleInput);

        var testPosition = new Vector2(2, 2);

        // ACT
        var result = ImageEnhancer.GetImageEnhancementIndex(image, testPosition);

        // ASSERT
        result.Should().Be(34);
        imageEnhancer.EnhancementAlgorithm[result].Should().Be('#');
        imageEnhancer.GetOutputPixel(image, testPosition).Char.Should().Be('#');
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(35);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().BeLessThan(4942);
        part1Result.Should().BeGreaterThan(4778);
        part1Result.Should().Be(4873);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(3351);
    }

    [Test]
    [LongRunningTest("~1.1 seconds")]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(16394);
    }
}
