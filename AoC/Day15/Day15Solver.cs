namespace AoC.Day15;

public class Day15Solver : SolverBase
{
    public override string DayName => "Chiton";

    public override long? SolvePart1(PuzzleInput input)
    {
        var cavern = Cavern.Parse(input);

        //return GetDefaultPath(cavern).TotalRiskLevel;

        var pathWithLowestRisk = GetPathWithLowestRisk(cavern);

        return pathWithLowestRisk.TotalRiskLevel;

        //var pathWithLowestTotalRisk = completePaths.MinBy(x => x.TotalRiskLevel);

        //return pathWithLowestTotalRisk?.TotalRiskLevel;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
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
                    .GetAdjacent(GridUtils.DirectionsExcludingDiagonal, currentPath.CurrentLocation.Position)
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

    public record Location(Vector2 Position, int RiskLevel)
    {
        public string Id { get; } = $"{Position.X},{Position.Y}:{RiskLevel}";
    }

    public record Cavern(Location[][] Grid)
    {
        public Location Start { get; } = Grid[0][0];
        public Location End { get; } = Grid.Last().Last();

        public static Cavern Parse(PuzzleInput input) => new(input.ReadLines().Select(
            (line, y) => line.Select(
                (c, x) => new Location(new Vector2(x, y), int.Parse(c.ToString()))).ToArray()).ToArray());

        public Path GetDefaultPath()
        {
            var manhattanDistance = MathUtils.ManhattanDistance(Start.Position, End.Position);

            var path = Path.Begin(Start);

            for (var step = 1; step <= manhattanDistance; step++)
            {
                var nextDirection = step % 2 == 0 ? MathUtils.South : MathUtils.East;
                var nextPosition = path.CurrentLocation.Position + nextDirection;
                var nextLocation = Grid.SafeGet(nextPosition) ?? throw new InvalidOperationException($"Next position {nextPosition} was invalid");
                path = path.Visit(nextLocation);
            }

            return path.CurrentLocation == End ? path : throw new InvalidOperationException("End not reached");
        }
    }

    public class Path
    {
        private Path(string name, int totalRiskLevel, Location currentLocation, IReadOnlySet<Location> visitedLocations)
        {
            Name = name;
            TotalRiskLevel = totalRiskLevel;
            CurrentLocation = currentLocation;
            VisitedLocations = visitedLocations;
        }

        public string Name { get; }

        public int TotalRiskLevel { get; }

        public Location CurrentLocation { get; }

        public IReadOnlySet<Location> VisitedLocations { get; }

        public override string ToString() => Name;

        public override bool Equals(object? obj) => obj is Path other && Equals(other);

        protected bool Equals(Path other) => Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();

        public Path Visit(Location location) => new(
            Name + '>' + location.Id,
            TotalRiskLevel + location.RiskLevel,
            location,
            new HashSet<Location>(VisitedLocations) {location});

        public static Path Begin(Location begin) => new(begin.Id, 0, begin, new HashSet<Location> { begin });
    }
}
