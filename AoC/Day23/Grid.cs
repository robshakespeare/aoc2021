using MoreLinq;

namespace AoC.Day23;

public class Grid
{
    private readonly IReadOnlyList<string> _grid;
    private readonly Template _template;

    public const int HallY = 1;

    public const int AHomeX = 3;
    public const int BHomeX = 5;
    public const int CHomeX = 7;
    public const int DHomeX = 9;

    public Grid(IReadOnlyList<string> grid, Template template)
    {
        _grid = grid;
        _template = template;
    }

    public string GridToString() => string.Join(Environment.NewLine, _grid);

    public void WriteToConsole() => _grid.ForEach(line => Console.WriteLine(line));

    public static Grid Parse(PuzzleInput input, bool insertAdditionalLines)
    {
        var lines = input.ReadLines().ToList();

        if (insertAdditionalLines)
        {
            lines.InsertRange(3, new[]
            {
                "  #D#C#B#A#",
                "  #D#B#A#C#"
            });
        }

        // Build a template, which is all the possible places where an Amphipod can move
        var template = lines
            .SelectMany((line, y) => line.Select((chr, x) => chr switch
            {
                'A' or 'B' or 'C' or 'D' => new { chr = '.', x, y },
                _ => new { chr, x, y }
            }))
            .Where(p => p.chr == '.')
            .Select(p => new Vector2(p.x, p.y))
            .ToArray();

        Home GetHome(char amphipod) => new(template.Where(pos => (int) pos.X == GetAmphipodHomeX(amphipod) && !IsInHall(pos)).ToArray(), amphipod);

        var homeA = GetHome('A');
        var homeB = GetHome('B');
        var homeC = GetHome('C');
        var homeD = GetHome('D');

        var hall = template.Where(IsInHall).ToArray();
        var hallExceptOutsideRoom = hall.Where(pos => !IsOutsideRoom(pos)).ToArray();

        return new Grid(
            grid: lines.Select(line => line).ToArray(),
            new Template(template, homeA, homeB, homeC, homeD, hall, hallExceptOutsideRoom));
    }

    public record Template(
        IReadOnlyList<Vector2> PossiblePlaces,
        Home HomeA,
        Home HomeB,
        Home HomeC,
        Home HomeD,
        IReadOnlyList<Vector2> Hall,
        IReadOnlyList<Vector2> HallExceptOutsideRoom);

    public record Home(IReadOnlyList<Vector2> Positions, char DestAmphipod);

    /// <summary>
    /// Returns true if the specified amphipod can move in to the specified home.
    /// Returns true if the specified home is the destination of the specified amphipod, and
    /// the specified home is either empty or contains just amphipods in their destination.
    /// </summary>
    public bool IsHomeOkToMoveTo(Home home, Amphipod amphipod) =>
        home.DestAmphipod == amphipod.Chr &&
        home.Positions.Select(GetChar).All(chr => chr == '.' || chr == home.DestAmphipod);

    /// <summary>
    /// Returns the home that contains the specified position, if any.
    /// </summary>
    public Home? GetHomeContainingPosition(Vector2 position) => IsInHall(position)
        ? null
        : (int) position.X switch
        {
            AHomeX => _template.HomeA,
            BHomeX => _template.HomeB,
            CHomeX => _template.HomeC,
            DHomeX => _template.HomeD,
            _ => null
        };

    /// <summary>
    /// Returns the specified Amphipod's home.
    /// </summary>
    public Home GetHomeOfAmphipod(Amphipod amphipod) => amphipod.Chr switch
    {
        'A' => _template.HomeA,
        'B' => _template.HomeB,
        'C' => _template.HomeC,
        'D' => _template.HomeD,
        _ => throw new InvalidOperationException($"Unexpected amphipod {amphipod}")
    };

