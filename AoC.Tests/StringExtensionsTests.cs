namespace AoC.Tests;

public class StringExtensionsTests
{
    // Carriage Return = \r
    // Line Feed = \n

    [Test]
    public void NormalizeLineEndings_DoesNormalize_LineFeed()
    {
        // ACT
        var result = "test\nvalue\nhere".NormalizeLineEndings();

        // ASSERT
        result.Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "value",
            "here");
    }

    [Test]
    public void NormalizeLineEndings_DoesNormalize_CarriageReturn()
    {
        // ACT
        var result = "test\rvalue\rhere".NormalizeLineEndings();

        // ASSERT
        result.Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "value",
            "here");
    }

    [Test]
    public void NormalizeLineEndings_DoesNormalize_CarriageReturnLineFeed()
    {
        // ACT
        var result = "test\r\nvalue\r\nhere".NormalizeLineEndings();

        // ASSERT
        result.Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "value",
            "here");
    }

    [Test]
    public void NormalizeLineEndings_DoesNormalize_Mixture()
    {
        // ACT
        var result = "test\r\n\nvalue\r\r\n\nhere".NormalizeLineEndings();

        // ASSERT
        result.Split(Environment.NewLine).Should().BeEquivalentTo(
            "test",
            "",
            "value",
            "",
            "",
            "here");
    }

    [Test]
    public void NormalizeLineEndings_DoesNormalize_MultipleBlankLines()
    {
        // ACT
        var result = "\r\n\r\n\r\n\n\n\n\r\r\r".NormalizeLineEndings();

        // ASSERT
        result.Should().BeEquivalentTo(string.Join("", Enumerable.Repeat(Environment.NewLine, 9)));
    }

    [Test]
    public void NormalizeLineEndings_WhenInputNull_ReturnsEmptyString()
    {
        // ACT
        var result = ((string?)null).NormalizeLineEndings();

        // ASSERT
        result.Should().BeEmpty();
    }

    [Test]
    public void ReadLines_DoesParseEachLineOfStringIntoArrayElements_And_DoesNormalizeLineEndings()
    {
        // ACT
        var result = "hello\nworld\r\n\r\nthis\ris\r\na\ntest".ReadLines();

        // ASSERT
        result.Should().BeEquivalentTo(
            "hello",
            "world",
            "",
            "this",
            "is",
            "a",
            "test");
    }

    [Test]
    public void ReadLines_DoesTrimTrailingLineEndings()
    {
        // ACT
        var result = "hello\r\n\r\n\n\n\r\nworld\r\n\n\n\r\n".ReadLines();

        // ASSERT
        result.Should().BeEquivalentTo(
            "hello",
            "",
            "",
            "",
            "",
            "world");
    }

    [Test]
    public void ReadLines_DoesTrimTrailingLineEndings_AndResultInAnEmptyCollectionIfInputIsJustNewLines()
    {
        // ACT
        var result = "\r\n\r\n\r\n\n\n\r\n".ReadLines();

        // ASSERT
        result.Should().BeEmpty();
    }

    [Test]
    public void ReadLines_WhenInputNull_ReturnsEmptyCollection()
    {
        // ACT
        var result = ((string?)null).ReadLines().ToArray();

        // ASSERT
        result.Should().BeEmpty();
    }
}
