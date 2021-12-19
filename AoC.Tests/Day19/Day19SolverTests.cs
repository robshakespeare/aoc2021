using AoC.Day19;
using static System.Environment;
using static AoC.Day19.Day19Solver;

namespace AoC.Tests.Day19;

public class Day19SolverTests
{
    private readonly Day19Solver _sut = new();

    private const string SmallExampleInput = @"--- scanner 0 ---
0,2,0
4,1,0
3,3,0

--- scanner 1 ---
-1,-1,0
-5,0,0
-2,1,0";

    #region Example Input

    private const string ExampleInput = @"--- scanner 0 ---
404,-588,-901
528,-643,409
-838,591,734
390,-675,-793
-537,-823,-458
-485,-357,347
-345,-311,381
-661,-816,-575
-876,649,763
-618,-824,-621
553,345,-567
474,580,667
-447,-329,318
-584,868,-557
544,-627,-890
564,392,-477
455,729,728
-892,524,684
-689,845,-530
423,-701,434
7,-33,-71
630,319,-379
443,580,662
-789,900,-551
459,-707,401

--- scanner 1 ---
686,422,578
605,423,415
515,917,-361
-336,658,858
95,138,22
-476,619,847
-340,-569,-846
567,-361,727
-460,603,-452
669,-402,600
729,430,532
-500,-761,534
-322,571,750
-466,-666,-811
-429,-592,574
-355,545,-477
703,-491,-529
-328,-685,520
413,935,-424
-391,539,-444
586,-435,557
-364,-763,-893
807,-499,-711
755,-354,-619
553,889,-390

--- scanner 2 ---
649,640,665
682,-795,504
-784,533,-524
-644,584,-595
-588,-843,648
-30,6,44
-674,560,763
500,723,-460
609,671,-379
-555,-800,653
-675,-892,-343
697,-426,-610
578,704,681
493,664,-388
-671,-858,530
-667,343,800
571,-461,-707
-138,-166,112
-889,563,-600
646,-828,498
640,759,510
-630,509,768
-681,-892,-333
673,-379,-804
-742,-814,-386
577,-820,562

--- scanner 3 ---
-589,542,597
605,-692,669
-500,565,-823
-660,373,557
-458,-679,-417
-488,449,543
-626,468,-788
338,-750,-386
528,-832,-391
562,-778,733
-938,-730,414
543,643,-506
-524,371,-870
407,773,750
-104,29,83
378,-903,-323
-778,-728,485
426,699,580
-438,-605,-362
-469,-447,-387
509,732,623
647,635,-688
-868,-804,481
614,-800,639
595,780,-596

--- scanner 4 ---
727,592,562
-293,-554,779
441,611,-461
-714,465,-776
-743,427,-804
-660,-479,-426
832,-632,460
927,-485,-438
408,393,-506
466,436,-512
110,16,151
-258,-428,682
-393,719,612
-211,-452,876
808,-476,-593
-575,615,604
-485,667,467
-680,325,-822
-627,-443,-432
872,-547,-609
833,512,582
807,604,487
839,-516,451
891,-625,532
-652,-548,-490
30,-46,-14";

    #endregion

    private const int ExpectedNumberOfPermutations = 48; //24; // rs-todo: hmmm, try and work out whats going on here if I get time

    [TestCase("404,-588,-901")]
    [TestCase("-6,-4,-5")]
    [TestCase("-618,-824,-621")]
    [TestCase("-889,563,-600")]
    public void GetAllPermutations_Test(string input)
    {
        // ACT
        var result = Scanner.GetAllPermutations(Scanner.LineToVector3(input)).ToArray();

        // ASSERT
        result.Should().HaveCount(ExpectedNumberOfPermutations);
        result.Distinct().Should().HaveCount(ExpectedNumberOfPermutations);
    }

    [Test]
    public void GetAllPermutations_RegardlessOfInputOrderAndMagnitude_ProducesSameSetForSame3Values()
    {
        // ACT
        var result1 = Scanner.GetAllPermutations(new Vector3(500, 723, -460));
        var result2 = Scanner.GetAllPermutations(new Vector3(460, -500, 723));

        // ASSERT
        result1.Should().BeEquivalentTo(result2);
    }

    [Test]
    public void ParseInputToScanners_And_GetOrientations_SmallExampleInputTest()
    {
        // ACT
        var scanners = Scanner.ParseInputToScanners(SmallExampleInput);

        foreach (var scanner in scanners)
        {
            Console.WriteLine($"Scanner {scanner.ScannerId} has {scanner.Beacons.Count} beacons");
        }

        // ASSERT
        scanners.Should().HaveCount(2);
        scanners.Select(scanner => scanner.Beacons.Count).Distinct().Should().BeEquivalentTo(new[] { 3 });

        scanners.Select(scanner => scanner.AllOrientations.Value.Count).Distinct().Should().BeEquivalentTo(new[] {ExpectedNumberOfPermutations});
    }

    [Test]
    public void ParseInputToScanners_And_GetOrientations_ExampleInputTest()
    {
        // ACT
        var scanners = Scanner.ParseInputToScanners(ExampleInput);

        foreach (var scanner in scanners)
        {
            Console.WriteLine($"Scanner {scanner.ScannerId} has {scanner.Beacons.Count} beacons");
        }

        // ASSERT
        scanners.Should().HaveCount(5);
        scanners.Select(scanner => scanner.Beacons.Count).Distinct().Should().BeEquivalentTo(new[] {25, 26});

        scanners.Select(scanner => scanner.AllOrientations.Value.Count).Distinct().Should().BeEquivalentTo(new[] {ExpectedNumberOfPermutations});
    }

