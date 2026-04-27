# Save — World save/load

## Purpose
Serialization and deserialization of the `World` state to disk. `ISaveSystem`
is the contract, `SaveSystem` is the implementation, `SaveFormat` is the
format version and file header (for migrations of old saves).

## Dependencies
- `DualFrontier.Core` — `World`, `ComponentStore`
- `DualFrontier.Contracts` — component interfaces

## Contents
- `ISaveSystem.cs` — `Save(path)` / `Load(path)` interface.
- `SaveSystem.cs` — implementation using a binary format (planned).
- `SaveFormat.cs` — file version and header, migrations.

## Rules
- `Save` / `Load` are **synchronous**. Asynchronous behavior (progress bar,
  etc.) is provided by the upstream layer. This is an explicit THREADING rule.
- Does not use `using Godot` — writes and reads ordinary files.
- The format version strictly increases. Incompatible saves are an explicit
  error with a migration hint.

## Usage examples
```csharp
ISaveSystem saves = new SaveSystem(world);
saves.Save("saves/slot1.dfsave");
// ...
saves.Load("saves/slot1.dfsave");
```

## TODO
- [ ] Phase 1 — minimal binary format (components + entities).
- [ ] Phase 3 — `SaveFormat` version migrations.
- [ ] Phase 3 — serialization of mod-dependent components by type id.
