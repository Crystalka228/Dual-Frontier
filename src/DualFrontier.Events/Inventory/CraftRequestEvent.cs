using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Inventory
{
    /// <summary>
    /// Published when a craft job is requested (by player or AI).
    /// This is NOT the start of crafting — JobSystem will
    /// prioritize and assign a pawn to execute it.
    /// </summary>
    public sealed record CraftRequestEvent : IEvent
    {
        /// <summary>Recipe identifier to craft.</summary>
        public required string RecipeId { get; init; }

        /// <summary>Target workbench entity. Null = any available.</summary>
        public EntityId? WorkbenchId { get; init; }

        /// <summary>Entity that requested the craft (player entity or pawn).</summary>
        public required EntityId RequesterId { get; init; }

        /// <summary>Priority. Higher = assigned sooner.</summary>
        public int Priority { get; init; } = 1;
    }
}