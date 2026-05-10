namespace DualFrontier.Components.Building;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

/// <summary>
/// Crafting station. Holds active recipe and progress.
/// Written exclusively by CraftSystem.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct WorkbenchComponent : IComponent
{
    /// <summary>
    /// Recipe currently being crafted. Empty interned-string sentinel
    /// (<see cref="InternedString.Empty"/>) signals idle. Set by CraftSystem
    /// via <c>NativeWorld.InternString</c> when a recipe begins; cleared
    /// back to default when the recipe completes or aborts.
    /// </summary>
    public InternedString ActiveRecipeId;

    /// <summary>Crafting progress 0..1. 1.0 = ready to complete.</summary>
    public float Progress;

    /// <summary>Ticks remaining to complete current recipe.</summary>
    public int TicksRemaining;

    /// <summary>Entity id of the pawn currently working at this bench. 0 = unoccupied.</summary>
    public int WorkerEntityIndex;

    /// <summary>True if a pawn is actively working here.</summary>
    public bool IsOccupied => WorkerEntityIndex > 0;

    /// <summary>True if no recipe is active.</summary>
    public bool IsIdle => ActiveRecipeId.IsEmpty;
}
