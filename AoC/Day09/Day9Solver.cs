namespace AoC.Day09;

public class Day9Solver : SolverBase
{
    public override string DayName => "Smoke Basin";

    public override long? SolvePart1(PuzzleInput input)
    {
        var grid = input.ReadLines()
            .Select((line, y) => line.Select((chr, x) => new HeightmapLocation(x, y, int.Parse($"{chr}"))).ToArray())
            .ToArray();

        var lowPoints = grid.SelectMany(locations => locations).Where(location => location.IsLowPoint(grid));

        return lowPoints.Sum(location => location.RiskLevel);
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public record HeightmapLocation(int X, int Y, int Height)
    {
        public long RiskLevel { get; } = Height + 1;

        public bool IsLowPoint(HeightmapLocation[][] grid) => GetAdjacentLocations(this, grid).All(l => Height < l.Height);

        private static readonly (int x, int y)[] PossibleMovements = {(0, -1), (-1, 0), (1, 0), (0, 1)};

        private static HeightmapLocation? SafeGetLocation(int x, int y, HeightmapLocation[][] grid)
        {
            if (y < 0 || y >= grid.Length)
                return null;

            var line = grid[y];
            return x < 0 || x >= line.Length ? null : line[x];
        }

        public static IEnumerable<HeightmapLocation> GetAdjacentLocations(HeightmapLocation location, HeightmapLocation[][] grid) =>
            PossibleMovements
                .Select(m => (x: location.X + m.x, y: location.Y + m.y))
                .Select(p => SafeGetLocation(p.x, p.y, grid))
                .Where(x => x != null)
                .Select(x => x!);
    }
}
