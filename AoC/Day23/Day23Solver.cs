namespace AoC.Day23;

public class Day23Solver : SolverBase
{
    public override string DayName => "Amphipod";

    public override long? SolvePart1(PuzzleInput input) => Solve(input, insertAdditionalLines: false);

    public override long? SolvePart2(PuzzleInput input) => Solve(input, insertAdditionalLines: true);

    private static long Solve(PuzzleInput input, bool insertAdditionalLines)
    {
        // Thinking A* Search will do it, where each node is the state of the grid, and get successors is all of the next possible grids of moves, with a cost
        // Q: what is the heuristic? Probably just use 1 for now to ignore heuristic; but might be able to use the minimum number of steps (manhattan distance) for each Amphipod to move home
        // A: Dijkstra Search is fine! :)

        var initialGrid = Grid.Parse(input, insertAdditionalLines);

        var (finalGrid, totalCost) = GridDijkstraSearch.FindSmallestCostToGridGoal(initialGrid);

        finalGrid.WriteToConsole();

        return totalCost;
    }
}
