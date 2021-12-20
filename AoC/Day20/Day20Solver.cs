using System.Collections.Immutable;
using static System.Environment;

namespace AoC.Day20;

public class Day20Solver : SolverBase
{
    public override string DayName => "Trench Map";

    public const char LightPixel = '#';
    public const char DarkPixel = '.';

    /// <summary>
    /// Apply the image enhancement algorithm twice, how many pixels are lit in the resulting image?
    /// </summary>
    public override long? SolvePart1(PuzzleInput input)
    {
        var (imageEnhancer, litPixels) = ParseInput(input);

        var resultLitPixels = imageEnhancer.ApplyImageEnhancementAlgorithm(litPixels, 2);

        return resultLitPixels.Count;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        return null;
    }

    public static (ImageEnhancer imageEnhancer, IReadOnlySet<Vector2> litPixels) ParseInput(PuzzleInput input)
    {
        var sections = input.ToString().Split($"{NewLine}{NewLine}");

        var pixels = sections[1].Split(NewLine)
            .SelectMany((line, y) => line.Select((chr, x) => new { chr, pos = new Vector2(x, y) }))
            .ToArray();

        return (new ImageEnhancer(sections[0]), new HashSet<Vector2>(pixels.Where(x => x.chr == LightPixel).Select(x => x.pos)));
    }

    public class ImageEnhancer
    {
        public string EnhancementAlgorithm { get; }

        public ImageEnhancer(string enhancementAlgorithm) => EnhancementAlgorithm = enhancementAlgorithm;

        public IReadOnlySet<Vector2> ApplyImageEnhancementAlgorithm(IReadOnlySet<Vector2> litPixels, int numberOfTimes)
        {
            for (var i = 0; i < numberOfTimes; i++)
            {
                litPixels = ApplyImageEnhancementAlgorithm(litPixels);

                ////Console.WriteLine("After step " + i);
                ////var grid = litPixels.ToStringGrid(x => x, _ => LightPixel, DarkPixel);
                ////foreach (var line in grid)
                ////{
                ////    Console.WriteLine(line);
                ////}
                ////Console.WriteLine();
            }

            return litPixels;
        }

        public IReadOnlySet<Vector2> ApplyImageEnhancementAlgorithm(IReadOnlySet<Vector2> litPixels)
        {
            const int expand = 1;
            var bounds = CalculateBounds(litPixels);
            List<Vector2> newLitPixels = new();

            for (var y = bounds.Y.Min - expand; y <= bounds.Y.Max + expand; y++)
            {
                for (var x = bounds.X.Min - expand; x <= bounds.X.Max + expand; x++)
                {
                    var newPosition = new Vector2(x, y);

                    var imageEnhancementIndex = GetImageEnhancementIndex(litPixels, newPosition);

                    var isNewPixelLit = EnhancementAlgorithm[imageEnhancementIndex] == LightPixel;

                    if (isNewPixelLit)
                    {
                        newLitPixels.Add(newPosition);
                    }
                }
            }

            return newLitPixels.ToImmutableHashSet();
        }

        public static int GetImageEnhancementIndex(IReadOnlySet<Vector2> litPixels, Vector2 position)
        {
            var pixels = string.Join("", CenterAndDirections
                .Select(dir => position + dir)
                .Select(pos => litPixels.Contains(pos) ? LightPixel : DarkPixel));

            var binaryNumberString = string.Join("", pixels.Select(pixel => pixel == LightPixel ? '1' : '0'));

            return Convert.ToInt32(binaryNumberString, 2);
        }

        private static readonly IReadOnlyList<Vector2> CenterAndDirections =
            Enumerable.Range(-1, 3).SelectMany(y => Enumerable.Range(-1, 3).Select(x => new Vector2(x, y))).ToArray();

        private record Bounds2d((int Min, int Max) X, (int Min, int Max) Y);

        private static Bounds2d CalculateBounds(IEnumerable<Vector2> litPixels)
        {
            var xMin = 0;
            var xMax = 0;
            var yMin = 0;
            var yMax = 0;

            foreach (var pixel in litPixels)
            {
                xMin = Math.Min(xMin, (int)pixel.X);
                xMax = Math.Max(xMax, (int)pixel.X);

                yMin = Math.Min(yMin, (int)pixel.Y);
                yMax = Math.Max(yMax, (int)pixel.Y);
            }

            return new Bounds2d((xMin, xMax), (yMin, yMax));
        }
    }
}
