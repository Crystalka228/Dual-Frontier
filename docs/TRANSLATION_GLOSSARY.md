---
title: Translation Glossary
nav_order: 99
---

# Translation Glossary — Dual Frontier

**Status:** **v1.0 (locked)** — produced by Pass 1 on 2026-04-26. Authoritative for Pass 2 (documentation translation) and Pass 3 (code translation). Items marked **✓ LOCKED v1.0** are final lexical commitments; items marked **⚠ ESCALATE TO HUMAN** require a human decision before Pass 2 can proceed.

**Version history:**

- v0.1 (initial draft) — term tables (§1–§10), decisions to lock (§11), style notes (§12).
- v0.2 — adds Preamble, §13 Reserved code identifiers, §14 Phrase patterns, §15 User-facing string contracts, §16 Maintenance & versioning. No removals. No section renumbering.
- **v1.0 (this version, 2026-04-26, produced by Pass 1)** — all `⚠ DECISION` markers resolved. 8 of 10 lexical decisions locked as `✓ LOCKED v1.0`; 2 escalated as `⚠ ESCALATE TO HUMAN` (§11.1 owner-role label, §11.7 race-vs-species lore question). All 5 §15 string contracts locked, with §15.1 split into 15.1a (read) / 15.1b (write) to match the actual two-method emission in `SystemExecutionContext`. Pass 1 also verified §15.5 against `docs/architecture/ISOLATION.md` lines 86–90 and aligned the Russian source to the verbose form documented there.

---

## Preamble — How to use this glossary ★ NEW IN v0.2

**Authority.** This glossary is the single authority for lexical choices during the Russian → English translation of Dual Frontier. During Pass 2 (documentation translation) and Pass 3 (code translation), every term substitution comes from here. Disagreement with the glossary is escalated to the human (per `TRANSLATION_PLAN.md` §6) — never resolved by improvisation.

**Scope of authority.** The glossary governs:

- Word- and phrase-level translation choices (§1–§10 term tables, §14 phrase patterns).
- Treatment of code identifiers and reserved terms (§13).
- Format and content of user-facing strings in code, documentation, and tests (§15).
- Style and register conventions (§12, §16).

The glossary does **not** govern: architectural decisions, design changes, methodological revisions. If a translation problem requires changing the underlying architecture, that is out of scope and triggers escalation, not glossary modification.

**The translator's loop.** When an agent (Opus, Sonnet, or human) is translating a passage:

1. For every Russian term that has a glossary entry — substitute the locked English equivalent. No variation, no contextual reinterpretation.
2. For every Russian phrase that matches a §14 pattern — apply the canonical English form.
3. For any Russian construction with no glossary entry — **stop and add to glossary as `⚠ ADDED v1.x`** per §16 maintenance rules. Do not translate ad-hoc.
4. For any English candidate that conflicts with §13 reserved identifiers — preserve the identifier untranslated.
5. For any user-facing string identified in §15 — substitute the canonical English form, update doc, code, and tests in lockstep (per Pass 3 protocol).

**The "no improvisation" rule.** This is the central operational invariant. If the glossary feels incomplete, the response is "stop, escalate, update" — not "guess." The methodology of the project (`METHODOLOGY.md` §2.2) treats contracts as IPC between agents; the glossary is one such contract, and its integrity comes from being the *only* source of lexical truth.

---

## Conventions

- **Code identifiers** (`DualFrontier.Contracts`, `IModContract`, `[SystemAccess]`, `[Deferred]`, `EntityId`, `World`) are *never* translated. Treat as foreign code, leave in backticks.
- **Industry-standard CS terms** (ECS, IPC, two-phase commit, semantic versioning, prompt injection, data exfiltration, lateral movement, heartbeat, lock contention, marker interface) keep English form even when the source spells them out in Russian.
- **Loanwords already in Russian** (pipeline, lease, intent, granted/refused, output, build, slot, snapshot, batch, drain, scope) stay English. Don't reverse-translate them.
- Capitalisation of architectural layer names (Presentation, Application, Domain, Infrastructure) is preserved.

---

## 1. Methodology & LLM pipeline

| Russian | English | Notes |
|---|---|---|
| методический эксперимент | methodology experiment | Project's self-description; consistent everywhere |
| исследовательский проект | research project | |
| гипотеза эксперимента | experimental hypothesis | |
| falsifiable утверждение | falsifiable claim | Popper's term; "claim" preferred over "statement" in academic English |
| структурная верификация | structural verification | Contrasted with "reputational" |
| репутационная (верификация) | reputational (verification) | i.e. "trust the model output because the model is good" |
| цикл верификации | verification cycle | |
| контракты как IPC между агентами | contracts as IPC between agents | Central methodology phrase — preserve verbatim |
| четырёхагентный pipeline | four-agent pipeline | |
| локальный исполнитель | local executor | Role label for Gemma |
| промт-генератор | prompt generator | Role label for Sonnet |
| архитектор и QA | architect and QA | Role label for Opus |
| **владелец смысла** | *(pending)* | **⚠ ESCALATE TO HUMAN** — see §11.1. Pass 1 verified that `Intent` is an actively-used technical term in §5 (resource model: `Intent → Granted/Refused`) and in `IntentBatcher` code, so `intent owner` collides with project terminology. The fallback `direction owner` is methodologically sound but reframes the human's role from "owner of meaning" to "owner of trajectory" — that is an identity choice for the project author, not a lexical one. Pass 2 cannot translate methodology documents until this is resolved. |
| десктопное приложение | desktop app | (As opposed to API/web) |
| подписочная модель | subscription model | |
| Pay-as-you-go | pay-as-you-go | English already |
| фиксированный тариф | fixed tier / fixed subscription | |
| фазовое ревью | phase review | |
| полное ревью | full review | |
| контекстное окно | context window | |
| квантизованная модель | quantised model | "Quantized" if you settle on US English (see §11) |
| 4-битная квантизация | 4-bit quantisation | |
| длинный контекст | long context | |
| архитектурная работа | architectural work | |
| архитектурный доход | architectural dividend | "payoff" or "ROI" alternatives; "dividend" matches the financial connotation |
| архитектурная компрессия | architectural compression | |
| изоляция от галлюцинаций | isolation from hallucinations | |
| структурная сила контрактов | structural strength of contracts | |
| vibe coding | vibe coding | English already |

