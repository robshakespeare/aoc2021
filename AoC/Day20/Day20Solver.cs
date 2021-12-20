using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
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
        var (imageEnhancer, image) = ParseInput(input);

        var resultImage = imageEnhancer.ApplyImageEnhancementAlgorithm(image, 2);

        return resultImage.LitPixelPositions.Count;
    }

    public override long? SolvePart2(PuzzleInput input)
    {
        var (imageEnhancer, image) = ParseInput(input);

        var resultImage = imageEnhancer.ApplyImageEnhancementAlgorithm(image, 50);

        return resultImage.LitPixelPositions.Count;
    }

    public static (ImageEnhancer imageEnhancer, Image image) ParseInput(PuzzleInput input)
    {
        var sections = input.ToString().Split($"{NewLine}{NewLine}");

        var pixels = sections[1].Split(NewLine)
            .SelectMany((line, y) => line.Select((chr, x) => new {chr, pos = new Vector2(x, y)}))
            .ToArray();

        return (
            new ImageEnhancer(sections[0]),
            new Image(new HashSet<Vector2>(pixels.Where(x => x.chr == LightPixel).Select(x => x.pos)), DarkPixel)
        );
    }

    public record Image(IReadOnlySet<Vector2> LitPixelPositions, char InfinitePixel)
    {
        public Bounds2d Bounds { get; } = CalculateBounds(LitPixelPositions);

        public char GetPixel(Vector2 position) =>
            LitPixelPositions.Contains(position)
                ? LightPixel
                : Bounds.Contains(position)
                    ? DarkPixel
                    : InfinitePixel;
    }

    public record Bounds2d((int Min, int Max) X, (int Min, int Max) Y)
    {
        public bool Contains(Vector2 position) =>
            position.X >= X.Min && position.X <= X.Max &&
            position.Y >= Y.Min && position.Y <= Y.Max;
    }

    public static Bounds2d CalculateBounds(IEnumerable<Vector2> litPixels)
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

    public class ImageEnhancer
    {
        public string EnhancementAlgorithm { get; }

        public ImageEnhancer(string enhancementAlgorithm) => EnhancementAlgorithm = enhancementAlgorithm;

        public Image ApplyImageEnhancementAlgorithm(Image image, int numberOfSteps)
        {
            for (var step = 0; step < numberOfSteps; step++)
            {
                image = ApplyImageEnhancementAlgorithmStep(image, step);

                ////Console.WriteLine("After step " + step);
                ////var grid = image.LitPixelPositions.ToStringGrid(x => x, _ => LightPixel, DarkPixel);
                ////foreach (var line in grid)
                ////{
                ////    Console.WriteLine(line);
                ////}
                ////Console.WriteLine();
            }

            return image;
        }

        private Image ApplyImageEnhancementAlgorithmStep(Image image, int step)
        {
            // The actual input has an ON for index 0, and OFF for index 511 (i.e. index pointed to when all 9 bits are on)
            // Meaning that for the actual input, a dark pixel surrounded by totally dark pixels would turn in to a lit pixel
            // So everything outside of the bounds is on after step 1, off after step 2, on after step 3, off after step 4, etc...
            // But the example input is the opposite has an OFF for index 0, and ON for index 511.

            const int expand = 1;
            var bounds = image.Bounds;
            List<Vector2> newLitPixels = new();

            for (var y = bounds.Y.Min - expand; y <= bounds.Y.Max + expand; y++)
            {
                for (var x = bounds.X.Min - expand; x <= bounds.X.Max + expand; x++)
                {
                    var newPosition = new Vector2(x, y);

                    var isNewPixelLit = ShouldLightOutputPixel(image, newPosition);

                    //var imageEnhancementIndex = GetImageEnhancementIndex(image, newPosition);

                    //var isNewPixelLit = EnhancementAlgorithm[imageEnhancementIndex] == LightPixel;

                    if (isNewPixelLit)
                    {
                        newLitPixels.Add(newPosition);
                    }
                }
            }

            var isNewInfinitePixelLit = ShouldLightOutputPixel(image, new Vector2(image.Bounds.X.Min - 100, image.Bounds.Y.Min - 100));

            return new Image(newLitPixels.ToImmutableHashSet(), isNewInfinitePixelLit ? LightPixel : DarkPixel);
        }

        public static int GetImageEnhancementIndex(Image image, Vector2 position)
        {
            //image.Bounds.Contains(position)

            var pixels = string.Join("", CenterAndDirections
                .Select(dir => position + dir)
                .Select(image.GetPixel));

            //var pixels = string.Join("", CenterAndDirections
            //    .Select(dir => position + dir)
            //    .Select(pos => image.LitPixelPositions.Contains(pos)
            //        ? LightPixel
            //        : image.Bounds.Contains(position)
            //            ? DarkPixel
            //            : image.InfinitePixel));

            var binaryNumberString = string.Join("", pixels.Select(pixel => pixel == LightPixel ? '1' : '0'));

            return Convert.ToInt32(binaryNumberString, 2);
        }

        // rs-todo: needed??
        public bool ShouldLightOutputPixel(Image image, Vector2 position) =>
            EnhancementAlgorithm[GetImageEnhancementIndex(image, position)] == LightPixel;

        public static readonly IReadOnlyList<Vector2> CenterAndDirections =
            Enumerable.Range(-1, 3).SelectMany(y => Enumerable.Range(-1, 3).Select(x => new Vector2(x, y))).ToArray();
    }
}
