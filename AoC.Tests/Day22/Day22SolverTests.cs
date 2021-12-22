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
    public void GetExceptionCubes_OtherCubeInMiddleAllAxis()
    {
        var cube1 = new Cube(new Vector3(0, 0, 0), new Vector3(3, 3, 3));
        var cube2 = new Cube(new Vector3(1, 1, 1), new Vector3(2, 2, 2));

        var intersection = Cube.GetIntersection(cube1, cube2);

        // (pre-assert)
        intersection.intersectionArea.Should().Be(1);
        intersection.intersection.Should().Be(cube2);

        // ACT
        var result = Cube.GetExceptionCubes(cube1, intersection.intersection);

        // ASSERT
        result.Should().BeEquivalentTo(new[]
        {
            new Cube(new Vector3(0, 0, 0), new Vector3(1, 3, 3)), // top slab
            new Cube(new Vector3(2, 0, 0), new Vector3(3, 3, 3)), // bottom slab
            new Cube(new Vector3(1, 0, 0), new Vector3(2, 3, 1)), // front slab
            new Cube(new Vector3(1, 0, 2), new Vector3(2, 3, 3)), // back slab
            new Cube(new Vector3(1, 0, 1), new Vector3(2, 1, 2)), // left slab
            new Cube(new Vector3(1, 2, 1), new Vector3(2, 3, 2)) // right slab
        }, opts => opts.WithStrictOrdering());
    }

    [Test]
    public void GetExceptionCubes_OtherCubeInMiddleAllAxis_ButAtTop()
    {
        var cube1 = new Cube(new Vector3(0, 0, 0), new Vector3(3, 3, 3));
        var cube2 = new Cube(new Vector3(0, 1, 1), new Vector3(1, 2, 2));

        var intersection = Cube.GetIntersection(cube1, cube2);

        // (pre-assert)
        intersection.intersectionArea.Should().Be(1);
        intersection.intersection.Should().Be(cube2);

        // ACT
        var result = Cube.GetExceptionCubes(cube1, intersection.intersection);

        // ASSERT
        result.Should().BeEquivalentTo(new[]
        {
            new Cube(new Vector3(1, 0, 0), new Vector3(3, 3, 3)), // bottom slab
            new Cube(new Vector3(0, 0, 0), new Vector3(1, 3, 1)), // front slab
            new Cube(new Vector3(0, 0, 2), new Vector3(1, 3, 3)), // back slab
            new Cube(new Vector3(0, 0, 1), new Vector3(1, 1, 2)), // left slab
            new Cube(new Vector3(0, 2, 1), new Vector3(1, 3, 2)) // right slab
        }, opts => opts.WithStrictOrdering());
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

    private const string Part2ExampleInput = @"on x=-5..47,y=-31..22,z=-19..33
on x=-44..5,y=-27..21,z=-14..35
on x=-49..-1,y=-11..42,z=-10..38
on x=-20..34,y=-40..6,z=-44..1
off x=26..39,y=40..50,z=-2..11
on x=-41..5,y=-41..6,z=-36..8
off x=-43..-33,y=-45..-28,z=7..25
on x=-33..15,y=-32..19,z=-34..11
off x=35..47,y=-46..-34,z=-11..5
on x=-14..36,y=-6..44,z=-16..29
on x=-57795..-6158,y=29564..72030,z=20435..90618
on x=36731..105352,y=-21140..28532,z=16094..90401
on x=30999..107136,y=-53464..15513,z=8553..71215
on x=13528..83982,y=-99403..-27377,z=-24141..23996
on x=-72682..-12347,y=18159..111354,z=7391..80950
on x=-1060..80757,y=-65301..-20884,z=-103788..-16709
on x=-83015..-9461,y=-72160..-8347,z=-81239..-26856
on x=-52752..22273,y=-49450..9096,z=54442..119054
on x=-29982..40483,y=-108474..-28371,z=-24328..38471
on x=-4958..62750,y=40422..118853,z=-7672..65583
on x=55694..108686,y=-43367..46958,z=-26781..48729
on x=-98497..-18186,y=-63569..3412,z=1232..88485
on x=-726..56291,y=-62629..13224,z=18033..85226
on x=-110886..-34664,y=-81338..-8658,z=8914..63723
on x=-55829..24974,y=-16897..54165,z=-121762..-28058
on x=-65152..-11147,y=22489..91432,z=-58782..1780
on x=-120100..-32970,y=-46592..27473,z=-11695..61039
on x=-18631..37533,y=-124565..-50804,z=-35667..28308
on x=-57817..18248,y=49321..117703,z=5745..55881
on x=14781..98692,y=-1341..70827,z=15753..70151
on x=-34419..55919,y=-19626..40991,z=39015..114138
on x=-60785..11593,y=-56135..2999,z=-95368..-26915
on x=-32178..58085,y=17647..101866,z=-91405..-8878
on x=-53655..12091,y=50097..105568,z=-75335..-4862
on x=-111166..-40997,y=-71714..2688,z=5609..50954
on x=-16602..70118,y=-98693..-44401,z=5197..76897
on x=16383..101554,y=4615..83635,z=-44907..18747
off x=-95822..-15171,y=-19987..48940,z=10804..104439
on x=-89813..-14614,y=16069..88491,z=-3297..45228
on x=41075..99376,y=-20427..49978,z=-52012..13762
on x=-21330..50085,y=-17944..62733,z=-112280..-30197
on x=-16478..35915,y=36008..118594,z=-7885..47086
off x=-98156..-27851,y=-49952..43171,z=-99005..-8456
off x=2032..69770,y=-71013..4824,z=7471..94418
on x=43670..120875,y=-42068..12382,z=-24787..38892
off x=37514..111226,y=-45862..25743,z=-16714..54663
off x=25699..97951,y=-30668..59918,z=-15349..69697
off x=-44271..17935,y=-9516..60759,z=49131..112598
on x=-61695..-5813,y=40978..94975,z=8655..80240
off x=-101086..-9439,y=-7088..67543,z=33935..83858
off x=18020..114017,y=-48931..32606,z=21474..89843
off x=-77139..10506,y=-89994..-18797,z=-80..59318
off x=8476..79288,y=-75520..11602,z=-96624..-24783
on x=-47488..-1262,y=24338..100707,z=16292..72967
off x=-84341..13987,y=2429..92914,z=-90671..-1318
off x=-37810..49457,y=-71013..-7894,z=-105357..-13188
off x=-27365..46395,y=31009..98017,z=15428..76570
off x=-70369..-16548,y=22648..78696,z=-1892..86821
on x=-53470..21291,y=-120233..-33476,z=-44150..38147
off x=-93533..-4276,y=-16170..68771,z=-104985..-24507";

    [TestCase(true)]
    [TestCase(false)]
    public void NonInitializationProcedureSteps_DoIntersectPerOnOrOffFlag(bool isSet)
    {
        var rebootSteps = ParseInput(Part2ExampleInput);
        var (_, nonInitializationProcedureSteps) = SplitSteps(rebootSteps);

        nonInitializationProcedureSteps = nonInitializationProcedureSteps.Where(x => x.IsSet == isSet).ToArray();

        Console.WriteLine("#steps: " + nonInitializationProcedureSteps.Count);

        // Filter out any that are totally contained within another
        nonInitializationProcedureSteps = nonInitializationProcedureSteps
            .Where(rebootStep => nonInitializationProcedureSteps
                .Where(otherStep => otherStep != rebootStep)
                .All(otherStep => !otherStep.Bounds.Contains(rebootStep.Bounds))).ToArray();

        Console.WriteLine("#steps after filter: " + nonInitializationProcedureSteps.Count);

        var allIntersections = new List<(RebootStep rebootStep, (long intersectionArea, Cube intersection)[] intersections)>();

        foreach (var rebootStep in nonInitializationProcedureSteps)
        {
            // Get any intersections
            var intersections = nonInitializationProcedureSteps
                .Where(x => x != rebootStep)
                .Select(otherStep => Cube.GetIntersection(rebootStep.Bounds, otherStep.Bounds))
                .Where(intersection => intersection.intersectionArea > 0)
                .Distinct()
                .ToArray();

            allIntersections.Add((rebootStep, intersections));

            Console.WriteLine(new { rebootStep, numintersections = intersections.Length });
        }

        allIntersections.Sum(x => x.intersections.Length).Should().BeGreaterThan(0);
    }

    [Test]
    public void AttemptToCreateNoneOverlappingBoxesForAllOnBounds()
    {
        var rebootSteps = ParseInput(Part2ExampleInput);
        var (_, nonInitializationProcedureSteps) = SplitSteps(rebootSteps);

        nonInitializationProcedureSteps = nonInitializationProcedureSteps.Where(x => x.IsSet).ToArray();

        var step0 = nonInitializationProcedureSteps.First();

        TestContext.Progress.WriteLine("Num of bounds before splitting: " + nonInitializationProcedureSteps.Count);

        var unionList = new List<Cube>();

        foreach (var step in nonInitializationProcedureSteps.Where(x => x != step0))
        {
            var (intersection, exceptionBoxes) = Cube.GetIntersectionAndExceptionCubes(step0.Bounds, step.Bounds);

            // at this point, we know that intersection and the exceptionBoxes do not overlap
            // idea is to have a special CubesUnion class, which deals with keeping the union of all cubes, ensuring nothing ever overlaps
            // This will give us everything which is on, but then we'd end to be able to remove any overlaps which are off

            if (intersection != null)
                unionList.Add(intersection);
            unionList.AddRange(exceptionBoxes);
        }

        var cubesUnion = new CubesUnion();

        var count = 0;

        // NOPE, this union idea will not work!
        //foreach (var step in nonInitializationProcedureSteps)
        //{
        //    TestContext.Progress.WriteLine($"Unioning new cube, counter is {count}, num cubes already in union: {cubesUnion.Cubes.Count}");
        //    cubesUnion.Union(step.Bounds);
        //    count++;
        //}

        static bool AnyDoOverlapThisCube(Cube cube, IReadOnlyList<Cube> cubes)
        {
            foreach (var otherCube in cubes.Where(x => x != cube))
            {
                var x = Cube.GetIntersection(cube, otherCube);

                if (x.intersectionArea > 0)
                {
                    return true;
                }
            }

            return false;
        }

        static bool AnyDoOverlap(IReadOnlyList<Cube> cubeList)
        {
            return cubeList.Any(c => AnyDoOverlapThisCube(c, cubeList));
        }

        Console.WriteLine("Num of bounds before splitting: " + unionList.Count);

        Console.WriteLine("AnyDoOverlap: " + AnyDoOverlap(unionList));

        Console.WriteLine("AnyDoOverlap: " + AnyDoOverlap(new Cube[]
        {
            new(new Vector3(0, 0, 0), new Vector3(10, 10, 10)),
            new(new Vector3(20, 20, 20), new Vector3(30, 30, 30)),
        }));

        Console.WriteLine("Num of cubes in union: " + cubesUnion.Cubes.Count);
        Console.WriteLine("AnyDoOverlap in cubes union: " + AnyDoOverlap(cubesUnion.Cubes));
    }

    [Test]
    public void Part2Example()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(Part2ExampleInput);

        // ASSERT
        part2ExampleResult.Should().Be(2758514936282235);
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
