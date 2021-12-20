using static System.Environment;

namespace AoC.Day20;

public class Day20Solver : SolverBase
{
    public override string DayName => "Trench Map";

    /// <summary>
    /// Apply the image enhancement algorithm twice. How many pixels are lit in the resulting image?
    /// </summary>
    public override long? SolvePart1(PuzzleInput input) => Solve(input, 2);

    /// <summary>
    /// Apply the image enhancement algorithm 50 times. How many pixels are lit in the resulting image?
    /// </summary>
    public override long? SolvePart2(PuzzleInput input) => Solve(input, 50);

    private static long? Solve(PuzzleInput input, int numberOfSteps)
    {
        var (imageEnhancer, image) = ParseInput(input);

        var resultImage = imageEnhancer.ApplyImageEnhancementAlgorithm(image, numberOfSteps);

        return resultImage.Pixels.SelectMany(line => line).Count(x => x.IsLit);
    }

    public static (ImageEnhancer imageEnhancer, Image image) ParseInput(PuzzleInput input)
    {
        var sections = input.ToString().Split($"{NewLine}{NewLine}");

        var pixels = sections[1].Split(NewLine)
            .Select(line => line.Select(chr => new Pixel(chr)).ToArray())
            .ToArray();

        return (new ImageEnhancer(sections[0]), new Image(pixels, Pixel.DarkPixel));
    }

    public record Image(IReadOnlyList<IReadOnlyList<Pixel>> Pixels, Pixel InfinitePixel)
    {
        public Bounds2d Bounds { get; } = CalculateBounds(Pixels);

        public Pixel GetPixel(Vector2 position) => Bounds.Contains(position)
            ? Pixels[(int) position.Y][(int) position.X]
            : InfinitePixel;
    }

    public readonly record struct Pixel(char Char)
    {
        public bool IsLit => Char == LightPixel.Char;
        public override string ToString() => Char.ToString();

        public static readonly Pixel LightPixel = new('#');
        public static readonly Pixel DarkPixel = new('.');
    }

    public record Bounds2d((int Min, int Max) X, (int Min, int Max) Y)
    {
        public bool Contains(Vector2 position) => position.X >= X.Min && position.X < X.Max &&
                                                  position.Y >= Y.Min && position.Y < Y.Max;
    }

    public static Bounds2d CalculateBounds(IReadOnlyList<IReadOnlyList<Pixel>> pixels)
    {
        const int xMin = 0;
        var xMax = pixels[0].Count;
        const int yMin = 0;
        var yMax = pixels.Count;
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
                image = ApplyImageEnhancementAlgorithm(image);
            }

            return image;
        }

        private Image ApplyImageEnhancementAlgorithm(Image image)
        {
            const int expand = 1;
            var bounds = image.Bounds;

            List<IReadOnlyList<Pixel>> newPixels = new();

            for (var y = bounds.Y.Min - expand; y <= bounds.Y.Max + expand; y++)
            {
                var newLine = new List<Pixel>();
                newPixels.Add(newLine);

                for (var x = bounds.X.Min - expand; x <= bounds.X.Max + expand; x++)
                {
                    var newPosition = new Vector2(x, y);
                    newLine.Add(GetOutputPixel(image, newPosition));
                }
            }

            // The actual input has an ON for index 0, and OFF for index 511 (i.e. index pointed to when all 9 bits are on)
            // Meaning that for the actual input, a dark pixel surrounded by totally dark pixels would turn in to a lit pixel
            // So everything outside of the bounds is on after step 1, off after step 2, on after step 3, off after step 4, etc...
            // But the example input is the opposite has an OFF for index 0, and ON for index 511.
            var newInfinitePixel = GetOutputPixel(image, new Vector2(image.Bounds.X.Min - 100, image.Bounds.Y.Min - 100));

            return new Image(newPixels, newInfinitePixel);
        }

        public static int GetImageEnhancementIndex(Image image, Vector2 position)
        {
            var pixels = GridUtils.CenterAndDirectionsIncludingDiagonal.Select(dir => position + dir).Select(image.GetPixel);
            var binaryNumberString = string.Join("", pixels.Select(pixel => pixel.IsLit ? '1' : '0'));
            return Convert.ToInt32(binaryNumberString, 2);
        }

        public Pixel GetOutputPixel(Image image, Vector2 position) => new(EnhancementAlgorithm[GetImageEnhancementIndex(image, position)]);
    }
}
