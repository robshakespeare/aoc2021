using static AoC.Day22.Day22Solver;

namespace AoC.Day22;

public class CubesUnion
{
    private readonly List<Region> _cubes = new();

    public IReadOnlyList<Region> Cubes => _cubes;

    public void Union(Region regionToAdd)
    {
        if (_cubes.Count == 0)
        {
            _cubes.Add(regionToAdd);
        }
        else
        {
            IReadOnlyList<Region> newCubes = new[] {regionToAdd};

            foreach (var existingCube in _cubes)
            {
                var allExceptionCubes = new List<Region>();
                foreach (var newCube in newCubes)
                {
                    var (_, exceptionCubes) = Region.GetIntersectionAndExceptionCubes(existingCube, newCube);
                    allExceptionCubes.AddRange(exceptionCubes);
                }

                newCubes = allExceptionCubes;
            }

            _cubes.AddRange(newCubes);
        }
    }
}
