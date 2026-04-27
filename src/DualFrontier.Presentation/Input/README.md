# Input — Input routing

## Purpose
Single point for handling user input. `InputRouter` catches Godot events
(clicks, keys, drags) and translates them into domain commands through the
`EventBus` / `PresentationBridge`.

## Dependencies
- `DualFrontier.Application` — event bus / bridge.
- `GodotSharp` (Phase 3).

## Contents
- `InputRouter.cs` — input dispatcher.

## Rules
- Presentation NEVER mutates world state directly. Any user action becomes
  an event/command that flows into the domain.
- Input is processed only on Godot's main thread.

## Usage examples
```csharp
// Phase 3+:
public override void _UnhandledInput(InputEvent @event)
{
    _router.Route(@event);
}
```

## TODO
- [ ] Phase 3 — implement `InputRouter.Route` for mouse/keyboard.
- [ ] Phase 3 — configurable bindings.
