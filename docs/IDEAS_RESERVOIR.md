# Ideas Reservoir

*Post-release ideas considered but not committed to the M0–M10 migration sequence. Each entry is recorded for future reference at the appropriate boundary; no entry is a commitment, and no entry blocks the active roadmap. The reservoir is intentionally separate from [ROADMAP](./ROADMAP.md) so that documenting an idea here does not imply scheduling it.*

*Version: 0.1 (2026-05-06). Living document.*

---

## 0. Purpose

After Phase 7 closure — game shipped, baseline performance measured, full vanilla mod set live — development continues in the form of post-release updates. This reservoir holds candidate ideas for those updates. Each idea here exists on the terms of "what if?", not on the terms of a deliverable.

The single criterion for inclusion is architectural compatibility with the foundation already specified in [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md), [METHODOLOGY](/docs/methodology/METHODOLOGY.md), and [ARCHITECTURE](/docs/architecture/ARCHITECTURE.md). An idea belongs here if implementing it would not require breaking any existing LOCKED spec. An idea that would require such a break belongs in a ratification proposal, not the reservoir.

The reservoir deliberately omits performance targets, success metrics, and acceptance criteria. Validation of these ideas happens in field work — through actual implementation against actual users — not through speculative numbers written before the work begins. Stating "90% success rate" or "60% win rate" at the speculative stage is marketing language; the reservoir is engineering scratch space.

---

## 1. Reading discipline

Read this document after Phase 7 closure, not before. Reading it during the active M-cycle introduces decision fatigue — every idea here looks plausible, and the temptation to begin "just one" before shipping is real. The roadmap is the active surface; the reservoir is dormant context.

The active surface for current development is [ROADMAP](./ROADMAP.md). The current scope explicitly excludes everything in this document, with the single exception of dynamic map expansion, which lives in the active backlog. All other entries are post-release.

The reservoir is a living document. Ideas may be added when a clean architectural fit is identified; ideas may be removed when they lose relevance. There is no version-locking discipline here — history lives in git, not in versioned `LOCKED` markers. Treat the reservoir the way you would treat a research notebook: high-trust source for thinking, not a contract.

---

## 2. Ideas

### 2.1 AI mod assistant

A local LLM generates a mod from a player's natural-language request. The capability system validates the result through the same path as a human-authored mod — no special trust extended to the LLM. Hot reload allows the player to iterate without restarting the session.

The hardware tier model is straightforward: RTX 4060+ class cards (8 GB VRAM) run Gemma 4B; RTX 4070+ (12 GB) runs Gemma 7B; RTX 4090+ (24 GB) runs Gemma 27B; CPU-only fallback runs TinyLlama and is restricted to tier-1 mods (single-system, kernel-capability-only). Documentation of the modding API doubles as the LLM's context. This inverts the usual reading: documentation is no longer "for humans" — it is for humans and models simultaneously, and the same document serves both.

The architectural fit is good: the ALC isolation already covers generated mods identically to authored ones; the capability validation path applies unchanged; static type checking acts as a validation layer before runtime; and the documentation-as-context pattern gives the LLM exactly the surface area it needs. The dependency that bites is API stability — generated mods written against an API snapshot stop working when the API evolves. This is the same triple-binding risk the translation pipeline faces (see [METHODOLOGY](/docs/methodology/METHODOLOGY.md) §7 on multi-surface synchronization), and is closer to a `TRANSLATION_GLOSSARY`-shaped problem than a feature-shaped one.

### 2.2 Voronoi-driven faction borders

Colony and faction borders emerge from weighted Voronoi geometry rather than being specified statically. Influence — population, military strength, cultural pull — supplies the weights. The model supports the eventual goal of an unbounded map: Voronoi cells extend beyond the visible region without special-casing.

The algorithm is Jump Flooding on the GPU, which runs in O(log n) parallel passes. Multi-level partitioning (civilizational → regional → local) lets the same machinery serve different zoom levels. Landmark anchoring contributes weight to specific points, so player-significant locations exert a gravitational pull on their region's borders.

The dependency that bites is GPU determinism. Floating-point reduction order, driver versions, and vendor differences (NVIDIA / AMD / Intel) all conspire against bit-identical output. Determinism is non-negotiable per [METHODOLOGY](/docs/methodology/METHODOLOGY.md) §7.1, so any GPU-compute integration that this idea depends on must be preceded by a determinism verification pass.

### 2.3 Topological data analysis for social dynamics

Persistence homology applied to a colony's social network. The game itself observes forming factions, bridge nodes, and structural holes — and surfaces these to the player before they become visible through narrative observation alone.

Library options are GUDHI or Ripser via P/Invoke. The compute is expensive, so the practical update cadence is once per ~1000 ticks rather than per tick. The output is a structured topology — faction membership graph, bridge node identity, persistent feature lifetimes — that the existing event bus can carry to UI surfaces or to mods that want to react.

