using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DualFrontier.Application.Scenario
{
    public sealed class ScenarioLoader
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        /// <summary>
        /// Loads a scenario definition from a JSON file at the given path.
        /// Throws FileNotFoundException if file does not exist.
        /// Throws InvalidOperationException if JSON is invalid or missing required fields.
        /// </summary>
        public ScenarioDef Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Scenario file not found: {path}", path);

            var json = File.ReadAllText(path);
            var def = JsonSerializer.Deserialize<ScenarioDef>(json, Options);

            if (def is null)
                throw new InvalidOperationException(
                    $"Failed to deserialize scenario from: {path}");

            return def;
        }

        /// <summary>
        /// Returns the built-in default scenario without reading any file.
        /// Used when no scenario file is available (first launch, tests).
        /// </summary>
        public ScenarioDef LoadDefault() => new ScenarioDef
        {
            Id = "default",
            Name = "Default Colony",
            StartingPawnCount = 3,
            MapWidth = 100,
            MapHeight = 100,
            WorldSeed = 0
        };
    }
}