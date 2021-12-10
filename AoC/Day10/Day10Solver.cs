using System.Text;

namespace AoC.Day10;

public class Day10Solver : SolverBase
{
    public override string DayName => "Syntax Scoring";

    public override long? SolvePart1(PuzzleInput input)
    {
        static long GetIllegalCharacterScore(char? illegalCharacter) => illegalCharacter switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => 0
        };

        var scores = input.ReadLines().Select(line =>
        {
            try
            {
                ParseLine(line);
                return (char?) null;
            }
            catch (CorruptedLineException e)
            {
                return e.IllegalCharacter;
            }
        }).GroupBy(chr => chr).Select(g => GetIllegalCharacterScore(g.Key) * g.Count());

        return scores.Aggregate(0L, (agg, cur) => agg + cur);
    }

    public override long? SolvePart2(PuzzleInput input) => GetIncompleteLines(input).Select(x => x.GetCompletionScore()).Median();

    public class Chunk
    {
        public char OpenBracket { get; }
        public char ExpectedCloseBracket { get; }
        public List<Chunk> Children { get; } = new();
        public Chunk? Parent { get; private set; }
        public bool IsRoot { get; }

        public Chunk(char openBracket)
        {
            OpenBracket = openBracket;
            IsRoot = false;
            ExpectedCloseBracket = GetExpectedClose(openBracket);
        }

        private Chunk()
        {
            IsRoot = true;
            OpenBracket = ExpectedCloseBracket = (char) 0;
        }

        public static Chunk NewRootChunk() => new();

        public static char GetExpectedClose(char openBracket) => openBracket switch
        {
            '(' => ')',
            '[' => ']',
            '{' => '}',
            '<' => '>',
            _ => throw new InvalidOperationException("Unexpected open bracket: " + openBracket)
        };

        public Chunk AddChild(Chunk childChunk)
        {
            if (childChunk.Parent != null)
            {
                throw new InvalidOperationException("Chunk has already been added as a child because it already has a parent");
            }

            Children.Add(childChunk);
            childChunk.Parent = this;

            return childChunk;
        }

        public string GetCompletionString()
        {
            var currentChunk = this;
            var completionString = new StringBuilder();
            while (currentChunk is {IsRoot: false})
            {
                completionString.Append(currentChunk.ExpectedCloseBracket);
                currentChunk = currentChunk.Parent;
            }

            return completionString.ToString();
        }

        public long GetCompletionScore()
        {
            static long GetCompletionCharacterScore(char completionCharacter) => completionCharacter switch
            {
                ')' => 1,
                ']' => 2,
                '}' => 3,
                '>' => 4,
                _ => throw new InvalidOperationException("Invalid completion character: " + completionCharacter)
            };

            return GetCompletionString()
                .Select(GetCompletionCharacterScore)
                .Aggregate(0L, (current, completionCharacterScore) => current * 5 + completionCharacterScore);
        }
    }

    /// <summary>
    /// Parses the specified line in to the root chunk, containing any child chunks.
    /// If the line is incomplete, null is returned.
    /// If the line is corrupted, an exception is thrown.
    /// </summary>
    public static (Chunk rootChunk, Chunk currentChunk) ParseLine(string line)
    {
        var rootChunk = Chunk.NewRootChunk();
        var currentChunk = rootChunk;

        foreach (var chr in line)
        {
            var isOpen = chr is '(' or '[' or '{' or '<';
            var isClose = chr is ')' or ']' or '}' or '>';

            if (isOpen)
            {
                currentChunk = currentChunk.AddChild(new Chunk(chr));
            }
            else if (isClose)
            {
                if (chr == currentChunk.ExpectedCloseBracket)
                {
                    currentChunk = currentChunk.Parent ?? throw new InvalidOperationException("Cannot navigate above root chunk");
                }
                else
                {
                    throw new CorruptedLineException(line, chr, currentChunk.ExpectedCloseBracket);
                }
            }
            else
            {
                throw new InvalidOperationException($"Unexpected character '{chr}' in line '{line}'");
            }
        }

        return (rootChunk, currentChunk);
    }

    public static IEnumerable<Chunk> GetIncompleteLines(PuzzleInput input)
    {
        foreach (var line in input.ReadLines())
        {
            Chunk? incompleteChunk = null;
            try
            {
                var (_, currentChunk) = ParseLine(line);
                if (!currentChunk.IsRoot)
                    incompleteChunk = currentChunk;
            }
            catch (CorruptedLineException)
            {
                // Discard the corrupted lines
            }

            if (incompleteChunk != null) yield return incompleteChunk;
        }
    }

    public class CorruptedLineException : Exception
    {
        public string Line { get; }
        public char IllegalCharacter { get; }
        public char ExpectedClose { get; }

        public CorruptedLineException(string line, char illegalCharacter, char expectedClose)
            : base($"Expected {expectedClose}, but found {illegalCharacter} instead.")
        {
            Line = line;
            IllegalCharacter = illegalCharacter;
            ExpectedClose = expectedClose;
        }
    }
}
