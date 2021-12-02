namespace AoC.Day01;

/// <summary>
/// Inspired by looking at other people's solutions, my original solution was <see cref="Day1Solver"/>.
/// </summary>
public class Day1SolverV2 : SolverBase
{
    public override string DayName => "Sonar Sweep";

    public override long? SolvePart1(PuzzleInput input) => CountIncrements(input.ReadLinesAsLongs().ToArray());

    public override long? SolvePart2(PuzzleInput input) => CountIncrements(CleanMeasurements(input.ReadLinesAsLongs().ToArray()));

    private static long CountIncrements(long[] values) => values.Select((v, i) => (v, i)).Skip(1).Count(x => x.v > values[x.i - 1]);

    private static long[] CleanMeasurements(long[] measurements) =>
        measurements.Select((v, i) => (v, i)).Skip(2).Select(x => measurements[x.i - 2] + measurements[x.i - 1] + measurements[x.i]).ToArray();
}
