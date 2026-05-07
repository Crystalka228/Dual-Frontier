# Native Migration — Progress Tracker

**Status**: LIVE document (не LOCKED) — обновляется при каждом milestone closure
**Created**: 2026-05-07
**Last updated**: 2026-05-07
**Scope**: Tracks combined K-series (kernel) + M9-series (runtime) migration progression
**Companion documents**: `KERNEL_ARCHITECTURE.md` (LOCKED v1.0), `RUNTIME_ARCHITECTURE.md` (LOCKED v1.0), `CPP_KERNEL_BRANCH_REPORT.md` (Discovery, reference)

---

## Purpose

Этот документ — единая точка наблюдения за продвижением миграции на native foundation (C++ ECS kernel + Vulkan runtime). НЕ является архитектурным документом — архитектура зафиксирована в `KERNEL_ARCHITECTURE.md` и `RUNTIME_ARCHITECTURE.md` как LOCKED v1.0.

**Что фиксируется здесь**:
- Status каждого milestone (not started / in progress / done / decision-pending / blocked)
- Commit hash + дата закрытия
- Текущие блокеры (если есть)
- Решения, принятые в процессе исполнения (вне рамок LOCKED архитектуры)
- Open questions, которые проявились в ходе работы

**Что НЕ фиксируется здесь** (см. соответствующие документы):
- Архитектурные решения K-L1..K-L10, L1..L10 — `KERNEL_ARCHITECTURE.md` Part 0, `RUNTIME_ARCHITECTURE.md` Part 0
- Детальные milestone briefs — `tools/briefs/`
- Discovery findings от experimental branch — `CPP_KERNEL_BRANCH_REPORT.md`
- Methodology adjustments — `METHODOLOGY.md`

---

## Current state snapshot

| | Value |
|---|---|
| **Active phase** | Pre-K0 (planning + scaffolding done, execution не начато) |
| **Last completed milestone** | M8.10 (coupled pause + DebugOverlay) — `7514e38` 2026-05-07 |
| **Next milestone (recommended)** | K0 (cherry-pick + cleanup от experimental branch) |
| **Sequencing strategy** | Open — decision deferred к after K2 measurement (см. §«Sequencing decision») |
| **Combined estimate** | 9-15 weeks (5-8 kernel + 4-7 runtime) |
| **Tests passing** | 472 (managed Domain) — preserved invariant throughout migration |

---

## Sequencing decision

**Status**: OPEN, deferred к after K2

Три валидных опции зафиксированы в `KERNEL_ARCHITECTURE.md` Part 8:

| Option | Sequence | Total time | Trade-off |
|---|---|---|---|
| **β5 — kernel fast-track** | K0–K2 → M9.0–M9.8 → K3–K8 | 10–16w | Validates bridge early, then visible runtime progress, kernel completion last |
| **β6 — kernel-first sequential** | K0–K8 → M9.0–M9.8 | 10–15w | Single architectural focus per period, max cleanness, no visible game progress for 5–8w |
| **β3 — interleaved** | K0 → M9.0–M9.5 → K1–K2 → M9.6–M9.8 → K3–K8 | 11–15w | Earlier visible progress, но context-switching между native стеками |

**Recommendation per Crystalka philosophy «cleanness > expediency»**: β5 или β6 over β3.

**Decision trigger**: K2 closure provides bridge-maturity evidence. Если бридж стабилен и P/Invoke patterns подтверждены — β5 valid (kernel pause + runtime sprint). Если K2 показал необходимость более глубокой kernel work до runtime integration — β6.

**Decision will be recorded в этом документе при K2 closure**.

---

## K-series progress (kernel)

### Overview

| Milestone | Title | Status | Estimate | Commit | Date closed |
|---|---|---|---|---|---|
| K0 | Cherry-pick + cleanup от experimental branch | NOT STARTED | 1–2 days | — | — |
| K1 | Batching primitive (bulk Add/Get + Span<T>) | NOT STARTED | 3–5 days | — | — |
| K2 | Type-id registry + bridge tests | NOT STARTED | 2–3 days | — | — |
| K3 | Native bootstrap graph + thread pool | NOT STARTED | 5–7 days | — | — |
| K4 | Component struct refactor (Path α) | NOT STARTED | 2–3 weeks | — | — |
| K5 | Span<T> protocol + write command batching | NOT STARTED | 1 week | — | — |
| K6 | Second-graph rebuild on mod change | NOT STARTED | 3–5 days | — | — |
| K7 | Performance measurement (tick-loop) | NOT STARTED | 3–5 days | — | — |
| K8 | Decision step + production cutover | NOT STARTED | 1 week | — | — |

