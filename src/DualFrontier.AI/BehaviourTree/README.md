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

---
register_id: DOC-F-SRC-AI-BEHAVIOURTREE
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-04-XX
last_modified: 2026-04-XX
content_language: en
next_review_due: null
title: AI BehaviourTree submodule
review_cadence: on-source-commit+quarterly
reviewer: Crystalka
---
