using AoC.Day18;
using static AoC.Day18.Day18Solver;

namespace AoC.Tests.Day18;

public class Day18SolverTests
{
    private readonly Day18Solver _sut = new();

    private const string ExampleInput = @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]";

    [Test]
    public void SnailfishNumber_ParseLine_Then_BackToString_Roundtrip_Test()
    {
        const string input = "[[[[[9,8],1],2],3],4]";
        var result = SnailfishNumber.ParseLine(input);
        result.ToString().Should().Be(input);
    }

    [TestCase(10, 5, 5)]
    [TestCase(11, 5, 6)]
    [TestCase(12, 6, 6)]
    public void RegularNumber_GetSplitParts_Tests(int input, int expectedLeft, int expectedRight)
    {
        var number = new RegularNumber(input);

        // ACT
        var (left, right) = number.GetSplitParts();

        // ASSERT
        left.Should().Be(expectedLeft);
        right.Should().Be(expectedRight);
    }

    [TestCase("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
    [TestCase("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
    [TestCase("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
    [TestCase("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
    public void Explode_Tests(string input, string expected)
    {
        var snailfishNumber = SnailfishNumber.ParseLine(input);

        // ACT
        snailfishNumber.Reduce();

        // ASSERT
        snailfishNumber.ToString().Should().Be(expected);
    }

    [Test]
    public void SnailfishNumber_Addition_Test()
    {
        var snailfishNumber1 = SnailfishNumber.ParseLine("[[[[4,3],4],4],[7,[[8,4],9]]]");
        var snailfishNumber2 = SnailfishNumber.ParseLine("[1,1]");

        // ACT
        var result = snailfishNumber1 + snailfishNumber2;

        // ASSERT
        result.ToString().Should().Be("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]");
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(4140);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(4641);
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
