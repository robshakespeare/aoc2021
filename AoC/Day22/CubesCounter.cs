using static AoC.Day22.Day22Solver;

namespace AoC.Day22;

public class CubesCounter
{
    //private readonly List<Region> _onRegions = new();

    //public IReadOnlyList<Region> OnRegions => _onRegions;

    //public long CountOfCubes { get; private set; }

    private readonly List<Region> _addedRegions = new();

    private readonly List<Region> _subtractedRegions = new();

    public long CountOfCubes => _addedRegions.Sum(x => x.AreaInclusive) - _subtractedRegions.Sum(x => x.AreaInclusive);

    public void RunRebootSteps(IEnumerable<RebootStep> rebootSteps)
    {
        foreach (var rebootStep in rebootSteps)
        {
            if (rebootStep.TurnOn)
            {
                TurnOnCubes(rebootStep.Region);
            }
            else
            {
                TurnOffCubes(rebootStep.Region);
            }

            Console.WriteLine("[NCC] countOfCubesOnAfterStep: " + CountOfCubes);
        }
    }

    private static IReadOnlyList<Region> GetIntersections(Region region, IEnumerable<Region> otherRegions)
    {
        IEnumerable<Region> GetIntersectionsUncleaned()
        {
            return
                from otherRegion in otherRegions
                where otherRegion != region
                select Region.GetIntersection(region, otherRegion) into result
                where result.intersectionArea > 0
                select result.intersection;
        }

        return Clean(GetIntersectionsUncleaned());
    }

    private static IReadOnlyList<Region> Clean(IEnumerable<Region> regions)
    {
        regions = regions.Distinct().ToArray();

        // Remove any region totally contained in another
        var toRemove = new List<Region>();
        foreach (var intersection in regions)
        {
            toRemove.AddRange(regions.Where(x => x != intersection).Where(inner => intersection.Contains(inner)));
        }

        return regions.Except(toRemove).ToArray();
    }

    /// <summary>
    /// Turns ON the cubes within the specified bounds
    /// </summary>
    public void TurnOnCubes(Region region)
    {
        // Keep the count of the cubes that are ON, and deal with not double counting
        if (_addedRegions.Count == 0)
        {
            _addedRegions.Add(region); // Initially the number of cubes that are ON is just the whole area of the region
        }
        else
        {
            // At each point "clean" the list of intersections, i.e. remove duplicates, and then remove any that are totally contained by another
            Add(new [] { region }, _addedRegions);
        }
    }

    private void Add(IReadOnlyList<Region> addRegions, IReadOnlyList<Region>? compareTo = null)
    {
        // Add/include method, that includes one or more regions
        // But there might be some overlaps that need excluding (because they are already added)

        var regionsToExclude = new List<Region>();

        foreach (var addRegion in addRegions)
        {
            regionsToExclude.AddRange(GetIntersections(addRegion, compareTo ?? addRegions));
        }

        _addedRegions.AddRange(addRegions);

        Subtract(Clean(regionsToExclude));
    }

    private void Subtract(IReadOnlyList<Region> subtractRegions)
    {
        // Subtract/exclude method, that excludes one or more regions
        // But there might be some overlaps that need including

        var regionsToInclude = new List<Region>();

        foreach (var subtractRegion in subtractRegions)
        {
            regionsToInclude.AddRange(GetIntersections(subtractRegion, subtractRegions));
        }

        _subtractedRegions.AddRange(subtractRegions);

        //_addedRegions.AddRange(Clean(regionsToInclude));
        Add(Clean(regionsToInclude));
        //Add();
    }

    ///// <summary>
    ///// Turns ON the cubes within the specified bounds
    ///// </summary>
    //public void TurnOnCubes(Region region)
    //{
    //    // Keep the count of the cubes that are ON, and deal with not double counting
    //    if (_onRegions.Count == 0)
    //    {
    //        _onRegions.Add(region);
    //        CountOfCubes = region.AreaInclusive; // Initially the number of cubes that are ON is just the whole area of the region
    //    }
    //    else
    //    {
    //        var adjustments = new List<long>();

    //        // Add the region's area (the number of cubes that are ON)
    //        //CountOfCubes += region.AreaInclusive;
    //        adjustments.Add(region.AreaInclusive);

