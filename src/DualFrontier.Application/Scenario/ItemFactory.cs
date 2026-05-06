using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.ECS;

namespace DualFrontier.Application.Scenario;

/// <summary>
/// Deterministic random item factory. Spawns 4 entity types at GameBootstrap
/// startup: food (ConsumableComponent restoring Satiety), water (WaterSource-
/// Component for persistent Hydration), bed (BedComponent with tracked
/// Occupant + per-tick sleep rate), decoration (DecorativeAuraComponent with
/// passive ambient Comfort radius).
///
/// Pattern parallels <see cref="RandomPawnFactory"/> — passable-tile pool from
/// NavGrid, shuffle, take prefix. Excluded positions parameter prevents items
/// spawning on pawn tiles. Tracks placed positions implicitly via shuffle
/// prefix consumption (no two items on one tile because shuffle yields each
/// passable tile at most once).
///
/// Architectural placement: lives in Application/Scenario/, not in
/// Vanilla.World mod. RandomPawnFactory precedent (Phase 4) established this
/// pattern — current IModApi doesn't expose World access or CreateEntity to
/// mods. Both factories migrate to their natural mods (Vanilla.{World,Pawn})
/// when IModApi extends with entity creation surface in M9+ Mod-OS Migration
/// work.
/// </summary>
internal sealed class ItemFactory
{
    private readonly Random _rng;
    private readonly NavGrid _navGrid;
    private readonly int _mapWidth;
    private readonly int _mapHeight;

    public ItemFactory(int seed, NavGrid navGrid, int mapWidth, int mapHeight)
    {
        _rng = new Random(seed);
        _navGrid = navGrid ?? throw new ArgumentNullException(nameof(navGrid));
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
    }

    /// <summary>
    /// Spawns the configured items into <paramref name="world"/>. Each item
    /// type spawned per its requested count, all placed on unique passable
    /// tiles disjoint from <paramref name="excludedPositions"/> (typically
    /// pawn spawn tiles).
    /// </summary>
    public void Spawn(
        World world,
        IReadOnlyCollection<GridVector> excludedPositions,
        int foodCount,
        int waterCount,
        int bedCount,
        int decorationCount)
    {
        if (world is null) throw new ArgumentNullException(nameof(world));
        if (excludedPositions is null) throw new ArgumentNullException(nameof(excludedPositions));
        if (foodCount       < 0) throw new ArgumentOutOfRangeException(nameof(foodCount));
        if (waterCount      < 0) throw new ArgumentOutOfRangeException(nameof(waterCount));
        if (bedCount        < 0) throw new ArgumentOutOfRangeException(nameof(bedCount));
        if (decorationCount < 0) throw new ArgumentOutOfRangeException(nameof(decorationCount));

        int totalItems = foodCount + waterCount + bedCount + decorationCount;

        // Build passable-tile pool, exclude pawn positions.
        var excludedSet = new HashSet<GridVector>(excludedPositions);
        var passable = new List<GridVector>(_mapWidth * _mapHeight);
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                var pos = new GridVector(x, y);
                if (_navGrid.IsPassable(x, y) && !excludedSet.Contains(pos))
                    passable.Add(pos);
            }
        }

        if (passable.Count < totalItems)
        {
            throw new InvalidOperationException(
                $"Not enough passable tiles to spawn {totalItems} items: " +
                $"only {passable.Count} available (excluded {excludedSet.Count}).");
        }

        // Fisher-Yates shuffle, take prefix of length totalItems.
        for (int i = passable.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(0, i + 1);
            (passable[i], passable[j]) = (passable[j], passable[i]);
        }

        int cursor = 0;

        // Food: ConsumableComponent { Satiety, 0.4 restore, 1 charge }
        for (int i = 0; i < foodCount; i++)
        {
            EntityId id = world.CreateEntity();
            world.AddComponent(id, new PositionComponent { Position = passable[cursor++] });
            world.AddComponent(id, new ConsumableComponent
            {
                RestoresKind      = NeedKind.Satiety,
                RestorationAmount = 0.4f,
                Charges           = 1,
            });
        }

        // Water: WaterSourceComponent { 0.5 restore, persistent }
        for (int i = 0; i < waterCount; i++)
        {
            EntityId id = world.CreateEntity();
            world.AddComponent(id, new PositionComponent { Position = passable[cursor++] });
            world.AddComponent(id, new WaterSourceComponent
            {
                RestorationAmount = 0.5f,
            });
        }

        // Bed: BedComponent { null occupant, 0.005/tick sleep restore }
        for (int i = 0; i < bedCount; i++)
        {
            EntityId id = world.CreateEntity();
            world.AddComponent(id, new PositionComponent { Position = passable[cursor++] });
            world.AddComponent(id, new BedComponent
            {
                Occupant                = null,
                SleepRestorationPerTick = 0.005f,
            });
        }

        // Decoration: DecorativeAuraComponent { 3-tile radius, 0.001/tick comfort }
        for (int i = 0; i < decorationCount; i++)
        {
            EntityId id = world.CreateEntity();
            world.AddComponent(id, new PositionComponent { Position = passable[cursor++] });
            world.AddComponent(id, new DecorativeAuraComponent
            {
                Radius         = 3,
                ComfortPerTick = 0.001f,
            });
        }
    }
}
