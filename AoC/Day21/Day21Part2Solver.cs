namespace AoC.Day21;

/// <summary>
/// This solution is not good, but it did the job. This is very slow, it takes about 1 minute 30 seconds to solve the problem.
/// Now having read up about other people's solutions, the game state approach is right, but its less about dealing with number of universes
/// and more about caching known results of game states. One to revisit later.
/// </summary>
public static class Day21Part2Solver
{
    public static long SolvePart2(int p1Start, int p2Start)
    {
        const int goal = 21;
        var p1Wins = 0L;
        var p2Wins = 0L;

        IReadOnlyList<AggregateUniverse> aggregateUniverses = new[]
        {
            new AggregateUniverse(
                new GameState(new PlayerState(p1Start, 0), new PlayerState(p2Start, 0)),
                NumOfUniverses: 1)
        };

        var isPlayer1Turn = true;
        var stepCount = 0;

        while (aggregateUniverses.Any())
        {
            var newUniverseAggregates = GetResultsOfTurn(aggregateUniverses, isPlayer1Turn);

            p1Wins += newUniverseAggregates.Where(x => x.GameState.Player1.Score >= goal).Sum(x => x.NumOfUniverses);
            p2Wins += newUniverseAggregates.Where(x => x.GameState.Player1.Score < goal && x.GameState.Player2.Score >= goal).Sum(x => x.NumOfUniverses);

            aggregateUniverses = newUniverseAggregates.Where(x => x.GameState.Player1.Score < goal && x.GameState.Player2.Score < goal).ToArray();

            isPlayer1Turn = !isPlayer1Turn;
            stepCount++;

            Console.WriteLine(new {stepCount, p1Wins, p2Wins, aggregateUniversesCount = aggregateUniverses.Count});
        }

        return Math.Max(p1Wins, p2Wins);
    }

    public record GameState(PlayerState Player1, PlayerState Player2)
    {
        public override string ToString() => $"P1: {Player1}; P2: {Player2}";
    }

    public readonly record struct PlayerState(int Position, int Score)
    {
        public override string ToString() => $"({Position}, {Score})";
    }

    public record AggregateUniverse(GameState GameState, long NumOfUniverses);

    public static IReadOnlyList<AggregateUniverse> GetResultsOfTurn(IEnumerable<AggregateUniverse> aggregateUniverses, bool isPlayer1Turn)
    {
        return aggregateUniverses
            .SelectMany(a => GetResultsOfTurn(a.GameState, isPlayer1Turn).Select(gameState => new {gameState, prevNumOfUniverses = a.NumOfUniverses}))
            .GroupBy(x => new {x.gameState, x.prevNumOfUniverses})
            .Select(grp => new AggregateUniverse(
                GameState: grp.Key.gameState,
                NumOfUniverses: grp.Key.prevNumOfUniverses * grp.Count()))
            .ToArray();
    }

    public static readonly IReadOnlyList<int> DiracDiceNumbers = new[] {1, 2, 3};

    public static IEnumerable<GameState> GetResultsOfTurn(GameState gameState, bool isPlayer1Turn) =>
        DiracDiceNumbers.SelectMany(
            n1 => DiracDiceNumbers.SelectMany(
                n2 => DiracDiceNumbers.Select(n3 =>
                {
                    var (position, score) = isPlayer1Turn ? gameState.Player1 : gameState.Player2;

                    var amount = n1 + n2 + n3;
                    var newPosition = (position + amount) % 10;
                    newPosition = newPosition == 0 ? 10 : newPosition;

                    var newPlayerState = new PlayerState(
                        Position: newPosition,
                        Score: score + newPosition);

                    return new GameState(
                        Player1: isPlayer1Turn ? newPlayerState : gameState.Player1,
                        Player2: !isPlayer1Turn ? newPlayerState : gameState.Player2);
                })));
}
