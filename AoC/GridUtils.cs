using System.Numerics;

namespace AoC;

public static class GridUtils
{
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
        char[][] newPixels = Enumerable.Range(0, Convert.ToInt32(newHeight)).Select(_ => new char[newWidth]).ToArray();

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
        var itemMap = items.GroupBy(positionSelector).ToDictionary(grp => positionSelector(grp.Last()), grp => grp.Last());

        var minBounds = new Vector2(items.Min(p => positionSelector(p).X), items.Min(p => positionSelector(p).Y));
        var maxBounds = new Vector2(items.Max(p => positionSelector(p).X), items.Max(p => positionSelector(p).Y));

        var grid = new List<IReadOnlyList<TOut>>();

        for (var y = minBounds.Y; y <= maxBounds.Y; y++)
        {
            var line = new List<TOut>();
            for (var x = minBounds.X; x <= maxBounds.X; x++)
            {
                line.Add(itemMap.TryGetValue(new Vector2(x, y), out var item)
                    ? resultItemSelector(item)
                    : resultItemFactory(new Vector2(x, y)));
            }
            grid.Add(line);
        }

        return grid;
    }
}
