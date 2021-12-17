using System.Diagnostics;

namespace AoC.Day17;

public class Day17Solver : SolverBase
{
    public override string DayName => "Trick Shot";

    /// <summary>
    /// Coordinate space is different to normal. X axis positive still goes to the right, but Y axis positive no goes up, and Y axis negative goes down.
    /// 
    /// The probe's x,y position starts at 0,0
    /// Then, it will follow some trajectory by moving in steps. On each step, these changes occur in the following order:
    ///  - The probe's x position increases by its x velocity.
    ///  - The probe's y position increases by its y velocity.
    ///  - Due to drag, the probe's x velocity changes by 1 toward the value 0; that is, it decreases by 1 if it is greater than 0, increases by 1 if it is less than 0, or does not change if it is already 0.
    ///  - Due to gravity, the probe's y velocity decreases by 1.
    ///
    /// For the probe to successfully make it into the trench, the probe must be on some trajectory that causes it to be within a target area after any step. 
    /// </summary>
    public override long? SolvePart1(PuzzleInput input)
    {
        // Doesn't make sense to start with downward velocity, since we're trying to go as high as possible
        // Initial velocity of X is the number of steps it will take before there's no change in X

        // Just to see what happens, lets start with velocity of 6,3 for example, and see what happens
        // If we are ever inside our target, we have success!
        // If we ever go beyond, then this trajectory is not correct

        //var initialVelocity = new Vector2(7, 2);

        // Just brute force it for now!

        var target = InputToTargetBounds(input);

        return TryVelocities(target/*, 1, 100, 1, 100*/).Max(result => result.MaxHeight);

        ////Console.WriteLine($"Target bounds = {target.TopLeft} .. {target.BottomRight}");

        //var initialVelocities = Enumerable.Range(1, 100).SelectMany(y => Enumerable.Range(1, 100).Select(x => new Vector2(x, y))).ToArray();

        //var maxHeight = initialVelocities
        //    .Select(initialVelocity => new
        //    {
        //        initialVelocity,
        //        result = TryVelocity(target, initialVelocity)
        //    })
        //    .Where(x => x.result.success)
        //    .Max(x => x.result.maxHeight);

        //return maxHeight;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var target = InputToTargetBounds(input);

        var minX = 1;
        var maxX = (int) target.BottomRight.X;

        var minY = (int) target.BottomRight.Y;
        var maxY = ((int) target.BottomRight.Y * -1) - 1;

        return TryVelocities(target/*, minX, maxX, minY, maxY*/).Count();

        //var initialVelocities = Enumerable.Range(-1000, 2000).SelectMany(y => Enumerable.Range(-1000, 2000).Select(x => new Vector2(x, y))).ToArray();

        //var results = initialVelocities
        //    .Select(initialVelocity => new
        //    {
        //        initialVelocity,
        //        result = TryVelocity(target, initialVelocity)
        //    })
        //    .Where(x => x.result.success)
        //    .ToArray();

        //Console.WriteLine("Start");
        //Console.WriteLine($"Target bounds = {target.TopLeft} .. {target.BottomRight}");
        //Console.WriteLine();
        //Console.WriteLine("MinX: " + results.Min(x => x.initialVelocity.X));
        //Console.WriteLine("MinY: " + results.Min(x => x.initialVelocity.Y));
        //Console.WriteLine();
        //Console.WriteLine("MaxX: " + results.Max(x => x.initialVelocity.X));
        //Console.WriteLine("MaxY: " + results.Max(x => x.initialVelocity.Y));
        //Console.WriteLine();
        //foreach (var result in results)
        //{
        //    Console.WriteLine(result.initialVelocity);
        //}

        //return results.Length;
    }

    private static IEnumerable<long> Range(long start, long end)
    {
        for (var i = start; i <= end; i++)
        {
            yield return i;
        }
    }

    public static IEnumerable<Result> TryVelocities(Bounds target) //, int minX, int maxX, int minY, int maxY)
    {
        var minX = 1;
        var maxX = (int)target.BottomRight.X;

        var minY = (int)target.BottomRight.Y;
        var maxY = -(int)target.BottomRight.Y - 1;

        var initialVelocities = Range(minY, maxY).SelectMany(y => Enumerable.Range(minX, maxX).Select(x => new Vector2(x, y)));

        return initialVelocities
            .Select(initialVelocity => TryVelocity(target, initialVelocity))
            .Where(x => x.Success);
    }

    public record Result(Vector2 InitialVelocity, bool Success, int MaxHeight);

    public static Result TryVelocity(Bounds target, Vector2 initialVelocity)
    {
        var position = new Vector2(0, 0);
        var velocity = initialVelocity;
        var maxHeight = 0;

        while (!target.HasPositionPassedRightOrBottom(position))
        {
            position += velocity;
            ////Console.WriteLine(position.X);
            maxHeight = Math.Max(maxHeight, (int) position.Y);
            velocity += new Vector2(velocity.X == 0 ? 0 : (0 - velocity.X) / Math.Abs(velocity.X), -1);

            if (target.Contains(position))
            {
                return new Result(initialVelocity, true, maxHeight);
            }
        }

        return new Result(initialVelocity, false, maxHeight);
    }

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
