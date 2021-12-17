namespace AoC.Day17;

public class Day17Solver : SolverBase
{
    public override string DayName => "Trick Shot";

    /// <summary>
    ///     Coordinate space is different to normal. X axis positive still goes to the right, but now Y axis positive goes up (i.e +ve X RIGHT, +ve Y UP).
    ///
    ///     The probe's x,y position starts at 0,0
    ///     Then, it will follow some trajectory by moving in steps. On each step, these changes occur in the following order:
    ///      - The probe's x position increases by its x velocity.
    ///      - The probe's y position increases by its y velocity.
    ///      - Due to drag, the probe's x velocity changes by 1 toward the value 0; that is, it decreases by 1 if it is greater than 0, increases by 1 if it is less than 0, or does not change if it is already 0.
    ///      - Due to gravity, the probe's y velocity decreases by 1.
    ///
    ///     For the probe to successfully make it into the trench, the probe must be on some trajectory that causes it to be within a target area after any step. 
    /// </summary>
    /// <remarks>
    ///     Doesn't make sense to start with downward velocity, since we're trying to go as high as possible.
    ///     Initial velocity of X is the number of steps it will take before there's no change in X.
    /// </remarks>
    public override long? SolvePart1(PuzzleInput input)
    {
        var target = InputToTargetBounds(input);
        return GetSuccessfulVelocities(target).Max(result => result.MaxHeight);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var target = InputToTargetBounds(input);
        return GetSuccessfulVelocities(target).Count();
    }

    public static IEnumerable<Result> GetSuccessfulVelocities(Bounds target)
    {
        static IEnumerable<int> Range(int start, int end) => Enumerable.Range(start, end - start + 1);

        const int minX = 1;
        var maxX = (int) target.BottomRight.X;

        var minY = (int) target.BottomRight.Y;
        var maxY = (int) -target.BottomRight.Y - 1;

        var initialVelocities = Range(minY, maxY).SelectMany(y => Range(minX, maxX).Select(x => new Vector2(x, y)));

        return initialVelocities
            .Select(initialVelocity => TryVelocity(target, initialVelocity))
            .Where(x => x.Success);
    }

    public static Result TryVelocity(Bounds target, Vector2 initialVelocity)
    {
        var position = new Vector2(0, 0);
        var velocity = initialVelocity;
        var maxHeight = 0;

        while (!target.HasPositionPassedRightOrBottom(position))
        {
            position += velocity;
            maxHeight = Math.Max(maxHeight, (int) position.Y);
            velocity += new Vector2(velocity.X == 0 ? 0 : (0 - velocity.X) / Math.Abs(velocity.X), -1);

            if (target.Contains(position))
            {
                return new Result(initialVelocity, true, maxHeight);
            }
        }

        return new Result(initialVelocity, false, maxHeight);
    }

    public record Result(Vector2 InitialVelocity, bool Success, int MaxHeight);

    public readonly record struct Bounds(Vector2 TopLeft, Vector2 BottomRight)
    {
        public bool Contains(Vector2 position) =>
            position.X >= TopLeft.X && position.Y <= TopLeft.Y &&
            position.X <= BottomRight.X && position.Y >= BottomRight.Y;

        public bool HasPositionPassedRightOrBottom(Vector2 position) =>
            position.X > BottomRight.X || position.Y < BottomRight.Y;
    }

    private static readonly Regex ParseInputRegex = new(@"x=(?<x1>\d+)..(?<x2>\d+), y=(?<y1>-?\d+)..(?<y2>-?\d+)", RegexOptions.Compiled);

    public static Bounds InputToTargetBounds(PuzzleInput input)
    {
        var match = ParseInputRegex.Match(input.ToString());
        if (!match.Success)
        {
            throw new InvalidOperationException("Puzzle input not valid");
        }

        var x1 = int.Parse(match.Groups["x1"].Value);
        var x2 = int.Parse(match.Groups["x2"].Value);
        var y1 = int.Parse(match.Groups["y1"].Value);
        var y2 = int.Parse(match.Groups["y2"].Value);

        return new Bounds(new Vector2(Math.Min(x1, x2), Math.Max(y1, y2)), new Vector2(Math.Max(x1, x2), Math.Min(y1, y2)));
    }
}
