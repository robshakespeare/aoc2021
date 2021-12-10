using AoC.Day10;

namespace AoC.Tests.Day10;

public class Day10SolverTests
{
    private readonly Day10Solver _sut = new();

    private const string ExampleInput = @"[({(<(())[]>[[{[]{<()<>>
[(()[<>])]({[<{<<[]>>(
{([(<{}[<>[]}>{[]{[(<()>
(((({<>}<{<{<>}{[]{[]{}
[[<[([]))<([[{}[[()]]]
[{[{({}]{}}([{[{{{}}([]
{<[[]]>}<{[{[{[]{()[[[]
[<(<(<(<{}))><([]([]()
<{([([[(<>()){}]>(<<{{
<{([{{}}[<[[[<>{}]]]>[]]";

    [TestCase("(]", "Expected ), but found ] instead.")]
    [TestCase("{()()()>", "Expected }, but found > instead.")]
    [TestCase("(((()))}", "Expected ), but found } instead.")]
    [TestCase("<([]){()}[{}])", "Expected >, but found ) instead.")]
    [TestCase("{([(<{}[<>[]}>{[]{[(<()>", "Expected ], but found } instead.")]
    [TestCase("[[<[([]))<([[{}[[()]]]", "Expected ], but found ) instead.")]
    [TestCase("[{[{({}]{}}([{[{{{}}([]", "Expected ), but found ] instead.")]
    [TestCase("[<(<(<(<{}))><([]([]()", "Expected >, but found ) instead.")]
    [TestCase("<{([([[(<>()){}]>(<<{{", "Expected ], but found > instead.")]
    public void ExamplesCorruptedChunk_Tests(string line, string expectedMessage)
    {
        var act = () => Day10Solver.ParseLine(line);

        // ACT & ASSERT
        act.Should().Throw<Day10Solver.CorruptedLineException>()
            .WithMessage(expectedMessage);
    }

    [Test]
    public void Part1Example()
    {
        // ACT
        var part1ExampleResult = _sut.SolvePart1(ExampleInput);

        // ASSERT
        part1ExampleResult.Should().Be(26397);
    }

    [Test]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(294195);
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
