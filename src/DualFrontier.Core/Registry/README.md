# Registry — Type registries

## Purpose
Registries for component and system types. They are needed for:
1. Assigning stable numeric IDs to components (for Save/Load and the mod
   network protocol).
2. Dynamic registration of components/systems by mods through `IModApi`.
3. Iterating over every registered system from the scheduler when the graph
   is built.

## Dependencies
- `DualFrontier.Contracts.Core` (`IComponent`).
- `DualFrontier.Core.ECS` (`SystemBase`).

## Contents
- `ComponentRegistry.cs` — `Type ↔ int ComponentTypeId` mapping.
- `SystemRegistry.cs` — storage of registered systems, iterator for the scheduler.

## Rules
- Registration happens at game startup or mod load. It is not called at runtime.
- A component's ID is stable across runs (important for Save/Load). For mods,
  IDs are assigned after the base types and depend on mod load order.
- `SystemRegistry` does not guarantee a stable iteration order.

## Usage examples
```csharp
var components = new ComponentRegistry();
components.Register<HealthComponent>();
int id = components.GetTypeId(typeof(HealthComponent));

var systems = new SystemRegistry();
systems.Register(new CombatSystem());
foreach (var system in systems.GetAll()) { /* build graph */ }
```

## TODO
- [ ] Phase 1 — implement `ComponentRegistry` with stable IDs.
- [ ] Phase 1 — implement `SystemRegistry`.
- [ ] Phase 2 — add Type ↔ ID mapping serialization to the save file.
