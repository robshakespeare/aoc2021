using static Crayon.Output;

namespace AoC;

public interface ISolver
{
    int DayNumber { get; }

    string DayName { get; }

    void Run();

    Result? Part1Result { get; }

    Result? Part2Result { get; }
}

public abstract class SolverBase : SolverBase<long?, long?>
{
}

public abstract class SolverBase<TOutputPart1, TOutputPart2> : ISolver
{
    private readonly InputLoader _inputLoader;
    private readonly Result?[] _results = new Result?[2];

    public int DayNumber { get; }

    public abstract string DayName { get; }

    public Result? Part1Result => _results[0];

    public Result? Part2Result => _results[1];

    protected SolverBase()
    {
        DayNumber = SolverFactory.GetDayNumber(this);
        _inputLoader = new InputLoader(this);
    }

    public void Run()
    {
        Console.WriteLine(Yellow($"Day {DayNumber}{(DayName is null or "" ? "" : ": " + DayName)}{Environment.NewLine}"));

        SolvePart1();
        SolvePart2();
    }

    private TOutput? SolvePartTimed<TOutput>(int partNum, PuzzleInput input, Func<PuzzleInput, TOutput?> solve)
    {
        using var timer = new TimingBlock($"Part {partNum}");
        var result = solve(input);
        var elapsed = timer.Stop();
        Console.WriteLine($"Part {partNum}: {Green(result?.ToString())}");
        if (result == null)
        {
            Console.WriteLine(Bright.Magenta($"Part {partNum} returned null / is not yet implemented"));
        }
        _results[partNum - 1] = new Result(result, elapsed);
        return result;
    }

    public TOutputPart1? SolvePart1() => SolvePartTimed(1, _inputLoader.PuzzleInputPart1, SolvePart1);

    public TOutputPart2? SolvePart2() => SolvePartTimed(2, _inputLoader.PuzzleInputPart2, SolvePart2);

    public abstract TOutputPart1? SolvePart1(PuzzleInput input);

    public abstract TOutputPart2? SolvePart2(PuzzleInput input);
}

public record Result(object? Value, TimeSpan Elapsed);
