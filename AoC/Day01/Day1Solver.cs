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
        var measurements = input.ReadLinesAsLongs().ToArray();
        long increments = 0;

        for (var i = 3; i < measurements.Length; i++)
        {
            if (measurements[i] > measurements[i - 3]) // Great tip for this simplification/optimisation from Daniel Childs: https://github.com/webbiscuit
            {
                increments++;
            }
        }

        return increments;
    }
}
