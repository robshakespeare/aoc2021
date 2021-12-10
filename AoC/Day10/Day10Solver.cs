namespace AoC.Day10;

public class Day10Solver : SolverBase
{
    public override string DayName => "Syntax Scoring";

    public override long? SolvePart1(PuzzleInput input)
    {
        static long GetIllegalCharacterScore(char illegalCharacter) => illegalCharacter switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new InvalidOperationException("Invalid illegal character: " + illegalCharacter)
        };

        var scores = input.ReadLines().Select(line =>
        {
            try
            {
                Chunk.ParseLine(line);
                return (char) 0;
            }
            catch (Chunk.CorruptedLineException e)
            {
                return e.IllegalCharacter;
            }
        }).Where(c => c != 0).GroupBy(chr => chr).Select(g => GetIllegalCharacterScore(g.Key) * g.Count());

        return scores.Aggregate(0L, (agg, cur) => agg + cur);
    }

    public override long? SolvePart2(PuzzleInput input) => GetIncompleteLines(input).Select(x => x.GetCompletionScore()).Median();

    public static IEnumerable<Chunk> GetIncompleteLines(PuzzleInput input)
    {
        foreach (var line in input.ReadLines())
        {
            Chunk? incompleteChunk = null;
            try
            {
                var (_, currentChunk) = Chunk.ParseLine(line);
                if (!currentChunk.IsRoot)
                    incompleteChunk = currentChunk;
            }
            catch (Chunk.CorruptedLineException)
            {
                // Discard the corrupted lines
            }

            if (incompleteChunk != null) yield return incompleteChunk;
        }
    }
}
