namespace AoC.Day23;

public class Day23Solver : SolverBase
{
    public override string DayName => "Amphipod";

    public override long? SolvePart1(PuzzleInput input) => new Day23Part1Solver(input).Solve();

    public override long? SolvePart2(PuzzleInput input)
    {
        // Thinking A* Search will do it, where each node is the state of the grid, and get successors is all of the next possible grids of moves, with a cost
        // Q: what is the heuristic? Probably just use 1 for now to ignore heuristic; but might be able to use the minimum number of steps (manhattan distance) for each Amphipod to move home

        var initialGrid = Grid.Parse(input, insertAdditionalLines: true);

        //initialGrid.WriteToConsole();

        ////foreach (var amphipod in initialGrid.GetNextAmphipodMovements())
        ////{
        ////    Console.WriteLine(amphipod);
        ////}

        //var nextOne = initialGrid.GetSuccessors().First();
        //nextOne.Grid.WriteToConsole();

        //Console.WriteLine(nextOne.Grid.GetSuccessors().Count());

        var result = GridDijkstraSearch.FindSmallestCostToGridGoal(initialGrid);

        return result.TotalCost;
    }
}
