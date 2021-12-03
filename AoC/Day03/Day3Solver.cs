namespace AoC.Day03;

public class Day3Solver : SolverBase
{
    public override string DayName => "";

    public override long? SolvePart1(PuzzleInput input)
    {
        var inputLines = input.ReadLines().ToArray();
        var width = inputLines.First().Length;
        var countOfOnes = new int[width];

        foreach (var line in inputLines)
        {
            for (var i = 0; i < width; i++)
            {
                if (line[i] == '1')
                {
                    countOfOnes[i]++;
                }
            }
        }

        var midPoint = inputLines.Length / 2;

        var gamma = Convert.ToInt64(string.Join("", countOfOnes.Select(count => count >= midPoint ? "1" : "0")), 2);
        var epsilon = Convert.ToInt64(string.Join("", countOfOnes.Select(count => count < midPoint ? "1" : "0")), 2);

        return gamma * epsilon;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }
}
