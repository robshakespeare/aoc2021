namespace AoC.Day10;

public class Day10Solver : SolverBase
{
    public override string DayName => "Syntax Scoring";

    public override long? SolvePart1(PuzzleInput input)
    {
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
        }).GroupBy(chr => chr).Select(g => LookUpScore(g.Key) * g.Count());

        return scores.Aggregate(0L, (agg, cur) => agg + cur);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static long LookUpScore(char? illegalCharacter) => illegalCharacter switch
    {
        ')' => 3,
        ']' => 57,
        '}' => 1197,
        '>' => 25137,
        _ => 0
    };

    public class Chunk
    {
        public char OpenBracket { get; }
        public List<Chunk> Children { get; } = new();
        public Chunk? Parent { get; private set; }
        public bool IsRoot { get; }

        public Chunk(char openBracket)
        {
            OpenBracket = openBracket;
            IsRoot = openBracket == 'R';
        }

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
    }

    /// <summary>
    /// Parses the specified line in to the root chunk, containing any child chunks.
    /// If the line is incomplete, null is returned.
    /// If the line is corrupted, an exception is thrown.
    /// </summary>
    public static Chunk? ParseLine(string line)
    {
        /*
            If a chunk opens with (, it must close with ).
            If a chunk opens with [, it must close with ].
            If a chunk opens with {, it must close with }.
            If a chunk opens with <, it must close with >.
         */

        var rootChunk = new Chunk('R');
        var currentChunk = rootChunk;

        //bool IsValidClose(char closeBracket) =>
        //    (currentChunk.OpenBracket == '(' && closeBracket == ')') ||
        //    (currentChunk.OpenBracket == '[' && closeBracket == ']') ||
        //    (currentChunk.OpenBracket == '{' && closeBracket == '}') ||
        //    (currentChunk.OpenBracket == '<' && closeBracket == '>');

        static char GetExpectedClose(char openBracket) => openBracket switch
        {
            '(' => ')',
            '[' => ']',
            '{' => '}',
            '<' => '>',
            _ => throw new InvalidOperationException("Unexpected open bracket: " + openBracket)
        };

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
                var expectedClose = GetExpectedClose(currentChunk.OpenBracket);
                if (chr == expectedClose)
                {
                    currentChunk = currentChunk.Parent ?? throw new InvalidOperationException("Cannot navigate above root chunk");
                }
                else
                {
                    throw new CorruptedLineException(line, chr, expectedClose);
                }
            }
            else
            {
                throw new InvalidOperationException($"Unexpected character '{chr}' in line '{line}'");
            }
        }

        return currentChunk.IsRoot ? rootChunk : null;
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
