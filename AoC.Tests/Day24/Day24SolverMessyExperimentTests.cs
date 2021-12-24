using AoC.Day24;
using static AoC.Day24.Day24Solver;
using static AoC.Tests.Day24.Day24SolverTests;

namespace AoC.Tests.Day24;

public class Day24SolverMessyExperimentTests
{
    [Test]
    public void AluCodePortedToCSharpV1_Test2()
    {
        var chars = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        // 4:
        var inputs = chars.SelectMany(
            c1 => chars.SelectMany(
                c2 => chars.SelectMany(
                    c3 => chars.Select(c4 => $"{c1}{c2}{c3}{c4}")))).ToArray();

        // 5:
        //var inputs = chars.SelectMany(
        //    c1 => chars.SelectMany(
        //        c2 => chars.SelectMany(
        //            c3 => chars.SelectMany(
        //                c4 => chars.Select(c5 => $"{c1}{c2}{c3}{c4}{c5}"))))).ToArray();

        // 6:
        //var inputs = chars.SelectMany(
        //    c1 => chars.SelectMany(
        //        c2 => chars.SelectMany(
        //            c3 => chars.SelectMany(
        //                c4 => chars.SelectMany(
        //                    c5 => chars.Select(c6 => $"{c1}{c2}{c3}{c4}{c5}{c6}")))))).ToArray();

        // Noticed, the / 26 that happens occasionally, if z < 26, that turns z back to zero, but only if there is a match, otherwise the input + arg2 at end gets added to z, making it non zero
        // So to end with, we must have a match, and must have z < 26
        // A match being: input == (z % 26) + arg1, and because in this case z must be < 26, the end match can be simply written: input == z + arg1
        // But, argument1 for block 14 is -12 !!!

        var program = PuzzleInputProgramFirst5Blocks;

        program.Instructions.First().Line.Should().Be("inp w");
        program.Instructions.Last().Line.Should().Be("add z y");

        var results = inputs.Select(input =>
        {
            var z = 0;
            var queue = new Queue<int>(input.Select(c => int.Parse(c.ToString())));

            // 1:
            z = AluCodePortedToCSharpV1(queue.Dequeue(), z, 11, 8);
            //Console.WriteLine("z: " + z);

            // 2:
            z = AluCodePortedToCSharpV1(queue.Dequeue(), z, 14, 13);
            //Console.WriteLine("z: " + z);

            // 3:
            z = AluCodePortedToCSharpV1(queue.Dequeue(), z, 10, 2);
            var z3 = z;
            //Console.WriteLine("z: " + z);

            // 4:
            z = AluCodePortedToCSharpV1(queue.Dequeue(), z, 0, 7/*, 26*/);
            var z4 = z;
            //Console.WriteLine("z: " + z);

            //// 5:
            //z = AluCodePortedToCSharpV1(queue.Dequeue(), z, 12, 11);
            //var prevZ = z;

            //// 6:
            //z = AluCodePortedToCSharpV1(queue.Dequeue(), z, 12, 4);

            //// 7:
            //z = AluCodePortedToCSharpV1(queue.Dequeue(), z, 12, 13);

            //// 8:
            //z = AluCodePortedToCSharpV1(queue.Dequeue(), z, -8, 13, 26);

            //// 9:
            //z = AluCodePortedToCSharpV1(queue.Dequeue(), z, -9, 10, 26);

            return new
            {
                input,
                z,
                z3,
                z4
            };
        })
            //.Where(res => res.z < 1000)
            .ToArray();

        Console.WriteLine("count: " + results.Length);
        Console.WriteLine("count: " + results.Count(x => x.z4 < x.z3));
        //Console.WriteLine("count where zero: " + results.Count(res => res.z == 0));
        //using var sw = new StreamWriter($@"C:\Dev\aoctemp\{DateTime.Now:yyyyMMdd-HHmmss}.txt");
        foreach (var result in results.OrderBy(x => x.z)) // .Where(res => res.z < 5000))
        {
            Console.WriteLine($"input: {result.input}; produces z: {result.z}");
            //sw.WriteLine($"input: {result.input}; produces z: {result.z}");
        }

        // No valid input of the first character produces a z of zero, meaning the zo must have a following effect
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 1, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 2, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 3, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 4, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 5, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 6, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 7, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 8, z: 0, 11, 8));
        //Console.WriteLine(AluCodePortedToCSharpV1(input: 9, z: 0, 11, 8));
    }

    [Test]
    public void AluCodePortedToCSharpV1_Test()
    {
        int z = 0;

        // 1:
        z = AluCodePortedToCSharpV1(input: 1, z, 11, 8);
        z.Should().Be(9);

        // 2:
        z = AluCodePortedToCSharpV1(input: 3, z, 14, 13);
        z.Should().Be(250);

        // 3:
        z = AluCodePortedToCSharpV1(input: 5, z, 10, 2);
        z.Should().Be(6507);

        // 4:
        z = AluCodePortedToCSharpV1(input: 7, z, 0, 7/*, 26*/);
        z.Should().Be(250);

        // 5:
        z = AluCodePortedToCSharpV1(input: 9, z, 12, 11);
        z.Should().Be(6520);

        // 6:
        z = AluCodePortedToCSharpV1(input: 2, z, 12, 4);
        z.Should().Be(169526);

        // 7:
        z = AluCodePortedToCSharpV1(input: 4, z, 12, 13);
        z.Should().Be(4407693);

        // 8:
        z = AluCodePortedToCSharpV1(input: 6, z, -8, 13/*, 26*/);
        z.Should().Be(4407695);

        // 9:
        z = AluCodePortedToCSharpV1(input: 8, z, -9, 10/*, 26*/);
        z.Should().Be(4407694);

        // 10:
        z = AluCodePortedToCSharpV1(input: 9, z, 11, 1);
        z.Should().Be(114600054);

        // 11:
        z = AluCodePortedToCSharpV1(input: 9, z, 0, 2/*, 26*/);
        z.Should().Be(114600055);

        // 12:
        z = AluCodePortedToCSharpV1(input: 9, z, -5, 14/*, 26*/);
        z.Should().Be(114600067);

        // 13:
        z = AluCodePortedToCSharpV1(input: 9, z, -6, 6/*, 26*/);
        z.Should().Be(114600059);

        // 14:
        z = AluCodePortedToCSharpV1(input: 9, z, -12, 14/*, 26*/);
        z.Should().Be(114600067);
    }

    private static readonly Program PuzzleInputProgram = Program.Parse(new InputLoader(new Day24Solver()).PuzzleInputPart1);

    private static readonly Program PuzzleInputProgramFirst3Blocks = Program.Parse(
        string.Join(Environment.NewLine, new InputLoader(
                new Day24Solver()).PuzzleInputPart1.ReadLines()
            .Select((line, index) => new { line, lineNumber = index + 1 })
            .TakeWhile(x => x.lineNumber <= 54)
            .Select(x => x.line)));

    private static readonly Program PuzzleInputProgramFirst5Blocks = Program.Parse(GetPuzzleInputLinesInclusive(1, 90));

    private static readonly Program PuzzleInputProgramLast3Blocks = Program.Parse(GetPuzzleInputLinesInclusive(199, 252));

    private static readonly Program PuzzleInputProgramLast1Block = Program.Parse(GetPuzzleInputLinesInclusive(235, 252));

    [Test]
    public void PuzzleInput_Some_Blocks_Tests()
    {
        var chars = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var inputs = chars.SelectMany(
            c1 => chars.SelectMany(
                c2 => chars.SelectMany(
                    c3 => chars.SelectMany(
                        c4 => chars.Select(c5 => $"{c1}{c2}{c3}{c4}{c5}"))))).ToArray();

        var program = PuzzleInputProgramFirst5Blocks;

        program.Instructions.First().Line.Should().Be("inp w");
        program.Instructions.Last().Line.Should().Be("add z y");

        var results = inputs.Select(input => new
        {
            input,
            state = program.Execute(Inputs(input.Select(c => int.Parse(c.ToString()))))
        })
            .Where(res => res.state.Z == 0)
            .ToArray();

        foreach (var result in results)
        {
            Console.WriteLine(string.Join(',', result.input.Select(x => x)));
            //Console.WriteLine(result.state);
            //Console.WriteLine();
        }
    }

    private static string GetPuzzleInputLinesInclusive(int start, int end) =>
        string.Join(Environment.NewLine, new InputLoader(
                new Day24Solver()).PuzzleInputPart1.ReadLines()
            .Select((line, index) => new { line, lineNumber = index + 1 })
            .SkipWhile(x => x.lineNumber < start)
            .TakeWhile(x => x.lineNumber <= end)
            .Select(x => x.line));

    [TestCase("13579246899999")]
    [TestCase("11111111111111")]
    [TestCase("31111111111117")]
    [TestCase("81111111111112")]
    [TestCase("12345678912345")]
    [TestCase("24242424242424")]
    [TestCase("99999999999999")]
    [TestCase("99999999999993")]
    public void PuzzleInputProgram_14DigitInput_Examples(string input)
    {
        // When MONAD checks a hypothetical fourteen-digit model number, it uses fourteen separate inp instructions,
        // each expecting a single digit of the model number in order of most to least significant.
        // When operating MONAD, each input instruction should only ever be given an integer value of at least 1 and at most 9.
        // Then, after MONAD has finished running all of its instructions,
        // it will indicate that the model number was valid by leaving a 0 in variable z.
        // However, if the model number was invalid, it will leave some other non-zero value in z.

        // So, its valid if Z is zero at the end.

        ////PuzzleInputProgram.EnableDiagnostics = true;

        var result = PuzzleInputProgram.Execute(Inputs(input.Select(c => int.Parse(c.ToString()))));

        TestContext.WriteLine(result);
    }

    //[TestCase("123")]
    //[TestCase("999")]
    [Test]
    public void PuzzleInputProgramFirst3Blocks_Tests()
    {
        var chars = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var inputs = chars.SelectMany(c1 => chars.SelectMany(c2 => chars.Select(c3 => $"{c1}{c2}{c3}"))).ToArray();

        var program = PuzzleInputProgramFirst3Blocks;

        program.Instructions.First().Line.Should().Be("inp w");
        program.Instructions.Last().Line.Should().Be("add z y");

        var results = inputs.Select(input => new
        {
            input,
            state = program.Execute(Inputs(input.Select(c => int.Parse(c.ToString()))))
        })
            .Where(res => res.state.Z == 0)
            .ToArray();

        foreach (var result in results)
        {
            Console.WriteLine(string.Join(',', result.input.Select(x => x)));
            //Console.WriteLine(result.state);
            //Console.WriteLine();
        }

        //program.Instructions.First().Line.Should().Be("inp w");
        //program.Instructions.Last().Line.Should().Be("add z y");

        //var result = program.Execute(Inputs(input.Select(c => int.Parse(c.ToString()))));

        //TestContext.WriteLine(result);
    }

    //[TestCase("111")]
    //[TestCase("123")]
    //[TestCase("999")]
    //[TestCase("756")]
    [Test]
    public void PuzzleInputProgramLast3Blocks_Tests()
    {
        var chars = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var inputs = chars.SelectMany(c1 => chars.SelectMany(c2 => chars.Select(c3 => $"{c1}{c2}{c3}"))).ToArray();

        var program = PuzzleInputProgramLast3Blocks;

        program.Instructions.First().Line.Should().Be("inp w");
        program.Instructions.Last().Line.Should().Be("add z y");

        var results = inputs.Select(input => new
        {
            input,
            state = program.Execute(Inputs(input.Select(c => int.Parse(c.ToString()))))
        })
            .Where(res => res.state.Z == 0)
            .ToArray();

        foreach (var result in results)
        {
            Console.WriteLine(string.Join(',', result.input.Select(x => x)));
            //Console.WriteLine(result.state);
            //Console.WriteLine();
        }
    }

    [Test]
    public void PuzzleInputProgramLast1Block_Tests()
    {
        var chars = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var inputs = chars.Select(c3 => $"{c3}");
        //var inputs = chars.SelectMany(c1 => chars.SelectMany(c2 => chars.Select(c3 => $"{c1}{c2}{c3}"))).ToArray();

        var program = PuzzleInputProgramLast1Block;

        program.Instructions.First().Line.Should().Be("inp w");
        program.Instructions.Last().Line.Should().Be("add z y");

        var results = inputs.Select(input => new
        {
            input,
            state = program.Execute(Inputs(input.Select(c => int.Parse(c.ToString()))))
        })
            .Where(res => res.state.Z == 0)
            .ToArray();

        foreach (var result in results)
        {
            Console.WriteLine(string.Join(',', result.input.Select(x => x)));
            //Console.WriteLine(result.state);
            //Console.WriteLine();
        }
    }
}