This is the most research-tier idea in the reservoir. No game in the colony-sim genre currently treats social topology as a first-class compute target. The first integration of a non-trivial library through P/Invoke is also the most likely to surface integration patterns the project will want to reuse for §2.4 and §2.5; treating §2.3 as a pilot in that sense reduces the integration risk for the others.

### 2.4 Symbolic regression on simulation data

The game discovers its own laws through symbolic regression run against deterministic simulation traces. The player sees actual mathematical formulas describing how happiness, productivity, or any other emergent metric responds to inputs. Mod-introduced effects become automatically detectable through formula comparison: a mod that changes how mood works produces a different formula than vanilla.

PySR is the natural library candidate, invoked through a subprocess or REST surface rather than in-process. Of the three Python-ML integrations the reservoir considers (§2.3, §2.4, §2.5), this one has the most benign failure mode: a less-precise formula is still useful, while a less-precise causal-inference result misleads. That makes §2.4 the strongest pilot integration pattern — the one to attempt first, refine the integration shape against, and replicate for §2.3 and §2.5 once proven.

### 2.5 Causal inference for player decision analysis

Counterfactual simulation, made cheap by determinism. The question "did the farm cause prosperity, or was it coincidence?" can be answered by replaying with and without the farm and observing the divergence. The deterministic simulation guarantees the replays are reproducible; ECS state snapshots are cheap (memcpy-bounded); the existing event log already records the decisions worth analyzing.

Library candidates: DoWhy from Microsoft Research, or a project-internal implementation. Sampling discipline matters — counterfactual replays approximately double the per-major-decision compute cost, so the budget supports analysis of major decisions, not every decision. The output is a per-decision causal effect estimate plus a causal graph; the UI surface is a decision-history screen showing the player which choices materially affected outcomes.

The privacy boundary is local-only. The same discipline applies as in §2.8: data never leaves the player's machine, and the existence of the analysis is transparent to the player.

### 2.6 AI opponents through behavior cloning

An antagonist faction whose decisions are recognizably human. Trained on anonymized replays, the AI plays with strategies a human would recognize — but executes them with perfect timing, ensemble consensus, and longer planning horizons. The result is a challenging-but-legible opponent rather than the alien-strategist failure mode that ML opponents traditionally fall into.

Architecture: a small transformer (~50M parameters) running CPU inference, which is acceptable for the latency budget of strategic decisions (sub-second is fine; sub-millisecond is not required). The faction integration uses the same capability system as any other mod — the AI is constrained by exactly the capabilities its faction holds, no more. Power scaling comes from ensemble + faster reactions, not from super-strategy: the AI does not invent moves no human plays; it executes the moves humans play, faster and with less hesitation.

The pre-launch training corpus is the developer's own playthroughs. Post-launch, opt-in community contribution extends the corpus. The privacy boundary needs explicit decision before any replay collection begins: what fields are included in "anonymized," what fields are stripped, and what consent flow the player sees. This is a LOCKED-spec-shaped decision, not a runtime decision — see [METHODOLOGY](/docs/methodology/METHODOLOGY.md) §7 on the discipline of locking sensitive contracts before implementation pressure begins.

### 2.7 Lambda calculus REPL for power users

A scripting layer with a minimalist mathematical foundation. Lambda calculus is the smallest Turing-complete substrate; capability checks apply at every primitive operation, so the security envelope is identical to the regular mod system. A visual programming layer maps drag-and-drop blocks to lambda expressions, lowering the entry cost for non-programmers; the Y-combinator handles recursion at the bottom of the stack.

This is the cheapest idea in the reservoir to implement (the evaluator is on the order of a few hundred lines) and the highest in geek appeal. The community potential is real — power users will share scripts the way they share mod load orders today. The natural integration point is the existing capability system: scripts hold the same kind of capability set a regular mod does, and the runtime check is shared.

### 2.8 Player model for adaptive personalization

The game observes the player, builds a local model, and adapts difficulty, content emphasis, and pacing. The privacy posture is "by design": local-only storage, no transmission, transparent surface in the settings UI, and full player control through export / reset / disable.

The cross-game potential is open: an optional player-model export would let players carry a self-snapshot to other games that support the format. No such format currently exists; someone has to start one. Whether Dual Frontier is the right project to start it is a strategic question, not an architectural one — the reservoir notes the option without committing.

The architectural fit is strong: the existing event log already captures the actions that feed the model, the determinism guarantees the model can be rebuilt from logs if storage is lost, and the capability system already mediates which mods can read the model. The dependency that bites is the boundary between "local-only player model" and "training corpus for the AI opponent in §2.6". If the AI opponent personalizes itself against the player model, the line between local data and exported training data becomes ambiguous unless the boundary is locked explicitly. This is a pre-implementation decision, not a post-implementation one.

### 2.9 FHE integration

The architectural commitment for fully homomorphic encryption is documented separately as a LOCKED specification: [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md) v1.0.

