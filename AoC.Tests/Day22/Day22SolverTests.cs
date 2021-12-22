using AoC.Day22;
using static AoC.Day22.Day22Solver;

namespace AoC.Tests.Day22;

public class Day22SolverTests
{
    private readonly Day22Solver _sut = new();

    private const string ExampleInputSmall = @"on x=10..12,y=10..12,z=10..12
on x=11..13,y=11..13,z=11..13
off x=9..11,y=9..11,z=9..11
on x=10..10,y=10..10,z=10..10";

    private const string ExampleInputLarge = @"on x=-20..26,y=-36..17,z=-47..7
on x=-20..33,y=-21..23,z=-26..28
on x=-22..28,y=-29..23,z=-38..16
on x=-46..7,y=-6..46,z=-50..-1
on x=-49..1,y=-3..46,z=-24..28
on x=2..47,y=-22..22,z=-23..27
on x=-27..23,y=-28..26,z=-21..29
on x=-39..5,y=-6..47,z=-3..44
on x=-30..21,y=-8..43,z=-13..34
on x=-22..26,y=-27..20,z=-29..19
off x=-48..-32,y=26..41,z=-47..-37
on x=-12..35,y=6..50,z=-50..-2
off x=-48..-32,y=-32..-16,z=-15..-5
on x=-18..26,y=-33..15,z=-7..46
off x=-40..-22,y=-38..-28,z=23..41
on x=-16..35,y=-41..10,z=-47..6
off x=-32..-23,y=11..30,z=-14..3
on x=-49..-5,y=-3..45,z=-29..18
off x=18..30,y=-20..-8,z=-3..13
on x=-41..9,y=-7..43,z=-33..15
on x=-54112..-39298,y=-85059..-49293,z=-27449..7877
on x=967..23432,y=45373..81175,z=27513..53682";

    [Test]
    public void GetIntersection_CubesThatDoNotOverlap_ShouldReturnZeroIntersectionArea()
    {
        var rebootSteps = ParseInput(ExampleInputLarge);

        // ACT
        var result = Cube.GetIntersection(rebootSteps.First().Bounds, rebootSteps.Last().Bounds).intersectionArea;

        // ASSERT
        result.Should().Be(0);
    }

    [Test]
    public void GetIntersection_CubesOutOfBounds_ShouldReturnZeroIntersectionArea()
    {
        var rebootSteps = ParseInput(ExampleInputLarge);

        // ACT
        var result = Cube.GetIntersection(rebootSteps.Last().Bounds, new Cube(new Vector3(-50, -50, -50), new Vector3(50, 50, 50))).intersectionArea;

        // ASSERT
        result.Should().Be(0);
    }

    [Test]
    public void GetIntersection_CubesThatExactlyOverlap_ShouldReturnExactlySameArea()
    {
        var rebootSteps = ParseInput(ExampleInputLarge);

        var cube = rebootSteps.First().Bounds;

        // ACT
        var result = Cube.GetIntersection(cube, cube);

        // ASSERT
        result.intersectionArea.Should().Be(cube.Area);
        result.intersection.Should().Be(cube);
        (result.intersection == cube).Should().BeTrue();
    }

    [Test]
    public void GetIntersection_CubesThatPartiallyOverlap_ShouldReturnAreaOfOverlap_Test1()
    {
        var cube1 = new Cube(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
        var cube2 = new Cube(new Vector3(-20, -20, -20), new Vector3(-10, -10, -10));

        // ACT
        var result = Cube.GetIntersection(cube1, cube2).intersectionArea;

        // ASSERT
        result.Should().Be(0);
    }

    [Test]
    public void GetIntersection_CubesThatPartiallyOverlap_ShouldReturnAreaOfOverlap_Test2()
    {
        var cube1 = new Cube(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
        var cube2 = new Cube(new Vector3(1, 1, 1), new Vector3(11, 11, 11));

        // ACT
        var result = Cube.GetIntersection(cube1, cube2);

        // ASSERT
        result.intersectionArea.Should().Be(9 * 9 * 9);
        result.intersection.Should().Be(new Cube(new Vector3(1, 1, 1), new Vector3(10, 10, 10)));
    }

    [Test]
    public void GetIntersection_SmallerCubesTotallyWithinLargerCube_ShouldReturnSmallerCubeAsIntersection()
    {
        var largeCube = new Cube(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
        var smallCube = new Cube(new Vector3(1, 1, 1), new Vector3(3, 3, 3));

        // ACT
        var result = Cube.GetIntersection(smallCube, largeCube);

        // ASSERT
        result.intersectionArea.Should().Be(2 * 2 * 2);
        result.intersection.Should().Be(smallCube);
    }

    [Test]
    public void Part1ExampleSmall()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInputSmall);

        // ASSERT
        part1ExampleResult.Should().Be(39);
    }

    [Test]
    public void Part1ExampleLarge()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInputLarge);

        // ASSERT
        part1ExampleResult.Should().Be(590784);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(553201);
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInputSmall);

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
