using System.Numerics;

namespace AoC;

public static class MathUtils
{
    /// <summary>
    /// Finds the greatest common divisor (GCD) of the two specified numbers.
    /// </summary>
    public static long GreatestCommonDivisor(long a, long b) => (long)BigInteger.GreatestCommonDivisor(a, b);

    /// <summary>
    /// Returns the least common multiple (LCM) of the two specified numbers.
    /// </summary>
    public static long LeastCommonMultiple(long a, long b) => a * b / GreatestCommonDivisor(a, b);

    /// <summary>
    /// Returns the least common multiple (LCM) of the three specified numbers.
    /// </summary>
    public static long LeastCommonMultiple(long a, long b, long c) => LeastCommonMultiple(LeastCommonMultiple(a, b), c);

    /// <summary>
    /// Calculates the angle, in degrees, between the two specified vectors.
    /// </summary>
    public static double AngleBetween(Vector2 vector1, Vector2 vector2)
    {
        var sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
        var cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

        var angleBetween = Math.Atan2(sin, cos) * (180 / Math.PI);
        return angleBetween < 0 ? 360 + angleBetween : angleBetween;
    }

    public static readonly Vector2 North = new(0, -1);
    public static readonly Vector2 East = new(1, 0);
    public static readonly Vector2 South = new(0, 1);
    public static readonly Vector2 West = new(-1, 0);

    private const double DegreesToRadiansFactor = Math.PI / 180;

    /// <summary>
    /// Converts the specified degrees in to radians.
    /// </summary>
    public static float DegreesToRadians(this int degrees) => Convert.ToSingle(degrees * DegreesToRadiansFactor);

    /// <summary>
    /// Rotates the specified direction vector, around the zero vector (0,0) center point, by the specified number of degrees.
    /// </summary>
    /// <remarks>
    /// Note: This method expects the direction vector to be centered around the zero vector (0,0).
    /// It could probably be extended, using the Matrix3x2 CreateRotation(float radians, Vector2 centerPoint) method,
    /// to cater for rotation around a non-zero center point.
    /// </remarks>
    public static Vector2 RotateDirection(Vector2 direction, int degrees)
    {
        var radians = degrees.DegreesToRadians();
        var rotationMatrix = Matrix3x2.CreateRotation(radians);
        return Vector2.Transform(direction, rotationMatrix);
    }

    /// <summary>
    /// Rounds the floating-point value to the nearest integer value, and rounds midpoint values away from zero.
    /// </summary>
    public static int Round(this float f) => (int) MathF.Round(f, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Returns the Manhattan Distance between two cartesian coordinates.
    /// The cartesian coordinates are expected to be on a integer grid,
    /// and will be rounded to the nearest integer before calculating the the Manhattan Distance calculation.
    /// </summary>
    public static int ManhattanDistance(Vector2 a, Vector2 b) => Math.Abs(a.X.Round() - b.X.Round()) + Math.Abs(a.Y.Round() - b.Y.Round());

    /// <summary>
    /// Returns the Manhattan Distance between the specified cartesian coordinates and the zero vector (0,0).
    /// The cartesian coordinates are expected to be on a integer grid,
    /// and will be rounded to the nearest integer before calculating the the Manhattan Distance calculation.
    /// </summary>
    public static int ManhattanDistanceFromZero(this Vector2 vector) => ManhattanDistance(vector, Vector2.Zero);

    /// <summary>
    /// Rotates the specified grid around its middle point.
    /// Expects the length of each line (width of the grid) to be equal all the way down.
    /// </summary>
    public static IReadOnlyList<string> RotateGrid(IReadOnlyList<string> pixels, int degrees)
    {
        if (degrees % 90 != 0)
        {
            throw new InvalidOperationException($"Only right angle rotations are supported. Rotation of {degrees}° is invalid.");
        }

        return TransformGrid(pixels, Matrix3x2.CreateRotation(degrees.DegreesToRadians()));
    }

    /// <summary>
    /// Scales the specified grid around its middle point.
    /// Expects the length of each line (width of the grid) to be equal all the way down.
    /// </summary>
    public static IReadOnlyList<string> ScaleGrid(IReadOnlyList<string> pixels, Vector2 scales) =>
        TransformGrid(pixels, Matrix3x2.CreateScale(scales));

    private static IReadOnlyList<string> TransformGrid(IReadOnlyList<string> pixels, Matrix3x2 matrix)
    {
        var newGrid = new Dictionary<Vector2, char>();

        foreach (var (line, y) in pixels.Select((line, y) => (line, y)))
        {
            foreach (var (chr, x) in line.Select((chr, x) => (chr, x)))
            {
                var newPoint = Vector2.Transform(new Vector2(x, y), matrix);
                newGrid.Add(newPoint, chr);
            }
        }

        var min = new Vector2(float.MaxValue);
        var max = new Vector2(float.MinValue);

        foreach (var (p, _) in newGrid)
        {
            min = Vector2.Min(min, p);
            max = Vector2.Max(max, p);
        }

        var newWidth = (max.X - min.X).Round() + 1;
        var newHeight = (max.Y - min.Y).Round() + 1;
        char[][] newPixels = Enumerable.Range(0, newHeight).Select(_ => new char[newWidth]).ToArray();

        var offset = Vector2.Zero - min;
        foreach (var (p, c) in newGrid)
        {
            var lp = p + offset;
            newPixels[lp.Y.Round()][lp.X.Round()] = c;
        }

        return newPixels.Select(newLine => new string(newLine)).ToArray();
    }

    /// <summary>
    /// Builds and returns 2D grid of text from the specified list of items that have a position.
    /// </summary>
    public static IReadOnlyList<string> ToStringGrid<T>(
        this IReadOnlyCollection<T> items,
        Func<T, Vector2> positionSelector,
        Func<T, char> charSelector,
        char defaultChar)
    {
        return items.ToGrid(positionSelector, charSelector, _ => defaultChar)
            .Select(line => new string(line.ToArray()))
            .ToArray();
    }

    /// <summary>
    /// Builds and returns 2D grid of items from the specified list of items that have a position.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<TOut>> ToGrid<TIn, TOut>(
        this IReadOnlyCollection<TIn> items,
        Func<TIn, Vector2> positionSelector,
        Func<TIn, TOut> resultItemSelector,
        Func<Vector2, TOut> resultItemFactory)
    {
        var pixelPositionMap = items.GroupBy(positionSelector)
            .ToDictionary(g => positionSelector(g.Last()), g => g.Last());

        var minBounds = new Vector2(items.Min(p => positionSelector(p).X), items.Min(p => positionSelector(p).Y));
        var maxBounds = new Vector2(items.Max(p => positionSelector(p).X), items.Max(p => positionSelector(p).Y));

        var grid = new List<IReadOnlyList<TOut>>();

        for (var y = minBounds.Y; y <= maxBounds.Y; y++)
        {
            var line = new List<TOut>();
            for (var x = minBounds.X; x <= maxBounds.X; x++)
            {
                line.Add(pixelPositionMap.TryGetValue(new Vector2(x, y), out var pixel)
                    ? resultItemSelector(pixel)
                    : resultItemFactory(new Vector2(x, y)));
            }
            grid.Add(line);
        }

        return grid;
    }
}
