# BehaviourTree

## Purpose
A generic behaviour tree for pawns and units. The BT does not store world
state — it accepts a `BTContext`, reads pawn data from it, and returns a
status (Running / Success / Failure).

## Dependencies
- `DualFrontier.Contracts` — `EntityId`.
- `DualFrontier.Components` — through `BTContext` (do not import directly into
  nodes).

## Contents
- `BTNode.cs` — base abstract class + the `BTStatus` enum.
- `BTContext.cs` — context for one BT tick (entity, services).
- `Selector.cs` — "try in order until one succeeds".
- `Sequence.cs` — "execute in order while every child succeeds".
- `Leaf.cs` — base class for concrete actions/conditions.

## Rules
- Nodes are pure: no global state, no singletons. All state is passed through
  `BTContext`.
- No `async/await` — the BT is synchronous; expensive operations (pathfinding)
  are sliced across ticks.
- `Leaf` — concrete work (walk to point, eat food).
- `Selector` / `Sequence` are composites; they return Running while a child
  is Running.

## Usage examples
```csharp
var root = new Selector(
    new Sequence(new IsHungryLeaf(), new EatLeaf()),
    new IdleLeaf()
);
var status = root.Tick(ctx);
```

## TODO
- [ ] Implement `Selector.Tick` / `Sequence.Tick` with current-child memoization.
- [ ] Implement `Leaf` as abstract with debugger-visible fields.
- [ ] Add `BTBlackboard` for per-pawn local state.
- [ ] BT JSON parser for mods.
