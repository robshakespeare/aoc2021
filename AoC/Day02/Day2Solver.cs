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
                    var (instruction, amount) = command;
                    return position + instruction switch
                    {
                        "forward" => new Vector2(amount, 0),
                        "down" => new Vector2(0, amount),
                        "up" => new Vector2(0, -amount),
                        _ => throw new InvalidOperationException("Invalid instruction: " + instruction)
                    };
                });

        return position.X.Round() * position.Y.Round();
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var submarine = input.ReadLines()
            .Select(ParseLine)
            .Aggregate(
                new Submarine(),
                (submarine, command) =>
                {
                    var (position, aim) = submarine;
                    var (instruction, amount) = command;
                    return instruction switch
                    {
                        "down" => submarine with {Aim = aim + amount},
                        "up" => submarine with {Aim = aim - amount},
                        "forward" => submarine with {Position = position + new Vector2(amount, aim * amount)},
                        _ => throw new InvalidOperationException("Invalid instruction: " + instruction)
                    };
                });

        return submarine.Position.X.Round() * submarine.Position.Y.Round();
    }

    private readonly record struct Submarine(Vector2 Position, long Aim);

    private static (string instruction, long x) ParseLine(string line)
    {
        var parts = line.Split(' ');
        return (parts[0], long.Parse(parts[1]));
    }
}