## 2. Architecture & design

| Russian | English | Notes |
|---|---|---|
| слой | layer | |
| главный поток | main thread | |
| доменная шина | domain bus | Project-specific; "bus" alone is ambiguous |
| контрактная шина | contract bus | Synonym in some contexts; prefer "domain bus" if the meaning is the IGameServices buses |
| маркер-интерфейс | marker interface | Standard CS term |
| маркер-событие | marker event | |
| декларативная изоляция | declarative isolation | |
| многопоточность по декларации | declarative multithreading | |
| параллельный планировщик | parallel scheduler | |
| граф зависимостей | dependency graph | |
| топологическая сортировка | topological sort | |
| фаза (планировщика) | (scheduler) phase | Distinct from "Phase 4" of the roadmap — see §11 |
| тик | tick | |
| направление зависимостей | dependency direction | |
| правила зависимостей | dependency rules | |
| внутренние сборки | internal assemblies | |
| публичный интерфейс | public interface | |
| приватный метод | private method | |
| рефлексия | reflection | |
| патчить через Harmony | patch via Harmony | RimWorld-specific reference |
| очередь команд | command queue | |
| однонаправленная очередь | one-way / unidirectional queue | |
| мост (PresentationBridge) | bridge (PresentationBridge) | Code identifier — keep `PresentationBridge` |
| связующий слой | glue layer | |
| ECS ядро | ECS core | |
| игровой цикл | game loop | |

## 3. Contracts & buses

| Russian | English | Notes |
|---|---|---|
| контракт | contract | Project-specific meaning: a public interface declaration |
| контрактная сборка | contract assembly | Refers to `DualFrontier.Contracts` |
| интерфейс-контракт | contract interface | When discussing IModContract specifically |
| шесть доменных шин | six domain buses | |
| декларация доступа | access declaration | i.e. `[SystemAccess]` |
| ломающее изменение | breaking change | |
| не ломающее изменение | non-breaking change | |
| мажорная версия | major version | SemVer term |
| семантическое версионирование | semantic versioning / SemVer | |
| манифест мода | mod manifest | |
| эволюция контрактов | contract evolution | |
| императивное намерение | imperative intent | re: ICommand |
| адресный (вызов) | addressed (call) | i.e. has a specific handler |
| двухшаговый хендшейк | two-step handshake | "хендшейк" is loanword; some prefer "two-step protocol" |
| **Intent → Granted/Refused** | Intent → Granted/Refused | Preserve verbatim, do not translate the arrows |

## 4. Isolation & error handling

| Russian | English | Notes |
|---|---|---|
| **сторож (изоляции)** | **isolation guard** | **✓ LOCKED v1.0** — see §11.2. `SystemExecutionContext.cs` already opens its XML doc with "Isolation guard for systems"; locking aligns the glossary with the only English form already present in the codebase. Alternatives `sentinel`/`watchdog`/`monitor` rejected as adding semantic noise. |
| незадекларированный доступ | undeclared access | |
| тихое нарушение | silent violation | |
| тихая порча состояния | silent state corruption | |
| краш с диагностикой | crash with diagnostics | |
| мягкая выгрузка (мода) | soft unload (of a mod) | |
| жёсткий краш | hard crash | |
| источник системы | system origin | re: SystemOrigin enum |
| мод-система | mod system | A system loaded from a mod |
| Core-система | core system | A system from `DualFrontier.Systems` |
| баннер (UI) | banner | Refers to mod-disabled notification |
| лог (мода) | (mod) log | |
| стектрейс | stack trace | |
| отписка (от шины) | unsubscription (from a bus) | |
| таймаут | timeout | |
| обнуление (контекста) | clearing / resetting (the context) | |
| исключение | exception | |
| необработанное исключение | unhandled exception | |
| ModFaultHandler | ModFaultHandler | Code identifier — keep |
| формат сообщений | message format | |
| подсказка (как исправить) | hint (how to fix) | re: error messages |
| сообщения по единому шаблону | messages built on a single template | |
| чек-лист добавления системы | system addition checklist | |

## 5. Resource models — Intent vs Lease

| Russian | English | Notes |
|---|---|---|
| модель управления ресурсами | resource model | |
| дискретный запрос | discrete request | |
| непрерывный расход | continuous consumption / continuous drain | "Drain" is preferred when describing the per-tick mechanism |
| аренда | lease | Already English-loanword in source; keep `Lease` |
| reserve-then-consume | reserve-then-consume | English already; do not translate |
| резерв | reservation / reserve | |
| дренаж / дренить | drain / to drain | Per-tick consumption |
| срыв (lease) | abort / failure (of a lease) | When mana runs out mid-channel |
| закрытие (lease) | close / closing (of a lease) | Initiated by owner |
| истечение (lease) | expiration (of a lease) | MaxDurationTicks reached |
| причина закрытия | close reason | re: CloseReason field |
| причина отказа | refusal reason | re: RefusalReason field |
| истощение резерва | reserve exhaustion | |
| продление (lease) | extension / renewal (of a lease) | |
| канал (заклинания) | (spell) channel | The continuous-drain magic mechanic |
| поддержка щита | shield maintenance | |
| активный ритуал | active ritual | |
| наивная схема | naive scheme / naive approach | re: "проверить на старте, списывать каждый тик" |
| двухфазный commit | two-phase commit | DB term, keep English form |

