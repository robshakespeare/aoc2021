namespace AoC.Day08;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
public class Day8Solver : SolverBase
{
    public override string DayName => "Seven Segment Search";

    public override long? SolvePart1(PuzzleInput input) =>
        SignalEntry.ParseToEntries(input).SelectMany(x => x.FourDigitOutputValue).Count(digit => digit.Length is 2 or 4 or 3 or 7);

    public override long? SolvePart2(PuzzleInput input) =>
        SignalEntry.ParseToEntries(input).Select(x => x.DetermineOutputValue()).Sum();

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

        public long DetermineOutputValue()
        {
            var digit1 = SignalPatterns.Single(x => x.Length == 2); // cf (unique)
            var digit4 = SignalPatterns.Single(x => x.Length == 4); // bcdf (unique)
            var digit7 = SignalPatterns.Single(x => x.Length == 3); // acf (unique)
            var digit8 = SignalPatterns.Single(x => x.Length == 7); // abcdefg (unique)

            var knownA = digit7.Except(digit1).Single();

            // candidates for bd are digit4 except digit1
            var candidatesBD = string.Join("", digit4.Except(digit1));

            // candidates for eg are digit8 except digit4 & digit7
            var candidatesEG = string.Join("", digit8.Except(digit4).Except(digit7));

            // for the 5 digit patterns, b only appears in number 5
            // for the 5 digit patterns, e only appears in number 2
            var all5DigitPatterns = string.Join("", SignalPatterns.Where(x => x.Length == 5));
            var candidatesBE = string.Join("", all5DigitPatterns.GroupBy(c => c).Where(g => g.Count() == 1).Select(g => g.Key));

            var knownB = candidatesBE.Intersect(candidatesBD).Single();
            var knownE = candidatesBE.Intersect(candidatesEG).Single();

            var knownD = candidatesBD.Except(candidatesBE).Single();
            var knownG = candidatesEG.Except(candidatesBE).Single();

            // when we remove all knowns from the 6 digit patterns, we should b left with 3 f's and 2 c's
            var all6DigitPatterns = string.Join("", SignalPatterns.Where(x => x.Length == 6));
            var knownChars = new[] {knownA, knownB, knownE, knownD, knownG};

            var remaining = string.Join("", all6DigitPatterns.Where(c => !knownChars.Contains(c)))
                .GroupBy(c => c).ToDictionary(g => g.Count(), g => g.Key);

            var knownF = remaining[3];
            var knownC = remaining[2];

            return MapOutputValue(knownA, knownB, knownC, knownD, knownE, knownF, knownG);
        }

        private long MapOutputValue(char a, char b, char c, char d, char e, char f, char g)
        {
            static string OrderKey(params char[] key) => string.Join("", key.OrderBy(chr => chr));

            var digitMaps = new Dictionary<string, long>
            {
                {OrderKey(a, b, c, e, f, g), 0},
                {OrderKey(c, f), 1},
                {OrderKey(a, c, d, e, g), 2},
                {OrderKey(a, c, d, f, g), 3},
                {OrderKey(b, c, d, f), 4},
                {OrderKey(a, b, d, f, g), 5},
                {OrderKey(a, b, d, e, f, g), 6},
                {OrderKey(a, c, f), 7},
                {OrderKey(a, b, c, d, e, f, g), 8},
                {OrderKey(a, b, c, d, f, g), 9}
            };

            return long.Parse(string.Join("", FourDigitOutputValue.Select(x => digitMaps[OrderKey(x.ToCharArray())])));
        }
    }
}
