using System.Numerics;
using MoreLinq;

namespace AoC.Day11;

public class Day11Solver : SolverBase
{
    public override string DayName => "Dumbo Octopus";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (grid, octopuses) = Parse(input);

        const int numberOfSteps = 100;

        long numberOfFlashes = 0;

        for (var step = 1; step <= numberOfSteps; step++)
        {
            octopuses.ForEach(octopus => octopus.BeginStep());

            octopuses.ForEach(octopus => octopus.UpdateFlash(grid));

            numberOfFlashes += octopuses.Select(octopus => octopus.EndStep()).Count(flashed => flashed);
        }

        return numberOfFlashes;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var (grid, octopuses) = Parse(input);

        int numberOfFlashes;
        var stepCounter = 0;

        do
        {
            stepCounter++;

            octopuses.ForEach(octopus => octopus.BeginStep());

            octopuses.ForEach(octopus => octopus.UpdateFlash(grid));

            numberOfFlashes = octopuses.Select(octopus => octopus.EndStep()).Count(flashed => flashed);
        } while (numberOfFlashes != octopuses.Length);

        return stepCounter;
    }

    private static (Octopus[][] grid, Octopus[] octopuses) Parse(PuzzleInput input)
    {
        var grid = input.ReadLines()
            .Select((line, y) => line.Select((c, x) => new Octopus(int.Parse($"{c}"), new Vector2(x, y))).ToArray())
            .ToArray();

        var octopuses = grid.SelectMany(line => line).ToArray();

        return (grid, octopuses);
    }

    public class Octopus
    {
        public int EnergyLevel { get; private set; }
        public Vector2 Position { get; }
        public bool Flash { get; private set; }

        public Octopus(int energyLevel, Vector2 position)
        {
            EnergyLevel = energyLevel;
            Position = position;
        }

        public void BeginStep()
        {
            Flash = false;

            // the energy level of each octopus increases by 1
            EnergyLevel++;
        }

        public void UpdateFlash(Octopus[][] grid)
        {
            // If we've already flashed this step, that means we've already had our energy level increased beyond 9 this step, so don't do it again
            if (Flash)
                return;

            // Any octopus with an energy level greater than 9 flashes
            if (EnergyLevel > 9)
            {
                Flash = true;

                // This increases the energy level of all adjacent octopuses by 1, including octopuses that are diagonally adjacent.
                // If this causes an octopus to have an energy level greater than 9, it also flashes.
                // This process continues as long as new octopuses keep having their energy level increased beyond 9.
                foreach (var adjacentOctopus in GetAdjacentOctopus(grid))
                {
                    adjacentOctopus.EnergyLevel++;
                    adjacentOctopus.UpdateFlash(grid);
                }
            }
        }

        public bool EndStep()
        {
            // Finally, any octopus that flashed during this step has its energy level set to 0, as it used all of its energy to flash.
            if (Flash)
            {
                EnergyLevel = 0;
                Flash = false;
                return true;
            }

            return false;
        }

        public IEnumerable<Octopus> GetAdjacentOctopus(Octopus[][] grid) =>
            Directions
                .Select(dir => Position + dir)
                .Select(position => SafeGetOctopus(position, grid))
                .Where(x => x != null)
                .Select(x => x!);
    }

    private static Octopus? SafeGetOctopus(Vector2 position, Octopus[][] grid)
    {
        var y = position.Y.Round();

        if (y < 0 || y >= grid.Length)
            return null;

        var x = position.X.Round();
        var line = grid[y];
        return x < 0 || x >= line.Length ? null : line[x];
    }

    private static readonly Vector2[] Directions =
    {
        new(-1, -1),
        new(0, -1),
        new(1, -1),

        new(-1, 0),
        new(1, 0),

        new(-1, 1),
        new(0, 1),
        new(1, 1)
    };
}
