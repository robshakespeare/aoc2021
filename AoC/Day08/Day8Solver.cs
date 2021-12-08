namespace AoC.Day08;

public class Day8Solver : SolverBase
{
    public override string DayName => "Seven Segment Search";

    public override long? SolvePart1(PuzzleInput input)
    {
        var entries = SignalEntry.ParseToEntries(input);

        return entries.SelectMany(x => x.FourDigitOutputValue).Count(digit => digit.Length is 2 or 4 or 3 or 7);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class SignalEntry
    {
        /// <summary>
        /// Ten unique signal patterns.
        /// </summary>
        public IReadOnlyList<string> SignalPatterns { get; }

        /// <summary>
        /// Four digit output value.
        /// </summary>
        public IReadOnlyList<string> FourDigitOutputValue { get; }

        public SignalEntry(IEnumerable<string> signalPatterns, IEnumerable<string> fourDigitOutputValue)
        {
            SignalPatterns = signalPatterns.ToReadOnlyArray();
            FourDigitOutputValue = fourDigitOutputValue.ToReadOnlyArray();

            if (SignalPatterns.Count != 10)
                throw new InvalidOperationException(
                    $"Invalid signal patterns count ({SignalPatterns.Count}): {string.Join(" ", SignalPatterns)}");

            if (FourDigitOutputValue.Count != 4)
                throw new InvalidOperationException(
                    $"Invalid four digit output value ({FourDigitOutputValue.Count}): {string.Join(" ", FourDigitOutputValue)}");
        }

        public static IReadOnlyList<SignalEntry> ParseToEntries(PuzzleInput puzzleInput) => puzzleInput.ReadLines().Select(line =>
        {
            var parts = line.Split(" | ");
            return new SignalEntry(parts[0].Split(' '), parts[1].Split(' '));
        }).ToReadOnlyArray();
    }
}
