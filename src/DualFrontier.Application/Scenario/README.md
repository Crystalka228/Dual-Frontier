# Scenario — Scenario loading

## Purpose
Start scenarios describe the initial world state: biome, generation seed,
the count and parameters of starting pawns, the starting inventory, and so on.
`ScenarioLoader` parses scenario files from disk; `ScenarioDef` is the DTO
description.

## Dependencies
- `DualFrontier.Core` — for building `World` from the definition.

## Contents
- `ScenarioLoader.cs` — scenario-file parser (JSON/TOML, TBD).
- `ScenarioDef.cs` — immutable scenario definition.

## Rules
- `ScenarioDef` is a pure DTO with no logic.
- Parsing is synchronous; asynchronous progress is the upstream layer's
  responsibility.
- A scenario MUST NOT reference concrete mod types by C# name — only by
  registered identifier.

## Usage examples
```csharp
var loader   = new ScenarioLoader();
ScenarioDef scenario = loader.Load("scenarios/default.json");
// Next — build World from scenario.
```

## TODO
- [x] Phase 3 — `ScenarioLoader.Load(path)` parses JSON via
      `System.Text.Json`.
- [x] Phase 3 — `ScenarioDef` with `Id`, `Name`, `StartingPawnCount`,
      `MapWidth`, `MapHeight`, `WorldSeed`, `StartingItems` fields.
- [ ] Schema validation and clear parse-error messages.
- [ ] `LoadDefault()` is used when the scenario file is missing
      (the method exists, but the caller does not yet fall back).
