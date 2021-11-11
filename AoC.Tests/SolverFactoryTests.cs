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
}
