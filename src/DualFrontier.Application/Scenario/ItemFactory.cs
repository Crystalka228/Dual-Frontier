using System;
using System.Collections.Generic;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.Interop;

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
/// spawning on pawn tiles.
///
/// K8.3+K8.4 cutover (2026-05-14, brief v2.0): NativeWorld is the sole
/// component-storage backend; the prior dual-write to a managed
/// <c>World</c> is gone.
/// </summary>
internal sealed class ItemFactory
{
    private readonly Random _rng;
    private readonly NavGrid _navGrid;
    private readonly int _mapWidth;
    private readonly int _mapHeight;
    private readonly NativeWorld _nativeWorld;

    public ItemFactory(int seed, NavGrid navGrid, int mapWidth, int mapHeight,
                       NativeWorld nativeWorld)
    {
        _rng = new Random(seed);
        _navGrid = navGrid ?? throw new ArgumentNullException(nameof(navGrid));
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;
        _nativeWorld = nativeWorld ?? throw new ArgumentNullException(nameof(nativeWorld));
    }

    /// <summary>
    /// Spawns the configured items into <paramref name="nativeWorld"/>.
    /// Each item type spawned per its requested count, all placed on
    /// unique passable tiles disjoint from <paramref name="excludedPositions"/>
    /// (typically pawn spawn tiles).
    /// </summary>
    public void Spawn(
        NativeWorld nativeWorld,
        IReadOnlyCollection<GridVector> excludedPositions,
        int foodCount,
        int waterCount,
        int bedCount,
        int decorationCount)
    {
        if (nativeWorld is null) throw new ArgumentNullException(nameof(nativeWorld));
        if (excludedPositions is null) throw new ArgumentNullException(nameof(excludedPositions));
        if (foodCount < 0) throw new ArgumentOutOfRangeException(nameof(foodCount));
        if (waterCount < 0) throw new ArgumentOutOfRangeException(nameof(waterCount));
        if (bedCount < 0) throw new ArgumentOutOfRangeException(nameof(bedCount));
        if (decorationCount < 0) throw new ArgumentOutOfRangeException(nameof(decorationCount));

        int totalItems = foodCount + waterCount + bedCount + decorationCount;

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

        for (int i = passable.Count - 1; i > 0; i--)
        {
            int j = _rng.Next(0, i + 1);
            (passable[i], passable[j]) = (passable[j], passable[i]);
        }

        int cursor = 0;

        SpawnTyped<ConsumableComponent>(
            nativeWorld, passable, ref cursor, foodCount,
            _ => new ConsumableComponent
            {
                RestoresKind      = NeedKind.Satiety,
                RestorationAmount = 0.4f,
                Charges           = 1,
            });

        SpawnTyped<WaterSourceComponent>(
            nativeWorld, passable, ref cursor, waterCount,
            _ => new WaterSourceComponent
            {
                RestorationAmount = 0.5f,
            });

        SpawnTyped<BedComponent>(
            nativeWorld, passable, ref cursor, bedCount,
            _ => new BedComponent
            {
                Occupant                = null,
                SleepRestorationPerTick = 0.005f,
            });

        SpawnTyped<DecorativeAuraComponent>(
            nativeWorld, passable, ref cursor, decorationCount,
            _ => new DecorativeAuraComponent
            {
                Radius         = 3,
                ComfortPerTick = 0.001f,
            });
    }

    private void SpawnTyped<T>(
        NativeWorld nativeWorld,
        List<GridVector> passable,
        ref int cursor,
        int count,
        Func<int, T> componentFactory) where T : unmanaged, IComponent
    {
        if (count == 0) return;

        var entities = new EntityId[count];
        var positions = new PositionComponent[count];
        var components = new T[count];

        for (int i = 0; i < count; i++)
        {
            entities[i] = nativeWorld.CreateEntity();
            positions[i] = new PositionComponent { Position = passable[cursor++] };
            components[i] = componentFactory(i);
        }

        nativeWorld.AddComponents<PositionComponent>(entities, positions);
        nativeWorld.AddComponents<T>(entities, components);
    }
}
