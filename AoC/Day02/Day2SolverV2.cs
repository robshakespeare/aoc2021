using System.Numerics;

namespace AoC.Day02;

public class Day2SolverV2 : SolverBase
{
    public override string DayName => "Dive!";

    public override long? SolvePart1(PuzzleInput input)
    {
        var position = new Vector2();

        foreach (var (instruction, amount) in input.ReadLines().Select(ParseLine))
            position += instruction switch
            {
                "forward" => new Vector2(amount, 0),
                "down" => new Vector2(0, amount),
                "up" => new Vector2(0, -amount),
                _ => throw new InvalidOperationException("Invalid instruction: " + instruction)
            };

        return position.X.Round() * position.Y.Round();
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        long horizontal = 0, depth = 0, aim = 0;

        foreach (var (instruction, amount) in input.ReadLines().Select(ParseLine))
            switch (instruction)
            {
                case "down":
                    aim += amount;
                    break;
                case "up":
                    aim -= amount;
                    break;
                case "forward":
                    horizontal += amount;
                    depth += aim * amount;
                    break;
            }

        return horizontal * depth;
    }

    private static (string instruction, long x) ParseLine(string line)
    {
        var parts = line.Split(' ');
        return (parts[0], long.Parse(parts[1]));
    }
}
