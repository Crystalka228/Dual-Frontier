namespace DualFrontier.Application.Scenario
{
    using System.Collections.Generic;

    /// <summary>
    /// Immutable starting scenario definition.
    /// Parsed from JSON by ScenarioLoader.
    /// </summary>
    public sealed class ScenarioDef
    {
        /// <summary>Unique scenario identifier.</summary>
        public string Id { get; init; } = "default";

        /// <summary>Display name shown in scenario selection UI.</summary>
        public string Name { get; init; } = "Default Colony";

        /// <summary>Number of starting pawns.</summary>
        public int StartingPawnCount { get; init; } = 3;

        /// <summary>World seed for terrain generation. 0 = random.</summary>
        public int WorldSeed { get; init; } = 0;

        /// <summary>Map width in tiles.</summary>
        public int MapWidth { get; init; } = 100;

        /// <summary>Map height in tiles.</summary>
        public int MapHeight { get; init; } = 100;

        /// <summary>Starting items by item id and quantity.</summary>
        public Dictionary<string, int> StartingItems { get; init; } = new();
    }
}