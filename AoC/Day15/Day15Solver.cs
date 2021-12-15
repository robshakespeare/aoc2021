using static AoC.AStarSearch;

namespace AoC.Day15;

public class Day15Solver : SolverBase
{
    public override string DayName => "Chiton";

    public override long? SolvePart1(PuzzleInput input) => Solve(Cavern.Parse(input));

    public override long? SolvePart2(PuzzleInput input) => Solve(Cavern.Parse(input).Expand(5));

    private static int Solve(Cavern cavern)
    {
        // Get the path with lowest risk, where risk is the cost in the A* search
        // And the heuristic is simply the manhattan distance, i.e. the remaining amount of minimum steps, even at risk level 1, it will take to move from the node to the end

        var search = new AStarSearch(
            getSuccessors: node => cavern.Grid.GetAdjacent(GridUtils.DirectionsExcludingDiagonal, node.Position),
            heuristic: (node, goal) => MathUtils.ManhattanDistance(node.Position, goal.Position));

        var pathWithLowestRisk = search.FindShortestPath(cavern.Start, cavern.End);

        return pathWithLowestRisk.TotalCost;
    }

    public record Cavern(Node[][] Grid)
    {
        public Node Start { get; } = Grid[0][0];
        public Node End { get; } = Grid.Last().Last();

        public static Cavern Parse(PuzzleInput input) => new(input.ReadLines().Select(
            (line, y) => line.Select(
                (c, x) => new Node(new Vector2(x, y), int.Parse(c.ToString()))).ToArray()).ToArray());

        /// <summary>
        /// The entire cave is actually five times larger in both dimensions than you thought;
        /// the area you originally scanned is just one tile in a 5x5 tile area that forms the full map.
        /// Your original map tile repeats to the right and downward;
        /// each time the tile repeats to the right or downward, all of its risk levels are 1 higher than the tile immediately up or left of it.
        /// However, risk levels above 9 wrap back around to 1.
        /// So, if your original map had some position with a risk level of 8
        /// Then next tile would have risk level 9
        /// Then next tile would have risk level 1
        /// </summary>
        public Cavern Expand(int amount)
        {
            var originalSize = Grid.Length;
            var newSize = Grid.Length * amount;

            var newCoords = Enumerable.Range(0, newSize).Select(y => Enumerable.Range(0, newSize).Select(x => (x, y)).ToArray()).ToArray();

            return new Cavern(newCoords.Select(line => line.Select(c =>
            {
                var (x, y) = c;
                var originalRisk = Grid[y % originalSize][x % originalSize].Cost;
                var increase = x / originalSize + y / originalSize;
                var newRisk = (originalRisk + increase) % 9;
                newRisk = newRisk == 0 ? 9 : newRisk;
                return new Node(new Vector2(x, y), newRisk);
            }).ToArray()).ToArray());
        }
    }
}
