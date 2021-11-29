namespace AoC;

public static class CollectionExtensions
{
    /// <summary>
    /// Returns the indexes in specified Range, from the inclusive start index of the Range to the exclusive end index of the Range.
    /// </summary>
    public static int[] ToArray(this Range range) => ToEnumerable(range).ToArray();

    /// <summary>
    /// Enumerates the indexes in specified Range, from the inclusive start index of the Range to the exclusive end index of the Range.
    /// </summary>
    public static IEnumerable<int> ToEnumerable(this Range range)
    {
        for (var i = range.Start.Value; i < range.End.Value; i++)
        {
            yield return i;
        }
    }

    /// <summary>
    /// Converts the specified enumerable/collection to a read only array.
    /// </summary>
    public static IReadOnlyList<TSource> ToReadOnlyArray<TSource>(this IEnumerable<TSource> source) => Array.AsReadOnly(source.ToArray());
}
