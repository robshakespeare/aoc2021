namespace AoC.Day24;

public class Day24Solver : SolverBase
{
    public override string DayName => "Arithmetic Logic Unit";

    public static readonly IReadOnlyList<int> ValidInputs = (1..10).ToArray();
    public const int ModelNumberLength = 14;

    public override long? SolvePart1(PuzzleInput input) => Solve(input, PathCostLargestModelNumber);

    public override long? SolvePart2(PuzzleInput input) => Solve(input, PathCostSmallestModelNumber);

    private long Solve(PuzzleInput input, Func<string, long> getPathCost)
    {
        var chunks = Program.ParseInToChunks(input);

        var start = new Node(-1, 0, 0);

        var targetModelNumber = FindTargetModelNumberAcceptedByMonad(
            start,
            getPathCost,
            getSuccessors: node =>
            {
                var chunkIndex = node.ChunkIndex + 1;
                var chunk = chunks.ElementAtOrDefault(chunkIndex);

                if (chunk == null)
                {
                    return Array.Empty<Node>();
                }

                return GetAllMovesToNextChunk(chunk, node.Z)
                    .Select(move => new Node(chunkIndex, move.InputNum, move.ResultZ));
            });

        return targetModelNumber;
    }

    private record Node(int ChunkIndex, int InputNum, int Z);

    private static long PathCostLargestModelNumber(string path) => 100000000000000 - long.Parse(path.PadRight(14, '0'));

    private static long PathCostSmallestModelNumber(string path) => long.Parse(path.PadRight(14, '0'));

    /// <summary>
    /// Based on the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    /// </summary>
    private long FindTargetModelNumberAcceptedByMonad(
        Node start,
        Func<string, long> getPathCost,
        Func<Node, IEnumerable<Node>> getSuccessors)
    {
        var explore = new PriorityQueue<(Node node, string path, long cost), long>();
        explore.Enqueue((start, "", 0), 0);

        var seen = new HashSet<Node>();

        while (explore.Count > 0)
        {
            var (node, path, cost) = explore.Dequeue(); // this takes out the top priority node

            // We have found the target model number if we have a full 14 digits
            // and Z is zero (it will indicate that the model number was valid by leaving a 0 in variable z)
            if (path.Length == ModelNumberLength && node.Z == 0)
            {
                var modelNumber = long.Parse(path);
                return modelNumber;
            }

            // if we've not already seen the node
            if (!seen.Contains(node))
            {
                foreach (var child in getSuccessors(node))
                {
                    var childPath = string.Concat(path, child.InputNum.ToString());
                    var stepCost = getPathCost(childPath);
                    explore.Enqueue((child, childPath, stepCost + cost), stepCost + cost);
                }

                seen.Add(node);
            }
        }

        throw new InvalidOperationException("No paths found");
    }

    private readonly record struct ChunkInput(int InputNum, int Z);

    private static IEnumerable<(int InputNum, int ResultZ)> GetAllMovesToNextChunk(ProgramChunk chunk, int initialZ)
    {
        // Each chunk, we need to try 1 to 9
        // At any point where we hit a chunk where Z could go down, exclude any where Z doesn't go down (so we get back down to zero eventually for a valid number)
        foreach (var inputNum in ValidInputs)
        {
            var resultState = chunk.Program.Execute(inputNum, new ProgramState(z: initialZ));
            var resultZ = resultState.Z;

            // These were both correct!
            //var resultZ = AluCodePortedToCSharpV1(inputNum, initialZ, chunk.Arg1, chunk.Arg2);
            //var resultZ = AluCodePortedToCSharpV2(inputNum, initialZ, chunk.Arg1, chunk.Arg2);

            if (!chunk.IsChunkThatReducesZ || resultZ < initialZ)
            {
                yield return (inputNum, resultZ);
            }
        }
    }

    public static int AluCodePortedToCSharpV2(int input, int z, int arg1, int arg2)
    {
        var notAMatch = input != z % 26 + arg1;

        // note: No need to pass in divAmount, if arg1 is <= 0 then it divides by 26, otherwise it divides by 1 which has no effect
        if (arg1 <= 0)
        {
            z /= 26;
        }

        if (notAMatch)
        {
            z = z * 26 + (input + arg2);
        }

        return z;
    }

    public static int AluCodePortedToCSharpV1(int input, int z, int arg1, int arg2)
    {
        var notAMatch = input != (z % 26) + arg1;

        // note: No need to pass in divAmount, if arg1 is <= 0 then it divides by 26, otherwise it divides by 1 which has no effect
        var divAmount = arg1 <= 0 ? 26 : 1;

        z = (z / divAmount) * (notAMatch ? 26 : 1);

        return z + (notAMatch ? input + arg2 : 0);
    }

