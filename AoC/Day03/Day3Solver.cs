namespace AoC.Day03;

/// <summary>
/// Slightly more optimized version of <see cref="Day3SolverOriginal"/>.
/// </summary>
public class Day3Solver : SolverBase
{
    public override string DayName => "Binary Diagnostic";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (mostCommonBinStr, leastCommonBinStr) = GetMostCommonAndLeastCommonBinStrings(input.ReadLines().ToArray());

        var gammaRate = BinStringToLong(mostCommonBinStr.Value);
        var epsilonRate = BinStringToLong(leastCommonBinStr.Value);

        return gammaRate * epsilonRate;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var inputs = input.ReadLines().ToArray();

        var oxygenGeneratorRating = FilterToSingleValue(inputs, true);
        var co2ScrubberRating = FilterToSingleValue(inputs, false);

        return oxygenGeneratorRating * co2ScrubberRating;
    }

    private static long FilterToSingleValue(string[] inputs, bool mostCommon)
    {
        var width = inputs.First().Length;

        for (var index = 0; index < width; index++)
        {
            var (mostCommonBinStr, leastCommonBinStr) = GetMostCommonAndLeastCommonBinStrings(inputs, index);
            var matchChar = mostCommon ? mostCommonBinStr.Value[index] : leastCommonBinStr.Value[index];
            var candidates = inputs.Where(candidate => candidate[index] == matchChar).ToArray();

            if (candidates.Length == 1)
                return BinStringToLong(candidates.Single());

            inputs = candidates;
        }

        throw new InvalidOperationException($"Failed to find single value for {(mostCommon ? "mostCommon" : "leastCommon")}");
    }

    private static (Lazy<string> mostCommonBinStr, Lazy<string> leastCommonBinStr) GetMostCommonAndLeastCommonBinStrings(string[] inputs, int? index = null)
    {
        var width = inputs.First().Length;
        var countOfOnes = new int[width];
        var indexUntil = index != null ? index.Value + 1 : width;

        foreach (var line in inputs)
            for (var i = index ?? 0; i < indexUntil; i++)
                if (line[i] == '1')
                    countOfOnes[i]++;

        var median = inputs.Length / 2m;

        Lazy<string> mostCommonBinStr = new(() => string.Join("", countOfOnes.Select(count => count >= median ? "1" : "0")));
        Lazy<string> leastCommonBinStr = new(() => string.Join("", countOfOnes.Select(count => count < median ? "1" : "0")));

        return (mostCommonBinStr, leastCommonBinStr);
    }

    private static long BinStringToLong(string binStr) => Convert.ToInt64(binStr, 2);
}