## 6. ECS, threading & event bus

| Russian | English | Notes |
|---|---|---|
| мир (World) | world (`World`) | Code identifier in backticks |
| сущность | entity | Standard ECS |
| компонент | component | Standard ECS |
| система (как ECS-абстракция) | system | Standard ECS |
| хранилище компонентов | component store | re: `ComponentStore` |
| `[Deferred]` доставка | deferred delivery | |
| `[Immediate]` доставка | immediate delivery | |
| дренаж очереди | queue drain | The buffered events are drained between phases |
| захват контекста подписчика | subscriber context capture | When an event handler runs in the publisher's `SystemExecutionContext` |
| чистый контекст | clean context | After PopContext |
| TickRate | tick rate | |
| подписчик | subscriber | |
| публикация (события) | (event) publishing | |
| публиковать в шину | publish to a bus | |
| читаемые компоненты | readable components / read set | re: `reads:` in `[SystemAccess]` |
| записываемые компоненты | writable components / write set | re: `writes:` |

## 7. Modding

| Russian | English | Notes |
|---|---|---|
| моддинг | modding | |
| мод | mod | |
| мод-автор | mod author | |
| модер | modder | |
| загрузчик модов | mod loader | re: `ModLoader` |
| двухфазная валидация (мода) | two-phase mod validation | |
| атомарная пересборка графа | atomic graph rebuild | |
| цикл загрузки | loading cycle / loading order | Context-dependent |
| жёсткая зависимость | hard dependency | |
| nuget | NuGet | Capitalisation matters |
| контракт между модами | inter-mod contract | |
| мод-контракт | mod contract | re: IModContract |

## 8. Game / world domain

| Russian | English | Notes |
|---|---|---|
| колониальный симулятор | colony simulator | Industry-standard genre name |
| колония | colony | |
| **пешка** | **pawn** | **✓ LOCKED v1.0** — see §11.5. Industry-standard term for the colony-sim genre (RimWorld). `colonist` rejected as too narrow (a pawn can be a raider, animal, golem, etc.); `unit` rejected as military-sterile. |
| ветка (технологическая) | tech branch / tech tree | "Tree" is more common in games, "branch" more accurate to the source which describes two parallel branches |
| индустриальная ветка | industrial branch | |
| **аркано-магическая ветка** | **arcane branch** | **✓ LOCKED v1.0** — see §11.3. The Russian compound `аркано-магическая` is redundant in English: `arcane` already implies magical. The shortened form reads cleaner and aligns with how RimWorld-adjacent docs describe magical tech trees. |
| биология рас | racial biology / species biology | "Race" is loaded in English social discourse — "species" may be safer for a game about non-human peoples; depends on lore |
| Gifted human | Gifted human | English already; capitalised as archetype name |
| голем | golem | |
| мана | mana | |
| **эфир** | **ether** | **✓ LOCKED v1.0** — see §11.4. Code already uses `EtherGridSystem`, `EtherNodeChanged`, `EtherSurge`, `EtherComponent`, `EtherNodeComponent`. Locking `ether` is the path of least resistance; switching to `aether` would require renaming reserved code identifiers (forbidden by §16.4). |
| школа Пустоты | School of the Void | Capitalised as a proper school name |
| битва богов (stress-сценарий) | Battle of the Gods (stress scenario) | Internal stress-test name; capitalise |
| снаряд | projectile | |
| урон | damage | |
| заклинание | spell | |
| ритуал | ritual | |
| раид | raid | |
| биом | biome | |
| погода | weather | |
| настроение | mood | |
| потребности | needs | |
| смерть | death | |
| статус-эффект | status effect | |

## 9. Process & development practice

| Russian | English | Notes |
|---|---|---|
| фаза (Phase 4 и т.д.) | phase | Capitalised when referring to roadmap phases: "Phase 4" |
| закрытие фазы | phase closure / closing a phase | |
| снимок (репозитория) | snapshot | |
| архитектурный долг | architectural debt | |
| дисциплина | discipline | |
| гигиена разработки | development hygiene | |
| чек-лист гигиены | hygiene checklist | |
| машинно-проверяемый инвариант | machine-checkable invariant | |
| машинно-проверяемый | machine-checkable | |
| наименование | naming | |
| структура файлов | file structure | |
| scope-префикс коммита | commit scope prefix | |
| ревью PR | PR review | |
| pull request отклоняется | PR is rejected | |
| эмпирические данные | empirical data | |
| эмпирика | empirical record / empirical evidence | "Empirics" exists but is rare in English |
| метрика | metric | |
| контекст (в токенах) | context (in tokens) | |
| артефакт (сгенерированный) | (generated) artifact | |
| исходный размер артефакта | output artifact size | |
| архивный (мейнтейнер) | maintenance / archived (maintainer) | Context-dependent |
| дефект | defect | |
| регрессия | regression | |
| stress-сценарий | stress scenario | English already |
| unit-тест | unit test | |
| интеграционный тест | integration test | |
| тест сторожа изоляции | isolation guard test | |
| тест моддинга | modding test | |

## 10. Threat model & security (§4.1 of METHODOLOGY)

