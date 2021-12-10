using System.Text;

namespace AoC.Day10;

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
            throw new InvalidOperationException("Chunk has already been added as a child because it already has a parent");

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

    /// <summary>
    /// Parses the specified line in to the root chunk, containing any child chunks.
    /// If the line is incomplete, null is returned.
    /// If the line is corrupted, an exception is thrown.
    /// </summary>
    public static (Chunk rootChunk, Chunk currentChunk) ParseLine(string line)
    {
        var rootChunk = NewRootChunk();
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
                else throw new CorruptedLineException(line, chr, currentChunk.ExpectedCloseBracket);
            }
            else throw new InvalidOperationException($"Unexpected character '{chr}' in line '{line}'");
        }

        return (rootChunk, currentChunk);
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
