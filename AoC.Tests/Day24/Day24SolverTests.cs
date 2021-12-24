using AoC.Day24;
using static AoC.Day24.Day24Solver;

namespace AoC.Tests.Day24;

public class Day24SolverTests
{
    private readonly Day24Solver _sut = new();

    private static Func<int> Inputs(IEnumerable<int> inputs) => Inputs(inputs.ToArray());

    private static Func<int> Inputs(params int[] inputs)
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

    //public static int ALUCodePortedToCSharpOrg(Func<int> getInput, int z)
    //{
    //    int input = getInput();

    //    int x = z; // x this time starts as the final Z value from last time

    //    x = x % 26; // ensure x (i.e. prezZ) is in the range 0 to 25

    //    z = z / 1; // pretty sure this doesn't do anything

    //    // 14 is our first parameter, *** FIRST OF ONLY 2 THINGS THAT ARE DIFFERENT, its always x being changed here, it is just the number that is different ***
    //    x += 14; // remember x = final z from last time, mod 26

    //    x = x != input ? 1 : 0; // so, if `input != (prevZ % 26) + 14`, then use 1 as X, otherwise use 0

    //    var y = 25;

    //    y *= x;

    //    y += 1;

    //    z = z * y;

    //    y = input;

    //    // 13 is our second parameter, *** SECOND OF ONLY 2 THINGS THAT ARE DIFFERENT, its always y being changed here, it is just the number that is different *** 
    //    y += 13;

    //    y *= x;

    //    z = z + y;

    //    return z;
    //}

    //public static int ALUCodePortedToCSharp(Func<int> getInput, int z, int arg1, int arg2, int divAmount = 1)
    //{
    //    int input = getInput();

    //    int x = z; // x this time starts as the final Z value from last time

    //    x %= 26; // ensure x (i.e. prezZ) is in the range 0 to 25

    //    z /= divAmount; // this is set to 1 a lot to begin with, then more frequently to 26, ending with 26 4 times in a row

    //    // First argument use, its always x being changed here, it is just the number that is different
    //    x += arg1; // remember x = final z from last time, mod 26

    //    x = input != x ? 1 : 0; // so, if `input != (prevZ % 26) + arg1`, then use 1 as X, otherwise use 0

    //    var y = 25;

    //    y *= x;

    //    y += 1;

    //    z *= y;

    //    y = input;

    //    // Second argument use, its always y being changed here, it is just the number that is different
    //    y += arg2;

    //    y *= x;

    //    z += y;

    //    return z;
    //}

    public static int ALUCodePortedToCSharp(int input, int z, int arg1, int arg2)
    {
        // Note z % 26 ensures z is in the range 0 to 25 here
        var notAMatch = input != z % 26 + arg1;

        // note: divAmount this is set to 1 a lot to begin with, then more frequently to 26, ending with 26 4 times in a row
        // No need to pass in divAmount, if arg1 is <= 0 then it divides by 26, otherwise it divides by 1 which has no effect
        if (arg1 <= 0)
        {
            z /= 26;
        }

        // First argument use, its always x being changed here, it is just the number that is different
        //x += arg1; // remember x = final z from last time, mod 26

        //x = input != z % 26 + arg1 ? 1 : 0; // so, if `input != (prevZ % 26) + arg1`, then use 1 as X, otherwise use 0

        if (notAMatch)
        {
            //var y = 26; //25 * 1 + 1;

            //y *= x;
            //y += 1;

            z = z * 26 + (input + arg2);

            // Second argument use, its always y being changed here, it is just the number that is different
            //y += arg2;

            //y *= 1; // if is a match, then y will be zero

            //z += input + arg2;
        }

        return z;
    }

    // does produce same results as:
    //public static int ALUCodePortedToCSharp(int input, int z, int arg1, int arg2)
    //{
    //    // CORRECT: if input == (z % 26) + arg1 then use 0
    //    // CORRECT: var theBool = input != (z % 26) + arg1;

    //    // Note z % 26 ensures z is in the range 0 to 25 here
    //    var notAMatch = input != (z % 26) + arg1;

