namespace DualFrontier.Contracts.Math;

/// <summary>
/// A 2D integer grid coordinate used throughout the simulation for tile positions, pathfinding, and spatial queries.
/// </summary>
public readonly record struct GridVector(int X, int Y)
{
    // Common constants
    /// <summary>The zero vector (0, 0).</summary>
    public static readonly GridVector Zero  = new(0, 0);
    /// <summary>A vector representing movement of magnitude 1 along both axes.</summary>
    public static readonly GridVector One   = new(1, 1);
    /// <summary>The Up vector (0, -1).</summary>
    public static readonly GridVector Up    = new(0, -1);
    /// <summary>The Down vector (0, 1).</summary>
    public static readonly GridVector Down  = new(0,  1);
    /// <summary>The Left vector (-1, 0).</summary>
    public static readonly GridVector Left  = new(-1, 0);
    /// <summary>The Right vector (1, 0).</summary>
    public static readonly GridVector Right = new(1,  0);

    // Arithmetic operators: +, -, * (scalar int)

    /// <summary>
    /// Adds two <see cref="GridVector"/> coordinates.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The second vector.</param>
    /// <returns>A new <see cref="GridVector"/> representing the sum of a and b.</returns>
    public static GridVector operator +(GridVector a, GridVector b) => new(a.X + b.X, a.Y + b.Y);

    /// <summary>
    /// Subtracts two <see cref="GridVector"/> coordinates.
    /// </summary>
    /// <param name="a">The first vector.</param>
    /// <param name="b">The vector to subtract from the first.</param>
    /// <returns>A new <see cref="GridVector"/> representing the difference between a and b.</returns>
    public static GridVector operator -(GridVector a, GridVector b) => new(a.X - b.X, a.Y - b.Y);

    /// <summary>
    /// Scales a vector by a scalar integer value.
    /// </summary>
    /// <param name="a">The original vector.</param>
    /// <param name="scalar">The integer scaling factor.</param>
    /// <returns>A new <see cref="GridVector"/> scaled by the given factor.</returns>
    public static GridVector operator *(GridVector a, int scalar)   => new(a.X * scalar, a.Y * scalar);

    /// <summary>
    /// Calculates the Manhattan distance between this vector and another <see cref="GridVector"/>.
    /// </summary>
    /// <param name="other">The target vector.</param>
    /// <returns>The Manhattan distance as an integer.</returns>
    public int ManhattanDistance(GridVector other)
        => System.Math.Abs(X - other.X) + System.Math.Abs(Y - other.Y);

    /// <summary>
    /// Calculates the Chebyshev distance (max of absolute differences) between this vector and another <see cref="GridVector"/>.
    /// </summary>
    /// <param name="other">The target vector.</param>
    /// <returns>The Chebyshev distance as an integer.</returns>
    public int ChebyshevDistance(GridVector other)
        => System.Math.Max(System.Math.Abs(X - other.X), System.Math.Abs(Y - other.Y));

    /// <summary>
    /// Converts the vector coordinates to a flat array index given the grid width.
    /// </summary>
    /// <param name="width">The width of the grid.</param>
    /// <returns>The corresponding 1D index.</returns>
    public int ToIndex(int width) => Y * width + X;

    /// <summary>
    /// Creates a <see cref="GridVector"/> from a flat array index given the grid width.
    /// </summary>
    /// <param name="index">The 1D index.</param>
    /// <param name="width">The width of the grid.</param>
    /// <returns>A new <see cref="GridVector"/> representing the coordinates at that index.</returns>
    public static GridVector FromIndex(int index, int width) => new(index % width, index / width);

    /// <summary>
    /// Checks if the vector's coordinates are within the specified bounds.
    /// </summary>
    /// <param name="width">The maximum allowed X coordinate + 1.</param>
    /// <param name="height">The maximum allowed Y coordinate + 1.</param>
    /// <returns>True if the vector is in bounds; otherwise, false.</returns>
    public bool IsInBounds(int width, int height)
        => X >= 0 && Y >= 0 && X < width && Y < height;

    /// <summary>
    /// Returns a string representation of the grid vector coordinates.
    /// </summary>
    /// <returns>A formatted string like "(X, Y)".</returns>
    public override string ToString() => $"({X}, {Y})";
}
