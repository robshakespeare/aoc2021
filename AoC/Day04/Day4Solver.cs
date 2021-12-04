using static System.Environment;

namespace AoC.Day04;

public class Day4Solver : SolverBase
{
    public override string DayName => "Giant Squid";

    public override long? SolvePart1(PuzzleInput input)
    {
        var bingoSubsystem = BingoSubsystem.Parse(input);
        var winningBoard = bingoSubsystem.PlayUntilWinningBoard();
        var sumOfAllUnmarkedNumbers = winningBoard.Cells.Where(cell => !cell.IsMarked).Sum(cell => cell.Number);

        return sumOfAllUnmarkedNumbers * bingoSubsystem.LastNumber;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public class BingoSubsystem
    {
        private readonly Queue<long> _drawNumbers;
        private readonly List<long> _playedNumbers = new();
        private readonly List<BingoBoard> _winningBoards = new();

        public BingoBoard[] Boards { get; }
        public long LastNumber => _playedNumbers.Any() ? _playedNumbers.Last() : -1;

        public BingoSubsystem(IEnumerable<long> drawNumbers, BingoBoard[] boards)
        {
            Boards = boards;
            _drawNumbers = new Queue<long>(drawNumbers);
        }

        public BingoBoard PlayUntilWinningBoard()
        {
            while (!_winningBoards.Any() && _drawNumbers.Any())
            {
                Update();
            }

            if (_winningBoards.Any())
            {
                return _winningBoards.First();
            }

            throw new InvalidOperationException("No winning board after playing all numbers");
        }

        public void Update()
        {
            if (_drawNumbers.Any())
            {
                var number = _drawNumbers.Dequeue();

                foreach (var board in Boards)
                {
                    board.MarkNumber(number);

                    if (board.IsComplete && !_winningBoards.Contains(board))
                    {
                        _winningBoards.Add(board);
                    }
                }

                _playedNumbers.Add(number);
            }
        }

        public static BingoSubsystem Parse(PuzzleInput input)
        {
            var firstLine = input.ReadLines().First();
            var drawNumbers = firstLine.Split(',').Select(long.Parse);

            var grids = string.Join(NewLine, input.ReadLines().Skip(1)).Trim().Split($"{NewLine}{NewLine}");
            var boards = grids
                .Select(grid => grid
                    .Split(NewLine)
                    .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(numStr => new BingoCell(long.Parse(numStr))).ToArray()).ToArray())
                .Select(bingoGrid => new BingoBoard(bingoGrid))
                .ToArray();

            return new BingoSubsystem(drawNumbers, boards);
        }
    }

    public class BingoBoard
    {
        private readonly BingoLine[] _rows;
        private readonly BingoLine[] _columns;
        private readonly Dictionary<long, BingoCell> _cellsByNumber;
        private BingoLine[] _completeRows = Array.Empty<BingoLine>();
        private BingoLine[] _completeColumns = Array.Empty<BingoLine>();

        public bool IsComplete => _completeRows.Any() || _completeColumns.Any();
        public IEnumerable<BingoCell> Cells => _cellsByNumber.Values;

        public BingoBoard(BingoCell[][] bingoGrid)
        {
            _rows = bingoGrid.Select(row => new BingoLine(row)).ToArray();
            _columns = BuildColumns(_rows).ToArray();
            _cellsByNumber = bingoGrid.SelectMany(row => row).ToDictionary(cell => cell.Number);
        }

        private static IEnumerable<BingoLine> BuildColumns(BingoLine[] rows)
        {
            var width = rows.First().Cells.Length;
            for (var col = 0; col < width; col++)
            {
                yield return new BingoLine(rows.Select(row => row.Cells[col]).ToArray());
            }
        }

        public void MarkNumber(long number)
        {
            if (_cellsByNumber.TryGetValue(number, out var cell))
            {
                cell.IsMarked = true;
            }

            _completeRows = _rows.Where(row => row.IsComplete).ToArray();
            _completeColumns = _columns.Where(column => column.IsComplete).ToArray();
        }
    }

    public class BingoLine
    {
        public BingoCell[] Cells { get; }
        public bool IsComplete => Cells.All(cell => cell.IsMarked);

        public BingoLine(BingoCell[] cells) => Cells = cells;

        public override string ToString() => string.Join("", Cells.Select(x => x.Number.ToString().PadLeft(3)));
    }

    public class BingoCell
    {
        public long Number { get; }
        public bool IsMarked { get; set; }

        public BingoCell(long number) => Number = number;

        public override string ToString() => $"{Number}: {(IsMarked ? "marked" : "unmarked")}";
    }
}
