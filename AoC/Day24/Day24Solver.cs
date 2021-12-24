namespace AoC.Day24;

public class Day24Solver : SolverBase
{
    public override string DayName => "Arithmetic Logic Unit";

    private static readonly IReadOnlyList<int> ValidInputs = (1..10).ToArray();
    //private static readonly IReadOnlyList<int> ValidInitialZValues = (0..26).ToArray();
    private static readonly IReadOnlyList<int> ValidInitialZValues = (0..100000).ToArray();
    private const int ModelNumberLength = 14;

    public override long? SolvePart1(PuzzleInput input)
    {
        // Must end with Z being zero, so we can work back from there

        var chunks = Program.ParseInToChunks(input);

        var start = new Node(-1, 0, 0);

        OnProgress?.Invoke("keh");

        var largestModelNumberAcceptedByMonad = FindLargestModelNumberAcceptedByMonad(start, getSuccessors: node =>
        {
            var chunkIndex = node.ChunkIndex + 1;

            //if (chunkIndex )
            //{
            //    return Array.Empty<Node>();
            //}

            var chunk = chunks.ElementAtOrDefault(chunkIndex); // [chunkIndex];

            if (chunk == null)
            {
                return Array.Empty<Node>();
            }

            return GetAllMovesToNextChunk(chunk, node.Z)
                .Select(move => new Node(chunkIndex, move.inputNum, move.z));
        });

        return largestModelNumberAcceptedByMonad;

        //var start = new Node(chunks.Count, new InputCombination());

        //var largestModelNumberAcceptedByMonad = FindLargestModelNumberAcceptedByMonad(start, getSuccessors: node =>
        //{
        //    var previousChunkIndex = node.ChunkIndex - 1;

        //    // Temp cut out
        //    if (previousChunkIndex < chunks.Count - 1)
        //    {
        //        return Array.Empty<Node>();
        //    }

        //    if (previousChunkIndex < 0)
        //    {
        //        return Array.Empty<Node>();
        //    }

        //    var previousChunk = chunks[previousChunkIndex];

        //    return GetInputCombinationsToProduceTargetZ(previousChunk, node.InputCombination.InitialZ)
        //        .Select(combination => new Node(previousChunkIndex, combination));
        //}, true);

        //Console.WriteLine("From start:");

        //FindLargestModelNumberAcceptedByMonad(new Node(-1, new InputCombination()), getSuccessors: node =>
        //{
        //    var previousChunkIndex = node.ChunkIndex + 1;

        //    if (previousChunkIndex != 0)
        //    {
        //        return Array.Empty<Node>();
        //    }

        //    var previousChunk = chunks[previousChunkIndex];

        //    return GetInputCombinationsToProduceTargetZ(previousChunk, node.InputCombination.InitialZ)
        //        .Select(combination => new Node(previousChunkIndex, combination));
        //}, true);

        //return largestModelNumberAcceptedByMonad;


        //foreach (var inputCombination in GetInputCombinationsToProduceTargetZ(chunks.Last(), 0))
        //{
        //    Console.WriteLine($"{inputCombination} produces targetZ of {0}");
        //}

        //var targetZ = 0;
        //var lastChunk = chunks.Last();

        //// Note ranges are inclusive lower bound, and exclusive upper bound
        //var validInputs = (1..10).ToArray();
        //var validInitialZValues = (0..100000).ToArray();

        ///*
        // * Ok, so, think I might have it!
        // * From a validity perspective, the z % 26 ensures z is in the range 0 to 25
        // *
        // * So, for each step, we only need to try those various inputs of Z (0 - 25) combined with 1 to 9 for the actual input
        // *
        // * Work backwards, run all those combinations, that will give us a result Z and the input num
        // * For the last stage, we want a result Z of zero
        // * So, which combinations produce that?
        // */

        //var combinations = validInputs.SelectMany(inputNum => validInitialZValues.Select(initialZ => new { input = inputNum, initialZ })).ToArray();

        //foreach (var combination in combinations)
        //{
        //    var result = lastChunk.Execute(combination.input, new ProgramState(z: combination.initialZ));

        //    if (result.Z == targetZ)
        //    {
        //        Console.WriteLine($"{combination} produces targetZ of {targetZ}");
        //    }
        //}

        //foreach (var instruction in program.Instructions)
        //{
        //    Console.WriteLine(instruction);
        //}

        return null;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    private record Node(int ChunkIndex, int InputNum, int Z);

    //private readonly record struct InputCombination(int InputNum, int InitialZ);

    //private static int NodeCost(Node node) => 9 - node.InputNum;

    private static long PathCost(int[] path) => 100000000000000 - long.Parse(string.Join("", path).PadRight(14, '0'));

    private static long PathCostSmallest(int[] path) => long.Parse(string.Join("", path).PadRight(14, '0'));

    public Action<string>? OnProgress;

    /// <summary>
    /// Finds the shortest path between the two specified locations in the specified grid.
    /// Written from the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    /// </summary>
    private long FindLargestModelNumberAcceptedByMonad(
        Node start,
        Func<Node, IEnumerable<Node>> getSuccessors,
        bool displayPath = false)
    {
        var explore = new PriorityQueue<(Node node, IReadOnlyList<int> path, long cost), long>();
        explore.Enqueue((start, Array.Empty<int>(), 0), 0);

        var seen = new HashSet<Node>();

        //var results = new List<long>();
        //OnProgress?.Invoke("started");

        while (explore.Count > 0)
        {
            var (node, path, cost) = explore.Dequeue(); // this takes out the top priority node

            // if node is the goal then we have our result number
            if (path.Count == ModelNumberLength && node.Z == 0)
            {
                var modelNumber = long.Parse(string.Join("", path/*.Reverse()*/));

                return modelNumber;

                //OnProgress?.Invoke($"modelNumber: {modelNumber}");

                ////return modelNumber;
                //results.Add(modelNumber);
            }

            // if we've not already seen the node
            if (!seen.Contains(node))
            {
                foreach (var child in getSuccessors(node))
                {
                    //var childPath = path.Append(child);
                    //explore.Enqueue(childPath, childPath.TotalCost + ValidInputToCost(child)); // the heuristic is added here as a part of the priority
                    var childPath = path.Concat(new[] { child.InputNum }).ToArray(); // Note prepend because we work backwards through the chunks

                    var stepCost = PathCostSmallest(childPath);

                    //if (displayPath || childPath.Length > 8)
                    //{
                    //    Console.WriteLine(string.Join("", childPath));
                    //}

                    explore.Enqueue((child, childPath, stepCost + cost), stepCost + cost);
                }

                seen.Add(node);
            }
        }

        //return results;

        throw new InvalidOperationException("No paths found");
        //return null;
    }

    //private static readonly IReadOnlyList<InputCombination> Combinations =
    //    ValidInputs.SelectMany(inputNum => ValidInitialZValues.Select(initialZ => new InputCombination(inputNum, initialZ))).ToArray();

    private static IEnumerable<(int inputNum, int z)> GetAllMovesToNextChunk(ProgramChunk chunk, int initialZ)
    {
        //var combinations = ValidInputs.SelectMany(inputNum => ValidInitialZValues.Select(initialZ => new InputCombination(inputNum, initialZ))).ToArray();

        // Each chunk, we need to try 1 to 9
        // At any point where we hit a chunk where Z could go down, exclude any where Z doesn't go down (so we get back down to zero eventually for a valid number)
        foreach (var inputNum in ValidInputs)
        {
            var resultState = chunk.Program.Execute(inputNum, new ProgramState(z: initialZ));

            if (!chunk.IsChunkThatReducesZ || resultState.Z < initialZ)
            {
                yield return (inputNum, resultState.Z);
            }

            //if (result.Z == targetZ)
            //{
            //    //Console.WriteLine($"{combination} produces targetZ of {targetZ}");
            //    yield return combination;
            //}
        }
    }

    ///// <summary>
    ///// Finds the shortest path between the two specified locations in the specified grid.
    ///// Written from the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    ///// </summary>
    //private static long? FindLargestModelNumberAcceptedByMonad(
    //    Node start,
    //    Func<Node, IEnumerable<Node>> getSuccessors,
    //    bool displayPath = false)
    //{
    //    var explore = new PriorityQueue<(Node node, IReadOnlyList<int> path, long cost), long>();
    //    explore.Enqueue((start, Array.Empty<int>(), 0), 0);

    //    var seen = new HashSet<Node>();

    //    while (explore.Count > 0)
    //    {
    //        var (node, path, cost) = explore.Dequeue(); // this takes out the top priority node

    //        // if node is the goal then we have our result number
    //        if (path.Count == ModelNumberLength)
    //        {
    //            var modelNumber = long.Parse(string.Join("", path/*.Reverse()*/));
    //            return modelNumber;
    //        }

    //        // if we've not already seen the node
    //        if (!seen.Contains(node))
    //        {
    //            foreach (var child in getSuccessors(node))
    //            {
    //                var stepCost = NodeCost(child);
    //                //var childPath = path.Append(child);
    //                //explore.Enqueue(childPath, childPath.TotalCost + ValidInputToCost(child)); // the heuristic is added here as a part of the priority
    //                var childPath = new[] { child.InputCombination.InputNum }.Concat(path).ToArray(); // Note prepend because we work backwards through the chunks

    //                if (displayPath || childPath.Length > 8)
    //                {
    //                    Console.WriteLine(string.Join("", childPath));
    //                }

    //                explore.Enqueue((child, childPath, stepCost + cost), stepCost + cost);
    //            }

    //            seen.Add(node);
    //        }
    //    }

    //    //throw new InvalidOperationException("No paths found");
    //    return null;
    //}


    //private static IEnumerable<InputCombination> GetInputCombinationsToProduceTargetZ(ProgramChunk chunk, int targetZ)
    //{
    //    //var combinations = ValidInputs.SelectMany(inputNum => ValidInitialZValues.Select(initialZ => new InputCombination(inputNum, initialZ))).ToArray();

    //    foreach (var combination in Combinations)
    //    {
    //        var result = chunk.Program.Execute(combination.InputNum, new ProgramState(z: combination.InitialZ));

    //        if (result.Z == targetZ)
    //        {
    //            //Console.WriteLine($"{combination} produces targetZ of {targetZ}");
    //            yield return combination;
    //        }
    //    }
    //}

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

        //public override string ToString() => $"Read an input value and write it to variable {A}.";
    }

    public record AddInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] += B.GetValue(state);

        //public override string ToString() => $"Add the value of variable {A} to the value of {B}, then store the result in variable {A}.";
    }

    public record MultiplyInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] *= B.GetValue(state);

        //public override string ToString() => $"Multiply the value of variable {A} by the value of {B}, then store the result in variable {A}.";
    }

    public record DivideInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] /= B.GetValue(state);

        //public override string ToString() =>
        //    $"Divide the value of variable {A} by the value of {B}, truncate the result to an integer, then store the result in variable {A}.";
    }

    public record ModInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput) => state[A] %= B.GetValue(state);

        //public override string ToString() =>
        //    $"(Mod instruction) Divide the value of variable {A} by the value of {B}, then store the remainder in variable {A}.";
    }

    public record EqualsInstruction(string Line, char A, BPart B) : Instruction(Line)
    {
        public override void Execute(ProgramState state, Func<int> getNextInput)
        {
            int a = state[A];
            int b = B.GetValue(state);
            state[A] = a == b ? 1 : 0;
        }

        //public override string ToString() =>
        //    $"If the value of variable {A} and {B} are equal, then store the value 1 in variable {A}. Otherwise, store the value 0 in variable {A}.";
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
