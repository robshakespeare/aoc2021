using System.Numerics;
using MoreLinq;

namespace AoC.Day11;

public class Day11Solver : SolverBase
{
    public override string DayName => "Dumbo Octopus";

    public override long? SolvePart1(PuzzleInput input)
    {
        var simulator = Simulator.Parse(input);

        const int numberOfSteps = 100;
        var numberOfFlashes = 0;

        for (var step = 1; step <= numberOfSteps; step++)
        {
            numberOfFlashes += simulator.SimulateStep();
        }

        return numberOfFlashes;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var simulator = Simulator.Parse(input);

        var step = 0;
        int numberOfFlashes;

        do
        {
            step++;
            numberOfFlashes = simulator.SimulateStep();
        } while (numberOfFlashes != simulator.Octopuses.Length);

        return step;
    }

    public record Simulator(Octopus[][] Grid, Octopus[] Octopuses)
    {
        /// <summary>
        /// Simulates a single step in the world state, and returns the number of flashes that occurred.
        /// </summary>
        public int SimulateStep()
        {
            Octopuses.ForEach(octopus => octopus.BeginStep());
            Octopuses.ForEach(octopus => octopus.UpdateFlash(Grid));
            return Octopuses.Select(octopus => octopus.EndStep()).Count(flashed => flashed);
        }

        public static Simulator Parse(PuzzleInput input)
        {
            var grid = input.ReadLines()
                .Select((line, y) => line.Select((c, x) => new Octopus(int.Parse($"{c}"), new Vector2(x, y))).ToArray())
                .ToArray();

            var octopuses = grid.SelectMany(line => line).ToArray();

            return new(grid, octopuses);
        }
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
            EnergyLevel++; // the energy level of each octopus increases by 1
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
                foreach (var adjacentOctopus in grid.GetAdjacent(Position))
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
    }
}