    /// <summary>
    /// Returns true if the specified Amphipod is in its destination home
    /// AND it doesn't need to move anywhere because its in the bottom block of its home shared with its own kind.
    /// </summary>
    public bool IsAmphipodInFinalDestination(Amphipod amphipod) //char amphipod, Vector2 amphipodPosition)
    {
        // Get the home associated with the Amphipod's X position, if any
        //var home = (int) amphipodPosition.X switch
        //{
        //    AHomeX => _template.HomeA,
        //    BHomeX => _template.HomeB,
        //    CHomeX => _template.HomeC,
        //    DHomeX => _template.HomeD,
        //    _ => null
        //};

        var (amphipodChar, amphipodPosition) = amphipod;
        var home = GetHomeContainingPosition(amphipodPosition);

        if (home?.DestAmphipod == amphipodChar)
        {
            // return true if it doesn't need to move anywhere because its in the bottom block of its home shared with its own kind
            var bottomChar = GetChar(home.Positions[^1]);
            return bottomChar == amphipodChar &&
                   home.Positions
                       .Reverse()
                       .Select(pos => (pos, chr: GetChar(pos)))
                       .TakeWhile(x => x.chr == amphipodChar)
                       .Select(x => x.pos)
                       .Contains(amphipodPosition);
        }

        return false;
    }

    public static bool IsAmphipod(char chr) => chr is 'A' or 'B' or 'C' or 'D';

    /// <summary>
    /// Returns true if the specified position is within the hall.
    /// Position is already expected to be in the bounds, i.e. within the template. This is NOT validated by this method to keep it quick.
    /// </summary>
    public static bool IsInHall(Vector2 pos) => (int) pos.Y == HallY;

    /// <summary>
    /// Returns true if the specified position is within a room.
    /// Remember, cannot finish the whole movement of an Amphipod outside of a room.
    /// Position is already expected to be in the bounds, i.e. within the template. This is NOT validated by this method to keep it quick.
    /// </summary>
    public static bool IsInRoom(Vector2 pos) => (int) pos.X is AHomeX or BHomeX or CHomeX or DHomeX && !IsInHall(pos);

    /// <summary>
    /// Returns true if the specified position is in the hall outside of a room.
    /// Position is already expected to be in the bounds, i.e. within the template. This is NOT validated by this method to keep it quick.
    /// </summary>
    public static bool IsOutsideRoom(Vector2 pos) => (int)pos.X is AHomeX or BHomeX or CHomeX or DHomeX && IsInHall(pos);

    public static long GetCostPerSpaceMoved(Amphipod amphipod) => amphipod.Chr switch
    {
        'A' => 1,
        'B' => 10,
        'C' => 100,
        'D' => 1000,
        _ => throw new InvalidOperationException($"Unexpected amphipod {amphipod}")
    };

    /// <summary>
    /// Returns the X coordinate of the specified Amphipod's home.
    /// </summary>
    public static int GetAmphipodHomeX(char amphipod) => amphipod switch
    {
        'A' => AHomeX,
        'B' => BHomeX,
        'C' => CHomeX,
        'D' => DHomeX,
        _ => throw new InvalidOperationException($"Unexpected amphipod {amphipod}")
    };

    private char GetChar(Vector2 pos) => _grid[(int) pos.Y][(int) pos.X];

    public readonly record struct Amphipod(char Chr, Vector2 Position);

    //public readonly record struct AmphipodMovement(Vector2 AmphipodPos, char Amphipod, Vector2 Dir, char NextChr);

    public readonly record struct AmphipodMovement(Amphipod Amphipod, Vector2 Destination);

    public IEnumerable<AmphipodMovement> GetNextAmphipodMovements() => _template.PossiblePlaces
        .Select(pos => (pos, chr: GetChar(pos)))
        .Where(x => IsAmphipod(x.chr))
        .Select(x => new Amphipod(x.chr, x.pos))
        .Where(a => !IsAmphipodInFinalDestination(a))
        .SelectMany(amphipod =>
        {
            if (IsInRoom(amphipod.Position))
            {
                return GetMovementsForAmphipodInRoom(amphipod);
            }

            if (IsInHall(amphipod.Position))
            {
                return GetMovementsForAmphipodInHall(amphipod);
            }

            throw new InvalidOperationException($"Unexpected state: {amphipod} is neither in room nor hall!");
        });
        //.SelectMany(amphipod => GetAdjacent(GridUtils.DirectionsExcludingDiagonal, amphipod.Position)
        //    .Select(a => new AmphipodMovement(amphipod.Position, amphipod.Chr, a.Dir, GetChar(a.Pos)))
        //    .Where(a => a.NextChr == '.'));

