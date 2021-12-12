using AoC.Day12;

namespace AoC.Tests.Day12;

public class Day12SolverTests
{
    private readonly Day12Solver _sut = new();

    private const string ExampleInput1 = @"start-A
start-b
A-c
A-b
b-d
A-end
b-end";

    private const string ExampleInput2 = @"dc-end
HN-start
start-kj
dc-start
dc-HN
LN-dc
HN-end
kj-sa
kj-HN
kj-dc";

    private const string ExampleInput3 = @"fs-end
he-DX
fs-he
start-DX
pj-DX
end-zg
zg-sl
zg-pj
pj-he
RW-he
fs-DX
pj-RW
zg-RW
start-pj
he-WI
zg-he
pj-fs
start-RW";

    [Test]
    public void Part1Example1()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput1);

        // ASSERT
        part1ExampleResult.Should().Be(10);
    }

    [Test]
    public void Part1Example2()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput2);

        // ASSERT
        part1ExampleResult.Should().Be(19);
    }

    [Test]
    public void Part1Example3()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput3);

        // ASSERT
        part1ExampleResult.Should().Be(226);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(3563);
    }

    [Test]
    public void Part2Example1()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput1);

        // ASSERT
        part2ExampleResult.Should().Be(36);
    }

    [Test]
    public void Part2Example2()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput2);

        // ASSERT
        part2ExampleResult.Should().Be(103);
    }

    [Test]
    public void Part2Example3()
    {
        // ACT
        var part2ExampleResult = _sut.SolvePart2(ExampleInput3);

        // ASSERT
        part2ExampleResult.Should().Be(3509);
    }

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(105453);
    }
}