**Cumulative estimate**: 5–8 weeks at hobby pace (~1h/day).

### K0 — Cherry-pick + cleanup от experimental branch

- **Status**: NOT STARTED
- **Brief**: `tools/briefs/K0_CHERRY_PICK_BRIEF.md` (SKELETON — full brief authoring required before execution)
- **Source branch**: `claude/cpp-core-experiment-cEsyH` (HEAD `e2bc2d9`)
- **Sequence**: 7 cherry-picks + cleanup commits
- **Acceptance criteria** (high-level):
  - All 7 substantive commits cherry-picked onto fresh branch от `origin/main`
  - `.gitignore` widening applied
  - Dead code removed или wired
  - `NATIVE_CORE.md` + `NATIVE_CORE_EXPERIMENT.md` marked как superseded
  - Native selftest passing
  - 472 managed tests still passing
- **Blockers**: none
- **Open questions**: none yet

### K1 — K8

Detailed entries будут добавлены при подходе к каждому milestone (full brief authoring + execution + closure recording).

---

## M9-series progress (runtime)

### Overview

| Milestone | Title | Status | Estimate | Commit | Date closed |
|---|---|---|---|---|---|
| M9.0 | Win32 window + Vulkan instance + clear color | NOT STARTED | 4–5 days | — | — |
| M9.1 | Textured quad pipeline (single sprite) | NOT STARTED | 3–4 days | — | — |
| M9.2 | Batched sprite renderer (instancing) | NOT STARTED | 4–5 days | — | — |
| M9.3 | TileMap parity (M8.9 visual reproduction) | NOT STARTED | 4–6 days | — | — |
| M9.4 | Input layer (Win32 message pump → IInputAdapter) | NOT STARTED | 3–4 days | — | — |
| M9.5 | Domain integration (M8.9 full parity, switchable backend) | NOT STARTED | 1 week | — | — |
| M9.6 | UI primitives (text rendering, panels) | NOT STARTED | 1–2 weeks | — | — |
| M9.7 | Coupled lifecycle (pause/focus parity с Godot M8.10) | NOT STARTED | 2–3 days | — | — |
| M9.8 | Godot deletion (point of no return для runtime) | NOT STARTED | 2–3 days | — | — |

**Cumulative estimate**: 4–7 weeks at hobby pace.

### M9.0 — M9.8

Detailed entries будут добавлены при подходе к каждому milestone.

---

## Cross-series coupling

Зависимости и взаимовлияния между K и M series, которые могут проявиться в ходе исполнения:

| K milestone | M milestone | Potential coupling |
|---|---|---|
| K4 (struct refactor) | M9.5 (Domain integration) | Если M9.5 идёт раньше K4, Domain integration работает с class-based components. После K4 — adjustment может потребоваться. |
| K5 (span protocol) | None direct | Self-contained в kernel layer |
| K8 (cutover) | M9.5 (Domain integration) | Если K8 Outcome 1 (native wins) до M9.5 — Domain integration сразу нацеливается на NativeWorld. Иначе — на managed World с возможным позднейшим switchover. |
| K7 (perf measurement) | None direct | Информирует K8 decision, не блокирует runtime |
| Все K | M9.8 (Godot deletion) | Godot deletion это точка невозврата ТОЛЬКО для runtime. Kernel decision (K8) независим — managed World остаётся fallback. |

**Invariant**: Kernel и runtime — два независимых стека под managed Application layer. Координация требуется только при cutover-точках (K8, M9.5, M9.8).

---

## Decisions log (operational, не архитектурные)

Решения, принятые в ходе исполнения миграции. Архитектурные LOCKED-решения см. в `KERNEL_ARCHITECTURE.md` Part 0 и `RUNTIME_ARCHITECTURE.md` Part 0.

### D1 — Single progress tracker для K и M
- **Date**: 2026-05-07
- **Decision**: Один файл `MIGRATION_PROGRESS.md`, не разделение на KERNEL_PROGRESS + RUNTIME_PROGRESS
- **Rationale**: sequencing β3/β5/β6 — это решение про взаимосвязь стеков. Раздельные файлы потеряют это видение. Cross-series coupling table требует обоих списков рядом.
- **Reversal trigger**: если документ превысит ~50KB или K и M прогресс начнут существенно расходиться по темпу — рассмотреть split.

### D2 — Sequencing decision deferred к after K2
- **Date**: 2026-05-07
- **Decision**: β3/β5/β6 не выбирается заранее. K0–K2 выполняются first как preservation + bridge maturity (общий шаг для всех вариантов).
- **Rationale**: K2 closure даёт evidence о зрелости bridge layer. Choice без evidence — speculation.
- **Recording trigger**: при K2 closure — фиксируется выбор β-variant в этом документе.

