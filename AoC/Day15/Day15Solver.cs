namespace AoC.Day15;

public class Day15Solver : SolverBase
{
    public override string DayName => "Chiton";

    public override long? SolvePart1(PuzzleInput input)
    {
        var cavern = Cavern.Parse(input);

        // Get the path with lowest risk, where risk is the cost in the A* search
        // And the heuristic is simply the manhattan distance, i.e. the remaining amount of minimum steps, even at risk level 1, it will take to move from the node to the end
        var pathWithLowestRisk = AStarSearch(cavern);

        return pathWithLowestRisk.TotalRiskLevel;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    /// <summary>
    /// Returns the path with the lowest total risk.
    /// From: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    /// rs-todo: needs more tidy
    /// rs-todo: make this generic, so it can be used easily.
    /// </summary>
    private static Path AStarSearch(Cavern cavern)
    {
        var explore = new PriorityQueue<(Node Node, Path Path, int Cost), long>();
        explore.Enqueue((cavern.Start, Path.Begin(cavern.Start), cavern.Start.RiskLevel), 0);

        var seen = new HashSet<Node>();

        while (explore.Count > 0)
        {
            var (node, path, cost) = explore.Dequeue(); // this takes out a node/state

            // if node is the goal return the path
            if (node == cavern.End)
            {
                return path;
            }

            // if not node in seen
            if (!seen.Contains(node))
            {
                //for child, direction, stepcost in problem.getSuccessors(node):
                foreach (var child in cavern.Grid.GetAdjacent(GridUtils.DirectionsExcludingDiagonal, node.Position))
                {
                    // explore.push((child, path + [direction], stepcost + cost), stepcost + cost + heuristic(child)) // the heursitic is added here as a part of the priority

                    var stepcost = child.RiskLevel;

                    // rs-todo: ideally change path.Visit(child) to path + child, and also I don't think we need to track VisitedNodes
                    explore.Enqueue((child, path.Visit(child), stepcost + cost), stepcost + cost + Heuristic(child, cavern.End));
                }
                seen.Add(node);
            }
        }

        throw new InvalidOperationException("No paths found");
    }

    private static long Heuristic(Node child, Node target) => MathUtils.ManhattanDistance(child.Position, target.Position);

    public record Node(Vector2 Position, int RiskLevel)
    {
        public string Id { get; } = $"{Position.X},{Position.Y}:{RiskLevel}";

        public override string ToString() => Id;
    }

    public record Cavern(Node[][] Grid)
    {
        public Node Start { get; } = Grid[0][0];
        public Node End { get; } = Grid.Last().Last();

        public static Cavern Parse(PuzzleInput input) => new(input.ReadLines().Select(
            (line, y) => line.Select(
                (c, x) => new Node(new Vector2(x, y), int.Parse(c.ToString()))).ToArray()).ToArray());
    }

    public class Path
    {
        private Path(string name, int totalRiskLevel, Node currentNode)
        {
            Name = name;
            TotalRiskLevel = totalRiskLevel;
            CurrentNode = currentNode;
        }

        public string Name { get; }

        public int TotalRiskLevel { get; }

        public Node CurrentNode { get; }

        public override string ToString() => Name;

        public override bool Equals(object? obj) => obj is Path other && Equals(other);

        protected bool Equals(Path other) => Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();

        public Path Visit(Node node) => new(
            Name + '>' + node.Id,
            TotalRiskLevel + node.RiskLevel,
            node);

        public static Path Begin(Node begin) => new(begin.Id, 0, begin);
    }
}
