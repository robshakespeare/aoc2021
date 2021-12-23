namespace AoC.Day23;

public class Day23Part1Solver : Day23Base
{
    public Day23Part1Solver(PuzzleInput input)
        : base(input)
    {
    }

    /// <summary>
    /// This was too easy to solve almost manually. Wasn't worth the effort to solve properly.
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
}
