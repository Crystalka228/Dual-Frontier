# Godot integration

Godot is an excellent engine for 2D simulation, but it has a hard limitation: the `SceneTree` and `Node` APIs work only from the main thread. Dual Frontier's Domain logic is multithreaded and must not know that Godot is even nearby. The link between the layers is built on a unidirectional command bridge.

## Status: Godot as DevKit

In v0.3 Godot is used as a **development tool**, not as the production runtime.
Scene editing, system-behavior testing, and visual debugging — all through Godot
Editor and `DualFrontier.Presentation`. The production game build runs on
`DualFrontier.Presentation.Native` (Silk.NET + OpenGL).

Both backends implement the `IRenderer`, `ISceneLoader`, and `IInputSource`
contracts from the Application layer. This document covers Godot-specific
details: the main thread and `PresentationBridge`. The general architecture of
the visual system lives in [VISUAL_ENGINE](./VISUAL_ENGINE.md).

## Integration status

- GodotSharp 4.6.1 is wired in through `Godot.NET.Sdk`.
- `DualFrontier.Presentation` is removed from `DualFrontier.sln` — Godot manages
  its own `csproj` independently.
- `GenerateAssemblyInfo=false` and `GenerateTargetFrameworkAttribute=false` are
  mandatory flags in `DualFrontier.Presentation.csproj` to avoid duplicate
  attributes.
- `main.tscn` loads as the main scene.
- `PawnVisual`, `TileMapRenderer`, `PawnLayer` — implemented.
- `RenderCommandDispatcher` — implemented.

---

## Main-thread limitation

Any attempt to call `AddChild`, `QueueFree`, `SetPosition`, `EmitSignal`, or `GetTree` from a background thread in Godot produces undefined behavior: typically a crash in a random place a few frames later, a race against the SceneTree's deferred-message queue. The bug is hard to reproduce and impossible to debug.

The solution: Domain and Application run in their own threads; Presentation lives exclusively in Godot's main thread. Domain → Presentation communication goes through `PresentationBridge`. Presentation → Domain communication goes through `InputRouter` and the buses.

## PresentationBridge — a unidirectional queue

`PresentationBridge` belongs to the Application layer. It holds a `ConcurrentQueue<IRenderCommand>`. Domain writes to the queue from any thread; Presentation drains the queue in `_Process()` on the main thread.

```csharp
public sealed class PresentationBridge
{
    // Domain writes here from any thread
    private readonly ConcurrentQueue<IRenderCommand> _commands = new();

    // Domain: report a pawn death
    public void EnqueuePawnDied(EntityId id, Vector2 position)
        => _commands.Enqueue(new PawnDiedCommand(id, position));

    // Godot _Process() — main thread only
    public void DrainCommands()
    {
        while (_commands.TryDequeue(out var cmd))
            cmd.Execute(_godotScene);
    }
}
```

Key properties:

- **One-way.** Presentation writes to the input queue, Domain writes to the render-command queue. But Presentation never reads Domain directly.
- **Thread safety.** `ConcurrentQueue` is enough: many writer threads, a single reader (the main thread).
- **No lock contention in Domain.** `Enqueue` is lock-free on every modern .NET runtime.
- **Backpressure is observable.** If Presentation falls behind, the queue grows and the `_commands.Count` graph shows it. Investigating the cause is a PERFORMANCE pipeline task.

## IRenderCommand — command list

The set of Domain → Presentation commands grows over time, but the core is fixed.

```csharp
public interface IRenderCommand : ICommand
{
    void Execute(object renderContext);
}
```

The `renderContext` parameter is the backend-specific root: for Godot it is the
`GameRoot` node, for Native it is the `NativeRenderer` instance. Commands cast
inside `Execute`.

| Command                       | Created by                            | Does                                            |
|-------------------------------|---------------------------------------|-------------------------------------------------|
| `PawnDiedCommand`             | `DamageSystem` → via `Application`    | Death animation, removal of `PawnVisual`        |
| `ProjectileSpawnedCommand`    | `ProjectileSystem`                    | Creates a `ProjectileVisual` node               |
| `SpellCastCommand`            | `SpellSystem`                         | Cast VFX, sound, animation                      |
| `UIUpdateCommand`             | `MoodSystem`, `NeedsSystem`, etc.     | Updates the pawn panel, alert icons             |
| `PawnMovedCommand`            | `JobSystem` / movement                | Updates `PawnVisual` position                   |
| `BuildingPlacedCommand`       | `CraftSystem`                         | Spawns a building node                          |

