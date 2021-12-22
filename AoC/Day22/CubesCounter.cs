using static AoC.Day22.Day22Solver;

namespace AoC.Day22;

public class CubesCounter
{
    private readonly List<Region> _onRegions = new();

    public IReadOnlyList<Region> OnRegions => _onRegions;

    public long CountOfCubes { get; private set; }

    /// <summary>
    /// Adds the cubes within the specified bounds, treating each as ON and keeping the count, and deals with not double counting.
    /// </summary>
    public void AddCubes(Region region)
    {
        if (_onRegions.Count == 0)
        {
            _onRegions.Add(region);
            CountOfCubes = region.AreaInclusive; // Initially the number of cubes that are ON is just the whole area of the region
        }
        else
        {
            // Add the region's area (the number of cubes that are ON)
            CountOfCubes += region.AreaInclusive;

            // Now, deal with any intersections with existing regions, to stop the duplicate counting
            // var intersections = GetIntersections(region).Select((intersection, index) => new {id = index, intersection}).ToArray(); // do we need the ID?

            var intersections = GetIntersections(region, _onRegions).ToArray();

            var overlappingIntersections = new List<Region>();

            foreach (var intersection in intersections)
            {
                CountOfCubes -= intersection.AreaInclusive;

                overlappingIntersections.AddRange(GetIntersections(intersection, intersections));
            }

            foreach (var groupOfOverlappingIntersections in overlappingIntersections.GroupBy(x => x))
            {
                CountOfCubes += groupOfOverlappingIntersections.Key.AreaInclusive * (groupOfOverlappingIntersections.Count() - 1);
            }

            _onRegions.Add(region);
        }
    }

    private static IEnumerable<Region> GetIntersections(Region region, IEnumerable<Region> otherRegions) // rs-todo: move to region class
    {
        return
            from otherRegion in otherRegions
            where otherRegion != region
            select Region.GetIntersection(region, otherRegion) into result
            where result.intersectionArea > 0
            select result.intersection;
    }
}
