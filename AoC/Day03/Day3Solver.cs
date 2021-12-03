namespace AoC.Day03;

public class Day3Solver : SolverBase
{
    public override string DayName => "";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (mostCommonBinStr, leastCommonBinStr) = GetMostCommonAndLeastCommonBinStrings(input.ReadLines().ToArray());

        var gamma = Convert.ToInt64(mostCommonBinStr, 2);
        var epsilon = Convert.ToInt64(leastCommonBinStr, 2);

        return gamma * epsilon;
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

            //var candidates = new HashSet<string>(inputs);

            //// Filter out any that don't match
            //foreach (var candidate in candidates.Where(candidate => candidate[index] != matchChar))
            //{
            //    candidates.Remove(candidate);
            //}

            var candidates = inputs.Where(candidate => candidate[index] == matchChar).ToArray();

            if (candidates.Length == 1)
            {
                return Convert.ToInt64(candidates.Single(), 2);
            }

            inputs = candidates.ToArray();
        }

        throw new InvalidOperationException($"Failed to find single value for {(mostCommon ? "mostCommon" : "leastCommon")}");
    }

    private static (string mostCommonBinStr, string leastCommonBinStr) GetMostCommonAndLeastCommonBinStrings(string[] inputs)
    {
        var width = inputs.First().Length;
        var countOfOnes = new int[width];

        foreach (var line in inputs)
        {
            for (var i = 0; i < width; i++)
            {
                if (line[i] == '1')
                {
                    countOfOnes[i]++;
                }
            }
        }

        var midPoint = inputs.Length / 2m;

        var mostCommonBinStr = string.Join("", countOfOnes.Select(count => count >= midPoint ? "1" : "0"));
        var leastCommonBinStr = string.Join("", countOfOnes.Select(count => count < midPoint ? "1" : "0"));

        // rs-todo: rem this
        Console.WriteLine($"midPoint: {midPoint}");
        foreach (var count in countOfOnes)
        {
            var thisCountOfOnes = count;
            var thisCountOfZeros = inputs.Length - thisCountOfOnes;
            Console.WriteLine($"0: {thisCountOfZeros} -- 1: {thisCountOfOnes}");
        }

        return (mostCommonBinStr, leastCommonBinStr);
    }
}