| Russian | English | Notes |
|---|---|---|
| модель угроз | threat model | |
| класс рисков | risk class | |
| broad-permissions агент | broad-permissions agent | English already |
| широкие права | broad permissions | |
| минимальные права | minimal permissions / least privilege | |
| привилегии | privileges | |
| канал атаки | attack channel | |
| канал исполнения | execution channel | |
| сетевой доступ | network access | |
| shell-доступ | shell access | |
| файловая система | filesystem | One word in technical English |
| цепочка действий | action chain | |
| подтверждение пользователя | user confirmation | |
| persistent backdoor | persistent backdoor | English already |
| data exfiltration | data exfiltration | English already |
| lateral movement | lateral movement | English already |
| skill (расширение агента) | skill | English loanword; OpenClaw uses this |
| skill-репозиторий | skill repository | |
| malicious skill | malicious skill | English already |
| heartbeat-режим | heartbeat mode | English already |
| структурная защита | structural defence | "Defense" if US spelling |
| структурно невозможно | structurally impossible | |

## 11. Translation decisions to lock before bulk translation

Pass 1 (2026-04-26) closed these. 7 of 9 are locked as `✓ LOCKED v1.0`; 2 are escalated to the project author as `⚠ ESCALATE TO HUMAN` because they are identity choices, not lexical ones.

### 11.1 "Владелец смысла" — role label for the human — ⚠ ESCALATE TO HUMAN

Russian source: "Владелец смысла. Отвечает за выбор контрактов, архитектурные решения, формулировку целей фаз, приёмку результата против критериев."

Candidates:

| Candidate | Pros | Cons |
|---|---|---|
| **Intent owner** (originally recommended) | Clean, parallels "intent owner" / "intent-driven design" idiom in product engineering; preserves «владелец X» structure | **`Intent` is an actively-used technical term** — confirmed in §5 (`Intent → Granted/Refused`), §3, and `IntentBatcher`/`ManaIntent`/`AmmoIntent`/`DamageIntent` code identifiers. `intent owner` would read as "owner of an Intent message" in any architecture context. |
| Direction owner | Captures "long-term trajectory" reading; no collision with project terminology | Slightly bureaucratic; reframes the role from "owns the meaning of the work" to "owns the long-term direction" — subtle identity shift |
| Meaning steward | Most literal | Sounds philosophical, not engineering |
| Architect of intent | Strongest, most poetic | Conflicts with "architect" already used for the Opus role |
| Owner of meaning | Literal | Reads like a translation, not an English term |

**Pass 1 verdict.** The collision risk flagged in v0.2 is real, not theoretical. The mechanical fallback (`direction owner`) is methodologically sound but it changes how the project author is described in every methodology document — that is an identity decision, not a glossary decision. Pass 2 cannot translate `METHODOLOGY.md` until the project author chooses one of: (a) `direction owner` (accept the reframing), (b) keep `intent owner` and accept the collision (Pass 2 disambiguates contextually), (c) propose a new candidate. **Awaiting human decision.**

### 11.2 "Сторож" — isolation runtime — ✓ LOCKED v1.0 → `isolation guard`

`SystemExecutionContext.cs` already opens with `/// Isolation guard for systems.` — the only English form already in the codebase. Locking aligns the glossary with reality. Alternatives `sentinel` / `watchdog` / `monitor` rejected as adding semantic noise without distinguishing benefit.

### 11.3 "Аркано-магическая ветка" — ✓ LOCKED v1.0 → `arcane branch`

The Russian compound `аркано-магическая` is tautological in English: `arcane` already implies magical. Pass 1 audited the lore docs (`docs/`) and found no evidence of an `arcane` vs `magical` distinction (e.g. rune-based vs spell-based). If lore later introduces such a distinction, the glossary is amended via `⚠ ADDED v1.x` (§16.2 lifecycle), not retroactively unlocked.

### 11.4 "Эфир" → ✓ LOCKED v1.0 → `ether`

Code identifiers already use `Ether*`: `EtherGridSystem`, `EtherNodeChanged`, `EtherSurge`, `EtherComponent`, `EtherNodeComponent`. Renaming code identifiers is forbidden (§16.4); locking `ether` is the path of least resistance. Alternative `aether` (fantasy/mystical register) rejected — would require renaming reserved identifiers.

### 11.5 "Пешка" → ✓ LOCKED v1.0 → `pawn`

RimWorld is the industry standard for the colony-sim genre. `pawn` covers raiders, animals, golems, and colonists; `colonist` is too narrow; `unit` is too military-sterile. Code already uses `Pawn`, `PawnBus`, `PawnDiedCommand`, `PawnSpawnedCommand`.

### 11.6 US vs UK spelling — ✓ LOCKED v1.0 → `US English`

Technical documentation uses **US English**: `color` / `behavior` / `defense` / `quantized` / `modeling` / `analyze` / `centralize` / `parallelize` / `synchronize`. Reasons: code identifiers already use US (`Color`, `Behavior`); GitHub/MDN/most tech docs default to US; US spelling has wider global recognition for technical English. **Implementation note for Pass 2/3:** existing English-column entries in this glossary that still use `-ise` / `-our` forms (e.g. `quantised`, `parallelise`, `centralise`, `defence`, `behaviour`) are non-blocking inconsistencies — Pass 2 normalizes them in-place when consuming the entry, and a follow-up `chore(normalize): align glossary English column to US lock` commit may sweep them in bulk.

### 11.7 "Race" vs "species" — ⚠ ESCALATE TO HUMAN

Pass 1 cannot resolve this without lore input. The choice depends on a content question only the project author can answer:

