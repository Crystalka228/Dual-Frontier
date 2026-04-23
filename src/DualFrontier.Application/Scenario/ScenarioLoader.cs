namespace DualFrontier.Application.Scenario
{
    using System;
    using System.IO;
    using System.Text.Json;

    /// <summary>
    /// Loads ScenarioDef from a JSON file or returns a built-in default.
    /// </summary>
    public sealed class ScenarioLoader
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling         = JsonCommentHandling.Skip,
            AllowTrailingCommas         = true
        };

        /// <summary>
        /// Loads scenario from JSON file at path.
        /// Throws FileNotFoundException if file does not exist.
        /// Throws InvalidOperationException if JSON is invalid.
        /// </summary>
        public ScenarioDef Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"Scenario file not found: {path}", path);

            var json = File.ReadAllText(path);
            var def  = JsonSerializer.Deserialize<ScenarioDef>(json, Options);

            return def ?? throw new InvalidOperationException(
                $"Failed to deserialize scenario from: {path}");
        }

        /// <summary>
        /// Returns the built-in default scenario without reading any file.
        /// Used on first launch and in tests.
        /// </summary>
        public ScenarioDef LoadDefault() => new ScenarioDef
        {
            Id                = "default",
            Name              = "Default Colony",
            StartingPawnCount = 3,
            MapWidth          = 100,
            MapHeight         = 100,
            WorldSeed         = 0
        };
    }
}