using Sprache;

namespace AoC.Day18;

/*
 * Trying to get next number to the left:
 *
 *
 */

public class Day18Solver : SolverBase
{
    public override string DayName => "Snailfish";

    public override long? SolvePart1(PuzzleInput input)
    {
        var snailfishNumbers = input.ReadLines().Select(SnailfishNumber.ParseLine).ToArray();

        var result = snailfishNumbers.Skip(1).Aggregate(snailfishNumbers.First(), (agg, cur) => agg + cur);

        return result.Magnitude;
    }

    /// <summary>
    /// What is the largest magnitude of any sum of two different snailfish numbers from the homework assignment?
    /// Note that snailfish addition is not commutative - that is, x + y and y + x can produce different results.
    /// </summary>
    public override long? SolvePart2(PuzzleInput input)
    {
        var snailfishNumbers = input.ReadLines().Select(SnailfishNumber.ParseLine).ToArray();

        var distinctABs = snailfishNumbers
            .SelectMany(a => snailfishNumbers
                .Select(b => new {a, b})
                .Where(x => x.a != x.b))
            .ToArray();

        return distinctABs.Max(x => Math.Max((x.a + x.b).Magnitude, (x.b + x.a).Magnitude));
    }

    public abstract class Element
    {
        public int Level { get; private set; }
        public Pair? Parent { get; private set; }
        ////public decimal SeqNum { get; private set; } = 100000;

        public virtual void SetParent(Pair parent)
        {
            Parent = parent;
            Level = parent.Level + 1;

            ////var isLeftChild = parent.Left == this;
        }

        public virtual Pair? GetFirstPairToExplode() => null;

        public abstract RegularNumber? GetFirstNumberToSplit();

        public abstract void CollectRegularNumbers(List<RegularNumber> regularNumbers);

        public abstract long Magnitude { get; }

        public abstract Element Clone();
    }

    public class RegularNumber : Element
    {
        public long Value { get; set; }

        public RegularNumber(long value) => Value = value;

        public override string ToString() => Value.ToString();

        public bool ShouldSplit => Value >= 10;

        public override RegularNumber? GetFirstNumberToSplit() => ShouldSplit ? this : null;

        public override void CollectRegularNumbers(List<RegularNumber> regularNumbers) => regularNumbers.Add(this);

        public override long Magnitude => Value;

        public override Element Clone() => new RegularNumber(Value);

        public void Split()
        {
            if (!ShouldSplit)
            {
                throw new InvalidOperationException("Not valid to split number " + Value);
            }

            var (left, right) = GetSplitParts();
            var newPair = new Pair(new RegularNumber(left), new RegularNumber(right));
            Parent?.ReplaceChild(this, newPair);
        }

        public (long left, long right) GetSplitParts() => (Value / 2, (long) Math.Ceiling(Value / 2m));
    }

    public class Pair : Element
    {
        public Element Left { get; private set; }
        public Element Right { get; private set; }

