namespace AoC.Day23;

/// <summary>
/// Almost paper based solver for only my puzzle of Day 22, Part 1.
/// </summary>
public class Day23MyPuzzlePart1Solver
{
    private readonly StringBuilder[] _grid;

    public long TotalCost { get; private set; }

    public Day23MyPuzzlePart1Solver(PuzzleInput input)
    {
        _grid = input.ToString().Split(Environment.NewLine)
            .Select(line => new StringBuilder(line))
            .ToArray();

        Display();
    }

    /// <summary>
    /// This was too easy to solve almost manually, but it turned out worthwhile doing this
    /// because it was a good basis and understanding of how to solve it properly!
    /// </summary>
    public long SolvePart1()
    {
        Move('A', V(5, 2), V(1, 1), false);
        Move('A', V(5, 3), V(2, 1), false);

        Move('B', V(7, 2), V(7, 1), false);
        Move('B', V(7, 1), V(5, 3));

        Move('D', V(9, 2), V(10, 1), false);

        Move('B', V(9, 3), V(9, 1));
        Move('B', V(9, 1), V(5, 2));

        Move('D', V(10, 1), V(9, 3));

        Move('D', V(7, 3), V(7, 1));
        Move('D', V(7, 1), V(9, 2));

        Move('C', V(3, 2), V(3, 1));
        Move('C', V(3, 1), V(7, 3));

        Move('C', V(3, 3), V(3, 1));
        Move('C', V(3, 1), V(7, 2));

        Move('A', V(2, 1), V(3, 3));
        Move('A', V(1, 1), V(3, 2));

        return TotalCost;
    }

    private void Display()
    {
        foreach (var line in _grid)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine($"TotalCost: {TotalCost}");
        Console.WriteLine();
    }

    private readonly record struct Vec2(int X, int Y)
    {
        public int this[Axis axis] => axis switch
        {
            Axis.X => X,
            Axis.Y => Y,
            _ => throw new InvalidOperationException("Invalid axis: " + axis)
        };
    }

    private static Vec2 V(int x, int y) => new(x, y);

    private char GetChar(Vec2 pos) => _grid[pos.Y][pos.X];

    private void SetChar(Vec2 pos, char chr) => _grid[pos.Y][pos.X] = chr;

    private static long GetCostPerMove(char amphipod) => amphipod switch
    {
        'A' => 1,
        'B' => 10,
        'C' => 100,
        'D' => 1000,
        _ => throw new InvalidOperationException($"Unexpected amphipod {amphipod}")
    };

    private void Move(char expected, Vec2 start, Vec2 end, bool xFirst = true)
    {
        var mid = xFirst ? new Vec2(end.X, start.Y) : new Vec2(start.X, end.Y);
        MoveA(expected, start, mid);
        MoveA(expected, mid, end);
    }

    private void MoveA(char expected, Vec2 start, Vec2 end)
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

    private enum Axis
    {
        X,
        Y
    };

    private void ValidateMove(char amphipod, Vec2 start, Vec2 end, Axis axis)
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
            {
                throw new InvalidOperationException($"Must move {amphipod} to empty space, expected . got {GetChar(newPos)} @ {newPos}");
            }
        }
    }
}
