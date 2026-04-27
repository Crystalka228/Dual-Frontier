# Building

## Purpose
Components for buildings and their infrastructure: power consumers and producers
(electricity and ether), storages, workbenches.

## Dependencies
- `DualFrontier.Contracts` — `IComponent`, `EntityId`.

## Contents
- `PowerConsumerComponent.cs` — power consumer (type + per-tick draw).
- `PowerProducerComponent.cs` — power producer (type + output).
- `StorageComponent.cs` — storage (capacity + item list).
- `WorkbenchComponent.cs` — workbench (kind + work speed).

## Rules
- Two independent power networks: electricity and ether. The `PowerType` field
  indicates which network the building is connected to. Mixing within a single
  network is forbidden by the PowerSystem validator.
- Network overload publishes `GridOverloadEvent`.
- `StorageComponent.Items` is a list of entity IDs of items in the world (items
  are separate entities with their own components).

## Usage examples
```csharp
var reactor = world.CreateEntity();
world.AddComponent(reactor, new PowerProducerComponent { /* Type = PowerType.Electric, Output = 1000 */ });

var golemForge = world.CreateEntity();
world.AddComponent(golemForge, new PowerConsumerComponent { /* Type = PowerType.Ether, WattsPerTick = 50 */ });
```

## TODO
- [ ] Define the `PowerType` enum (Electric, Ether).
- [ ] Define the `WorkbenchKind` enum (Cooking, Smithing, Research, GolemForge …).
- [ ] Plan equipment degradation/breakage — a separate `DurabilityComponent`.
