using System;
using System.Collections.Generic;
using System.Diagnostics;
using DualFrontier.AI.Pathfinding;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Math;
using DualFrontier.Core.ECS;
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
///
/// K8.3+K8.4 combined milestone (2026-05-14) — constructor gained a
/// <see cref="NativeWorld"/> parameter (item entities now mint on BOTH worlds
/// in lockstep), and Spawn adopted the two-phase pattern (lockstep mint →
/// bulk <see cref="NativeWorld.AddComponents{T}"/> per type → dual-write to
/// managed <see cref="World"/> during transition). None of the four item
/// component types embed K8.1 primitives, so Phase A is trivial value-struct
/// construction. Phase 5 commit 21 removes the managed-side mint + dual-write.
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
    /// Spawns the configured items into both <paramref name="nativeWorld"/>
    /// and <paramref name="world"/> (dual-write transition). Each item type
    /// spawned per its requested count, all placed on unique passable tiles
    /// disjoint from <paramref name="excludedPositions"/> (typically pawn
    /// spawn tiles).
    /// </summary>
    public void Spawn(
        NativeWorld nativeWorld,
        World world,
        IReadOnlyCollection<GridVector> excludedPositions,
        int foodCount,
        int waterCount,
        int bedCount,
        int decorationCount)
    {
        if (nativeWorld is null) throw new ArgumentNullException(nameof(nativeWorld));
        if (world is null) throw new ArgumentNullException(nameof(world));
        if (excludedPositions is null) throw new ArgumentNullException(nameof(excludedPositions));
        if (foodCount < 0) throw new ArgumentOutOfRangeException(nameof(foodCount));
        if (waterCount < 0) throw new ArgumentOutOfRangeException(nameof(waterCount));
        if (bedCount < 0) throw new ArgumentOutOfRangeException(nameof(bedCount));
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

        // ── Food: ConsumableComponent { Satiety, 0.4 restore, 1 charge } ─────
        SpawnTyped<ConsumableComponent>(
            nativeWorld, world, passable, ref cursor, foodCount,
            _ => new ConsumableComponent
            {
                RestoresKind      = NeedKind.Satiety,
                RestorationAmount = 0.4f,
                Charges           = 1,
            });

        // ── Water: WaterSourceComponent { 0.5 restore, persistent } ──────────
        SpawnTyped<WaterSourceComponent>(
            nativeWorld, world, passable, ref cursor, waterCount,
            _ => new WaterSourceComponent
            {
                RestorationAmount = 0.5f,
            });

        // ── Bed: BedComponent { null occupant, 0.005/tick sleep restore } ────
        SpawnTyped<BedComponent>(
            nativeWorld, world, passable, ref cursor, bedCount,
            _ => new BedComponent
            {
                Occupant                = null,
                SleepRestorationPerTick = 0.005f,
            });

        // ── Decoration: DecorativeAuraComponent { 3-tile radius, 0.001/tick } ─
        SpawnTyped<DecorativeAuraComponent>(
            nativeWorld, world, passable, ref cursor, decorationCount,
            _ => new DecorativeAuraComponent
            {
                Radius         = 3,
                ComfortPerTick = 0.001f,
            });
    }

    /// <summary>
    /// Two-phase spawn for a single item type: lockstep mint on both worlds,
    /// build typed component arrays, bulk-add to NativeWorld, dual-write to
    /// managed World. Generic over <typeparamref name="T"/> so all four item
    /// types share one implementation; the per-type variation is supplied via
    /// the <paramref name="componentFactory"/> delegate.
    /// </summary>
    private void SpawnTyped<T>(
        NativeWorld nativeWorld,
        World world,
        List<GridVector> passable,
        ref int cursor,
        int count,
        Func<int, T> componentFactory) where T : unmanaged, IComponent
    {
        if (count == 0) return;

        var entities = new EntityId[count];
        var positions = new PositionComponent[count];
        var components = new T[count];

        // Phase A: lockstep mint + component construction
        for (int i = 0; i < count; i++)
        {
            EntityId managedId = world.CreateEntity();
            EntityId nativeId = nativeWorld.CreateEntity();
            Debug.Assert(managedId.Index == nativeId.Index,
                "Managed/native entity index drift during dual-write transition");
            entities[i] = managedId;

            positions[i] = new PositionComponent { Position = passable[cursor++] };
            components[i] = componentFactory(i);
        }

        // Phase B: bulk-add to NativeWorld
        nativeWorld.AddComponents<PositionComponent>(entities, positions);
        nativeWorld.AddComponents<T>(entities, components);

        // Dual-write to managed World during transition (removed Phase 5 commit 21)
        for (int i = 0; i < count; i++)
        {
            world.AddComponent(entities[i], positions[i]);
            world.AddComponent(entities[i], components[i]);
        }
    }
}