- If the lore has **only one taxonomic level** (e.g. humans, dwarves, golems are biologically distinct peoples and there is no concept of "races within humans") → lock as `species` (neutral, biologically accurate, avoids social-discourse loading).
- If the lore has **two taxonomic levels** (e.g. humans subdivide into `Northern`, `Desert`, `Forest` races; separately there are dwarves as a distinct species) → both terms are needed with explicit distinction: `species` for the cross-people level, `race` for intra-species variation.

**Awaiting human decision.** Pass 2 cannot translate `RaceComponent.cs`, `биология рас`, or any pawn-biology section of `METHODOLOGY.md` until this is resolved.

### 11.8 "Falsifiable утверждение" — ✓ LOCKED v1.0 → `falsifiable claim`

Popper's terminology. `falsifiable statement` rejected (less common in scientific-method discourse); `testable claim` rejected (loses Popperian register — testability ≠ falsifiability).

### 11.9 Verbal register — ✓ LOCKED v1.0 → `active voice (passive only for roadmap items)`

Russian source uses passive constructions ("Решение по batch-API отложено", "Реализация перенесена") that map cleanly to English active voice ("The batch-API decision is deferred", "Implementation is moved"). Lock: prefer active for normative claims and architectural narrative; passive is acceptable for roadmap entries that match the project register. See §14.1, §14.2, §14.7 for canonical phrase patterns. Pass 2 applies this rule uniformly to every translated paragraph.

---

## 12. Style notes for the translator

These are register-level shifts, not glossary entries.

1. **Sentence length.** Russian source has long, dense sentences with many subordinate clauses. English technical writing prefers shorter sentences. When in doubt, split.

2. **Inversions.** Russian "Решение по batch-API отложено" → English "The batch-API decision is deferred" (active object). Don't preserve the inversion.

3. **"Это [structure]"** Russian "Это позволяет планировщику…" → English "This lets the scheduler…" (active verb, not the «to-permit» literal).

4. **Capitalisation of architectural terms.** Layers (Presentation, Application, Domain, Infrastructure) are capitalised when referring to *the layer*, lowercase when referring to *generic concept*. "The Domain layer is multithreaded." vs. "domain logic prefers events."

5. **Code identifiers in prose.** Always backticked: `[SystemAccess]`, `IModContract`, `World`. Never translated, never localised.

6. **Quotation marks.** Russian «», English "" (curly) or `""` (straight, in code). Pick straight quotes for consistency in markdown.

7. **Em-dashes.** Russian frequently uses em-dashes for emphasis: "Игра — test case." English uses them more sparingly. Often a colon or a comma reads better: "The game is a test case." or "The game: a test case."

8. **"Принципиально" / "Существенно" / "По существу"** — Russian uses these for emphasis, English idiom prefers "Crucially," / "The key point is," / "What matters is,". Don't transliterate.

---

## 13. Reserved code identifiers — NEVER translated ★ NEW IN v0.2

This section enumerates every name treated as code, not text. These never translate, never localise, never abbreviate, and never get italicised in prose. They are always rendered in backticks when appearing in markdown.

The principle behind the list: if the name appears in the C# source as a class, interface, namespace, attribute, method, property, file path, or NuGet package reference, it is reserved. The Russian source already preserved most of these in their English form, but enumerating them removes the residual ambiguity.

### 13.1 Project namespaces and assemblies

`DualFrontier.Contracts`, `DualFrontier.Core`, `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Systems`, `DualFrontier.AI`, `DualFrontier.Application`, `DualFrontier.Presentation`, `DualFrontier.Persistence`, `DualFrontier.Mod.Example` and any `DualFrontier.*` extension — all reserved.

### 13.2 Architectural layer assemblies (always capitalised, never translated)

Presentation, Application, Domain, Infrastructure, Contracts. When used as proper nouns referring to *the layer*, capitalised; when referring to a generic concept ("domain logic"), lowercase. See §12.4.

### 13.3 Core ECS identifiers

`World`, `EntityId`, `IComponent`, `IEvent`, `SystemBase`, `ComponentStore`, `SparseSet`, `Query`, `DependencyGraph`, `ParallelSystemScheduler`, `SystemPhase`, `SystemRegistry`, `ComponentRegistry`, `SystemExecutionContext`, `IsolationViolationException`, `ModIsolationException`, `SystemOrigin`.

### 13.4 Bus and contract identifiers

`IGameServices`, `ICombatBus`, `IInventoryBus`, `IMagicBus`, `IPawnBus`, `IWorldBus`, `IPowerBus`, `IEventBus`, `DomainEventBus`, `IntentBatcher`, `IModContract`, `IModApi`, `IMod`.

### 13.5 Attributes

`[SystemAccess]`, `[TickRate]`, `[Deferred]`, `[Immediate]`, `[BridgeImplementation]`, `[InternalsVisibleTo]`. Always shown with brackets when standalone, without brackets when only the type name is referenced (`SystemAccessAttribute`).

### 13.6 Identifier-shaped game terms

These are words from §8 (game/world domain) that became code identifiers. The C# code wins — the English form in code is the canonical English form everywhere:

| Russian | C# code identifier | English in prose |
|---|---|---|
| мана | `Mana`, `ManaComponent`, `ManaSystem`, `ManaIntent` | mana |
| эфир | `Ether`, `EtherGridSystem`, `EtherNodeChanged`, `EtherSurge` | ether |
| голем | `Golem`, `GolemBondComponent`, `GolemSystem` | golem |
| пешка | `Pawn`, `PawnBus` (the bus is named `Pawns`, plural) | pawn |
| здоровье | `Health`, `HealthComponent` | health |
| настроение | `Mood`, `MoodComponent`, `MoodBreakEvent` | mood |
| урон | `Damage`, `DamageEvent`, `DamageIntent`, `DamageSystem` | damage |
| заклинание | `Spell`, `SpellSystem`, `SpellCastEvent` | spell |
| ритуал | `Ritual` (when in code) | ritual |

