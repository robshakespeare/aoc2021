using static AoC.Day22.Day22Solver;

namespace AoC.Day22;

public class CubesUnion
{
    private readonly List<Cube> _cubes = new();

    public IReadOnlyList<Cube> Cubes => _cubes;

    public void Union(Cube cubeToAdd)
    {
        if (_cubes.Count == 0)
        {
            _cubes.Add(cubeToAdd);
        }
        else
        {
            IReadOnlyList<Cube> newCubes = new[] {cubeToAdd};

            foreach (var existingCube in _cubes)
            {
                var allExceptionCubes = new List<Cube>();
                foreach (var newCube in newCubes)
                {
                    var (_, exceptionCubes) = Cube.GetIntersectionAndExceptionCubes(existingCube, newCube);
                    allExceptionCubes.AddRange(exceptionCubes);
                }

                newCubes = allExceptionCubes;
            }

            _cubes.AddRange(newCubes);
        }
    }
}