    public class Program
    {
        public IReadOnlyList<Instruction> Instructions { get; }

        public bool EnableDiagnostics { get; set; }

        public Program(IReadOnlyList<Instruction> instructions) => Instructions = instructions;

        public ProgramState Execute(int input, ProgramState? state = null) => Execute(new[] {input}, state);

        public ProgramState Execute(IEnumerable<int> inputs, ProgramState? state = null)
        {
            var queue = new Queue<int>(inputs);
            return Execute(() => queue.Dequeue(), state);
        }

        public ProgramState Execute(Func<int> getNextInput, ProgramState? state = null)
        {
            state ??= new ProgramState();

            foreach (var instruction in Instructions)
            {
                if (EnableDiagnostics)
                {
                    Console.WriteLine(instruction);
                }

                instruction.Execute(state, getNextInput);

                if (EnableDiagnostics)
                {
                    Console.WriteLine(state);
                    Console.WriteLine();
                }
            }

            return state;
        }

        private static Instruction ParseLine(string line)
        {
            var parts = line.Split(' ');
            var ins = parts[0];
            var a = parts[1].Single();
            var b = () => new BPart(parts[2]);

            return ins switch
            {
                "inp" => new InputInstruction(line, a),
                "add" => new AddInstruction(line, a, b()),
                "mul" => new MultiplyInstruction(line, a, b()),
                "div" => new DivideInstruction(line, a, b()),
                "mod" => new ModInstruction(line, a, b()),
                "eql" => new EqualsInstruction(line, a, b()),
                _ => throw new InvalidOperationException("Invalid instruction line: " + line)
            };
        }

        public static Program Parse(PuzzleInput input) => new(input.ReadLines().Select(ParseLine).ToArray());

        private static readonly string ChunkSeparator = $"inp w{Environment.NewLine}";

        public static IReadOnlyList<ProgramChunk> ParseInToChunks(PuzzleInput input) =>
            input.ToString()
                .Split(ChunkSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(chunk => Parse($"{ChunkSeparator}{chunk}"))
                .Select((chunk, i) => new ProgramChunk(
                    i + 1,
                    chunk,
                    chunk.Instructions[4].Line == "div z 26",
                    ((AddInstruction) chunk.Instructions[5]).B.Number,
                    ((AddInstruction) chunk.Instructions[15]).B.Number))
                .ToArray();
    }

    public record ProgramChunk(int ChunkNum, Program Program, bool IsChunkThatReducesZ, int Arg1, int Arg2);

    public abstract record Instruction(string Line)
    {
        public abstract void Execute(ProgramState state, Func<int> getNextInput);

        public sealed override string ToString() => Line;
    }

    public record InputInstruction(string Line, char A) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] = getNextInput();
    }

    public record AddInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] += B.GetValue(state);
    }

    public record MultiplyInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] *= B.GetValue(state);
    }

    public record DivideInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] /= B.GetValue(state);
    }

    public record ModInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] %= B.GetValue(state);
    }

    public record EqualsInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput)
        {
            int a = state[A];
            int b = B.GetValue(state);
            state[A] = a == b ? 1 : 0;
        }
    }

    public record BPart
    {
        public string B { get; }
        public bool IsNumber { get; }
        public int Number { get; }
        public char VariableName { get; }

        public BPart(string b)
        {
            B = b;
            IsNumber = int.TryParse(b, out var num);
            Number = num;
            VariableName = !IsNumber ? b.Single() : default;
        }

        public int GetValue(ProgramState state) => IsNumber ? Number : state[VariableName];

        public override string ToString() => IsNumber ? Number.ToString() : $"variable {VariableName}";
    }

    public class ProgramState
    {
        public int W { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public ProgramState(int w = 0, int x = 0, int y = 0, int z = 0)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public int this[char variableName]
        {
            get => variableName switch
            {
                'w' => W,
                'x' => X,
                'y' => Y,
                'z' => Z,
                _ => throw new InvalidOperationException("Unable to get variable. Invalid variable: " + variableName)
            };
            set
            {
                switch (variableName)
                {
                    case 'w':
                        W = value;
                        break;
                    case 'x':
                        X = value;
                        break;
                    case 'y':
                        Y = value;
                        break;
                    case 'z':
                        Z = value;
                        break;
                    default:
                        throw new InvalidOperationException("Unable to set variable. Invalid variable: " + variableName);
                }
            }
        }

        public override string ToString() => $"w: {W}, x: {X}, y: {Y}, z: {Z}";
    }
}