    // rs-todo: instead of GetAdjacent, can be more clever, because knowing that final destination ones are excluded, amphipod can either move up out of room, or back down out of hall, and also only move ones whose path is not obstructed
    //private static IEnumerable<(Vector2 Pos, Vector2 Dir)> GetAdjacent(IEnumerable<Vector2> directions, Vector2 position) =>
    //    directions.Select(dir => (position + dir, dir));

    /// <summary>
    /// Should only be called for Amphipods known to be in a room that is not their final destination.
    /// Returns all valid movements that the Amphipod can make, note there may be none.
    /// Amphipod can only move up out of room, and go left or right, not stay outside the room.
    /// </summary>
    private IEnumerable<AmphipodMovement> GetMovementsForAmphipodInRoom(Amphipod amphipod)
    {
        // Amphipods in a room must first move up, if that's valid, they can then move to every valid position in the hall.
        if (IsValidMove(amphipod, Axis.Y, HallY, out var midPoint))
        {
            return _template.HallExceptOutsideRoom
                .Where(hallPos => IsValidMove(midPoint, Axis.X, (int) hallPos.X, out _))
                .Select(hallPos => new AmphipodMovement(amphipod, hallPos));
        }

        return Array.Empty<AmphipodMovement>();
    }

    /// <summary>
    /// Should only be called for Amphipods known to be in the hall.
    /// Returns all valid movements that the Amphipod can make, note there may be none.
    /// Amphipods can only move from hall in to their own room.
    /// Amphipods can only move from hall in to their own room if its empty/only occupied by their type.
    /// </summary>
    private IEnumerable<AmphipodMovement> GetMovementsForAmphipodInHall(Amphipod amphipod)
    {
        if (IsValidMove(amphipod, Axis.X, GetAmphipodHomeX(amphipod.Chr), out var midPoint))
        {
            var home = GetHomeOfAmphipod(amphipod);
            if (IsHomeOkToMoveTo(home, amphipod))
            {
                // Get "lowest" space in the room that isn't occupied
                var targetInRoom = home.Positions.Last(pos => GetChar(pos) == '.');

                if (IsValidMove(midPoint, Axis.Y, (int) targetInRoom.Y, out _))
                {
                    yield return new AmphipodMovement(amphipod, targetInRoom);
                }
            }
        }
    }

    private enum Axis
    {
        X,
        Y
    };

    private bool IsValidMove(Amphipod amphipod, Axis axis, int target, out Vector2 targetPos) =>
        IsValidMove(amphipod.Position, axis, target, out targetPos);

    private bool IsValidMove(Vector2 amphipodPosition, Axis axis, int target, out Vector2 targetPos)
    {
        targetPos = axis switch
        {
            Axis.X => new Vector2(target, amphipodPosition.Y),
            Axis.Y => new Vector2(amphipodPosition.X, target),
            _ => throw new InvalidOperationException("Invalid axis: " + axis)
        };

        var startPos = amphipodPosition;
        if (targetPos == startPos)
        {
            return false; // This is not a valid movement if we are staying still
        }

        var movement = targetPos - startPos;
        var dir = Vector2.Normalize(movement);

        //var otherAxis = axis switch
        //{
        //    Axis.X => Axis.Y,
        //    Axis.Y => Axis.X,
        //    _ => throw new InvalidOperationException("Invalid axis: " + axis)
        //};
        //var other = start[otherAxis];
        //var startC = start[axis];
        //var endC = end[axis];
        //var delta = endC - startC;
        //if (delta == 0)
        //{
        //    return;
        //}
        //var dir = delta / Math.Abs(delta);

        var pos = startPos;
        while (pos != targetPos)
        {
            pos += dir;

            // If the new position is not a space (i.e. its occupied), this is not a valid movement
            if (GetChar(pos) != '.')
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns all next possible moves as a new Grid, with an associated cost of the move.
    /// </summary>
    public void GetSuccessors()
    {
        // Note that GetNextAmphipodMovements only returns Amphipods that can move,
        // and could return the same Amphipod more than once, but each direction is exclusive per Amphipod.

        // For each AmphipodMovement, it could move to various places

        // For any Amphipod is in its home, and their home only contains just their type, then that Amphipod shouldn't be moved

        // If the Amphipod that can move is not in its room, but is in a room,
        // then it can move up and then from that positions, it can move to any of the available positions in the hall.
        // i.e. must never stop whole movement outside of a room.

        // Once in a hall, Amphipods can only move to their home, and only if their home is empty or contains just their type.
    }
}
