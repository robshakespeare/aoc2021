namespace AoC.Day24;

public class Day24Solver : SolverBase
{
    public override string DayName => "Arithmetic Logic Unit";

    public override long? SolvePart1(PuzzleInput input)
    {
        return null;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class Program
    {
        private readonly IReadOnlyList<IInstruction> _instructions;

        public Program(IReadOnlyList<IInstruction> instructions)
        {
            _instructions = instructions;
        }

        public ProgramState Execute(Func<int> getNextInput)
        {
            var state = new ProgramState();

            foreach (var instruction in _instructions)
            {
                instruction.Execute(state, getNextInput);
            }

            return state;
        }

        public static Program Parse(PuzzleInput input) => new(input.ReadLines().Select(line =>
        {
            var parts = line.Split(' ');
            var ins = parts[0];
            var a = parts[1].Single();
            var b = () => new BPart(parts[2]);

            return (IInstruction) (ins switch
            {
                "inp" => new InputInstruction(a),
                "add" => new AddInstruction(a, b()),
                "mul" => new MultiplyInstruction(a, b()),
                "div" => new DivideInstruction(a, b()),
                "mod" => new ModInstruction(a, b()),
                "eql" => new EqualsInstruction(a, b()),
                _ => throw new InvalidOperationException("Invalid instruction line: " + line)
            });
        }).ToArray());
    }

    public interface IInstruction
    {
        void Execute(ProgramState state, Func<int> getNextInput);
    }

    public record InputInstruction(char A) : IInstruction
    {
        public void Execute(ProgramState state, Func<int> getNextInput) => state[A] = getNextInput();

        public override string ToString() => $"Read an input value and write it to variable {A}.";
    }

    public record AddInstruction(char A, BPart B) : IInstruction
    {
        public void Execute(ProgramState state, Func<int> getNextInput) => state[A] += B.GetValue(state);

        public override string ToString() => $"Add the value of variable {A} to the value of {B}, then store the result in variable {A}.";
    }

    public record MultiplyInstruction(char A, BPart B) : IInstruction
    {
        public void Execute(ProgramState state, Func<int> getNextInput) => state[A] *= B.GetValue(state);

        public override string ToString() => $"Multiply the value of variable {A} by the value of {B}, then store the result in variable {A}.";
    }

    public record DivideInstruction(char A, BPart B) : IInstruction
    {
        public void Execute(ProgramState state, Func<int> getNextInput) => state[A] /= B.GetValue(state);

        public override string ToString() =>
            $"Divide the value of variable {A} by the value of {B}, truncate the result to an integer, then store the result in variable {A}.";
    }

    public record ModInstruction(char A, BPart B) : IInstruction
    {
        public void Execute(ProgramState state, Func<int> getNextInput) => state[A] %= B.GetValue(state);

        public override string ToString() =>
            $"(Mod instruction) Divide the value of variable {A} by the value of {B}, then store the remainder in variable {A}.";
    }

    public record EqualsInstruction(char A, BPart B) : IInstruction
    {
        public void Execute(ProgramState state, Func<int> getNextInput)
        {
            int a = state[A];
            int b = B.GetValue(state);
            state[A] = a == b ? 1 : 0;
        }

        public override string ToString() =>
            $"If the value of variable {A} and {B} are equal, then store the value 1 in variable {A}. Otherwise, store the value 0 in variable {A}.";
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
