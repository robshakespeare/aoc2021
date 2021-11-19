using System.Numerics;
using FluentAssertions.Equivalency;

namespace AoC.Tests;

public class MathUtilsTests
{
    private static EquivalencyAssertionOptions<T> WithStrictOrdering<T>(EquivalencyAssertionOptions<T> options) => options.WithStrictOrdering();

    public class TheLeastCommonMultipleMethod
    {
        [Test]
        public void LeastCommonMultiple_With2Arguments_ReturnsExpectedResult()
        {
            MathUtils.LeastCommonMultiple(28, 63).Should().Be(252);
        }

        [Test]
        public void LeastCommonMultiple_With3Arguments_ReturnsExpectedResult()
        {
            MathUtils.LeastCommonMultiple(12, 15, 18).Should().Be(180);
        }
    }

    public class TheAngleBetweenMethod
    {
        [TestCase(0, 1, 0)]
        [TestCase(1, 1, 315)]
        [TestCase(1, 0, 270)]
        [TestCase(1, -1, 225)]
        [TestCase(0, -1, 180)]
        [TestCase(-1, -1, 135)]
        [TestCase(-1, 0, 90)]
        [TestCase(-1, 1, 45)]
        public void AngleBetween_Tests(int x, int y, int expectedAngle)
        {
            var angles = Enumerable
                .Range(1, 5)
                .Select(n => new Vector2(x * n * 100, y * n * 100))
                .Select(v =>
                {
                    Console.WriteLine(v);
                    return MathUtils.AngleBetween(Vector2.UnitY, v);
                })
                .ToArray();

            angles.Should().AllBeEquivalentTo(expectedAngle);
        }
    }

    public class TheRotateDirectionMethod
    {
        [Test]
        public void Rotate_East_90DegreesToRight_ShouldBe_South()
        {
            var input = MathUtils.East;

            // ACT
            var result = MathUtils.RotateDirection(input, 90);

            // ASSERT
            result.Should().Be(MathUtils.South);
        }

        [Test]
        public void Rotate_East_360DegreesToRight_ShouldBe_Still_East()
        {
            var input = MathUtils.East;

            // ACT
            var result = MathUtils.RotateDirection(input, 360);

            // ASSERT
            result.Should().Be(MathUtils.East);
        }

        [Test]
        public void Rotate_East_90DegreesToLeft_ShouldBe_North()
        {
            var input = MathUtils.East;

            // ACT
            var result = MathUtils.RotateDirection(input, -90);

            // ASSERT
            result.Should().Be(MathUtils.North);
        }

        [Test]
        public void Rotate_North_180DegreesToLeft_ShouldBe_South()
        {
            var input = MathUtils.North;

            // ACT
            var result = MathUtils.RotateDirection(input, -180);

            // ASSERT
            result.Should().Be(MathUtils.South);
        }

        [Test]
        public void Rotate_North_180DegreesToRight_ShouldBe_South()
        {
            var input = MathUtils.North;

            // ACT
            var result = MathUtils.RotateDirection(input, 180);

            // ASSERT
            result.Should().Be(MathUtils.South);
        }

        [Test]
        public void Rotate_South_270DegreesToRight_ShouldBe_East()
        {
            var input = MathUtils.South;

            // ACT
            var result = MathUtils.RotateDirection(input, 270);

            // ASSERT
            result.Should().Be(MathUtils.East);
        }

        [Test]
        public void Rotate_South_270DegreesToLeft_ShouldBe_West()
        {
            var input = MathUtils.South;

            // ACT
            var result = MathUtils.RotateDirection(input, -270);

            // ASSERT
            result.Should().Be(MathUtils.West);
        }

        [Test]
        public void Rotate_Waypoint_Day12_Example1_Test()
        {
            var input = new Vector2(10, -4);

            // ACT
            var result = MathUtils.RotateDirection(input, 90);

            // ASSERT
            result.Should().Be(new Vector2(4, 10));
        }
    }

    public class TheRoundMethod
    {
        [TestCase(1.0f, 1)]
        [TestCase(2.0f, 2)]
        [TestCase(-1.0f, -1)]
        [TestCase(3.25f, 3)]
        [TestCase(3.75f, 4)]
        [TestCase(3.5f, 4)]
        [TestCase(4.5f, 5)]
        [TestCase(1234.00001f, 1234)]
        [TestCase(1234.49f, 1234)]
        [TestCase(1234.5f, 1235)]
        [TestCase(1234.9999f, 1235)]
        [TestCase(9345678.75f, 9345679)]
        public void RoundTests(float f, int expectedResult)
        {
            f.Round().Should().Be(expectedResult);
        }
    }

    public class ManhattanDistanceTests
    {
        [TestCase(0, 0, 0, 0, 0)]
        [TestCase(0, 0, 0, 1, 1)]
        [TestCase(0, 0, 0, -1, 1)]
        [TestCase(0, 0, 2, 3, 5)]
        [TestCase(0, 0, -2, -3, 5)]
        [TestCase(1, 6, -1, 5, 3)]
        [TestCase(2, 3, -1, 5, 5)]
        [TestCase(2, 3, 1, 6, 4)]
        [TestCase(2, 3, 3, 5, 3)]
        public void ManhattanDistance_Tests(int x1, int y1, int x2, int y2, int expectedResult)
        {
            var vectorA = new Vector2(x1, y1);
            var vectorB = new Vector2(x2, y2);

            // ACT
            var result = MathUtils.ManhattanDistance(vectorA, vectorB);

            // ASSERT
            result.Should().Be(expectedResult);
        }

