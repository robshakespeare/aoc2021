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
                (position, command) =>
                {
                    var (instruction, x) = command;
                    return position + instruction switch
                    {
                        "forward" => new Vector2(x, 0),
                        "down" => new Vector2(0, x),
                        "up" => new Vector2(0, -x),
                        _ => throw new InvalidOperationException("Invalid instruction: " + instruction)
                    };
                });

        return (position.X * position.Y).Round();
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var submarine = input.ReadLines()
            .Select(ParseLine)
            .Aggregate(
                new Submarine(),
                (submarine, command) =>
                {
                    var (horizontal, depth, aim) = submarine;
                    var (instruction, x) = command;
                    return instruction switch
                    {
                        "down" => submarine with {Aim = aim + x},
                        "up" => submarine with {Aim = aim - x},
                        "forward" => submarine with
                        {
                            Horizontal = horizontal + x,
                            Depth = depth + aim * x
                        },
                        _ => throw new InvalidOperationException("Invalid instruction: " + instruction)
                    };
                });

        return submarine.Horizontal * submarine.Depth;
    }

    private readonly record struct Submarine(long Horizontal, long Depth, long Aim);

    private static (string instruction, long x) ParseLine(string line)
    {
        var parts = line.Split(' ');
        return (parts[0], long.Parse(parts[1]));
    }
}
