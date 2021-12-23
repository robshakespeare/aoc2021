using System.Text;

namespace AoC.Day23;

public abstract class Day23Base
{
    private readonly StringBuilder[] _grid;

    public long TotalCost { get; private set; }

    public string GridToString() => string.Join(Environment.NewLine, _grid.Select(x => x.ToString()));

    protected Day23Base(PuzzleInput input)
    {
        _grid = input.ToString().Split(Environment.NewLine)
            .Select(line => new StringBuilder(line))
            .ToArray();

        Display();
    }

    public void Display()
    {
        foreach (var line in _grid)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine($"TotalCost: {TotalCost}");
        Console.WriteLine();
    }

    public readonly record struct Vec2(int X, int Y)
    {
        public int this[Axis axis] => axis switch
        {
            Axis.X => X,
            Axis.Y => Y,
            _ => throw new InvalidOperationException("Invalid axis: " + axis)
        };
    }

    public static Vec2 V(int x, int y) => new(x, y);

    public static long GetCostPerMove(char amphipod) => amphipod switch
    {
        'A' => 1,
        'B' => 10,
        'C' => 100,
        'D' => 1000,
        _ => throw new InvalidOperationException($"Unexpected amphipod {amphipod}")
    };

    public char GetChar(Vec2 pos) => _grid[pos.Y][pos.X];

    protected void SetChar(Vec2 pos, char chr) => _grid[pos.Y][pos.X] = chr;

    protected void Move(char expected, Vec2 start, Vec2 end, bool xFirst = true)
    {
        var mid = xFirst ? new Vec2(end.X, start.Y) : new Vec2(start.X, end.Y);
        MoveA(expected, start, mid);
        MoveA(expected, mid, end);
    }

    protected void MoveA(char expected, Vec2 start, Vec2 end)
    {
        var amphipod = GetChar(start);

        // Validate
        if (amphipod != expected)
            throw new InvalidOperationException($"Expected {expected} got {amphipod}");

        ValidateMove(amphipod, start, end, Axis.X);
        ValidateMove(amphipod, start, end, Axis.Y);

        // Update chars in grid
        SetChar(start, '.');
        SetChar(end, amphipod);

        // Calculate cost
        var deltaX = Math.Abs(start.X - end.X);
        var deltaY = Math.Abs(start.Y - end.Y);

        var costPerSpace = GetCostPerMove(amphipod);
        var cost = deltaX * costPerSpace + deltaY * costPerSpace;
        TotalCost += cost;

        Display();
    }

    public enum Axis
    {
        X,
        Y
    };

    public void ValidateMove(char amphipod, Vec2 start, Vec2 end, Axis axis)
    {
        var otherAxis = axis switch
        {
            Axis.X => Axis.Y,
            Axis.Y => Axis.X,
            _ => throw new InvalidOperationException("Invalid axis: " + axis)
        };

        var other = start[otherAxis];
        var startC = start[axis];
        var endC = end[axis];
        var delta = endC - startC;
        if (delta == 0)
        {
            return;
        }

        var dir = delta / Math.Abs(delta);
        var posC = startC;
        while (posC != endC)
        {
            posC += dir;
            var newPos = new Vec2(axis == Axis.X ? posC : other, axis == Axis.Y ? posC : other);
            if (GetChar(newPos) != '.')
                throw new InvalidOperationException($"Must move {amphipod} to empty space, expected . got {GetChar(newPos)} @ {newPos}");
        }
    }
}