    //        // Now, deal with any intersections with existing regions, to stop the duplicate counting
    //        // var intersections = GetIntersections(region).Select((intersection, index) => new {id = index, intersection}).ToArray(); // do we need the ID?

    //        var intersections = GetIntersections(region, _onRegions).Distinct().ToArray();

    //        //// Remove any intersections totally contained in another
    //        //var toRem = new List<Region>();
    //        //foreach (var intersection in intersections)
    //        //{
    //        //    foreach (var inner in intersections.Where(x => x != intersection))
    //        //    {
    //        //        if (intersection.Contains(inner))
    //        //        {
    //        //            toRem.Add(inner);
    //        //        }
    //        //    }
    //        //}

    //        //intersections = intersections.Except(toRem).ToArray();

    //        static IReadOnlyList<Region> AdjustForOverlaps(IReadOnlyList<Region> intersections, int adjustmentDirection, List<long> adjustments)
    //        {
    //            var overlappingIntersections = new List<Region>();

    //            foreach (var intersection in intersections)
    //            {
    //                //CountOfCubes -= intersection.AreaInclusive;
    //                adjustments.Add(adjustmentDirection * intersection.AreaInclusive);

    //                overlappingIntersections.AddRange(GetIntersections(intersection, intersections));
    //            }

    //            return overlappingIntersections;
    //        }

    //        AdjustForOverlaps(AdjustForOverlaps(AdjustForOverlaps(intersections, -1, adjustments), 1, adjustments), -1, adjustments);

    //        ////var overlappingIntersections = new List<Region>();

    //        ////foreach (var intersection in intersections)
    //        ////{
    //        ////    //CountOfCubes -= intersection.AreaInclusive;
    //        ////    adjustments.Add(-intersection.AreaInclusive);

    //        ////    overlappingIntersections.AddRange(GetIntersections(intersection, intersections));
    //        ////}

    //        ////var overlappingIntersections2 = new List<Region>();

    //        ////foreach (var intersection in overlappingIntersections)
    //        ////{
    //        ////    //CountOfCubes -= intersection.AreaInclusive;
    //        ////    adjustments.Add(intersection.AreaInclusive);

    //        ////    overlappingIntersections2.AddRange(GetIntersections(intersection, intersections));
    //        ////}

    //        //foreach (var groupOfOverlappingIntersections in overlappingIntersections.GroupBy(x => x))
    //        //{
    //        //    //CountOfCubes += groupOfOverlappingIntersections.Key.AreaInclusive * (groupOfOverlappingIntersections.Count() - 1);
    //        //    adjustments.Add(groupOfOverlappingIntersections.Key.AreaInclusive * (groupOfOverlappingIntersections.Count() - 1));
    //        //}

    //        var totalAdjustments = adjustments.Sum();

    //        CountOfCubes += totalAdjustments;
    //        _onRegions.Add(region);
    //    }
    //}

    /// <summary>
    /// Turns OFF the cubes within the specified bounds.
    /// </summary>
    public void TurnOffCubes(Region region)
    {
        //// Turn OFF any cubes that is on, by reducing the count of the cubes that are ON, and deal with not double subtracting
        //if (_onRegions.Count == 0)
        //{
        //    // If we have no regions, our count will be zero, so there's no to turn off
        //}
        //else
        //{
        //    // rs-todo!!
        //    //// Add the region's area (the number of cubes that are ON)
        //    //CountOfCubes += region.AreaInclusive;

        //    //// Now, deal with any intersections with existing regions, to stop the duplicate counting
        //    //// var intersections = GetIntersections(region).Select((intersection, index) => new {id = index, intersection}).ToArray(); // do we need the ID?

        //    //var intersections = GetIntersections(region, _onRegions).ToArray();

        //    //var overlappingIntersections = new List<Region>();

        //    //foreach (var intersection in intersections)
        //    //{
        //    //    CountOfCubes -= intersection.AreaInclusive;

        //    //    overlappingIntersections.AddRange(GetIntersections(intersection, intersections));
        //    //}

        //    //foreach (var groupOfOverlappingIntersections in overlappingIntersections.GroupBy(x => x))
        //    //{
        //    //    CountOfCubes += groupOfOverlappingIntersections.Key.AreaInclusive * (groupOfOverlappingIntersections.Count() - 1);
        //    //}

        //    //_onRegions.Add(region);
        //}
    }
}