        public Pair(Element left, Element right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString() => $"[{Left},{Right}]";

        public override void SetParent(Pair parent)
        {
            base.SetParent(parent);

            Left.SetParent(this);
            Right.SetParent(this);
        }

        public override Pair? GetFirstPairToExplode() => Level == 4 ? this : Left.GetFirstPairToExplode() ?? Right.GetFirstPairToExplode();

        public override RegularNumber? GetFirstNumberToSplit() => Left.GetFirstNumberToSplit() ?? Right.GetFirstNumberToSplit();

        public override void CollectRegularNumbers(List<RegularNumber> regularNumbers)
        {
            Left.CollectRegularNumbers(regularNumbers);
            Right.CollectRegularNumbers(regularNumbers);
        }

        public void ReplaceChild(Element currentChild, Element newChild)
        {
            if (Left == currentChild)
            {
                Left = newChild;
                Left.SetParent(this);
            }
            else if (Right == currentChild)
            {
                Right = newChild;
                Right.SetParent(this);
            }
            else
            {
                throw new InvalidOperationException($"Current child {currentChild} not found in {this}");
            }
        }

        public override long Magnitude => 3 * Left.Magnitude + 2 * Right.Magnitude;

        public override Element Clone() => new Pair(Left.Clone(), Right.Clone());

        public void Explode(/*List<RegularNumber> regularNumbers*/)
        {
            // Exploding pairs will always consist of two regular numbers.
            if (Left is RegularNumber left && Right is RegularNumber right)
            {
                // the pair's left value is added to the first regular number to the left of the exploding pair (if any)
                // the pair's right value is added to the first regular number to the right of the exploding pair (if any)

                ////var leftIndex = regularNumbers.IndexOf(left) - 1;
                var firstLeft = GetNextRegularNumberToLeft(); ////regularNumbers.ElementAtOrDefault(leftIndex);
                if (firstLeft != null)
                {
                    firstLeft.Value += left.Value;
                }

                ////var rightIndex = regularNumbers.IndexOf(right) + 1;
                var firstRight = GetNextRegularNumberToRight(); ////regularNumbers.ElementAtOrDefault(rightIndex);
                if (firstRight != null)
                {
                    firstRight.Value += right.Value;
                }

                // Then, the entire exploding pair is replaced with the regular number 0.
                Parent?.ReplaceChild(this, new RegularNumber(0));
            }
            else
            {
                throw new InvalidOperationException($"Cannot explode {this} because it does not consist of two regular numbers");
            }
        }

        /// <summary>
        /// Returns the next regular number to the left of this pair, or null if there are no regular numbers to the left.
        /// </summary>
        public RegularNumber? GetNextRegularNumberToLeft()
        {
            //if (Parent == null)
            //{
            //    return null;
            //}

            Element? GoUpUntilGetALeft(Element self) =>
                self.Parent == null
                    ? null
                    : self.Parent.Right == self ? self.Parent.Left : GoUpUntilGetALeft(self.Parent);

            var childElement = GoUpUntilGetALeft(this); //Parent.Right == this ? Parent.Left : todoGoUpUntilGetALeft;

            while (childElement is Pair childPair)
            {
                childElement = childPair.Right;
            }

            return childElement is RegularNumber number ? number : null; //throw new InvalidOperationException("Failed to GetNextRegularNumberToLeft");

            //var isRightChildOfParent = ;
            //if (isRightChildOfParent)
            //{
                
            //}

            // if this pair is the right child of the parent pair, then from the keep going down to the bottom of the right of the left child
            //var isRightChildOfParent = Parent.Right == this;
            //if (isRightChildOfParent)
            //{
            //    var childElement = Parent.Left;

            //    while (childElement is Pair childPair)
            //    {
            //        childElement = childPair.Right;
            //    }

            //    return childElement is RegularNumber number ? number : throw new InvalidOperationException("Failed to traverse down");
            //}

            // Go up chain of parents, check each left for not being us, repeat until null or top
        }

        /// <summary>
        /// Returns the next regular number to the right of this pair, or null if there are no regular numbers to the right.
        /// </summary>
        public RegularNumber? GetNextRegularNumberToRight()
        {
            Element? GoUpUntilGetARight(Element self) =>
                self.Parent == null
                    ? null
                    : self.Parent.Left == self ? self.Parent.Right : GoUpUntilGetARight(self.Parent);

            var childElement = GoUpUntilGetARight(this);

            while (childElement is Pair childPair)
            {
                childElement = childPair.Left;
            }

            return childElement is RegularNumber number ? number : null;
        }
    }

    public record SnailfishNumber(Pair Pair)
    {
        public override string ToString() => Pair.ToString();

        public long Magnitude => Pair.Magnitude;

        public static SnailfishNumber operator +(SnailfishNumber a, SnailfishNumber b)
        {
            var snailfishNumber = new SnailfishNumber(new Pair(a.Pair.Clone(), b.Pair.Clone()));
            snailfishNumber.Reduce();
            return snailfishNumber;
        }

        public void Reduce()
        {
            // set the parents and levels
            Pair.Left.SetParent(Pair);
            Pair.Right.SetParent(Pair);

            bool actionOccurred;
            do
            {
                actionOccurred = false;

                // If any pair is nested inside four pairs, the leftmost such pair explodes.
                var explode = Pair.GetFirstPairToExplode();
                if (explode != null)
                {
                    // Visit all the regular numbers to build a list of them
                    //var regularNumbers = new List<RegularNumber>();
                    //Pair.CollectRegularNumbers(regularNumbers);

                    explode.Explode(/*regularNumbers*/);
                    actionOccurred = true;
                }
                else
                {
                    // If any regular number is 10 or greater, the leftmost such regular number splits.
                    var split = Pair.GetFirstNumberToSplit();
                    if (split != null)
                    {
                        split.Split();
                        actionOccurred = true;
                    }
                }
            } while (actionOccurred);
        }

        #region Parsing

        public static SnailfishNumber ParseLine(string line) => new(Parser.Parse(line));

        private static readonly Parser<Element> NumberElement =
            from number in Parse.Digit.AtLeastOnce().Text().Token()
            select new RegularNumber(long.Parse(number));

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
