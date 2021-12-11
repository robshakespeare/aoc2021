namespace AoC.Day09;

public class Day9Solver : SolverBase
{
    public override string DayName => "Smoke Basin";

    public override long? SolvePart1(PuzzleInput input) => GetLowPoints(ParseToGrid(input)).Sum(location => location.RiskLevel);

    public override long? SolvePart2(PuzzleInput input)
    {
        var grid = ParseToGrid(input);
        var basins = grid.SelectMany(locations => locations)
            .Select(location => location.GetBasin(grid))
            .OrderByDescending(basin => basin.Count);
        return basins.Take(3).Aggregate(1, (agg, basin) => agg * basin.Count);
    }

    public static HeightmapLocation[][] ParseToGrid(PuzzleInput input) =>
        input.ReadLines()
            .Select((line, y) => line.Select((chr, x) => new HeightmapLocation(new Vector2(x, y), int.Parse($"{chr}"))).ToArray())
            .ToArray();

    public static IEnumerable<HeightmapLocation> GetLowPoints(HeightmapLocation[][] grid) =>
        grid.SelectMany(locations => locations).Where(location => location.IsLowPoint(grid));

    public record HeightmapLocation(Vector2 Position, int Height)
    {
        public long RiskLevel { get; } = Height + 1;

        public bool IsLowPoint(HeightmapLocation[][] grid) => GetAdjacentLocations(grid).All(l => Height < l.Height);

        public IEnumerable<HeightmapLocation> GetAdjacentLocations(HeightmapLocation[][] grid) =>
            grid.GetAdjacent(GridUtils.DirectionsExcludingDiagonal, Position);

        public IEnumerable<HeightmapLocation> GetAdjacentBasinLocations(HeightmapLocation[][] grid) =>
            GetAdjacentLocations(grid)
                .Where(adjacent => adjacent.Height > Height && adjacent.Height < 9);

        private HashSet<HeightmapLocation> EnumerateBasin(HashSet<HeightmapLocation> basin, HeightmapLocation[][] grid)
        {
            basin.Add(this);
            foreach (var adjacent in GetAdjacentBasinLocations(grid))
                if (!basin.Contains(adjacent))
                    adjacent.EnumerateBasin(basin, grid);
            return basin;
        }

        public IReadOnlyCollection<HeightmapLocation> GetBasin(HeightmapLocation[][] grid) =>
            IsLowPoint(grid)
                ? EnumerateBasin(new HashSet<HeightmapLocation>(), grid)
                : Array.Empty<HeightmapLocation>();
    }
}