### 13.7 .NET / BCL terms

Always English form: `AssemblyLoadContext`, `Reflection`, `ConcurrentDictionary`, `ThreadLocal`, `ParallelOptions`, `Task`, `async`/`await`, `nameof`, `typeof`, `Action<T>`, `Func<T>`, `IEnumerable<T>`, `IDisposable`, `IComparable`, etc. Russian source sometimes spells these out in Cyrillic ("рефлексия", "тред-локальный") — translate to the English form.

### 13.8 Industry-standard CS terms (kept English)

ECS, IPC, RPC, REST, JSON, XML, SemVer, P/Invoke, GC, JIT, NUMA, BCL, DI, MVVM, OOP, FP, TOCTOU, RLE, A*, OAuth, MCP, LLM, QA. When the Russian source uses transliterated form ("ИПС", "ДжиСи"), normalise to the standard English acronym.

### 13.9 Tooling

`Godot`, `Silk.NET`, `LM Studio`, `Cline`, `VS Code`, `xUnit`, `FluentAssertions`, `BenchmarkDotNet`, `Harmony`, `git`, `dotnet`, `NuGet`, `Roslyn`, `MSBuild`. Capitalisation and dotted form preserved exactly.

### 13.10 Model names

`Gemma`, `Sonnet`, `Opus`, `Claude`, `GPT-4`, `Claude Code`, `Claude Opus 4.7`, `Claude Sonnet 4.6`, `Gemma 4 E4B`. When Russian source writes "Соннет", "Опус", normalise to English.

---

## 14. Phrase-level translation patterns ★ NEW IN v0.2

Word-for-word substitution from §1–§10 is necessary but not sufficient. Russian and English diverge on idiomatic constructions: the literal calque is grammatical but reads as translatese. This table catalogues recurring Russian patterns in the source and their canonical English forms.

### 14.1 "Это [permits/allows/lets]" constructions

| Russian | Avoid (translatese) | Canonical English |
|---|---|---|
| Это позволяет планировщику параллелить системы | This permits the scheduler to parallelise systems | The scheduler can parallelise systems / This lets the scheduler parallelise systems |
| Это даёт возможность мод-автору... | This gives the mod author the possibility to... | Mod authors can... |
| Это обеспечивает развязку | This provides decoupling | Decoupling follows / The result is decoupling |

Rule: prefer naming the actor as subject ("the scheduler can…") over abstract "this/it permits/allows." Drop "this" when the antecedent is the immediately preceding sentence.

### 14.2 Roadmap and decision passive

| Russian | Canonical English |
|---|---|
| Решение по batch-API отложено до Phase 9 | The batch-API decision is deferred to Phase 9 |
| Принято решение использовать ether вместо aether | We chose `ether` over `aether` |
| Реализация перенесена в Phase 6 | Implementation is moved to Phase 6 |

Rule: passive is acceptable for roadmap items (it matches the project register). Prefer active ("we chose", "we deferred") for explicitly justified decisions.

### 14.3 Emphasis adverbs

| Russian | Avoid | Canonical |
|---|---|---|
| Принципиально, что X | Principially, X | Crucially, X / The key point: X |
| По существу, X | Essentially, X (overused) | At its core, X / Effectively, X |
| Существенно, что X | It is substantial that X | What matters: X / Crucially, X |
| По сути X | In essence X | X (often dropping the prefix entirely) |
| Грубо говоря | Roughly speaking | Roughly: / In broad strokes: |
| Строго говоря | Strictly speaking | Strictly: / Precisely: |

### 14.4 Concession and contrast

| Russian | Canonical |
|---|---|
| С одной стороны… с другой… | On one hand… on the other… |
| Тем не менее | Even so / Nonetheless |
| Несмотря на это | Despite this / That said |
| Однако | However / But |
| Вместе с тем | At the same time / Also |

### 14.5 Discipline and obligation language (RFC 2119 register)

The Russian source uses imperative "обязан", "должен", "запрещён" extensively for architectural rules. Map to RFC-2119-style English in technical contexts:

| Russian | Canonical |
|---|---|
| Система обязана задекларировать доступ | A system MUST declare its access |
| Не должен ссылаться напрямую | MUST NOT reference directly |
| Запрещено использовать async | `async` is forbidden / Using `async` is forbidden |
| Должен быть зелёным | MUST be green |
| Рекомендуется | SHOULD / Recommended |
| Желательно | SHOULD (weaker) / Preferable |

Rule: for architectural rules and invariants — uppercase MUST/MUST NOT/SHOULD. For prose and casual contexts — lowercase form.

### 14.6 Causal and consequential constructions

| Russian | Canonical |
|---|---|
| Поэтому X | Hence X / Therefore X |
| Из-за этого X | As a result, X |
| Это приводит к X | This produces X / X follows |
| Следовательно | Consequently |

### 14.7 Architectural narrative

| Russian | Avoid | Canonical |
|---|---|---|
| Сторож следит, чтобы X | The guard watches that X | The guard ensures X / The guard rejects X otherwise |
| Шина не держит ссылок на системы | The bus does not hold references to systems | The bus holds no system references |
| Контракт фиксирует не пожелание, а условие | The contract fixes not a wish but a condition | The contract specifies a condition, not a preference |

### 14.8 Hedging and confidence

