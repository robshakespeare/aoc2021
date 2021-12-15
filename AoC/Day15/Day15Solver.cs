namespace AoC.Day15;

public class Day15Solver : SolverBase
{
    public override string DayName => "Chiton";

    public override long? SolvePart1(PuzzleInput input)
    {
        var grid = input.ReadLines().Select(
            (line, y) => line.Select(
                (c, x) => new Location(new Vector2(x, y), int.Parse(c.ToString()))).ToArray()).ToArray();

        var completePaths = GetCompletePaths(grid);

        var pathWithLowestTotalRisk = completePaths.MinBy(x => x.TotalRiskLevel);

        return pathWithLowestTotalRisk?.TotalRiskLevel;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static readonly Vector2[] LimitedDirections = { MathUtils.East, MathUtils.South };

    private static IReadOnlyCollection<Path> GetCompletePaths(Location[][] grid)
    {
        var currentPaths = new HashSet<Path>();
        var completePaths = new List<Path>();

        var start = grid[0][0];
        var end = grid.Last().Last();

        currentPaths.Add(Path.Begin(start));

        var lowestTotalRisk = int.MaxValue;

        while (currentPaths.Any())
        {
            var newPaths = new HashSet<Path>();

            foreach (var currentPath in currentPaths)
            {
                var adjacentLocations = grid
                    .GetAdjacent(LimitedDirections, currentPath.CurrentLocation.Position)
                    .Except(currentPath.VisitedLocations)
                    .ToArray();

                var minRiskLevel = adjacentLocations.Min(x => x.RiskLevel);

                foreach (var adjacentLocation in adjacentLocations.Where(x => x.RiskLevel == minRiskLevel))
                {
                    var newPath = currentPath.Visit(adjacentLocation);

                    if (adjacentLocation == end)
                    {
                        completePaths.Add(newPath);
                        lowestTotalRisk = Math.Min(newPath.TotalRiskLevel, lowestTotalRisk);
                    }
                    else if (newPath.TotalRiskLevel < lowestTotalRisk)
                    {
                        newPaths.Add(newPath);
                    }
                }
            }

            currentPaths = newPaths;
        }

        return completePaths;
    }

    public record Location(Vector2 Position, int RiskLevel)
    {
        public string Id { get; } = $"{Position.X},{Position.Y}:{RiskLevel}";
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
