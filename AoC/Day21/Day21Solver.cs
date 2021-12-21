namespace AoC.Day21;

public class Day21Solver : SolverBase
{
    public override string DayName => "Dirac Dice";

    public override long? SolvePart1(PuzzleInput input)
    {
        var (p1Start, p2Start) = ParseStartingPositions(input);
        var die = new DeterministicDie();
        var game = new Game(p1Start, p2Start, 1000, die);
        var (_, losingPlayer) = game.Play();
        return losingPlayer.Score * game.NumberOfDieRolls;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var (p1Start, p2Start) = ParseStartingPositions(input);
        return new Day21Part2Solver().SolvePart2(p1Start, p2Start);
    }

    private static readonly Regex StartingPositionRegex = new(@"starting position: (?<start>\d)");

    public static (int p1Start, int p2Start) ParseStartingPositions(PuzzleInput input)
    {
        var starts = StartingPositionRegex.Matches(input.ToString()).Select(m => int.Parse(m.Groups["start"].Value)).ToArray();
        return (starts[0], starts[1]);
    }

    public interface IDie
    {
        int Roll();
    }

    public class DeterministicDie : IDie
    {
        private int _number;

        public int Roll()
        {
            _number++;
            _number %= 100;
            if (_number == 0) _number = 100;
            return _number;
        }
    }

    public class Game
    {
        private readonly IDie _die;
        private readonly Player _player1;
        private readonly Player _player2;

        public Game(int p1Start, int p2Start, int goal, IDie die)
        {
            _die = die;
            Goal = goal;
            _player1 = new Player(1, p1Start, 0);
            _player2 = new Player(2, p2Start, 0);
        }

        public int Goal { get; }

        public long NumberOfDieRolls { get; private set; }

        public (Player winningPlayer, Player losingPlayer) Play()
        {
            var nextPlayer = _player1;

            while (_player1.Score < Goal && _player2.Score < Goal)
            {
                MovePlayer(nextPlayer);
                nextPlayer = OppositePlayer(nextPlayer);
            }

            var winningPlayer = _player1.Score >= _player2.Score ? _player1 : _player2;
            var losingPlayer = OppositePlayer(winningPlayer);

            return (winningPlayer, losingPlayer);
        }

        private int RollDie()
        {
            NumberOfDieRolls++;
            return _die.Roll();
        }

        private Player OppositePlayer(Player player) => player == _player1 ? _player2 : _player1;

        private void MovePlayer(Player player)
        {
            var amount = RollDie() + RollDie() + RollDie();
            player.Move(amount);
        }
    }

    public class Player
    {
        public int Number { get; }
        public int Position { get; private set; }
        public long Score { get; private set; }

        public Player(int number, int position, long score)
        {
            Number = number;
            Position = position;
            Score = score;
        }

        public void Move(int amount)
        {
            var newPosition = (Position + amount) % 10;
            if (newPosition == 0) newPosition = 10;

            Position = newPosition;
            Score += newPosition;
        }
    }
}
