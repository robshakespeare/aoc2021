global using FluentAssertions;
global using NUnit.Framework;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC.Tests
{
    [SetUpFixture]
    public static class OneTimeSetUpFixture
    {
        private static AnsiEscapeCodeCleanerTextWriter? _textWriter;

        /// <summary>
        /// Ran only once, before all tests, but after the test discovery phase.
        /// </summary>
        [OneTimeSetUp]
        public static void RunOnceBeforeAllTests()
        {
            _textWriter = new AnsiEscapeCodeCleanerTextWriter();
            Console.SetOut(_textWriter);
        }

        /// <summary>
        /// Ran only once, after all tests.
        /// </summary>
        [OneTimeTearDown]
        public static void RunOnceAfterAllTests()
        {
            _textWriter?.Dispose();
        }

        private sealed class AnsiEscapeCodeCleanerTextWriter : TextWriter
        {
            private static readonly Regex AnsiEscapeCodeRegex = new("\x001B.+?m", RegexOptions.Compiled | RegexOptions.CultureInvariant);

            public AnsiEscapeCodeCleanerTextWriter() => NewLine = "\n";

            public override Encoding Encoding { get; } = Encoding.Default;

            public override void Write(string? value)
            {
                if (value != null)
                {
                    TestContext.Progress.Write(AnsiEscapeCodeRegex.Replace(value, ""));
                }
            }

            public override void Write(char value) => TestContext.Progress.Write(value);
        }
    }
}
