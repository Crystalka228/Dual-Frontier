using System;
using System.Collections;

namespace DualFrontier.AI.Pathfinding;

/// <summary>
/// Represents a grid map used for pathfinding and movement cost calculation in the simulation.
/// </summary>
public sealed class NavGrid
{
    // Private readonly boolean array: true = can walk, false = impassable. Indexed by y * Width + x.
    private readonly bool[] _passable;

    // Private readonly byte array: 1 = normal cost, 2 = slow terrain, 255 = impassable. Indexed by y * Width + x.
    private readonly byte[] _cost;

    /// <summary>
    /// Gets the width (number of tiles) of the navigation grid.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height (number of tiles) of the navigation grid.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Initializes a new instance of the NavGrid class.
    /// </summary>
    /// <param name="width">The width of the grid.</param>
    /// <param name="height">The height of the grid.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if width or height is less than or equal to zero.</exception>
    public NavGrid(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException("Width and Height must be greater than zero.");
        }

        Width = width;
        Height = height;

        // Allocate arrays of size width * height.
        _passable = new bool[width * height];
        _cost = new byte[width * height];

        int totalSize = width * height;

        for (int i = 0; i < totalSize; i++)
        {
            // Initialize all tiles as passable and with normal cost.
            _passable[i] = true;
            _cost[i] = 1;
        }
    }

    /// <summary>
    /// Checks if a specific tile coordinate is within the grid boundaries.
    /// </summary>
    /// <param name="x">The X-coordinate of the tile.</param>
    /// <param name="y">The Y-coordinate of the tile.</param>
    /// <returns>True if coordinates are in bounds, otherwise false.</returns>
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    /// <summary>
    /// Calculates the flat array index corresponding to the given (x, y) coordinates.
    /// </summary>
    private int Index(int x, int y) => y * Width + x;

    /// <summary>
    /// Determines if a tile at the specified coordinates is passable.
    /// </summary>
    /// <param name="x">The X-coordinate of the tile.</param>
    /// <param name="y">The Y-coordinate of the tile.</param>
    /// <returns>False if out of bounds, or if the tile is marked as impassable.</returns>
    public bool IsPassable(int x, int y)
    {
        if (!IsInBounds(x, y))
        {
            return false;
        }
        return _passable[Index(x, y)];
    }

    /// <summary>
    /// Retrieves the movement cost associated with a tile at the specified coordinates.
    /// </summary>
    /// <param name="x">The X-coordinate of the tile.</param>
    /// <param name="y">The Y-coordinate of the tile.</param>
    /// <returns>The movement cost (byte). Returns 255 if out of bounds or not passable.</returns>
    public byte GetCost(int x, int y)
    {
        if (!IsInBounds(x, y))
        {
            return 255; // Out of bounds is treated as impassable/max cost.
        }

        // If not passable, return 255 immediately to enforce minimum cost logic.
        if (!_passable[Index(x, y)])
        {
            return 255;
        }

        return _cost[Index(x, y)];
    }

    /// <summary>
    /// Sets the passability status of a tile at the specified coordinates.
    /// </summary>
    /// <remarks>
    /// If set to non-passable (false), the movement cost is automatically set to 255. No action is taken if out of bounds.
    /// </remarks>
    /// <param name="x">The X-coordinate of the tile.</param>
    /// <param name="y">The Y-coordinate of the tile.</param>
    /// <param name="passable">The new passability status.</param>
    public void SetPassable(int x, int y, bool passable)
    {
        if (!IsInBounds(x, y)) return;

        _passable[Index(x, y)] = passable;

        if (!passable)
        {
            // If not passable, set cost to max.
            _cost[Index(x, y)] = 255;
        }
    }

    /// <summary>
    /// Sets the movement cost for a tile at the specified coordinates.
    /// </summary>
    /// <remarks>
    /// No action is taken if out of bounds. The cost update only affects passable tiles.
    /// </remarks>
    /// <param name="x">The X-coordinate of the tile.</param>
    /// <param name="y">The Y-coordinate of the tile.</param>
    /// <param name="cost">The new movement cost (1 = normal, 2 = slow, etc.).</param>
    public void SetCost(int x, int y, byte cost)
    {
        if (!IsInBounds(x, y)) return;

        _cost[Index(x, y)] = cost;
    }
}