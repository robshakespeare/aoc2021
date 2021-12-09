namespace AoC.Day09;

public class Day9Solver : SolverBase
{
    public override string DayName => "Smoke Basin";

    public override long? SolvePart1(PuzzleInput input) => GetLowPoints(ParseToGrid(input)).Sum(location => location.RiskLevel);

    public override long? SolvePart2(PuzzleInput input)
    {
        var grid = ParseToGrid(input);
        //var lowPoints = GetLowPoints(grid);
        var basins = grid.SelectMany(locations => locations)
            .Select(location => location.GetBasin(grid))
            .OrderByDescending(basin => basin.Count);

        return basins.Take(3).Aggregate(1, (agg, basin) => agg * basin.Count);

        //var grid = ParseToGrid(input);
        ////var lowPoints = GetLowPoints(grid);
        //var basinSizes = grid.SelectMany(locations => locations)
        //    .Select(location => location.GetBasinSize(grid))
        //    .OrderByDescending(size => size);
        //return basinSizes
        //    .Take(3)
        //    .Aggregate(1L, (agg, cur) => agg * cur);
    }

    public static HeightmapLocation[][] ParseToGrid(PuzzleInput input) =>
        input.ReadLines()
            .Select((line, y) => line.Select((chr, x) => new HeightmapLocation(x, y, int.Parse($"{chr}"))).ToArray())
            .ToArray();

    public static IEnumerable<HeightmapLocation> GetLowPoints(HeightmapLocation[][] grid) =>
        grid.SelectMany(locations => locations).Where(location => location.IsLowPoint(grid));

    public record HeightmapLocation(int X, int Y, int Height)
    {
        public long RiskLevel { get; } = Height + 1;

        public bool IsLowPoint(HeightmapLocation[][] grid) => GetAdjacentLocations(grid).All(l => Height < l.Height);

        private static readonly (int x, int y)[] PossibleMovements = {(0, -1), (-1, 0), (1, 0), (0, 1)};

        private static HeightmapLocation? SafeGetLocation(int x, int y, HeightmapLocation[][] grid)
        {
            if (y < 0 || y >= grid.Length)
                return null;

            var line = grid[y];
            return x < 0 || x >= line.Length ? null : line[x];
        }

        public IEnumerable<HeightmapLocation> GetAdjacentLocations(HeightmapLocation[][] grid) =>
            PossibleMovements
                .Select(m => (x: X + m.x, y: Y + m.y))
                .Select(p => SafeGetLocation(p.x, p.y, grid))
                .Where(x => x != null)
                .Select(x => x!);

        public IEnumerable<HeightmapLocation> GetAdjacentBasinLocations(HeightmapLocation[][] grid) =>
            GetAdjacentLocations(grid)
                .Where(adjacent => adjacent.Height > Height && adjacent.Height < 9);

        private void EnumerateBasin(/*HeightmapLocation location,*/ HashSet<HeightmapLocation> basin, HeightmapLocation[][] grid)
        {
            basin.Add(this);

            //foreach (var adjacent in GetAdjacentLocations(grid)
            //    .Where(adjacent => adjacent.Height >= location.Height && adjacent.Height < 9))
            foreach (var adjacent in GetAdjacentBasinLocations(grid))
                //foreach (var adjacent in location.GetAdjacentLocations(grid)
                //             .Where(adjacent => adjacent.Height > location.Height && adjacent.Height < 9))
            {
                if (!basin.Contains(adjacent))
                {
                    adjacent.EnumerateBasin(basin, grid);
                }
            }
        }

        public IReadOnlyCollection<HeightmapLocation> GetBasin(HeightmapLocation[][] grid)
        {
            if (!IsLowPoint(grid))
            {
                return Array.Empty<HeightmapLocation>();
            }

            // count self
            // count distinct adjacent basin locations recursively
            var basin = new HashSet<HeightmapLocation>();
            EnumerateBasin(basin, grid);
            return basin;
        }

        //public long GetBasinSize(HeightmapLocation[][] grid)
        //{
        //    if (!IsLowPoint(grid))
        //    {
        //        return 0;
        //    }

        //    // count self
        //    // count distinct adjacent basin locations recursively

        //    var basin = new HashSet<HeightmapLocation>();

        //    void EnumerateBasin(HeightmapLocation location)
        //    {
        //        basin.Add(location);

        //        //foreach (var adjacent in GetAdjacentLocations(grid)
        //        //    .Where(adjacent => adjacent.Height >= location.Height && adjacent.Height < 9))
        //        foreach (var adjacent in GetAdjacentBasinLocations(grid))
        //        {
        //            if (!basin.Contains(adjacent))
        //            {
        //                EnumerateBasin(adjacent);
        //            }
        //        }
        //    }

        //    EnumerateBasin(this);

        //    return basin.Count;
        //}
    }
}