Unlike the other entries in this reservoir, FHE has a formal architectural contract under the same discipline as [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md). The contract is ratified; activation is conditional on the three independent conditions enumerated in §D1 of the contract; the dormant period is unbounded.

The deliberate asymmetry between "idea in the reservoir" and "ratified LOCKED contract" reflects the asymmetric structure of the problem. FHE is simultaneously the entry most distant from implementation and the entry most precisely specifiable in architectural terms. When the implementation specifics are unknowable — because the library does not yet exist — what remains specifiable is the principle. When the specifics are knowable, the principle has typically already been compromised. The contract captures the principle while it is still capturable.

---

## 3. Cross-feature combinations

The combinations are more interesting than the individual ideas. If implementation work begins on any single idea above, the choice of which one to begin with should account for what combinations the choice enables downstream.

| Combination | Effect |
|---|---|
| Voronoi (§2.2) + TDA (§2.3) | Geographic and social topology together — emergent geopolitics where physical borders and social fracture lines are computed from independent mathematics and then compared. |
| Causal inference (§2.5) + symbolic regression (§2.4) | Discovered game laws validated against counterfactual replay — formulas with confidence intervals derived from the simulation itself. |
| Player model (§2.8) + AI opponents (§2.6) | Adaptive adversaries calibrated against the individual player's recognizable strategy patterns. |
| Lambda calculus REPL (§2.7) + player model (§2.8) | Scripting with style-aware autocompletion — the REPL's suggestions reflect what this specific player tends to do. |
| AI mod assistant (§2.1) + player model (§2.8) | Generated mods reflect the player's stated and observed preferences, not just the natural-language request. |
| TDA (§2.3) + lambda calculus REPL (§2.7) | Programmatic access to topology — the player writes their own analytical queries against the social graph. |

The §2.2 + §2.3 combination is the strongest. Geopolitics that emerges from two independent mathematical models — one geometric, one topological — produces narrative the developer did not script and the player did not anticipate. This is exactly the failure mode of authored-narrative colony sims: the story is whatever the writer wrote. The combination here points toward stories no one wrote.

---

## 4. Out of scope

Some ideas were considered and rejected at this stage. The rejections are recorded so the question does not re-emerge as a fresh proposal:

- **Online leaderboards or social features.** Conflicts with the local-only data discipline that the player model and AI opponent training rely on. The same discipline that enables those features specifically forecloses leaderboards.
- **Microtransactions.** Incompatible with the project's commercial model and with the methodology's positioning ([METHODOLOGY](/docs/methodology/METHODOLOGY.md) §0). Not an architectural objection — a values objection.
- **Procedural narrative through a runtime LLM.** Slow, non-deterministic, and incompatible with the deterministic-replay invariant in [METHODOLOGY](/docs/methodology/METHODOLOGY.md) §7.1. The AI mod assistant in §2.1 is build-time; runtime narrative generation is a different shape of problem.
- **Blockchain anything.** No architectural need that another mechanism does not already address. The capability system handles authorization; the deterministic simulation handles reproducibility; the mod-signing system handles authorship. There is no remaining problem for which a distributed ledger is the answer.

These are not moral judgments. They are architectural fit decisions. The same ideas in different projects can be implemented correctly; they do not fit Dual Frontier specifically.

---

## 5. Reservoir discipline

The reservoir lives by these conventions, which are deliberately lighter than the LOCKED-spec discipline:

- Adding an idea is allowed without ratification. If an idea fits the §0 inclusion criterion, it can be added in a regular commit with a brief rationale in the commit message.
- Removing an idea is also allowed. If an idea has lost relevance or has been falsified by new architectural decisions, it is better to remove than to leave a stale entry.
- Inclusion in the reservoir is not a commitment. The presence of an idea here implies only that it has been considered and found architecturally compatible, not that it will be implemented.
- There is no version-locking. History lives in git. Major changes to the document's shape (a new top-level section, a restructure of the categories) warrant a commit, not a version bump.
- When an idea transitions from reservoir to active development, its specification moves to a dedicated `docs/research/<feature>.md` document — the reservoir entry remains as the genesis record, while the active spec lives as a normal LOCKED-discipline document. This mirrors the relationship the reservoir already has with [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md): the reservoir entry references the formal contract; the contract itself follows project discipline.

---

## See also

- [ROADMAP](./ROADMAP.md) — the active surface, including the "Beyond ship" section that links here.
- [MOD_OS_ARCHITECTURE](/docs/architecture/MOD_OS_ARCHITECTURE.md) — the modding architecture every reservoir entry is required to fit within.
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — the determinism invariant, the discipline of locking sensitive contracts before implementation, and the boundary against speculative metrics.
- [FHE_INTEGRATION_CONTRACT](/docs/architecture/FHE_INTEGRATION_CONTRACT.md) — the one reservoir idea that has already crossed the line into formal ratification.
