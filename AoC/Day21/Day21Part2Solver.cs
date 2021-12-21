namespace AoC.Day21;

public static class Day21Part2Solver
{
    public static long SolvePart2(int p1Start, int p2Start)
    {
        const int goal = 21;
        var p1Wins = 0L;
        var p2Wins = 0L;

        IReadOnlyList<AggregateUniverse> aggregateUniverses = new[]
            {new AggregateUniverse(new GameState(new PlayerState(p1Start, 0), new PlayerState(p2Start, 0)), 1)};

        var player1Turn = true;
        var stepCount = 0;
        ////const int maxCount = 5;

        while (aggregateUniverses.Any())
        {
            var newUniverseAggregates = GetResultsOfTurn(aggregateUniverses, player1Turn);

            var newP1Wins = newUniverseAggregates.Where(x => x.GameState.Player1.Score >= goal).Sum(x => x.NumOfUniverses);
            var newP2Wins = newUniverseAggregates.Where(x => x.GameState.Player1.Score < goal && x.GameState.Player2.Score >= goal).Sum(x => x.NumOfUniverses);

            aggregateUniverses = newUniverseAggregates.Where(x => x.GameState.Player1.Score < goal && x.GameState.Player2.Score < goal).ToArray();

            player1Turn = !player1Turn;

            p1Wins += newP1Wins;
            p2Wins += newP2Wins;

            stepCount++;

            Console.WriteLine(new {stepCount, p1Wins, p2Wins, aggregateUniversesCount = aggregateUniverses.Count});

            //if (count <= maxCount)
            //{
            //	if (count == maxCount)
            //	{
            //		aggregateUniverses.Select(x => new
            //		{
            //			gameState = x.GameState.ToString(),
            //			x.NumOfUniverses
            //		}).Dump();
            //	}
            //	
            //	if (count == maxCount) break;
            //}		
        }

        //new {p1Wins, p2Wins}.Dump();

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

    public static IReadOnlyList<AggregateUniverse> GetResultsOfTurn(IEnumerable<AggregateUniverse> aggregateUniverses, bool player1Turn)
    {
        return aggregateUniverses
            .SelectMany(a => GetResultsOfTurn(a.GameState, player1Turn).Select(gameState => new {gameState, prevNumOfUniverses = a.NumOfUniverses}))
            .GroupBy(x => new
            {
                p1Pos = x.gameState.Player1.Position,
                p1Score = x.gameState.Player1.Score,
                p2Pos = x.gameState.Player2.Position,
                p2Score = x.gameState.Player2.Score,
                x.prevNumOfUniverses
            })
            .Select(grp => new AggregateUniverse(
                new GameState(
                    new PlayerState(grp.Key.p1Pos, grp.Key.p1Score),
                    new PlayerState(grp.Key.p2Pos, grp.Key.p2Score)),
                grp.Key.prevNumOfUniverses * grp.Count()))
            .ToArray();
    }

    public static readonly IReadOnlyList<int> DiracDiceNumbers = new[] {1, 2, 3};

    public static IEnumerable<GameState> GetResultsOfTurn(GameState gameState, bool player1Turn) =>
        DiracDiceNumbers.SelectMany(
            n1 => DiracDiceNumbers.SelectMany(
                n2 => DiracDiceNumbers.Select(n3 =>
                {
                    var (position, score) = player1Turn ? gameState.Player1 : gameState.Player2;

                    var amount = n1 + n2 + n3;
                    var newPosition = (position + amount) % 10;

                    if (newPosition == 0) newPosition = 10; // rs-todo: should be: newPosition == 0 ? 10 : newPosition;

                    var newPlayerState = new PlayerState(newPosition, score + newPosition);

                    var player1State = player1Turn ? newPlayerState : gameState.Player1;
                    var player2State = !player1Turn ? newPlayerState : gameState.Player2;

                    return new GameState(player1State, player2State);
                })));

    //public static long SolvePart2(int p1Start, int p2Start)
    //{
    //    const int goal = 21;
    //    var p1Wins = 0L;
    //    var p2Wins = 0L;

    //    var universeAggregates = new[] { new AggregateUniverse(p1Start, 0, p2Start, 0, 1) };

    //    while (universeAggregates.Any())
    //    {
    //        var newUniverseAggregates = ConsolidateAggregates(universeAggregates.SelectMany(GetResultsOfBothPlayerTurns));

    //        var p1HasWon = newUniverseAggregates.Where(x => x.P1Score >= goal).ToArray();
    //        var p2HasWon = newUniverseAggregates.Where(x => x.P2Score >= goal).Except(p1HasWon).ToArray();

    //        universeAggregates = newUniverseAggregates.Except(p2HasWon).Except(p2HasWon).ToArray();

    //        p1Wins += p1HasWon.Sum(x => x.NumOfUniverses);
    //        p2Wins += p2HasWon.Sum(x => x.NumOfUniverses);

    //        //// Filter out winners
    //        //var newP1Wins = newUniverseAggregates.Where(x => x.P1Score >= goal).Sum(x => x.NumOfUniverses);
    //        //var newP2Wins = newUniverseAggregates.Where(x => x.P1Score < goal && x.P2Score >= goal).Sum(x => x.NumOfUniverses);

    //        //universeAggregates = newUniverseAggregates.Where(x => x.P1Score < goal && x.P2Score < goal).ToArray();

    //        //var tot = newUniverseAggregates.Count(x => x.P1Score >= goal) +
    //        //          newUniverseAggregates.Count(x => x.P1Score < goal && x.P2Score >= goal) +
    //        //          universeAggregates.Length;

    //        //if (tot != newUniverseAggregates.Length)
    //        //{
    //        //    throw new InvalidOperationException("Filter is incorrect");
    //        //}

    //        //p1Wins += newP1Wins;
    //        //p2Wins += newP2Wins;
    //    }

    //    return Math.Max(p1Wins, p2Wins);
    //}

    //public record AggregateUniverse(int P1Position, int P1Score, int P2Position, int P2Score, long NumOfUniverses);

    //public readonly record struct TurnResult(int EndPos, int EndScore);

    //public static AggregateUniverse[] ConsolidateAggregates(IEnumerable<AggregateUniverse> aggregateUniverses)
    //{
    //    // return AggregateUniverse[]

    //    //aggregateUniverses.DistinctBy(a => new { a.P1Position, a.P1Score, a.P2Position, a.P2Score }).Count().Dump();

    //    //aggregateUniverses.GroupBy(a => new { a.P1Position, a.P1Score, a.P2Position, a.P2Score })
    //    //	.Select(grp => new AggregateUniverse(grp.Key.P1Position, grp.Key.P1Score, grp.Key.P2Position, grp.Key.P2Score, grp.Sum(x => x.NumOfUniverses)))
    //    //	.Dump();

    //    return aggregateUniverses.GroupBy(a => new { a.P1Position, a.P1Score, a.P2Position, a.P2Score })
    //        .Select(grp => new AggregateUniverse(grp.Key.P1Position, grp.Key.P1Score, grp.Key.P2Position, grp.Key.P2Score, grp.Sum(x => x.NumOfUniverses)))
    //        .ToArray();
    //}

    //public static AggregateUniverse[] GetResultsOfBothPlayerTurns(AggregateUniverse aggregateUniverse)
    //{
    //    return GetResultsOfTurn(aggregateUniverse.P1Position, aggregateUniverse.P1Score)
    //        .SelectMany(p1 => GetResultsOfTurn(aggregateUniverse.P2Position, aggregateUniverse.P2Score)
    //            .Select(p2 => new { p1Pos = p1.EndPos, p1Score = p1.EndScore, p2Pos = p2.EndPos, p2Score = p2.EndScore }))
    //        .GroupBy(x => new { x.p1Pos, x.p1Score, x.p2Pos, x.p2Score })
    //        .Select(grp => new { grp.Key.p1Pos, grp.Key.p1Score, grp.Key.p2Pos, grp.Key.p2Score, numNewUniverses = grp.Count() })
    //        .Select(a => new AggregateUniverse(a.p1Pos, a.p1Score, a.p2Pos, a.p2Score, aggregateUniverse.NumOfUniverses * a.numNewUniverses))
    //        .ToArray();
    //}

    //private static readonly int[] DiracDiceNumbers = {1, 2, 3};

    //public static IEnumerable<TurnResult> GetResultsOfTurn(int startPos, int startScore) =>
    //    DiracDiceNumbers.SelectMany(
    //        n1 => DiracDiceNumbers.SelectMany(
    //            n2 => DiracDiceNumbers.Select(n3 =>
    //            {
    //                var amount = n1 + n2 + n3;
    //                var newPosition = (startPos + amount) % 10;

    //                if (newPosition == 0) newPosition = 10; // rs-todo: should be: newPosition == 0 ? 10 : newPosition;

    //                return new TurnResult
    //                {
    //                    EndPos = newPosition,
    //                    EndScore = startScore + newPosition
    //                };
    //            })));

    //public static long SolvePart2(int p1Start, int p2Start)
    //{
    //    const int goal = 21;
    //    var p1Wins = 0L;
    //    var p2Wins = 0L;

    //    var universeAggregates = new[] { new AggregateUniverse(p1Start, 0, p2Start, 0, 1) };

    //    while (universeAggregates.Any())
    //    {
    //        var newUniverseAggregates = ConsolidateAggregates(universeAggregates.SelectMany(GetResultsOfBothPlayerTurns));

    //        // Filter out winners
    //        p1Wins += newUniverseAggregates.Where(x => x.P1Score >= goal).Sum(x => x.NumOfUniverses);
    //        p2Wins += newUniverseAggregates.Where(x => x.P2Score >= goal).Sum(x => x.NumOfUniverses);

    //        universeAggregates = newUniverseAggregates.Where(x => x.P1Score < goal && x.P2Score < goal).ToArray();
    //    }

    //    return Math.Max(p1Wins, p2Wins);
    //}

    //public record AggregateUniverse(int P1Position, int P1Score, int P2Position, int P2Score, long NumOfUniverses);

    //public readonly record struct TurnResult(int EndPos, int EndScore);

    //public static IReadOnlyList<AggregateUniverse> ConsolidateAggregates(IEnumerable<AggregateUniverse> aggregateUniverses)
    //{
    //    // return AggregateUniverse[]

    //    //aggregateUniverses.DistinctBy(a => new { a.P1Position, a.P1Score, a.P2Position, a.P2Score }).Count().Dump();

    //    return aggregateUniverses.GroupBy(a => new { a.P1Position, a.P1Score, a.P2Position, a.P2Score })
    //        .Select(grp => new AggregateUniverse(grp.Key.P1Position, grp.Key.P1Score, grp.Key.P2Position, grp.Key.P2Score, grp.Sum(x => x.NumOfUniverses)))
    //        .ToArray();
    //}

    //public static IReadOnlyList<AggregateUniverse> GetResultsOfBothPlayerTurns(AggregateUniverse aggregateUniverse)
    //{
    //    return GetResultsOfTurn(aggregateUniverse.P1Position, aggregateUniverse.P1Score)
    //        .SelectMany(p1 => GetResultsOfTurn(aggregateUniverse.P2Position, aggregateUniverse.P2Score)
    //            .Select(p2 => new {p1Pos = p1.EndPos, p1Score = p1.EndScore, p2Pos = p2.EndPos, p2Score = p2.EndScore}))
    //        .GroupBy(x => new {x.p1Pos, x.p1Score, x.p2Pos, x.p2Score})
    //        .Select(grp => new {grp.Key.p1Pos, grp.Key.p1Score, grp.Key.p2Pos, grp.Key.p2Score, numNewUniverses = grp.Count()})
    //        .Select(a => new AggregateUniverse(a.p1Pos, a.p1Score, a.p2Pos, a.p2Score, aggregateUniverse.NumOfUniverses * a.numNewUniverses))
    //        .ToArray();
    //}

    //private static readonly int[] DiracDiceNumbers = { 1, 2, 3 };

    //public static IEnumerable<TurnResult> GetResultsOfTurn(int startPos, int startScore) =>
    //    DiracDiceNumbers.SelectMany(
    //        n1 => DiracDiceNumbers.SelectMany(
    //            n2 => DiracDiceNumbers.Select(n3 =>
    //            {
    //                var amount = n1 + n2 + n3;
    //                var newPosition = (startPos + amount) % 10;

    //                if (newPosition == 0) newPosition = 10; // rs-todo: should be: newPosition == 0 ? 10 : newPosition;

    //                return new TurnResult
    //                {
    //                    EndPos = newPosition,
    //                    EndScore = startScore + newPosition
    //                };
    //            })));


    //--------------------------------------


    //public static long SolvePart2(int p1Start, int p2Start)
    //{
    //    const int goal = 21;
    //    var p1Wins = 0L;
    //    var p2Wins = 0L;

    //    var universeAggregates = new[] { new UniverseAggregate(p1Start, 0, p2Start, 0, 1) };

    //    while (universeAggregates.Any())
    //    {
    //        var newUniverseAggregates = GetResultUniverses(universeAggregates);

    //        // Filter out winners
    //        p1Wins += newUniverseAggregates.Where(x => x.P1Score >= goal).Sum(x => x.NumOfUniverses);
    //        p2Wins += newUniverseAggregates.Where(x => x.P2Score >= goal).Sum(x => x.NumOfUniverses);

    //        universeAggregates = newUniverseAggregates.Where(x => x.P1Score < goal && x.P2Score < goal).ToArray();
    //    }

    //    return Math.Max(p1Wins, p2Wins);
    //}

    //public record UniverseAggregate(int P1Position, int P1Score, int P2Position, int P2Score, long NumOfUniverses);

    ////public record Result(int p1EndPos, int p1EndScore, int p2EndPos, int p2EndScore, long startNumUniverses);

    //public record Result(int p1EndPos, int p1EndScore, int p2EndPos, int p2EndScore, long numUniverses);

    //private static readonly int[] nums = new[] { 1, 2, 3 };

    //public static IReadOnlyCollection<UniverseAggregate> GetResultUniverses(IReadOnlyCollection<UniverseAggregate> universeAggregates)
    //{
    //    return universeAggregates
    //        .SelectMany(a => GetResults(a.P1Position, a.P1Score, a.P2Position, a.P2Score, a.NumOfUniverses))
    //        .GroupBy(x => new { x.p1EndPos, x.p1EndScore, x.p2EndPos, x.p2EndScore, x.numUniverses })
    //        .Select(grp => new UniverseAggregate(grp.Key.p1EndPos, grp.Key.p1EndScore, grp.Key.p2EndPos, grp.Key.p2EndScore, grp.Sum(x => x.numUniverses) /*grp.Key.startNumUniverses + grp.Count()*/))
    //        .ToArray();
    //}

    //public static IEnumerable<Result> GetResults(int p1StartPos, int p1StartScore, int p2StartPos, int p2StartScore, long startNumUniverses)
    //{
    //    return nums.SelectMany(
    //        n1 => nums.SelectMany(
    //            n2 => nums.Select(n3 =>
    //            {
    //                var nsum = n1 + n2 + n3;

    //                var p1EndPos = (p1StartPos + nsum) % 10;
    //                p1EndPos = p1EndPos == 0 ? 10 : p1EndPos;

    //                var p2EndPos = (p2StartPos + nsum) % 10;
    //                p2EndPos = p2EndPos == 0 ? 10 : p2EndPos;

    //                var numUniverses = startNumUniverses * 27 * 27;

    //                return new Result
    //                (
    //                    p1EndPos,
    //                    p1StartScore + p1EndPos,
    //                    p2EndPos,
    //                    p2StartScore + p2EndPos,
    //                    numUniverses
    //                    //startNumUniverses,
    //                    //27 * 27
    //                );
    //            })));
    //}
}
