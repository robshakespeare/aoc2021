namespace AoC.Tests;

public class SolverFactoryTests
{
    [Test]
    public void DoesHaveAllDaysRegistered()
    {
        foreach (var dayNumber in Enumerable.Range(1, 25))
        {
            var solver = SolverFactory.Instance.TryCreateSolver(dayNumber.ToString());

            solver.Should().NotBeNull($"Day {dayNumber} should have a solver");
            solver!.DayNumber.Should().Be(dayNumber);
        }
    }

    [Test]
    public void DoesHaveTestDayRegistered()
    {
        var solver = SolverFactory.Instance.TryCreateSolver("0");

        solver.Should().NotBeNull("Day 0 (Test Day) should have a solver");
        solver!.DayNumber.Should().Be(0);
    }

    [TestCaseSource(nameof(DefaultDayTestCases))]
    public int GetDefaultDay_Tests(DateTime inputDate) => SolverFactory.GetDefaultDay(inputDate);

    public static IEnumerable<TestCaseData> DefaultDayTestCases
    {
        get
        {
            // Before Dec (but after Feb), display the 1st
            yield return new TestCaseData(new DateTime(2021, 11, 29)).Returns(1);
            yield return new TestCaseData(new DateTime(2021, 11, 30)).Returns(1);

            // For Dec, return the current day number, or 25 if the day is past the 25th
            yield return new TestCaseData(new DateTime(2021, 12, 1)).Returns(1);
            yield return new TestCaseData(new DateTime(2021, 12, 2)).Returns(2);
            yield return new TestCaseData(new DateTime(2021, 12, 5)).Returns(5);
            yield return new TestCaseData(new DateTime(2021, 12, 10)).Returns(10);
            yield return new TestCaseData(new DateTime(2021, 12, 24)).Returns(24);
            yield return new TestCaseData(new DateTime(2021, 12, 25)).Returns(25);
            yield return new TestCaseData(new DateTime(2021, 12, 31)).Returns(25);

            // For Jan & Feb, always display the 25th
            yield return new TestCaseData(new DateTime(2022, 1, 1)).Returns(25);
            yield return new TestCaseData(new DateTime(2022, 1, 10)).Returns(25);
            yield return new TestCaseData(new DateTime(2022, 1, 31)).Returns(25);
            yield return new TestCaseData(new DateTime(2022, 2, 1)).Returns(25);
            yield return new TestCaseData(new DateTime(2022, 2, 5)).Returns(25);
            yield return new TestCaseData(new DateTime(2022, 2, 10)).Returns(25);
            yield return new TestCaseData(new DateTime(2022, 2, 27)).Returns(25);
            yield return new TestCaseData(new DateTime(2022, 2, 28)).Returns(25);

            // For months after Feb, display the 1st
            yield return new TestCaseData(new DateTime(2022, 3, 1)).Returns(1);
            yield return new TestCaseData(new DateTime(2022, 3, 5)).Returns(1);
            yield return new TestCaseData(new DateTime(2022, 3, 10)).Returns(1);
            yield return new TestCaseData(new DateTime(2022, 3, 27)).Returns(1);
            yield return new TestCaseData(new DateTime(2022, 6, 1)).Returns(1);
            yield return new TestCaseData(new DateTime(2022, 6, 5)).Returns(1);
            yield return new TestCaseData(new DateTime(2022, 6, 10)).Returns(1);
            yield return new TestCaseData(new DateTime(2022, 6, 27)).Returns(1);
        }
    }
}
