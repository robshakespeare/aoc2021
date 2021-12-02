using System.Numerics;

namespace AoC.Day02;

public class Day2Solver : SolverBase
{
    public override string DayName => "Dive!";

    public override long? SolvePart1(PuzzleInput input)
    {
        var position = input.ReadLines()
            .Select(ParseLine)
            .Aggregate(
                new Vector2(),
                (position, instruction) =>
                {
                    var (direction, amount) = instruction;
                    return position + direction switch
                    {
                        "forward" => new Vector2(amount, 0),
                        "down" => new Vector2(0, amount),
                        "up" => new Vector2(0, -amount),
                        _ => throw new InvalidOperationException("Invalid direction: " + direction)
                    };
                });

        return (position.X * position.Y).Round();
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    private static (string direction, int amount) ParseLine(string line)
    {
        var parts = line.Split(' ');
        return (parts[0], int.Parse(parts[1]));
    }
}
