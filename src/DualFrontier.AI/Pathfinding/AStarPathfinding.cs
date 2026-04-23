using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Math;

namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// Synchronous A* over NavGrid. No async — preserves ThreadLocal
/// isolation guard. Long paths are capped at MaxIterations;
/// caller retries next tick if TryFindPath returns false.
/// </summary>
public sealed class AStarPathfinding : IPathfindingService
{
    private const int MaxIterations = 2000;

    private readonly NavGrid _grid;

    public AStarPathfinding(NavGrid grid)
        => _grid = grid ?? throw new ArgumentNullException(nameof(grid));

    public bool TryFindPath(GridVector from, GridVector to,
                            out IReadOnlyList<GridVector> path)
    {
        path = new List<GridVector>();

        if (!_grid.IsPassable(to.X, to.Y))
            return false;

        // Note: PriorityQueue is assumed available (e.g., using a custom implementation or .NET 6+ feature)
        var open   = new PriorityQueue<GridVector, float>();
        var cameFrom = new Dictionary<GridVector, GridVector>();
        var gScore   = new Dictionary<GridVector, float>();

        gScore[from] = 0f;
        open.Enqueue(from, Heuristic(from, to));

        int iterations = 0;
        while (open.Count > 0 && iterations++ < MaxIterations)
        {
            var current = open.Dequeue();

            if (current.Equals(to))
            {
                ReconstructPath(cameFrom, current, path);
                return true;
            }

            foreach (var neighbor in Neighbors(current))
            {
                float tentative = gScore.GetValueOrDefault(current, float.MaxValue)
                                  + _grid.GetCost(neighbor.X, neighbor.Y);

                if (tentative < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor]   = tentative;
                    float f = tentative + Heuristic(neighbor, to);
                    open.Enqueue(neighbor, f);
                }
            }
        }

        return false;
    }

    private IEnumerable<GridVector> Neighbors(GridVector v)
    {
        int x = v.X, y = v.Y;
        if (_grid.IsPassable(x,   y-1)) yield return new GridVector(x,   y-1);
        if (_grid.IsPassable(x,   y+1)) yield return new GridVector(x,   y+1);
        if (_grid.IsPassable(x-1, y  )) yield return new GridVector(x-1, y  );
        if (_grid.IsPassable(x+1, y  )) yield return new GridVector(x+1, y  );
    }

    private static float Heuristic(GridVector a, GridVector b) =>
        Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    private static void ReconstructPath(
        Dictionary<GridVector, GridVector> cameFrom,
        GridVector current,
        List<GridVector> path)
    {
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
    }
}
