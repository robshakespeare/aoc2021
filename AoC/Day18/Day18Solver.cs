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

    public abstract record Element
    {
        public int Level { get; private set; }
        public Element? Parent { get; private set; }

        public virtual void SetParent(Element parent)
        {
            Parent = parent;
            Level = parent.Level + 1;
        }

        public virtual Pair? GetFirstPairToExplode() => null;

        public abstract RegularNumber? GetFirstNumberToSplit();
    }

    public record RegularNumber(int Value) : Element
    {
        public override string ToString() => Value.ToString();

        public override RegularNumber? GetFirstNumberToSplit() => Value >= 10 ? this : null;
    }

    public record Pair(Element Left, Element Right) : Element
    {
        public override string ToString() => $"[{Left},{Right}]";

        public override void SetParent(Element parent)
        {
            base.SetParent(parent);

            Left.SetParent(this);
            Right.SetParent(this);
        }

        public override Pair? GetFirstPairToExplode() => Level == 4 ? this : Left.GetFirstPairToExplode() ?? Right.GetFirstPairToExplode();

        public override RegularNumber? GetFirstNumberToSplit() => Left.GetFirstNumberToSplit() ?? Right.GetFirstNumberToSplit();
    }

    public record SnailfishNumber(Pair Pair)
    {
        public override string ToString() => Pair.ToString();

        public static SnailfishNumber operator +(SnailfishNumber a, SnailfishNumber b)
        {
            var snailfishNumber = ParseLine(new Pair(a.Pair, b.Pair).ToString()); // This is a simple way to clone the whole tree so we don't mutate the arguments
            snailfishNumber.Reduce();
            return snailfishNumber;
        }

        public void Reduce()
        {
            var actionOccurred = false;
            do
            {
                // If any pair is nested inside four pairs, the leftmost such pair explodes.
                var explode = Pair.GetFirstPairToExplode();
                if (explode != null)
                {
                    // rs-todo: explode!!
                    // rs-todo: actionOccurred = true;
                }
                else
                {
                    // If any regular number is 10 or greater, the leftmost such regular number splits.
                    var split = Pair.GetFirstNumberToSplit();
                    if (split != null)
                    {
                        // rs-todo: split!!
                        // rs-todo: actionOccurred = true;
                    }
                }
            } while (actionOccurred);
        }

        #region Parsing

        public static SnailfishNumber ParseLine(string line)
        {
            var snailfishNumber = new SnailfishNumber(Parser.Parse(line));

            // set the parents and levels
            var topPair = snailfishNumber.Pair;
            topPair.Left.SetParent(topPair);
            topPair.Right.SetParent(topPair);

            return snailfishNumber;
        }

        private static readonly Parser<Element> NumberElement =
            from number in Parse.Digit.AtLeastOnce().Text().Token()
            select new RegularNumber(int.Parse(number));

        private static Parser<Pair> PairElement =>
            from lp in Parse.Char('[').Token()
            from left in PairOrNumberElement
            from sep in Parse.Char(',').Token()
            from right in PairOrNumberElement
            from rp in Parse.Char(']').Token()
            select new Pair(left, right);

        private static Parser<Element> PairOrNumberElement =>
            PairElement.XOr(NumberElement);

        private static readonly Parser<Pair> Parser = PairElement.End();

        #endregion
    }
}