    //    // note: divAmount this is set to 1 a lot to begin with, then more frequently to 26, ending with 26 4 times in a row
    //    // No need to pass in divAmount, if arg1 is <= 0 then it divides by 26, otherwise it divides by 1 which has no effect

    //    z = (z / divAmount) * (notAMatch ? 26 : 1);

    //    return z + (notAMatch ? input + arg2 : 0);
    //}

    //public static int ALUCodePortedToCSharp(Func<int> getInput, int z, int arg1, int arg2, int divAmount = 1)
    //{
    //    int input = getInput();

    //    //int x = z; // x this time starts as the final Z value from last time
    //    //x %= 26; // ensure x (i.e. prezZ) is in the range 0 to 25
    //    // First argument use, its always x being changed here, it is just the number that is different
    //    //x += arg1; // remember x = final z from last time, mod 26
    //    //x = input != x ? 1 : 0; // so, if `input != (prevZ % 26) + arg1`, then use 1 as X, otherwise use 0

    //    // Note z % 26 ensures z is in the range 0 to 25 here
    //    int x = input != (z % 26) + arg1 ? 1 : 0;

    //    var y = (25 * x) + 1;

    //    //y *= x;
    //    //y += 1;

    //    // note: divAmount this is set to 1 a lot to begin with, then more frequently to 26, ending with 26 4 times in a row
    //    z = (z / divAmount) * y;

    //    //z *= y;

    //    // Second argument use, its always y being changed here, it is just the number that is different
    //    y = (input + arg2) * x;

    //    //y += arg2;
    //    //y *= x;

    //    z += y;

    //    return z;
    //}

    //public static int ALUCodePortedToCSharp(Func<int> getInput, int z, int arg1, int arg2, int divAmount = 1)
    //{
    //    var input = getInput();

    //    // CORRECT: if input == (z % 26) + arg1 then use 0
    //    // CORRECT: var theBool = input != (z % 26) + arg1;

    //    // Note z % 26 ensures z is in the range 0 to 25 here
    //    var notAMatch = input != (z % 26) + arg1;

    //    var y1 = notAMatch ? 26 : 1;

    //    // note: divAmount this is set to 1 a lot to begin with, then more frequently to 26, ending with 26 4 times in a row
    //    z = (z / divAmount) * y1;

    //    var y2 = notAMatch ? input + arg2 : 0;

    //    return z + y2;
    //}

    [Test]
    public void WtfTest()
    {
        var chunks = Program.ParseInToChunks(PuzzleInput);

        //var programChunks = chunks.Select((chunk, i) =>
        //{
        //    //var addNumberInstructions = chunk.Instructions.Where(x => x is AddInstruction add && add.B.IsNumber).Cast<AddInstruction>().ToArray();
        //    return new ProgramChunk(
        //        i + 1,
        //        chunk,
        //        chunk.Instructions[4].Line == "div z 26",
        //        ((AddInstruction) chunk.Instructions[5]).B.Number,
        //        ((AddInstruction) chunk.Instructions[15]).B.Number);
        //    //isChunkThatReducesZ = chunk.Instructions.Any(x => x.Line == "div z 26"),
        //    //arg1 = addNumberInstructions.First(),
        //    //arg2 = addNumberInstructions.Last()
        //}).ToArray();

        //foreach (var result in programChunks)
        //{
        //    Console.WriteLine(result);
        //}


    }

    //public record ProgramChunk(int ChunkNum, Program Chunk, bool IsChunkThatReducesZ, int Arg1, int Arg2);

