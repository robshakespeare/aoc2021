namespace AoC.Day19;

public class Day19Solver : SolverBase
{
    public override string DayName => "Beacon Scanner";

    public override long? SolvePart1(PuzzleInput input)
    {
        var points = new[] {404, -588, -901};

        var perms = GetAllPermutations(points).Select(x => string.Join(", ", x)).Distinct().ToArray();

        Console.WriteLine($"There are {perms.Length} perms:");

        foreach (var perm in perms)
        {
            Console.WriteLine(perm);
        }

        return null;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static IEnumerable<int[]> GetAllPermutations(IEnumerable<int> vectorCoords)
    {
        var vector = vectorCoords.Select(Math.Abs).ToArray();

        yield return new[] {vector[0], vector[1], vector[2]};
        yield return new[] {-vector[0], vector[1], vector[2]};
        yield return new[] {-vector[0], -vector[1], vector[2]};
        yield return new[] {vector[0], -vector[1], vector[2]};
        yield return new[] {vector[0], -vector[1], -vector[2]};
        yield return new[] {vector[0], vector[1], -vector[2]};
        yield return new[] {-vector[0], vector[1], -vector[2]};
        yield return new[] {-vector[0], -vector[1], -vector[2]};

        yield return new[] {vector[2], vector[0], vector[1]};
        yield return new[] {-vector[2], vector[0], vector[1]};
        yield return new[] {-vector[2], -vector[0], vector[1]};
        yield return new[] {vector[2], -vector[0], vector[1]};
        yield return new[] {vector[2], -vector[0], -vector[1]};
        yield return new[] {vector[2], vector[0], -vector[1]};
        yield return new[] {-vector[2], vector[0], -vector[1]};
        yield return new[] {-vector[2], -vector[0], -vector[1]};

        yield return new[] {vector[1], vector[2], vector[0]};
        yield return new[] {-vector[1], vector[2], vector[0]};
        yield return new[] {-vector[1], -vector[2], vector[0]};
        yield return new[] {vector[1], -vector[2], vector[0]};
        yield return new[] {vector[1], -vector[2], -vector[0]};
        yield return new[] {vector[1], vector[2], -vector[0]};
        yield return new[] {-vector[1], vector[2], -vector[0]};
        yield return new[] {-vector[1], -vector[2], -vector[0]};
    }
}
