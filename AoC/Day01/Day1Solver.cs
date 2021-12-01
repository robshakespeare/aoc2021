namespace AoC.Day01;

public class Day1Solver : SolverBase
{
    public override string DayName => "Sonar Sweep";

    public override long? SolvePart1(PuzzleInput input)
    {
        long? previousDepth = null;
        long increments = 0;

        foreach (var depth in input.ReadLinesAsLongs())
        {
            if (depth > previousDepth)
            {
                increments++;
            }

            previousDepth = depth;
        }

        return increments;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        long? previousSum = null;
        var measurements = input.ReadLinesAsLongs().ToArray();
        long increments = 0;

        for (var i = 2; i < measurements.Length; i++)
        {
            var sum = measurements[i - 2] + measurements[i - 1] + measurements[i];

            if (sum > previousSum)
            {
                increments++;
            }

            previousSum = sum;
        }

        return increments;
    }
}
