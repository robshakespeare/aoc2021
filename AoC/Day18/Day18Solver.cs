using Sprache;

namespace AoC.Day18;

public class Day18Solver : SolverBase
{
    public override string DayName => "Snailfish";

    public override long? SolvePart1(PuzzleInput input)
    {
        return null;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public interface IElement
    {
        
    }

    public readonly record struct RegularNumber(int Value) : IElement
    {
        public override string ToString() => Value.ToString();
    }

    public readonly record struct Pair(IElement Left, IElement Right) : IElement
    {
        public override string ToString() => $"[{Left},{Right}]";
    }

    public record SnailfishNumber(Pair Pair)
    {
        public override string ToString() => Pair.ToString();

        public static SnailfishNumber operator +(SnailfishNumber a, SnailfishNumber b)
        {
            var pair = new Pair(a.Pair, b.Pair);

            // rs-todo: reduce!!
            // If any pair is nested inside four pairs, the leftmost such pair explodes.
            // If any regular number is 10 or greater, the leftmost such regular number splits.


            return new SnailfishNumber(pair);
        }
    }

    public static class SnailfishNumberParser
    {
        public static SnailfishNumber ParseLine(string line) => new(PairElement.Parse(line));

        private static readonly Parser<IElement> RegularNumberElement =
            from number in Parse.Digit.AtLeastOnce().Text().Token()
            select new RegularNumber(int.Parse(number));

        private static Parser<Pair> PairElement =>
            from lp in Parse.Char('[').Token()
            from left in Element
            from sep in Parse.Char(',').Token()
            from right in Element
            from rp in Parse.Char(']').Token()
            select new Pair(left, right);

        private static Parser<IElement> Element =>
            PairElement.XOr(RegularNumberElement);
    }
}