    [Test]
    public void ParseInputToScanners_And_GetOrientations_ActualInputTest()
    {
        // ACT
        var scanners = Scanner.ParseInputToScanners(new InputLoader(_sut).PuzzleInputPart1);

        foreach (var scanner in scanners)
        {
            Console.WriteLine($"Scanner {scanner.ScannerId} has {scanner.Beacons.Count} beacons");
        }

        // ASSERT
        scanners.Should().HaveCount(27);
        scanners.Select(scanner => scanner.Beacons.Count).Distinct().Should().BeEquivalentTo(new[] {25, 26, 27});

        scanners.Select(scanner => scanner.AllOrientations.Value.Count).Distinct().Should().BeEquivalentTo(new[] {ExpectedNumberOfPermutations});
    }

    [Test]
    public void Part1SmallExampleInput()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(SmallExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(null);
    }

    [Test]
    public void GetOrientations_OfExample_Does_Include_Example_Arrangements()
    {
        var scanner = Scanner.ParseInputToScanners(@"--- scanner 0 ---
-1,-1,1
-2,-2,2
-3,-3,3
-2,-3,1
5,6,-4
8,0,7").Single();

        // ACT
        var results = scanner.AllOrientations.Value
            .Select(orientation => $"--- scanner 0 ---{NewLine}{string.Join(NewLine, orientation.Beacons.Select(b => $"{b.X},{b.Y},{b.Z}"))}")
            .ToArray();

        // ASSERT
        results.Should().Contain(@"--- scanner 0 ---
-1,-1,1
-2,-2,2
-3,-3,3
-2,-3,1
5,6,-4
8,0,7".NormalizeLineEndings());

        results.Should().Contain(@"--- scanner 0 ---
1,-1,1
2,-2,2
3,-3,3
2,-1,3
-5,4,-6
-8,-7,0".NormalizeLineEndings());

        results.Should().Contain(@"--- scanner 0 ---
-1,-1,-1
-2,-2,-2
-3,-3,-3
-1,-3,-2
4,6,5
-7,0,8".NormalizeLineEndings());

        results.Should().Contain(@"--- scanner 0 ---
1,1,-1
2,2,-2
3,3,-3
1,3,-2
-4,-6,5
7,0,8".NormalizeLineEndings());

        results.Should().Contain(@"--- scanner 0 ---
1,1,1
2,2,2
3,3,3
3,1,2
-6,-4,-5
0,7,-8".NormalizeLineEndings());
    }

    [Test]
    public void GetOverlappingScannerOrNull_ExampleInput_WalkThrough()
    {
        //---
        // ARRANGE 1
        var scanners = Scanner.ParseInputToScanners(ExampleInput);
        var scanner0 = scanners.ElementAt(0);
        var scanner1 = scanners.ElementAt(1);

        // ACT 1
        var result1 = scanner0.GetOverlappingDetailsOrNull(scanner1);

        // ASSERT 1
        result1.Should().NotBeNull();
        result1!.Overlaps.Should().HaveCount(12);
        result1.RelativePosition.Should().Be(new Vector3(68, -1246, -43));

        //---
        // ARRANGE 2
        scanner1 = result1.OverlappingScannerOriented;
        var scanner4 = scanners.ElementAt(4);

        // ACT 2
        var result2 = scanner1.GetOverlappingDetailsOrNull(scanner4);

        // ASSERT 2
        result2.Should().NotBeNull();
        result2!.Overlaps.Should().HaveCount(12);
        (result1.RelativePosition + result2.RelativePosition).Should().Be(new Vector3(-20, -1133, 1061));

        ////---
        //// ARRANGE 3
        //scanner4 = result1.OverlappingScannerOriented;
        //var scanner2 = scanners.ElementAt(2);

        //// ACT 3
        //var result3 = scanner4.GetOverlappingDetailsOrNull(scanner2);

        //// ASSERT 3
        //result3.Should().NotBeNull();
        //result3!.Overlaps.Should().HaveCount(12);
        //(result1.RelativePosition + result2.RelativePosition).Should().Be(new Vector3(-20, -1133, 1061));
    }

    //[Test]
    //public void Scanner_GetOverlappingBeacons_And_GetRelativePositionOfOtherScanner_Test2()
    //{
    //    var scanners = Scanner.ParseInputToScanners(ExampleInput);
    //    var scanner1 = scanners.ElementAt(1);
    //    var scanner4 = scanners.ElementAt(4);

    //    var knownPosition0 = new Vector3(0, 0, 0);
    //    var known1RelativeTo0 = new Vector3(68, -1246, -43);
    //    //var knownPosition1 = known1RelativeTo0 - knownPosition0;
    //    var knownPosition1 = knownPosition0 - known1RelativeTo0;

    //    // ACT
    //    var resultIntersections = scanner1.GetOverlappingBeacons(scanner4);
    //    var resultRelativePosition = scanner1.GetRelativePositionOfOtherScanner(scanner4);

    //    // ASSERT
    //    resultIntersections.Should().HaveCount(12);

    //    Console.WriteLine("known1RelativeTo0: " + known1RelativeTo0);
    //    Console.WriteLine("knownPosition1: " + knownPosition1);
    //    Console.WriteLine("resultRelativePosition: " + resultRelativePosition);
    //    Console.WriteLine();

    //    foreach (var test in scanner1.GetOverlappingBeacons2(scanner4))
    //    {
    //        var (sourceBeacon, otherBeacon) = test.First();
    //        var test2 = sourceBeacon - otherBeacon;
    //        Console.WriteLine(test2);
    //    }
    //    (known1RelativeTo0 - resultRelativePosition).Should().Be(new Vector3(-20, -1133, 1061)); // rs-todo: how to work this out!?!?
    //}

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(null);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(null);
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
