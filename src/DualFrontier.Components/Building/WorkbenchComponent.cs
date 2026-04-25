namespace DualFrontier.Components.Building;

using DualFrontier.Contracts.Core;

/// <summary>
/// Crafting station. Holds active recipe and progress.
/// Written exclusively by CraftSystem.
/// </summary>
public sealed class WorkbenchComponent : IComponent
{
    /// <summary>Recipe currently being crafted. Null = idle.</summary>
    public string? ActiveRecipeId;

    /// <summary>Crafting progress 0..1. 1.0 = ready to complete.</summary>
    public float Progress;

    /// <summary>Ticks remaining to complete current recipe.</summary>
    public int TicksRemaining;

    /// <summary>Entity id of the pawn currently working at this bench. 0 = unoccupied.</summary>
    public int WorkerEntityIndex;

    /// <summary>True if a pawn is actively working here.</summary>
    public bool IsOccupied => WorkerEntityIndex > 0;

    /// <summary>True if no recipe is active.</summary>
    public bool IsIdle => ActiveRecipeId is null;
}