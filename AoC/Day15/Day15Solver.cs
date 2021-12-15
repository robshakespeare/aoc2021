namespace AoC.Day15;

public class Day15Solver : SolverBase
{
    public override string DayName => "Chiton";

    public override long? SolvePart1(PuzzleInput input)
    {
        var cavern = Cavern.Parse(input);

        //return GetDefaultPath(cavern).TotalRiskLevel;

        var pathWithLowestRisk = AStarSearch(cavern);

        return pathWithLowestRisk.TotalRiskLevel;

        //var pathWithLowestTotalRisk = completePaths.MinBy(x => x.TotalRiskLevel);

        //return pathWithLowestTotalRisk?.TotalRiskLevel;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    // https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
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
                    explore.Enqueue((child, path.Visit(child), stepcost + cost), stepcost + cost + heuristic(child, cavern.End));
                }
                seen.Add(node);
            }
        }

        throw new InvalidOperationException("No paths found");
    }

    private static long heuristic(Node child, Node target)
    {
        return MathUtils.ManhattanDistance(child.Position, target.Position);
    }


    private static Path GetPathWithLowestRisk(Cavern cavern)
    {
        var currentPaths = new HashSet<Path>();
        var completePaths = new List<Path>();

        currentPaths.Add(Path.Begin(cavern.Start));

        var lowestTotalRisk = cavern.GetDefaultPath().TotalRiskLevel;

        while (currentPaths.Any())
        {
            var newPaths = new HashSet<Path>();

            foreach (var currentPath in currentPaths)
            {
                var adjacentLocations = cavern.Grid
                    .GetAdjacent(GridUtils.DirectionsExcludingDiagonal, currentPath.CurrentNode.Position)
                    .Except(currentPath.VisitedLocations);

                foreach (var adjacentLocation in adjacentLocations)
                {
                    var newPath = currentPath.Visit(adjacentLocation);

                    if (adjacentLocation == cavern.End)
                    {
                        completePaths.Add(newPath);
                        lowestTotalRisk = Math.Min(newPath.TotalRiskLevel, lowestTotalRisk);
                    }
                    else if (newPath.TotalRiskLevel < lowestTotalRisk)
                    {
                        var manhattanDistance = MathUtils.ManhattanDistance(adjacentLocation.Position, cavern.End.Position);

                        var remainingMinRisk = lowestTotalRisk - newPath.TotalRiskLevel;

                        var reject = newPath.TotalRiskLevel >= lowestTotalRisk ||
                                     manhattanDistance > remainingMinRisk;

                        if (!reject)
                        {
                            newPaths.Add(newPath);
                        }
                    }
                }
            }

            currentPaths = newPaths;
        }

        return completePaths.MinBy(x => x.TotalRiskLevel) ?? throw new InvalidOperationException("No paths found");
    }

    //public record Node(Vector2 Position, int RiskLevel)
    //{
    //    public string Id { get; } = $"{Position.X},{Position.Y} ({RiskLevel})";

    //    public override string ToString() => Id;
    //}

    public record Node(Vector2 Position, int RiskLevel)
    {
        public string Id { get; } = $"{Position.X},{Position.Y}:{RiskLevel}";
    }

    public record Cavern(Node[][] Grid)
    {
        public Node Start { get; } = Grid[0][0];
        public Node End { get; } = Grid.Last().Last();

        public static Cavern Parse(PuzzleInput input) => new(input.ReadLines().Select(
            (line, y) => line.Select(
                (c, x) => new Node(new Vector2(x, y), int.Parse(c.ToString()))).ToArray()).ToArray());

        public Path GetDefaultPath()
        {
            var manhattanDistance = MathUtils.ManhattanDistance(Start.Position, End.Position);

            var path = Path.Begin(Start);

            for (var step = 1; step <= manhattanDistance; step++)
            {
                var nextDirection = step % 2 == 0 ? MathUtils.South : MathUtils.East;
                var nextPosition = path.CurrentNode.Position + nextDirection;
                var nextLocation = Grid.SafeGet(nextPosition) ?? throw new InvalidOperationException($"Next position {nextPosition} was invalid");
                path = path.Visit(nextLocation);
            }

            return path.CurrentNode == End ? path : throw new InvalidOperationException("End not reached");
        }
    }

    public class Path
    {
        private Path(string name, int totalRiskLevel, Node currentNode, IReadOnlySet<Node> visitedLocations)
        {
            Name = name;
            TotalRiskLevel = totalRiskLevel;
            CurrentNode = currentNode;
            VisitedLocations = visitedLocations;
        }

        public string Name { get; }

        public int TotalRiskLevel { get; }

        public Node CurrentNode { get; }

        public IReadOnlySet<Node> VisitedLocations { get; }

        public override string ToString() => Name;

        public override bool Equals(object? obj) => obj is Path other && Equals(other);

        protected bool Equals(Path other) => Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();

        public Path Visit(Node node) => new(
            Name + '>' + node.Id,
            TotalRiskLevel + node.RiskLevel,
            node,
            new HashSet<Node>(VisitedLocations) {node});

        public static Path Begin(Node begin) => new(begin.Id, 0, begin, new HashSet<Node> { begin });
    }
}
