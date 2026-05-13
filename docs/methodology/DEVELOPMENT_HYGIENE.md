---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-DEVELOPMENT_HYGIENE
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-DEVELOPMENT_HYGIENE
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-DEVELOPMENT_HYGIENE
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-DEVELOPMENT_HYGIENE
---
# Development hygiene

The project is deliberately developed along two parallel tracks: the **game** Dual Frontier (Phases 0–7, main branch) and the **engine** — a generic ECS core that forks into a separate product after the game ships (see [ROADMAP §"Phase 9 — Native Runtime"](./ROADMAP.md#phase-9--native-runtime)). To make the fork cheap, the engine/game boundary must stay clean through Phases 4–7, when the temptation to "cut corners" is at its highest.

This document is not architectural theory (that lives in [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md) and [CODING_STANDARDS](./CODING_STANDARDS.md)) — it is an **applied checklist for every PR**. If every item is green, the boundary does not degrade. If anything is red, it stops the merge — not "we'll fix it later."

## Core invariant

Engine assemblies never reference game assemblies. Everything else in this document is a way to verify that the invariant has not been broken by accident.

| Engine (generic, reusable)              | Game (specific to Dual Frontier)            |
|-----------------------------------------|---------------------------------------------|
| `DualFrontier.Contracts`                | `DualFrontier.Components`                   |
| `DualFrontier.Core`                     | `DualFrontier.Events`                       |
| `DualFrontier.Core.Interop`             | `DualFrontier.Systems`                      |
| `native/DualFrontier.Core.Native/`      | `DualFrontier.AI`                           |
| `DualFrontier.Presentation.Native`      | `DualFrontier.Presentation` (Godot DevKit)  |
| Modding section of `DualFrontier.Application` | Game-loop part of `DualFrontier.Application` |

Full list and rationale: [ARCHITECTURE §"Dependency rules"](/docs/architecture/ARCHITECTURE.md#dependency-rules).

## Checklist for every PR

Five checks. They run sequentially; one red one and the PR does not ship.

### 1. The engine does not import the game

```bash
grep -rn "using DualFrontier\.\(Components\|Systems\|Events\|AI\)" \
    src/DualFrontier.Contracts/ \
    src/DualFrontier.Core/ \
    src/DualFrontier.Core.Interop/ \
    src/DualFrontier.Presentation.Native/ \
    src/DualFrontier.Application/Modding/
```

Expected output: empty. A single line and the PR does not merge.

### 2. `dotnet build` is green

```bash
./tools/build-all.sh --config Release          # Linux / WSL / macOS
.\tools\build-all.ps1 -Configuration Release   # Windows
```

These wrappers run two builds back-to-back: `dotnet build DualFrontier.sln` and `dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj`. The Godot project is deliberately not in `.sln` — the `Godot.NET.Sdk` reference is dev-machine-specific and would break a `dotnet build DualFrontier.sln` on any machine without Godot installed (CI runners included). Top-level `.sln` build silently skips `Presentation`; the wrapper scripts close that gap.

`TreatWarningsAsErrors=true` in [Directory.Build.props](../Directory.Build.props). Any warning is a blocker. Nullable warnings, CS warnings — they all count.

Manual invocation without the wrapper is two commands:

```bash
dotnet build DualFrontier.sln -c Release
dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj -c Release
```

Full build (both surfaces) is **mandatory** before:

- Any commit touching `src/DualFrontier.Presentation/` directly.
- Any commit touching kernel surface that `Presentation` depends on (`DualFrontier.Application` public API; `DualFrontier.Contracts` types referenced by the bridge).
- Closure review of any M-phase that exercised `Presentation` (M7.5.B.2 onwards).
- F5 manual verification handoff.

The split was registered in [docs/audit/M7_CLOSURE_REVIEW.md §10.3](./audit/M7_CLOSURE_REVIEW.md) and resolved by the TD-2 housekeeping commit that introduced the wrapper scripts. Adding `Presentation` to `.sln` was rejected to keep the solution CI-portable on machines without Godot SDK.

### 3. All tests pass

```bash
dotnet test DualFrontier.sln -c Release
```

Numbers grow, they do not shrink. If main was at 82/82, the branch is expected at ≥82/82. A regression is a blocker.

### 4. Commits carry scope prefixes

```bash
git fetch origin main
git log origin/main..HEAD --no-merges --format='%s' \
    | grep -v '^\(contracts\|core\|interop\|native\|modding\|presentation-native\|experiment\|feat\|fix\|docs\|test\|chore\|build\|refactor\)[(:]'
```

Expected output: empty. Any line means a commit without a scope; rewrite it via `git rebase -i`. The prefixes are documented in [CODING_STANDARDS §"Commit messages"](./CODING_STANDARDS.md#commit-messages).

`--no-merges` is needed so that merge commits (`Merge pull request #…`) do not count as violations. `origin/main..HEAD` after `git fetch` guarantees the comparison runs against the current main, not against a stale local branch.

### 5. Commits do not mix engine and game

Manual review: every commit on the branch must touch either only engine assemblies (prefix `core:`, `contracts:`, `interop:`, `native:`, `modding:`, `presentation-native:`) or only game assemblies (`feat(pawn):`, `feat(combat):`, `feat(presentation):`, …). A mixed commit must be split in two:

```bash
git log origin/main..HEAD --no-merges --stat | less
# visually verify each commit touches one side of the boundary
```

This costs a minute. After the fork, that minute pays back as the ability to pull the entire engine history with one command: `git log --grep "^\(core\|contracts\|interop\|native\|modding\): "`.

## Typical situations

### Adding a new game system (Phases 4–7)

Example: `HaulSystem` in Phase 4.

- Files go **only** in game assemblies: `DualFrontier.Systems/`, with new components in `DualFrontier.Components/` and events in `DualFrontier.Events/` as needed.
- The system inherits from `SystemBase` (engine API). No changes to `SystemBase` for the sake of this system. If `SystemBase` is missing something, it is a signal from §"Red flags" below.
- Access declaration through `[SystemAccess(reads: …, writes: …, buses: …)]` (engine attribute, in `Contracts.Attributes`).
- Commit: `feat(inventory): add HaulSystem`.

### Extending the ECS API

Example: a new method is needed on `SystemExecutionContext`.

- The method MUST be **generic** — no mention of `HealthComponent`, `PawnComponent`, etc.
- Add it to `DualFrontier.Core/ECS/SystemExecutionContext.cs`.
- If a new attribute is needed, it lives in `DualFrontier.Contracts/Attributes/`.
- Commit: `core: add <something>` or `contracts: add <something>`.

### Adding Intent/Granted/Refused to an existing bus

- Records (`record struct`) go in `DualFrontier.Events/` (game assembly). That is fine — events are already game-side.
- The bus in `DualFrontier.Contracts/Bus/` is **not** extended, unless this is generic infrastructure (for example, `IDeferredBus<T>`). Concrete Intents do not land in Contracts.
- Commit: `feat(combat): add ShieldRefillIntent to combat bus`.

### Adding a new component

- The file goes in `DualFrontier.Components/<Area>/<Name>Component.cs`.
- POCO, no logic.
- If a component is potentially generic (e.g., `PositionComponent`, `FactionComponent`), it still lives in `Components`. Components do not belong to the engine; only the `IComponent` marker does.

### Temptation: "I'll just import `Components` into `Core` this once"

This is **always** a blocker. Examples of how to properly solve a task that pushes toward this import:

- **"`Core` must know the format of `HealthComponent` to do X."** No, it must not. If an operation over an arbitrary component is needed, express it through `IComponent` + reflection/generics. If the operation is only over one concrete type, it lives in a system, not in `Core`.
- **"I need `DamageEvent` in `Core` for logging."** Logging goes through the engine's `IEventBus.Subscribe<T>` — `T` is parameterized in Systems, not in Core.
- **"Performance: generics are slow, I want a concrete type."** Measure it. In 95% of cases the JIT removes the overhead. If the remaining 5% really is critical, that is a reason to discuss, not a reason to break the boundary.

## Red flags

Early symptoms that the architecture is starting to degrade. Each is not a disaster on its own, but a signal to stop and discuss.

| Symptom                                                                  | Meaning                                                | What to do                                                                                       |
|--------------------------------------------------------------------------|--------------------------------------------------------|--------------------------------------------------------------------------------------------------|
| `SystemBase` accumulates methods for specific scenarios                  | Game patterns are migrating into the engine            | Extract the pattern into a helper under `Systems/Shared/`; keep `SystemBase` generic             |
| A `switch` over the `Type` of a concrete component appears in `Core`     | The engine has started to know domain types            | Move the logic into a system, or generalize through `IComponent` + a marker attribute            |
| `Contracts` is growing faster than `Systems`                             | Contracts are being published for tasks that do not yet exist | Roll it back; add the contract only when a second consumer actually needs it              |
| Example mods are not updated for months                                  | Modding infrastructure evolves in a vacuum             | Either add at least one real mod as a second customer, or freeze modding development             |
| `native/` lags behind `Core`                                             | The C++ experiment is dead but still in the repo       | Either catch it up, or close the experiment and split it into a separate artifact branch         |
| Commits like "core: fix X, also feat(pawn): Y" in one                    | The boundary is blurring in the author's head          | Demand splitting at review time, even if it costs a `git rebase -i`                              |
| `DualFrontier.Presentation` contains business logic                      | The game is reaching into the Godot layer with gameplay decisions | Business logic goes in `Systems`; `Presentation` only translates commands into Godot calls |

## Quick reference — commit scope prefixes

Full list with examples: [CODING_STANDARDS §"Commit messages"](./CODING_STANDARDS.md#commit-messages).

**Engine** (will move to the fork after release):
- `contracts:`, `core:`, `interop:`, `native:`, `modding:`, `presentation-native:`
- `experiment:` — research branches before merge

**Game** (stay with the game after the fork):
- `feat(pawn):`, `feat(combat):`, `feat(magic):`, `feat(world):`, `feat(inventory):`, `feat(ai):`
- `fix(…):` — bug fixes in the same areas
- `feat(application):` — game loop; `feat(presentation):` — Godot DevKit
- `feat(bootstrap):` — wiring when a new system appears

**Neutral**: `docs:`, `test:`, `chore:`, `build:`, `refactor:`.

## See also

- [ARCHITECTURE §"Dependency rules"](/docs/architecture/ARCHITECTURE.md#dependency-rules)
- [CODING_STANDARDS §"Commit messages"](./CODING_STANDARDS.md#commit-messages)
- [ROADMAP §"Phase 9 — Native Runtime"](./ROADMAP.md#phase-9--native-runtime)
- [ISOLATION](/docs/architecture/ISOLATION.md)
- [MODDING](/docs/architecture/MODDING.md)
