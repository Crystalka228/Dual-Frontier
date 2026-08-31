using System;
using DualFrontier.Contracts.Services;

namespace DualFrontier.AI.Pathfinding
{
    /// <summary>
    /// Passability bitmap for A* pathfinding.
    /// Rebuilt in batches when map changes (building placed/destroyed).
    /// Read-only during a single pathfinding request.
    ///
    /// <para>
    /// Implements <see cref="INavGridService"/> (W4) so a mod can author terrain and query
    /// passability through the SDK. The interface was shaped to this class rather than the other
    /// way round: every member below already had this exact signature and behaviour, so the
    /// declaration is additive and no call site moved.
    /// </para>
    /// </summary>
    public sealed class NavGrid : INavGridService
    {
        public int Width  { get; }
        public int Height { get; }

        private readonly bool[] _passable;
        private readonly byte[] _cost;

        public NavGrid(int width, int height)
        {
            Width  = width;
            Height = height;
            _passable = new bool[width * height];
            _cost     = new byte[width * height];
            // default: all passable, cost 1
            Array.Fill(_passable, true);
            Array.Fill(_cost, (byte)1);
        }

        public bool IsPassable(int x, int y) =>
            InBounds(x, y) && _passable[y * Width + x];

        public byte GetCost(int x, int y) =>
            InBounds(x, y) ? _cost[y * Width + x] : byte.MaxValue;

        public void SetTile(int x, int y, bool passable, byte cost = 1)
        {
            if (!InBounds(x, y)) return;
            _passable[y * Width + x] = passable;
            _cost[y * Width + x]     = cost;
        }

        private bool InBounds(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;
    }
}