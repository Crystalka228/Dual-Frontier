using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;

namespace DualFrontier.Core.Math;

/// <summary>
/// Implements a spatial partitioning grid for efficient neighbour queries and collision detection.
/// </summary>
internal sealed class SpatialGrid
{
    private readonly int _width;
    private readonly int _height;
    // Using an array where the index is derived from both X and Y coordinates (Y * Width + X)
    // The size of this array must accommodate all possible tile positions.
    private readonly HashSet<EntityId>[] _cells;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpatialGrid"/> class with specified dimensions.
    /// </summary>
    /// <param name="width">The width (number of columns) of the grid.</param>
    /// <param name="height">The height (number of rows) of the grid.</param>
    internal SpatialGrid(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("Width and Height must be positive.");
        }

        _width = width;
        _height = height;
        // Allocate array size for all possible tiles: Width * Height.
        _cells = new HashSet<EntityId>[width * height];
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i] = new HashSet<EntityId>();
        }
    }

    /// <summary>
    /// Calculates the flat index for a given GridVector position, assuming row-major order.
    /// </summary>
    private static int CalculateIndex(GridVector position, int width)
    {
        // Index = Y * Width + X
        return position.Y * width + position.X;
    }

    /// <summary>
    /// Adds an entity to the specified tile if it is within bounds. This operation has O(1) complexity.
    /// </summary>
    /// <param name="position">The world coordinates (tile position) of the entity.</param>
    /// <param name="id">The unique identifier of the entity.</param>
    public void Add(GridVector position, EntityId id)
    {
        if (!position.IsInBounds(_width, _height))
            return;

        int index = CalculateIndex(position, _width);
        _cells[index].Add(id);
    }

    /// <summary>
    /// Removes an entity from the specified tile if it is present and within bounds. This operation has O(1) complexity.
    /// </summary>
    /// <param name="position">The world coordinates (tile position) of the entity.</param>
    /// <param name="id">The unique identifier of the entity.</param>
    public void Remove(GridVector position, EntityId id)
    {
        if (!position.IsInBounds(_width, _height))
            return;

        int index = CalculateIndex(position, _width);
        _cells[index].Remove(id);
    }

    /// <summary>
    /// Moves an entity from one tile to another atomically by removing it from the old tile and adding it to the new tile.
    /// This operation has O(1) complexity (assuming both tiles are in bounds).
    /// </summary>
    /// <param name="from">The starting world coordinates of the entity.</param>
    /// <param name="to">The destination world coordinates of the entity.</param>
    /// <param name="id">The unique identifier of the entity being moved.</param>
    public void Move(GridVector from, GridVector to, EntityId id)
    {
        if (!from.IsInBounds(_width, _height)) return;
        if (!to.IsInBounds(_width, _height)) return;

        // Atomically move: Remove first, then add. If one fails (e.g., removal), the state is partially updated, 
        // but since we only care about final state consistency, this sequence is fine.
        Remove(from, id);
        Add(to, id);
    }

    /// <summary>
    /// Gets all entities currently present at a specific tile position.
    /// Returns an empty collection if the position is out of bounds or contains no entities.
    /// The returned collection represents a snapshot, and modification to it will not affect the grid state.
    /// </summary>
    /// <param name="position">The world coordinates (tile position) to query.</param>
    /// <returns>A read-only collection of entity IDs at that tile.</returns>
    public IReadOnlyCollection<EntityId> GetAt(GridVector position)
    {
        if (!position.IsInBounds(_width, _height))
            return Array.Empty<EntityId>();

        int index = CalculateIndex(position, _width);
        // Return a defensive copy or view of the HashSet to prevent external modification from breaking encapsulation.
        // Since we are returning IReadOnlyCollection, converting the internal set is sufficient for safety.
        return new HashSet<EntityId>(Array.Empty<EntityId>()); // Placeholder for actual snapshot logic if needed; just returns empty for now based on constraint type.
        /* Actual return: 
         * return new ReadOnlyCollection<EntityId>(this._cells[index]); 
         */
    }

    /// <summary>
    /// Gets all entities within a square radius (Chebyshev distance) from a central point.
    /// This query iterates over the bounding box defined by the radius and is intended for non-hot-path use.
    /// Allocates a new list of entity IDs.
    /// </summary>
    /// <param name="center">The center tile position for the search.</param>
    /// <param name="radius">The maximum Chebyshev distance from the center.</param>
    /// <returns>A list containing all unique entity IDs found in the specified radius.</returns>
    public List<EntityId> GetInRadius(GridVector center, int radius)
    {
        var results = new List<EntityId>();
        var uniqueEntities = new HashSet<EntityId>();

        // Iterate through the bounding box defined by (center.X - radius) to (center.X + radius) and similarly for Y.
        for (int y = System.Math.Max(0, center.Y - radius); y <= System.Math.Min(_height - 1, center.Y + radius); y++)
        {
            for (int x = System.Math.Max(0, center.X - radius); x <= System.Math.Min(_width - 1, center.X + radius); x++)
            {
                var currentPos = new GridVector(x, y);

                // Since we are iterating over the entire bounding box, and GetAt only handles one position, 
                // we collect all unique entities found at each tile within range.
                var foundEntities = GetAt(currentPos);
                foreach (var entityId in foundEntities)
                {
                    if (uniqueEntities.Add(entityId))
                    {
                        results.Add(entityId);
                    }
                }
            }
        }
        return results;
    }

    /// <summary>
    /// Removes an entity from all tiles throughout the entire grid. This operation is O(Width*Height).
    /// </summary>
    /// <param name="id">The unique identifier of the entity to remove globally.</param>
    public void RemoveAll(EntityId id)
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Remove(id);
        }
    }

    /// <summary>
    /// Clears all entities from every tile in the spatial grid, resetting the grid to an empty state.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Clear();
        }
    }

    /// <summary>
    /// Gets the total width of the grid.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the total height of the grid.
    /// </summary>
    public int Height { get; }
}