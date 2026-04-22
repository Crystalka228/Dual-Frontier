using System.Collections.Generic;
using DualFrontier.Contracts.Math;

namespace DualFrontier.AI.Pathfinding;

public sealed class AStarPathfinding : IPathfindingService
{
    private readonly NavGrid _grid;
    private const int MaxIterationsPerTick = 500;

    private static readonly GridVector[] Directions =
    {
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    };

    public AStarPathfinding(NavGrid grid)
    {
        _grid = grid ?? throw new System.ArgumentNullException(nameof(grid));
    }

    public bool TryFindPath(GridVector from, GridVector to, out IReadOnlyList<GridVector> path)
    {
        if (!_grid.IsInBounds(from.X, from.Y) || !_grid.IsPassable(from.X, from.Y) ||
            !_grid.IsInBounds(to.X, to.Y) || !_grid.IsPassable(to.X, to.Y))
        {
            path = System.Array.Empty<GridVector>();
            return false;
        }

        if (from == to)
        {
            path = new[] { from };
            return true;
        }

        var openSet = new MinHeap();
        var cameFrom = new Dictionary<GridVector, GridVector>();
        var gScore = new Dictionary<GridVector, int> { [from] = 0 };

        openSet.Push(from.ManhattanDistance(to), from);

        int iterations = 0;

        while (!openSet.IsEmpty)
        {
            if (iterations++ >= MaxIterationsPerTick)
            {
                path = System.Array.Empty<GridVector>();
                return false;
            }

            var (_, current) = openSet.Pop();

            if (current == to)
            {
                path = ReconstructPath(cameFrom, current);
                return true;
            }

            int currentG = gScore.TryGetValue(current, out int cg) ? cg : int.MaxValue;

            foreach (var dir in Directions)
            {
                var neighbor = new GridVector(current.X + dir.X, current.Y + dir.Y);

                if (!_grid.IsInBounds(neighbor.X, neighbor.Y) || !_grid.IsPassable(neighbor.X, neighbor.Y))
                    continue;

                int tentativeG = currentG + _grid.GetCost(neighbor.X, neighbor.Y);

                if (gScore.TryGetValue(neighbor, out int existingG) && tentativeG >= existingG)
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;

                int f = tentativeG + neighbor.ManhattanDistance(to);
                openSet.Push(f, neighbor);
            }
        }

        path = System.Array.Empty<GridVector>();
        return false;
    }

    private static IReadOnlyList<GridVector> ReconstructPath(Dictionary<GridVector, GridVector> cameFrom, GridVector current)
    {
        var result = new List<GridVector> { current };
        while (cameFrom.TryGetValue(current, out var prev))
        {
            result.Add(prev);
            current = prev;
        }
        result.Reverse();
        return result;
    }

    private sealed class MinHeap
    {
        private readonly List<(int Priority, GridVector Node)> _items = new();

        public int Count => _items.Count;
        public bool IsEmpty => _items.Count == 0;

        public void Push(int priority, GridVector node)
        {
            _items.Add((priority, node));
            BubbleUp(_items.Count - 1);
        }

        public (int Priority, GridVector Node) Pop()
        {
            var root = _items[0];
            int last = _items.Count - 1;
            _items[0] = _items[last];
            _items.RemoveAt(last);
            if (_items.Count > 0)
                SinkDown(0);
            return root;
        }

        private void BubbleUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_items[parent].Priority <= _items[index].Priority)
                    break;
                (_items[parent], _items[index]) = (_items[index], _items[parent]);
                index = parent;
            }
        }

        private void SinkDown(int index)
        {
            int count = _items.Count;
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                int smallest = index;

                if (left < count && _items[left].Priority < _items[smallest].Priority)
                    smallest = left;
                if (right < count && _items[right].Priority < _items[smallest].Priority)
                    smallest = right;

                if (smallest == index)
                    break;

                (_items[smallest], _items[index]) = (_items[index], _items[smallest]);
                index = smallest;
            }
        }
    }
}