The Russian source frequently hedges with "по-видимому", "возможно", "похоже". English technical writing reads better with cleaner confidence levels:

| Russian | Canonical |
|---|---|
| По-видимому | Apparently / It appears |
| Возможно | Possibly (avoid in spec contexts; commit one way) |
| Похоже, что X | It looks like X / Likely X |
| Скорее всего | Most likely / Probably |
| Очевидно | Clearly / Obviously |

Rule: in normative documentation (CONTRACTS, ISOLATION, ARCHITECTURE) — minimise hedging. Either commit to a claim or remove it.

---

## 15. User-facing string contracts ★ NEW IN v0.2

This section enumerates strings that are simultaneously: (a) emitted by code at runtime, (b) documented as format specifications, (c) asserted in tests via substring matching. These are the most rigid translation surface in the project — changing any single one is a three-way change (doc + code + tests) that must commit atomically.

Pass 1 (2026-04-26) locked the English forms below as `✓ LOCKED v1.0`. Pass 3 will execute the three-way change atomically using the `IsolationDiagnostics` constant pool specified in `docs/NORMALIZATION_REPORT.md` §5.

### 15.1a Isolation guard — undeclared **read** access — ✓ LOCKED v1.0

**Source location:** `SystemExecutionContext.BuildReadViolationMessage` (called from `GetComponent<T>` and `Query<T>`/`Query<T1,T2>`). File `src/DualFrontier.Core/ECS/SystemExecutionContext.cs`, lines 282–292.

**Russian (current source):**
```
[ИЗОЛЯЦИЯ НАРУШЕНА]
Система '{systemName}'
обратилась к '{typeName}'
без декларации доступа.
Добавь: [SystemAccess(reads: new[]{typeof({typeName})})]
```

**English (locked target):**
```
[ISOLATION VIOLATED]
System '{systemName}'
accessed '{typeName}'
without an access declaration.
Add: [SystemAccess(reads: new[]{typeof({typeName})})]
```

**Notes:** "обратилась" → "accessed" (past tense, completed action). The hint preserves exact C# code that the developer copy-pastes — `typeof({typeName})` syntax is untranslatable. Newline structure preserved (each clause on its own line for readability in console output). Test-asserted substring tokens: `accessed`, `without an access declaration`, `Add: [SystemAccess`.

### 15.1b Isolation guard — undeclared **write** access — ✓ LOCKED v1.0

**Source location:** `SystemExecutionContext.BuildWriteViolationMessage` (called from `SetComponent<T>`). File `src/DualFrontier.Core/ECS/SystemExecutionContext.cs`, lines 294–304. Pass 1 added this as a separate sub-entry because the v0.2 draft conflated the read and write variants into one entry (§15.1), but the code emits them through two distinct methods with distinct verb and reason tokens.

**Russian (current source):**
```
[ИЗОЛЯЦИЯ НАРУШЕНА]
Система '{systemName}'
модифицирует '{typeName}'
без декларации записи.
Добавь: [SystemAccess(writes: new[]{typeof({typeName})})]
```

**English (locked target):**
```
[ISOLATION VIOLATED]
System '{systemName}'
modified '{typeName}'
without a write declaration.
Add: [SystemAccess(writes: new[]{typeof({typeName})})]
```

**Notes:** "модифицирует" → "modified" (past tense, parallel to 15.1a). Test-asserted substring tokens: `modified`, `without a write declaration`, `Add: [SystemAccess`.

### 15.2 Isolation guard — direct system access — ✓ LOCKED v1.0

**Source location:** `SystemExecutionContext.GetSystem<TSystem>` (always throws, including in Release builds — semantic violation, not a debug-only check). File `src/DualFrontier.Core/ECS/SystemExecutionContext.cs`, lines 256–262.

**Russian (current source):**
```
[IsolationViolationException]
Прямой доступ к системам запрещён.
Используй EventBus вместо прямой ссылки на систему.
```

**English (locked target):**
```
[IsolationViolationException]
Direct access to systems is forbidden.
Use the EventBus instead of a direct system reference.
```

**Notes:** Pass 1 verified that the actual code emits the body and hint joined by `Environment.NewLine` (two lines, not three as the v0.2 draft showed). Glossary aligned to code reality. Test-asserted substring token: `Direct access to systems is forbidden`.

### 15.3 Isolation guard — direct World access — ✓ LOCKED v1.0 (doc-only, not yet emitted by code)

**Source location:** Currently a documentation-only specification in `docs/architecture/ISOLATION.md` lines 168–172. **Not yet implemented in `SystemExecutionContext`.** Pass 1 verified by reading the file: the runtime check that would emit this message does not exist; the doc shows the *intended* future format. A future phase (likely Phase 5 of the original roadmap, or whichever phase wires `World`-bypass detection) implements the runtime check; this lock pre-commits the wording so doc and code converge.

**Russian (current docs/architecture/ISOLATION.md text):**
```
[IsolationViolationException]
Система 'XxxSystem' вызвала World.GetComponentUnsafe напрямую.
Используй SystemBase.GetComponent / Query вместо прямого World-доступа.
```

**English (locked target):**
```
[IsolationViolationException]
System '{systemName}' called World.GetComponentUnsafe directly.
Use SystemBase.GetComponent / Query instead of direct World access.
```

### 15.4 Isolation guard — wrong bus publishing — ✓ LOCKED v1.0 (doc-only, not yet emitted by code)

**Source location:** Currently a documentation-only specification in `docs/architecture/ISOLATION.md` lines 178–183. **Not yet implemented in `SystemExecutionContext`.** Pass 1 confirmed by inspection of the constructor (`_allowedBuses` is captured but never validated against an actual `Publish` call). A future phase wires the bus-publication check; this lock pre-commits wording.

