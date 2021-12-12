namespace AoC.Day12;

public class Day12Solver : SolverBase
{
    public override string DayName => "Passage Pathing";

    /// <summary>
    /// How many paths through this cave system are there that visit small caves at most once?
    /// </summary>
    public override long? SolvePart1(PuzzleInput input)
    {
        var caveSystem = CaveSystem.Parse(input);

        var currentPaths = new HashSet<Path>();
        var completePaths = new HashSet<Path>();

        currentPaths.Add(new Path(new[] {caveSystem.Start}));

        while (currentPaths.Any())
        {
            foreach (var currentPath in currentPaths.ToArray())
            {
                foreach (var connectedCave in currentPath.CurrentCave.ConnectedCaves)
                {
                    // all paths should visit small caves at most once, and can visit big caves any number of times
                    var visitCave = !connectedCave.IsSmall || !currentPath.SmallCaves.Contains(connectedCave);
                    if (visitCave)
                    {
                        var newPath = new Path(currentPath.Caves.Concat(new[] { connectedCave }).ToArray());
                        currentPaths.Add(newPath);

                        if (connectedCave.IsEnd)
                        {
                            completePaths.Add(newPath);
                        }
                    }
                }

                currentPaths.Remove(currentPath);
            }
        }

        return completePaths.Count;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class Path
    {
        public Path(IReadOnlyCollection<Cave> caves)
        {
            CurrentCave = caves.Last();
            Name = string.Join(" > ", caves.Select(x => x.Name));
            Caves = caves;
            SmallCaves = caves.Where(x => x.IsSmall).ToArray();
        }

        public string Name { get; }

        public IReadOnlyCollection<Cave> Caves { get; }

        public IReadOnlyCollection<Cave> SmallCaves { get; }

        public Cave CurrentCave { get; }

        public override string ToString() => Name;

        public override bool Equals(object? obj) => obj is Path other && Equals(other);

        protected bool Equals(Path other) => Name == other.Name;

        public override int GetHashCode() => Name.GetHashCode();
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

    public class Cave /*: IEquatable<Cave>*/
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

        //public override bool Equals(object? obj) => obj is Cave other && Equals(other);

        //public bool Equals(Cave? other) => other != null && Name == other.Name;

        //public override int GetHashCode() => Name.GetHashCode();

        //public static bool operator ==(Cave? left, Cave? right) => Equals(left, right);

        //public static bool operator !=(Cave? left, Cave? right) => !Equals(left, right);

        public override string ToString() => $"{Name} -> {string.Join(",", ConnectedCaves.Select(x => x.Name))}";
    }
}