Systems do not create commands directly — they publish domain events, and the Application bridge listens and enqueues the commands. This keeps the Domain layer clean: no reference to a Godot type at all.

## Why Presentation does not call Domain

A direct call from Presentation into Domain would break everything:

1. **Main-thread blocking.** Systems are multithreaded; calling them would require synchronization, and the main thread would wait for the result, tanking FPS.
2. **Breaks the isolation guard.** Presentation is not registered in `SystemExecutionContext`; a back-door call to `world.GetComponent` would void the declarations.
3. **Breaks moddability.** Mods extend Domain; if Presentation calls Domain through concrete class names, every mod edit would require recompiling Presentation.

The correct input flow: `InputRouter` publishes an event to the bus, a Domain system processes the event, and based on the result publishes another event or enqueues a command to the bridge.

## InputRouter

`InputRouter` is the single point where Godot input becomes domain events. It lives in `DualFrontier.Presentation/Input`.

```csharp
public partial class InputRouter : Node
{
    [Export] public GameServicesHolder Services { get; set; } = null!;

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            var grid = ScreenToGrid(mouseButton.Position);
            Services.Bus.Pawns.Publish(new SelectPawnIntent { GridPosition = grid });
        }

        if (Input.IsActionJustPressed("build_mode"))
        {
            Services.Bus.World.Publish(new ToggleBuildModeEvent());
        }
    }
}
```

`InputEvent` does not leak into Domain — the key/button-to-domain-event mapping happens in the router. This gives flexibility (new controls = edits only in the router) and cleanliness (Domain knows nothing about Godot).

## Godot node vs Entity lifecycle

An entity and a Godot node are two different objects with different lifetimes.

| Object            | Lives while                                   | Created by                            |
|-------------------|-----------------------------------------------|---------------------------------------|
| `EntityId`        | `world.DestroyEntity` has not been called     | `Application` / systems               |
| Component         | Attached to a living entity                   | `Application` / systems               |
| Godot `Node`      | `QueueFree` has not been called               | Presentation, on a command from the bridge |

Rule: a node needs an entity, but an entity can exist without a node (for example, server simulation or headless tests). The node is the visual shadow of the entity. The link is through the `EntityId` stored as a field on the node.

```csharp
public partial class PawnVisual : Node2D
{
    public EntityId EntityId;

    public override void _Process(double delta)
    {
        // Read PresentationBridge only — there is no direct access to World.
    }
}
```

On receiving `PawnDiedCommand`, Presentation finds the node by `EntityId`, plays the death animation, and on completion calls `QueueFree`. The entity has long been destroyed in Domain by then.

The flip side: if Domain destroys an entity mid-phase while the node-removal command is still in the queue, nothing breaks. The command arrives in the next `_Process`, the node calls `QueueFree`, Godot removes it. Desynchronization is safe because the direction is always one-way.

## UI Development Cycle

One UI iteration after each phase:
1. New components → new fields in `PawnStateCommand`.
2. New sections in `PawnDetail` or new panels.
3. F5 → visually confirm the phase works.

The UI is a live dashboard for the simulation — visual bugs often
point to logic bugs in Domain.

### Current UI status (Phase 4)

Implemented:
- `GameHUD` (CanvasLayer, layer=10).
- `ColonyPanel` — pawn list with mini mood bars.
- `PawnDetail` — pawn details (needs, mood, job, skills).
- `PawnStateReporterSystem` (SLOW) — publishes data through the `Pawns` bus.
- `PawnStateCommand` — the only way to feed data into the HUD.

Data flow: `PawnStateReporterSystem.Update` → `PawnStateChangedEvent`
on the `Pawns` bus → subscriber in `GameBootstrap` → `bridge.Enqueue(PawnStateCommand)`
→ `RenderCommandDispatcher` → `GameHUD.UpdatePawn` → panels.

Style: Grimdark Warhammer — dark background `#1a1814`, parchment text
`#c8b89a`, gothic fonts (system serif fallback).

### Next UI iteration (Phase 5)
- Add `HealthComponent` to `PawnStateCommand`.
- Add a "Health" section to `PawnDetail`.
- Add a visual damage indicator on pawns in `PawnLayer`.

## See also

- [ARCHITECTURE](./ARCHITECTURE.md)
- [THREADING](./THREADING.md)
- [EVENT_BUS](./EVENT_BUS.md)