    [Test]
    public void ALUCodePortedToCSharp_Test2()
    {
        //Assert.Ignore("This takes far too long, it is the wrong approach!");

        var chars = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        // 4:
        //var inputs = chars.SelectMany(
        //    c1 => chars.SelectMany(
        //        c2 => chars.SelectMany(
        //            c3 => chars.Select(c4 => $"{c1}{c2}{c3}{c4}")))).ToArray();

        // 5:
        //var inputs = chars.SelectMany(
        //    c1 => chars.SelectMany(
        //        c2 => chars.SelectMany(
        //            c3 => chars.SelectMany(
        //                c4 => chars.Select(c5 => $"{c1}{c2}{c3}{c4}{c5}"))))).ToArray();

        // 6:
        var inputs = chars.SelectMany(
            c1 => chars.SelectMany(
                c2 => chars.SelectMany(
                    c3 => chars.SelectMany(
                        c4 => chars.SelectMany(
                            c5 => chars.Select(c6 => $"{c1}{c2}{c3}{c4}{c5}{c6}")))))).ToArray();

        // Noticed, the / 26 that happens occasionally, if z < 26, that turns z back to zero, but only if there is a match, otherwise the input + arg2 at end gets added to z, making it non zero
        // So to end with, we must have a match, and must have z < 26
        // A match being: input == (z % 26) + arg1, and because in this case z must be < 26, the end match can be simply written: input == z + arg1
        // But, argument1 for block 14 is -12 !!!

        //Console.WriteLine(ALUCodePortedToCSharp(input: 9, z: 5, 4, 8, 26)); // <-- this produces a zero!

        var program = PuzzleInputProgramFirst5Blocks;

        program.Instructions.First().Line.Should().Be("inp w");
        program.Instructions.Last().Line.Should().Be("add z y");

        var results = inputs.Select(input =>
            {
                var z = 0;
                var queue = new Queue<int>(input.Select(c => int.Parse(c.ToString())));

                // 1:
                z = ALUCodePortedToCSharp(queue.Dequeue(), z, 11, 8);
                //Console.WriteLine("z: " + z);

                // 2:
                z = ALUCodePortedToCSharp(queue.Dequeue(), z, 14, 13);
                //Console.WriteLine("z: " + z);

                // 3:
                z = ALUCodePortedToCSharp(queue.Dequeue(), z, 10, 2);
                var z3 = z;
                //Console.WriteLine("z: " + z);

                // 4:
                z = ALUCodePortedToCSharp(queue.Dequeue(), z, 0, 7/*, 26*/);
                var z4 = z;
                //Console.WriteLine("z: " + z);

                // 5:
                z = ALUCodePortedToCSharp(queue.Dequeue(), z, 12, 11);
                var prevZ = z;

                // 6:
                z = ALUCodePortedToCSharp(queue.Dequeue(), z, 12, 4);

                //// 7:
                //z = ALUCodePortedToCSharp(queue.Dequeue(), z, 12, 13);

                //// 8:
                //z = ALUCodePortedToCSharp(queue.Dequeue(), z, -8, 13, 26);

                //// 9:
                //z = ALUCodePortedToCSharp(queue.Dequeue(), z, -9, 10, 26);

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
        using var sw = new StreamWriter($@"C:\Dev\aoctemp\{DateTime.Now:yyyyMMdd-HHmmss}.txt");
        foreach (var result in results.OrderBy(x => x.z)) // .Where(res => res.z < 5000))
        {
            Console.WriteLine($"input: {result.input}; produces z: {result.z}");
            sw.WriteLine($"input: {result.input}; produces z: {result.z}");
        }

        // No valid input of the first character produces a z of zero, meaning the zo must have a following effect
        //Console.WriteLine(ALUCodePortedToCSharp(input: 1, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 2, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 3, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 4, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 5, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 6, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 7, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 8, z: 0, 11, 8));
        //Console.WriteLine(ALUCodePortedToCSharp(input: 9, z: 0, 11, 8));
    }

    [Test]
    public void ALUCodePortedToCSharp_Test()
    {
        int z = 0;

        // 1:
        z = ALUCodePortedToCSharp(input: 1, z, 11, 8);
        z.Should().Be(9);

        // 2:
        z = ALUCodePortedToCSharp(input: 3, z, 14, 13);
        z.Should().Be(250);

        // 3:
        z = ALUCodePortedToCSharp(input: 5, z, 10, 2);
        z.Should().Be(6507);

        // 4:
        z = ALUCodePortedToCSharp(input: 7, z, 0, 7/*, 26*/);
        z.Should().Be(250);

        // 5:
        z = ALUCodePortedToCSharp(input: 9, z, 12, 11);
        z.Should().Be(6520);

        // 6:
        z = ALUCodePortedToCSharp(input: 2, z, 12, 4);
        z.Should().Be(169526);

        // 7:
        z = ALUCodePortedToCSharp(input: 4, z, 12, 13);
        z.Should().Be(4407693);

        // 8:
        z = ALUCodePortedToCSharp(input: 6, z, -8, 13/*, 26*/);
        z.Should().Be(4407695);

        // 9:
        z = ALUCodePortedToCSharp(input: 8, z, -9, 10/*, 26*/);
        z.Should().Be(4407694);

        // 10:
        z = ALUCodePortedToCSharp(input: 9, z, 11, 1);
        z.Should().Be(114600054);

        // 11:
        z = ALUCodePortedToCSharp(input: 9, z, 0, 2/*, 26*/);
        z.Should().Be(114600055);

        // 12:
        z = ALUCodePortedToCSharp(input: 9, z, -5, 14/*, 26*/);
        z.Should().Be(114600067);

        // 13:
        z = ALUCodePortedToCSharp(input: 9, z, -6, 6/*, 26*/);
        z.Should().Be(114600059);

        // 14:
        z = ALUCodePortedToCSharp(input: 9, z, -12, 14/*, 26*/);
        z.Should().Be(114600067);
    }

    private static readonly Program PuzzleInputProgram = Program.Parse(new InputLoader(new Day24Solver()).PuzzleInputPart1);

    private static readonly Program PuzzleInputProgramFirst3Blocks = Program.Parse(
        string.Join(Environment.NewLine, new InputLoader(
                new Day24Solver()).PuzzleInputPart1.ReadLines()
            .Select((line, index) => new { line, lineNumber = index + 1 })
            .TakeWhile(x => x.lineNumber <= 54)
            .Select(x => x.line)));

    private static readonly Program PuzzleInputProgramFirst4Blocks = Program.Parse(GetPuzzleInputLinesInclusive(1, 72));

    private static readonly Program PuzzleInputProgramFirst5Blocks = Program.Parse(GetPuzzleInputLinesInclusive(1, 90));

    private static readonly Program PuzzleInputProgramLast3Blocks = Program.Parse(GetPuzzleInputLinesInclusive(199, 252));

    private static readonly Program PuzzleInputProgramLast1Block = Program.Parse(GetPuzzleInputLinesInclusive(235, 252));

    [Test]
    public void PuzzleInput_Some_Blocks_Tests()
    {
        Assert.Ignore("wrong approach!");

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

    //private static readonly Program PuzzleInputProgramLast3Blocks = Program.Parse(
    //    string.Join(Environment.NewLine, new InputLoader(
    //            new Day24Solver()).PuzzleInputPart1.ReadLines()
    //        .Select((line, index) => new { line, lineNumber = index + 1 })
    //        .SkipWhile(x => x.lineNumber < 199)
    //        .Select(x => x.line)));

    private static readonly Program PuzzleInputProgramLast4Blocks = Program.Parse(
        string.Join(Environment.NewLine, new InputLoader(
                new Day24Solver()).PuzzleInputPart1.ReadLines()
            .Select((line, index) => new { line, lineNumber = index + 1 })
            .SkipWhile(x => x.lineNumber < 181)
            .Select(x => x.line)));

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

    //private const string ExampleInput = @"";

    //[Test]
    //public void Part1Example()
    //{
    //    // ACT
    //    var part1ExampleResult = _sut.SolvePart1(ExampleInput);

    //    // ASSERT
    //    part1ExampleResult.Should().Be(null);
    //}

    [Test]
    public void Part1ReTest()
    {
        _sut.OnProgress = TestContext.Progress.WriteLine;
        _sut.OnProgress("test");

        // ACT
        var part1Result = _sut.SolvePart1();

        // ASSERT
        part1Result.Should().Be(92793949489995);
    }

    //[Test]
    //public void Part2Example()
    //{
    //    // ACT
    //    var part2ExampleResult = _sut.SolvePart2(ExampleInput);

    //    // ASSERT
    //    part2ExampleResult.Should().Be(null);
    //}

    [Test]
    public void Part2ReTest()
    {
        // ACT
        var part2Result = _sut.SolvePart2();

        // ASSERT
        part2Result.Should().Be(51131616112781);
    }
}
