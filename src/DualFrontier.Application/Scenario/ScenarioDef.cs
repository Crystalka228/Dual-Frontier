using System.Collections.Generic;

namespace DualFrontier.Application.Scenario
{
    /// <summary>Immutable definition of a starting scenario.
    /// Parsed from JSON by ScenarioLoader.
    /// </summary>
    public sealed class ScenarioDef
    {
        /// <summary>Unique scenario identifier, e.g. "default".</summary>
        public string Id { get; init; } = "default";

        /// <summary>Display name shown in scenario selection UI.</summary>
        public string Name { get; init; } = "Default Colony";

        /// <summary>Starting pawn count.</summary>
        public int StartingPawnCount { get; init; } = 3;

        /// <summary>World seed for terrain generation. 0 = random.</summary>
        public int WorldSeed { get; init; } = 0;

        /// <summary>Map width in tiles.</summary>
        public int MapWidth { get; init; } = 100;

        /// <summary>Map height in tiles.</summary>
        public int MapHeight { get; init; } = 100;

        /// <summary>Starting item definitions by item id and quantity.</summary>
        public Dictionary<string, int> StartingItems { get; init; } = new();
    }
}
