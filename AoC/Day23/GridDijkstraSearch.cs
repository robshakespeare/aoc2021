namespace AoC.Day23;

public static class GridDijkstraSearch
{
    /// <summary>
    /// Finds the smallest cost to reach the goal, i.e. the desired end state of the grid.
    /// Written from the pseudocode at: https://cse442-17f.github.io/A-Star-Search-and-Dijkstras-Algorithm/
    /// </summary>
    public static (Grid Destination, long TotalCost) FindSmallestCostToGridGoal(Grid start)
    {
        var explore = new PriorityQueue<(Grid Grid, long TotalCost), long>();
        explore.Enqueue((start, 0), 0);

        var seen = new HashSet<Grid>();

        while (explore.Count > 0)
        {
            var (node, cost) = explore.Dequeue(); // this takes out the top priority node

            // if node is the goal return it and the total cost
            if (node.IsGoalReached)
            {
                return (node, cost);
            }

            // if we've not already seen the node
            if (!seen.Contains(node))
            {
                foreach (var (child, stepCost) in node.GetSuccessors())
                {
                    explore.Enqueue((child, stepCost + cost), stepCost + cost);
                }

                seen.Add(node);
            }
        }

        throw new InvalidOperationException("No paths found");
    }
}