**Russian (current docs/architecture/ISOLATION.md text):**
```
[IsolationViolationException]
Система 'CombatSystem' публикует 'ItemAddedEvent'
в шину 'Inventory', но декларирует только 'Combat'.
Либо измени шину события, либо добавь в декларацию системы.
```

**English (locked target):**
```
[IsolationViolationException]
System '{systemName}' publishes '{eventName}'
to bus '{busName}' but declares only '{declaredBus}'.
Either change the event's bus or add it to the system's declaration.
```

### 15.5 Mod fault banner (UI-facing) — ✓ LOCKED v1.0 (doc-only, not yet emitted by code)

**Source location:** Currently a documentation-only specification in `docs/architecture/ISOLATION.md` lines 86–90. **Not yet emitted by code:** Pass 1 audited the codebase and found no UI banner implementation in `PresentationBridge`, `AlertPanel`, or any `ModFaultHandler` (the latter does not yet exist as a concrete class — only the specification in ISOLATION.md §"ModFaultHandler — жизненный цикл при нарушении мода"). A future phase implements the banner; this lock pre-commits the wording so the future implementation can drop in canonical text without re-litigation.

**Russian (verbatim from `docs/architecture/ISOLATION.md` lines 86–90, lock-aligned):**
```
Мод «{modName}» нарушил изоляцию
и был автоматически выключен.
Игра продолжит работу без него.
Подробности — в логе: logs/mods/{modId}.log
```

**English (locked target):**
```
Mod "{modName}" violated isolation
and was automatically disabled.
The game will continue without it.
Details in log: logs/mods/{modId}.log
```

**Notes:** Pass 1 corrected the v0.2 draft's "presumed" Russian — the actual `ISOLATION.md` text is more verbose (4 lines, separate explanation and log path) and uses guillemets (`«»`) around `{modName}`. The English form normalises to ASCII double quotes per §12.6. Tone is informative-neutral, not accusatory ("violated isolation" rather than "is broken"). Test-asserted substring tokens (when implemented): `violated isolation`, `automatically disabled`, `Details in log`.

### 15.6 PR rejection format (developer-facing, doc-only)

`docs/architecture/ISOLATION.md` ends with: «Если хотя бы один пункт не выполнен — pull request отклоняется.» This is doc text, not code, but it sets a register for the project. Canonical English: "If any item is not satisfied, the PR is rejected." Use this exact phrasing throughout.

### 15.7 Test assertion patterns

After Pass 3 introduces `IsolationDiagnostics` constants, tests assert against constant references, not literals. The wildcard prefix in `WithMessage("*Foo*Bar*")` patterns stays unchanged because `Foo`/`Bar` are identifiers (system name, type name) — not localised text. Only the prose substring portion of `.Contain(...)` switches from Russian literal to constant reference.

---

## 16. Glossary maintenance & versioning ★ NEW IN v0.2

### 16.1 Lifecycle

- **v0.x (draft).** Content under construction. ⚠ DECISION items present. Not safe for bulk translation.
- **v1.0 (locked).** All ⚠ DECISION items resolved. Pass 1 produces this. Pass 2 and 3 use it as authoritative.
- **v1.x (post-launch additions).** Terms encountered during Pass 2/3 that were missing from v1.0 and required ad-hoc decisions are added here, marked `⚠ ADDED v1.x`, and reviewed by the human after the pass closes.
- **v2.0+ (revision).** Reserved for major changes (new domain area, methodology shift, changed authority). Out of scope for the initial translation campaign.

### 16.2 The "stop, escalate, update" rule

When any agent (Opus, Sonnet, human) encounters a Russian term during translation that is **not** in §1–§10, **not** covered by §13 reserved identifiers, and **not** matched by §14 phrase patterns:

1. **Stop translating that passage.**
2. **Open `PASS_N_NOTES.md`**, add an entry under "Glossary additions needed":
   - The Russian term in context.
   - The proposed English equivalent.
   - The rationale.
   - The first source location encountered.
3. **Continue with other passages** that don't depend on this term.
4. At pass closure, the human (or Opus during a follow-up session) reviews and either adds to glossary as `⚠ ADDED v1.x` or escalates to a design decision.

This rule applies symmetrically: even if the proposed English equivalent feels obvious, agents do not insert it without the audit trail. The cost of one extra commit is negligible; the cost of a silent ad-hoc lexical choice is invisible drift.

### 16.3 Conflict resolution

When two glossary entries pull in different directions for a single passage (e.g., "intent owner" from §1 conflicts with "intent" from §3 in the same paragraph), the agent **always prefers preservation of distinction** over substitution convenience. If "intent owner" makes a paragraph confusing, the answer is to rephrase the paragraph (or rename the role label, escalation), not to pick a different translation for "intent" mid-passage.

### 16.4 Out-of-scope flags

The glossary does not authorise:

- Renaming code identifiers. If a name is reserved (§13), it is reserved. Translation never renames it.
- Changing methodology. Even if a glossary term seems methodologically wrong, Pass 1's job is lexical, not methodological.
- Resolving open architectural questions in `TRANSLATION_PLAN.md` §6. Those go through human + Pass 1, not via glossary edit.

### 16.5 Compliance verification

Pass 4 (verification) explicitly checks glossary compliance via random-sample audit (per `PASS_4_PROMPT.md` task 3). A finding of systematic non-compliance triggers a Pass 5 surgical-fix session, not a glossary update. The glossary is the standard; deviations get fixed *to* the standard, not *from* it.

---