        [Test]
        public void ManhattanDistance_DoesRound_BeforeCalculating()
        {
            var vectorA = new Vector2(1.2f, 1.4f);
            var vectorB = new Vector2(3.5f, 4.5f);

            // ACT
            var result = MathUtils.ManhattanDistance(vectorA, vectorB);

            // ASSERT
            result.Should().Be((4 - 1) + (5 - 1));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 1, 1)]
        [TestCase(0, -1, 1)]
        [TestCase(2, 3, 5)]
        [TestCase(-2, -3, 5)]
        [TestCase(10, 16, 26)]
        [TestCase(-10, 16, 26)]
        [TestCase(10, -16, 26)]
        [TestCase(-10, -16, 26)]
        public void ManhattanDistanceFromZero_Tests(int x, int y, int expectedResult)
        {
            var vector = new Vector2(x, y);

            // ACT
            var result = vector.ManhattanDistanceFromZero();

            // ASSERT
            result.Should().Be(expectedResult);
        }
    }

    public class TheRotateGridMethod
    {
        [Test]
        public void RotateGrid_SimpleTest()
        {
            var input = new[]
            {
                "12",
                "34"
            };

            // ACT
            var result = MathUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "31",
                    "42"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_0deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = MathUtils.RotateGrid(input, 0);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "123",
                    "456",
                    "789"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_90deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = MathUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "741",
                    "852",
                    "963"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_180deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = MathUtils.RotateGrid(input, 180);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "987",
                    "654",
                    "321"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_3x3_270deg()
        {
            var input = new[]
            {
                "123",
                "456",
                "789"
            };

            // ACT
            var result = MathUtils.RotateGrid(input, 270);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "369",
                    "258",
                    "147"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_4x4_90deg()
        {
            var input = new[]
            {
                "   •",
                "  • ",
                " •• ",
                "••••"
            };

            // ACT
            var result = MathUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "•   ",
                    "••  ",
                    "••• ",
                    "•  •"
                },
                WithStrictOrdering);
        }

        [Test]
        public void RotateGrid_SeaMonster_90deg()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = MathUtils.RotateGrid(input, 90);

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    " # ",
                    "#  ",
                    "   ",
                    "   ",
                    "#  ",
                    " # ",
                    " # ",
                    "#  ",
                    "   ",
                    "   ",
                    "#  ",
                    " # ",
                    " # ",
                    "#  ",
                    "   ",
                    "   ",
                    "#  ",
                    " # ",
                    " ##",
                    " # "
                },
                WithStrictOrdering);
        }
    }

    public class TheScaleGridMethod
    {
        [Test]
        public void ScaleGrid_SimpleTest()
        {
            var input = new[]
            {
                "12",
                "34"
            };

            // ACT
            var result = MathUtils.ScaleGrid(input, new Vector2(-1, 1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "21",
                    "43"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_NoScaleTest()
        {
            var input = new[]
            {
                "12",
                "34"
            };

            // ACT
            var result = MathUtils.ScaleGrid(input, new Vector2(1, 1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "12",
                    "34"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_SeaMonster_FlipY()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = MathUtils.ScaleGrid(input, new Vector2(1, -1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    " #  #  #  #  #  #   ",
                    "#    ##    ##    ###",
                    "                  # "
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_SeaMonster_FlipX()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = MathUtils.ScaleGrid(input, new Vector2(-1, 1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    " #                  ",
                    "###    ##    ##    #",
                    "   #  #  #  #  #  # "
                },
                WithStrictOrdering);
        }

        [Test]
        public void ScaleGrid_SeaMonster_FlipXAndY()
        {
            var input = new[]
            {
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };

            // ACT
            var result = MathUtils.ScaleGrid(input, new Vector2(-1, -1));

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "   #  #  #  #  #  # ",
                    "###    ##    ##    #",
                    " #                  "
                },
                WithStrictOrdering);
        }
    }

    public class ToStringGridMethod
    {
        [Test]
        public void ToStringGrid_DoesTranslateGridThatStartsWithNegativeBounds()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(-1, -1), 'A'),
                (new Vector2(1, 1), 'B')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, ' ');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "A  ",
                    "   ",
                    "  B"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesTranslateGridThatStartsWithPositiveBounds()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(10, 10), 'A'),
                (new Vector2(12, 12), 'B')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, ' ');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "A  ",
                    "   ",
                    "  B"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesIgnorePreviousDuplicateItemsInTheSamePosition()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(-1, -1), 'A'),
                (new Vector2(1, 1), 'B'),
                (new Vector2(1, 1), 'C')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, ' ');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "A  ",
                    "   ",
                    "  C"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesCreateLargeGrid()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(1, 1), '0'),
                (new Vector2(1, 2), '1'),
                (new Vector2(5, 5), '5')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, '-');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "0----",
                    "1----",
                    "-----",
                    "-----",
                    "----5"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesCreateRow()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(0, 0), '0'),
                (new Vector2(1, 0), '1'),
                (new Vector2(5, 0), '5')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, '-');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "01---5"
                },
                WithStrictOrdering);
        }

        [Test]
        public void ToStringGrid_DoesCreateColumn()
        {
            (Vector2 pos, char chr)[] input =
            {
                (new Vector2(0, 0), 'a'),
                (new Vector2(0, 1), 'b'),
                (new Vector2(0, 5), 'c')
            };

            // ACT
            var result = input.ToStringGrid(x => x.pos, x => x.chr, '#');

            // ASSERT
            result.Should().BeEquivalentTo(
                new[]
                {
                    "a",
                    "b",
                    "#",
                    "#",
                    "#",
                    "c"
                },
                WithStrictOrdering);
        }
    }
}