### D3 — Native organicity level: Lvl 1 (independent projects, contract-based)
- **Date**: 2026-05-07
- **Decision**: Kernel и Runtime остаются двумя **независимыми** native проектами. Каждый — отдельный `.dll`, отдельный CMake build, отдельный Interop bridge. Никакого shared native code (общий thread pool / allocator / logger), никакого объединения в single DLL.
- **Rationale**:
  - Consistency с операционным принципом проекта «никто никуда не лезет» — на уровне mods (ALC), Domain↔Presentation (PresentationBridge), system scheduling (SystemExecutionContext), Native↔Managed (single ownership boundary). Native↔Native теперь следует тому же правилу.
  - Сохраняет «kernel could be open-sourced separately» property (KERNEL_ARCHITECTURE.md §1.2) и аналогичное для runtime.
  - Характеристики работы фундаментально разные: kernel thread pool idle после bootstrap, runtime Vulkan thread активен per-frame. Shared infrastructure = premature optimization без выигрыша.
  - Conceptual integrity: ECS storage и Vulkan rendering не имеют общего concern. Объединять их = семантическая ошибка.
- **Rejected alternatives**:
  - **Lvl 2** (shared native infrastructure — общий thread pool / allocator): premature, no evidence of need
  - **Lvl 3** (single native DLL): ломает open-source-separately property, ломает headless dedicated server use-case для модов
- **Reversal trigger**: только если через 12+ месяцев profiling на weak hardware покажет что thread oversubscription / fragmentation реальная боль. Тогда — отдельный архитектурный milestone с amendment к KERNEL_ARCHITECTURE/RUNTIME_ARCHITECTURE.
- **Implication для cross-series coupling table**: остаётся как есть — координация только в cutover-точках (K8, M9.5, M9.8), никаких shared native artifacts.

---

## Open questions

Вопросы, которые проявились в процессе планирования но решение которых отложено к моменту, когда появятся данные.

### OQ1 — Branch strategy для миграции
- **Question**: Один long-lived `feature/native-kernel` branch для всей K-series, или per-K feature branches с merge на main?
- **Trigger to resolve**: до K0 execution (требуется в K0 brief)
- **Lean**: per-K feature branches + atomic merge на main per closure (matches existing M8.x pattern)

### OQ2 — Rollback policy для K4 (struct refactor)
- **Question**: K4 это 2-3 weeks scope с ~50-80 component conversions. Если в середине обнаружится фундаментальная проблема (например, behavior class split не работает для X компонентов) — есть ли клин-rollback?
- **Trigger to resolve**: при K4 brief authoring
- **Lean**: incremental merge per 5-10 components (each commit independent), revert simple

### OQ3 — Cross-document drift между KERNEL_ARCHITECTURE и RUNTIME_ARCHITECTURE
- **Question**: Два LOCKED v1.0 документа эволюционируют параллельно. Как избежать дрейфа decisions (например, K-L4 explicit registry vs L-аналог в RUNTIME)?
- **Trigger to resolve**: при первом обнаружении конфликта
- **Lean**: при amendment одного документа — обязательная проверка cross-references в другом

### OQ4 — Что если K7 показал, что native не нужен?
- **Question**: K8 Outcome 2/3 означают что native kernel parking. Что делать с уже выполненными K0-K7 (особенно K3 native scheduler, K5 span protocol)?
- **Trigger to resolve**: после K7 measurements
- **Lean**: K8 brief предусматривает три outcome-варианта; recording rationale + lessons learned обязательны

---

## Closure protocol

Когда milestone закрывается:

1. **Run final verification**: `dotnet build`, `dotnet test`, native selftest (для K-series), F5 verification (для M-series)
2. **Atomic commit** с scope prefix (`feat`/`fix`/`refactor`/`test`/`docs`/`native`/`interop`)
3. **Update этот документ**:
   - Status: NOT STARTED → DONE
   - Commit hash + date filled
   - Add «Lessons learned» entry если что-то нетривиальное обнаружилось
   - Update `Current state snapshot` table
   - Если milestone разрешил OQ — переместить из «Open questions» в «Decisions log»
4. **Update brief**: mark brief as EXECUTED + add link к commit в `tools/briefs/K{N}_*.md`

**Pre-flight для самого update**: pre-flight grep на «NOT STARTED» / «IN PROGRESS» — убедиться что только один milestone в IN PROGRESS одновременно.

---

## Document end

Single point of view на 9-15 weeks of work. Обновляется per milestone closure. Архитектурные решения остаются LOCKED в companion documents.
