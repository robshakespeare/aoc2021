global using FluentAssertions;
global using FluentAssertions.Equivalency;
global using NUnit.Framework;
global using System.Numerics;

namespace AoC.Tests;

[SetUpFixture]
public static class OneTimeSetUpFixture
{
    /// <summary>
    /// Ran only once, before all tests, but after the test discovery phase.
    /// </summary>
    [OneTimeSetUp]
    public static void RunOnceBeforeAllTests()
    {
        Crayon.Output.Disable();
    }
}
