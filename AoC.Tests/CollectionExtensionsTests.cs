namespace AoC.Tests;

public class CollectionExtensionsTests
{
    [Test]
    public void ToEnumerable_Test()
    {
        (5..10).ToEnumerable().Should().BeEquivalentTo(
            new[] {5, 6, 7, 8, 9},
            opts => opts.WithStrictOrdering());
    }

    [Test]
    public void ToArray_Test()
    {
        (0..4).ToArray().Should().BeEquivalentTo(
            new[] {0, 1, 2, 3},
            opts => opts.WithStrictOrdering());
    }
}
