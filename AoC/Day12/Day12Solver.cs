namespace AoC.Day12;

public class Day12Solver : SolverBase
{
    public override string DayName => "Passage Pathing";

    /// <summary>
    /// All paths should visit small caves at most once, and can visit big caves any number of times.
    /// How many paths through this cave system are there that visit small caves at most once?
    /// </summary>
    public override long? SolvePart1(PuzzleInput input)
    {
        var caveSystem = CaveSystem.Parse(input);
        var completePaths = GetCompletePaths(
            caveSystem,
            (currentPath, connectedCave) => !connectedCave.IsSmall || !currentPath.SmallCaves.Contains(connectedCave));
        return completePaths.Count;
    }

    /// <summary>
    /// Big caves can be visited any number of times.
    /// A single small cave can be visited at most twice, and the remaining small caves can be visited at most once. 
    /// However, the caves named `start` and `end` can only be visited exactly once each: once you leave the `start` cave,
    /// you may not return to it, and once you reach the end cave, the path must `end` immediately.
    /// How many paths through this cave system are there
    /// </summary>
    public override long? SolvePart2(PuzzleInput input)
    {
        var caveSystem = CaveSystem.Parse(input);

        var completePaths = GetCompletePaths(
            caveSystem,
            (currentPath, connectedCave) =>
            {
                if (!connectedCave.IsSmall)
                {
                    return true; // Big caves can be visited any number of times.
                }

                var smallCaveVisitedTwice = currentPath.SmallCaves.GroupBy(cave => cave.Name).Any(grp => grp.Count() > 1);

                if (smallCaveVisitedTwice || connectedCave.IsStart || connectedCave.IsEnd)
                {
                    return !currentPath.SmallCaves.Contains(connectedCave);
                }

                return currentPath.SmallCaves.Count(x => x.Name == connectedCave.Name) < 2;
            });

        return completePaths.Count;
    }

    private static IReadOnlyCollection<Path> GetCompletePaths(CaveSystem caveSystem, Func<Path, Cave, bool> shouldVisitCave)
    {
        var currentPaths = new HashSet<Path>();
        var completePaths = new List<Path>();

        currentPaths.Add(Path.Begin(caveSystem.Start));

        while (currentPaths.Any())
        {
            var newPaths = new HashSet<Path>();

            foreach (var currentPath in currentPaths)
            {
                foreach (var connectedCave in currentPath.CurrentCave.ConnectedCaves)
                {
                    if (shouldVisitCave(currentPath, connectedCave))
                    {
                        var newPath = currentPath.Concat(connectedCave);

                        if (connectedCave.IsEnd)
                        {
                            completePaths.Add(newPath);
                        }
                        else
                        {
                            newPaths.Add(newPath);
                        }
                    }
                }
            }

            currentPaths = newPaths;
        }

        return completePaths;
    }

    public class Path
    {
        private Path(string name, Cave currentCave, /*IReadOnlyCollection<Cave> caves,*/ IReadOnlyCollection<Cave> smallCaves)
        {
            Name = name;
            CurrentCave = currentCave;
            //Caves = caves;
            SmallCaves = smallCaves;
        }

        public string Name { get; }

        public Cave CurrentCave { get; }

        //public IReadOnlyCollection<Cave> Caves { get; }

        public IReadOnlyCollection<Cave> SmallCaves { get; }

        public override string ToString() => Name;

        public override bool Equals(object? obj) => obj is Path other && Equals(other);

        protected bool Equals(Path other) => Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();

        public Path Concat(Cave connectedCave) => new(
            Name + '>' + connectedCave.Name,
            connectedCave,
            //Caves.Concat(new[] {connectedCave}).ToArray(),
            connectedCave.IsSmall ? SmallCaves.Concat(new[] { connectedCave }).ToArray() : SmallCaves);

        public static Path Begin(Cave start)
        {
            if (!start.IsStart) throw new InvalidOperationException();
            return new Path(start.Name, start, /*new[] {start},*/ new[] {start});
        }
    }

    public record CaveSystem(Cave Start, Cave End, Dictionary<string, Cave> Caves)
    {
        private static readonly Regex ParseInputRegex = new(@"(?<beginningCaveName>[\w]+)-(?<endingCaveName>[\w]+)", RegexOptions.Compiled);

        public static CaveSystem Parse(PuzzleInput input)
        {
            var caves = new Dictionary<string, Cave>();
            Cave GetOrAdd(string name) => caves.TryGetValue(name, out var cave) ? cave : caves[name] = new Cave(name);

            foreach (Match match in ParseInputRegex.Matches(input.ToString()))
            {
                var beginningCave = GetOrAdd(match.Groups["beginningCaveName"].Value);
                var endingCave = GetOrAdd(match.Groups["endingCaveName"].Value);

                beginningCave.AddConnectedCave(endingCave);
            }

            return new CaveSystem(caves["start"], caves["end"], caves);
        }
    }

    public class Cave
    {
        private readonly HashSet<Cave> _connectedCaves = new();

        public Cave(string name)
        {
            Name = name;
            IsStart = Name == "start";
            IsEnd = Name == "end";
            IsSmall = Name.All(c => c is >= 'a' and <= 'z');
        }

        public string Name { get; }

        public bool IsStart { get; }

        public bool IsEnd{ get; }

        public bool IsSmall { get; }

        public IReadOnlyCollection<Cave> ConnectedCaves => _connectedCaves;

        public void AddConnectedCave(Cave connectedCave)
        {
            _connectedCaves.Add(connectedCave);
            connectedCave._connectedCaves.Add(this);
        }

        public override string ToString() => $"{Name} (connected: {string.Join(",", ConnectedCaves.Select(x => x.Name))})";
    }
}
