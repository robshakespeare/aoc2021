// .:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:.
// .                                                                                                     .
// .       *         __  __                         _____ _          _     _                       _     .
// .      /.\       |  \/  |                       / ____| |        (_)   | |                     | |    .
// .     /..'\      | \  / | ___ _ __ _ __ _   _  | |    | |__  _ __ _ ___| |_ _ __ ___   __ _ ___| |    .
// .     /'.'\      | |\/| |/ _ \ '__| '__| | | | | |    | '_ \| '__| / __| __| '_ ` _ \ / _` / __| |    .
// .    /.''.'\     | |  | |  __/ |  | |  | |_| | | |____| | | | |  | \__ \ |_| | | | | | (_| \__ \_|    .
// .    /.'.'.\     |_|  |_|\___|_|  |_|   \__, |  \_____|_| |_|_|  |_|___/\__|_| |_| |_|\__,_|___(_)    .
// .   /'.''.'.\                            __/ |                                                        .
// .   ^^^[_]^^^                           |___/                                                         .
// .                                                                                                     .
// .:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:._.:*~*:*~*:._.:*~*:.

namespace AoC.Day25;

public class Day25Solver : SolverBase<long, string>
{
    public override string DayName => "Sea Cucumbers";

    public override long SolvePart1(PuzzleInput input) => new Grid(input).StepUntilNoMovements();

    public override string SolvePart2(PuzzleInput input)
    {
        return @"Sleigh keys detected!
Energy source detected.
Integrating energy source from device ""sleigh keys""...done.
Installing device drivers...done.
Recalibrating experimental antenna...done.
Boost strength due to matching signal phase: 1 star

Day 25 Part 2 was free after having collected all previous 49 stars! :)

Advent of Code 2021 is complete.

Merry Christmas & Happy New Year!";
    }

    public class Grid
    {
        private readonly IReadOnlyList<StringBuilder> _grid;
        private readonly IReadOnlyList<SeaCucumber> _seaCucumbersEastFacing;
        private readonly IReadOnlyList<SeaCucumber> _seaCucumbersSouthFacing;

        public Grid(PuzzleInput input)
        {
            _grid = input.ReadLines().Select(line => new StringBuilder(line)).ToArray();

            var seaCucumbers = _grid.SelectMany((line, y) => line.ToString().Select((chr, x) => new {pos = new Vector2(x, y), chr}))
                .Where(p => p.chr != '.')
                .Select(p => p.chr switch
                {
                    '>' => new SeaCucumber(p.pos, GridUtils.East, p.chr), // east-facing
                    'v' => new SeaCucumber(p.pos, GridUtils.South, p.chr), // south-facing
                    _ => throw new InvalidOperationException("Invalid Sea Cucumber char: " + p.chr)
                })
                .ToArray();

            _seaCucumbersEastFacing = seaCucumbers.Where(x => x.IsFacingEast).ToArray();
            _seaCucumbersSouthFacing = seaCucumbers.Where(x => !x.IsFacingEast).ToArray();
        }

        public override string ToString() => string.Join(Environment.NewLine, _grid.Select(line => line.ToString()));

        public long StepUntilNoMovements()
        {
            var numOfMovementPerStep = new List<long>();
            long numOfMovementsThisStep;

            do
            {
                numOfMovementsThisStep = Step();
                numOfMovementPerStep.Add(numOfMovementsThisStep);
            } while (numOfMovementsThisStep > 0);

            return numOfMovementPerStep.Count;
        }

        public long Step() => MoveSeaCucumbers(_seaCucumbersEastFacing) + MoveSeaCucumbers(_seaCucumbersSouthFacing);

        private int MoveSeaCucumbers(IEnumerable<SeaCucumber> seaCucumbers)
        {
            var movements = new List<(SeaCucumber SeaCucumber, Vector2 NewPosition)>();

            foreach (var seaCucumber in seaCucumbers)
            {
                if (CanMove(seaCucumber, out var newPosition))
                {
                    movements.Add((seaCucumber, newPosition));
                }
            }

            foreach (var (seaCucumber, newPosition) in movements)
            {
                Move(seaCucumber, newPosition);
            }

            return movements.Count;
        }

        private bool CanMove(SeaCucumber seaCucumber, out Vector2 newPosition)
        {
            newPosition = seaCucumber.Position + seaCucumber.Direction;

            // Deal with overlap
            var grid = _grid;

            if (newPosition.Y >= grid.Count)
            {
                newPosition.Y = 0;
            }

            if (newPosition.X >= grid[(int)newPosition.Y].Length)
            {
                newPosition.X = 0;
            }

            // Check new position is free
            return this[newPosition] == '.';
        }

        private void Move(SeaCucumber seaCucumber, Vector2 newPosition)
        {
            this[seaCucumber.Position] = '.';
            this[newPosition] = seaCucumber.Chr;

            seaCucumber.Position = newPosition;
        }

        private char this[Vector2 position]
        {
            get => _grid[(int) position.Y][(int) position.X];
            set => _grid[(int) position.Y][(int) position.X] = value;
        }
    }

    public class SeaCucumber
    {
        public Vector2 Position { get; set; }
        public Vector2 Direction { get; }
        public char Chr { get; }
        public bool IsFacingEast => Direction == GridUtils.East;

        public SeaCucumber(Vector2 position, Vector2 direction, char chr)
        {
            Position = position;
            Direction = direction;
            Chr = chr;
        }
    }
}
