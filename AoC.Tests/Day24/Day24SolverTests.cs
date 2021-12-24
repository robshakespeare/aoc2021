using AoC.Day24;
using static AoC.Day24.Day24Solver;

namespace AoC.Tests.Day24;

public class Day24SolverTests
{
    private readonly Day24Solver _sut = new();

    internal static Func<int> Inputs(IEnumerable<int> inputs) => Inputs(inputs.ToArray());

    internal static Func<int> Inputs(params int[] inputs)
    {
        var queue = new Queue<int>(inputs);
        return () => queue.Dequeue();
    }

    [TestCase(10, -10)]
    [TestCase(-10, 10)]
    [TestCase(65, -65)]
    [TestCase(-65, 65)]
    public void Program_TakesInput_Negates_StoresInZ_Example_Test(int input, int expectedX)
    {
        var sut = Program.Parse(@"inp x
mul x -1");

        // ACT
        var state = sut.Execute(Inputs(input));

        // ASSERT
        using (new AssertionScope())
        {
            state.W.Should().Be(0);
            state.X.Should().Be(expectedX);
            state.Y.Should().Be(0);
            state.Z.Should().Be(0);
        }
    }

    [TestCase(1, 3, 1)]
    [TestCase(1, 4, 0)]
    [TestCase(7, 21, 1)]
    [TestCase(7, 20, 0)]
    public void Program_Takes2Inputs_StoresInZWhetherSecondInputIsThreeTimesLargerThanFirstInput_Example_Test(int input1, int input2, int expectedZ)
    {
        var sut = Program.Parse(@"inp z
inp x
mul z 3
eql z x");

        // ACT
        var state = sut.Execute(Inputs(input1, input2));

        // ASSERT
        using (new AssertionScope())
        {
            state.W.Should().Be(0);
            state.X.Should().Be(input2);
            state.Y.Should().Be(0);
            state.Z.Should().Be(expectedZ);
        }
    }

    [TestCase(0, 0, 0, 0, 0)]
    [TestCase(1, 0, 0, 0, 1)]
    [TestCase(2, 0, 0, 1, 0)]
    [TestCase(4, 0, 1, 0, 0)]
    [TestCase(8, 1, 0, 0, 0)]
    [TestCase(3, 0, 0, 1, 1)]
    [TestCase(6, 0, 1, 1, 0)]
    [TestCase(11, 1, 0, 1, 1)]
    [TestCase(15, 1, 1, 1, 1)]
    public void Program_BinConvertExample_Test(int input, int expectedW, int expectedX, int expectedY, int expectedZ)
    {
        var sut = Program.Parse(@"inp w
add z w
mod z 2
div w 2
add y w
mod y 2
div w 2
add x w
mod x 2
div w 2
mod w 2");

        // ACT
        var state = sut.Execute(Inputs(input));

        // ASSERT
        using (new AssertionScope())
        {
            state.W.Should().Be(expectedW);
            state.X.Should().Be(expectedX);
            state.Y.Should().Be(expectedY);
            state.Z.Should().Be(expectedZ);
        }
    }

    private static readonly PuzzleInput PuzzleInput = new InputLoader(new Day24Solver()).PuzzleInputPart1;

    [Test]
    public void ParseInToChunks_Test()
    {
        // ACT
        var chunks = Program.ParseInToChunks(PuzzleInput);

        // ASSERT
        chunks.Should().HaveCount(14);
        chunks.Where(x => x.IsChunkThatReducesZ).Should().HaveCount(7);
        chunks.Where(x => !x.IsChunkThatReducesZ).Should().HaveCount(7);

        foreach (var chunk in chunks)
        {
            chunk.Program.Instructions.First().Line.Should().Be("inp w");
            chunk.Program.Instructions.Last().Line.Should().Be("add z y");
        }
    }

    [Test]
    [LongRunningTest("14 seconds!")]
    public void Part1ReTest()
    {
        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(92793949489995);
    }

    [Test]
    [LongRunningTest("1 min 2 seconds!!")]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(51131616112781);
    }
}
