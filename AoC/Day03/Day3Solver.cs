namespace AoC.Day03;

public class Day3Solver : SolverBase
{
    public override string DayName => "Binary Diagnostic";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (mostCommonBinStr, leastCommonBinStr) = GetMostCommonAndLeastCommonBinStrings(input.ReadLines().ToArray());

        var gammaRate = BinStringToLong(mostCommonBinStr);
        var epsilonRate = BinStringToLong(leastCommonBinStr);

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
            var (mostCommonBinStr, leastCommonBinStr) = GetMostCommonAndLeastCommonBinStrings(inputs);
            var matchChar = mostCommon ? mostCommonBinStr[index] : leastCommonBinStr[index];
            var candidates = inputs.Where(candidate => candidate[index] == matchChar).ToArray();

            if (candidates.Length == 1)
                return BinStringToLong(candidates.Single());

            inputs = candidates.ToArray();
        }

        throw new InvalidOperationException($"Failed to find single value for {(mostCommon ? "mostCommon" : "leastCommon")}");
    }

    private static (string mostCommonBinStr, string leastCommonBinStr) GetMostCommonAndLeastCommonBinStrings(string[] inputs)
    {
        var width = inputs.First().Length;
        var countOfOnes = new int[width];

        foreach (var line in inputs)
            for (var i = 0; i < width; i++)
                if (line[i] == '1')
                    countOfOnes[i]++;

        var midPoint = inputs.Length / 2m;

        var mostCommonBinStr = string.Join("", countOfOnes.Select(count => count >= midPoint ? "1" : "0"));
        var leastCommonBinStr = string.Join("", countOfOnes.Select(count => count < midPoint ? "1" : "0"));

        return (mostCommonBinStr, leastCommonBinStr);
    }

    private static long BinStringToLong(string binStr) => Convert.ToInt64(binStr, 2);
}
