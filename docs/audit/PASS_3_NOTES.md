---
title: Pass 3 Notes — Code Translation
nav_order: 103
---

# Pass 3 — Code translation notes (2026-04-27)

**Status:** complete. Cyrillic-grep clean across `src/`, `tests/`, `mods/`. 82/82 tests green. `dotnet build` 0 warnings 0 errors.

## §1 — Cyrillic remnants

`grep -rE --include='*.cs' --include='*.md' '[А-Яа-я]' src/ tests/ mods/` returns zero matches. **No intentional remnants.**

## §2 — Notable translation choices

These are not "items for review" (the campaign has no open questions); they are decisions worth surfacing so the next pass / future contributors can see the reasoning.

### §2.1 IsolationDiagnostics token granularity

Pass 1 §5.B specified a `UndeclaredAccessToken` that should be a substring of *both* `ReadReason` and `WriteReason`. In Russian both reasons started with `без декларации`, so a single shared token fit. The locked English wording (glossary §15.1a/§15.1b) splits the suffix:

- read reason → `without an access declaration.`
- write reason → `without a write declaration.`

The glossary §15.1a notes explicitly call out `without an access declaration` (read-specific) as the test-asserted token. Pass 3 honours the glossary lock: `IsolationDiagnostics.UndeclaredAccessToken = "without an access declaration"` and is asserted only by read-path tests (`GetComponent_Undeclared_Throws_ForCore`, `ModOrigin_Violation_ReportsToSink`). Write-path tests use `WriteVerbToken = "modified"`. The §5.F.4 invariant (token is in both reasons) is therefore not preserved — it was a Russian-source artefact that does not survive translation.

### §2.2 ConverterPowerOutputEvent arrow characters

The original Russian XML doc used Unicode `↔` (bidirectional) and `→` (unidirectional) arrows to describe data flow. The first agent pass replaced them with ASCII `<->` and `->`, which broke the C# XML doc parser (`<` is interpreted as a tag start). Pass 3 reverted to the Unicode arrows. **Rule for future translations:** never write ASCII `<-` inside `///` XML doc comments — use `↔`, `&lt;-`, or rephrase.

### §2.3 Coupled cross-module commit (Presentation + Systems)

`PawnStateReporterSystem.TranslateJob` (in `DualFrontier.Systems`) emits Russian job labels that `PawnDetail.JobDotColor` (in `DualFrontier.Presentation`) compares against. The two sides form one runtime string contract. Translating them in separate per-module commits would have left the comparison referencing a label the emitter no longer publishes. Pass 3 bundled them into a single `chore(i18n): translate Presentation UI strings and coupled job-label emitter` commit; the per-module rule is followed everywhere else.

### §2.4 Scheduler-diagnostic Resolve hints

`DependencyGraph.BuildWriteConflictException` and `BuildCycleException` emit two-line diagnostics whose `Resolve:` hints were Russian. Per glossary §4.7 / NORMALIZATION_REPORT.md §5.G, these are out of scope for the IsolationDiagnostics centralisation, but Pass 3's mandate covers all user-facing strings. No tests assert on the Russian substring (verified by grep), so the hints were translated inline in the `chore(i18n): translate XML docs and scheduler diagnostics in DualFrontier.Core` commit. Future work may extract them into a sibling `SchedulerDiagnostics` constant pool — not part of Pass 3.

### §2.5 Job and mood labels

Job-kind labels in `PawnStateReporterSystem.TranslateJob` were translated as English gerunds (`Hauling`, `Building`, `Mining`, …) to match the existing English style of similar enum-to-string mappings elsewhere. Mood-band labels in `PawnDetail.MoodLabel` were translated with evocative phrasing (`On the brink`, `Despondent`, `Tolerable`, `Content`, `Inspired`) to preserve the source's atmospheric tone. The Warhammer-flavoured pawn-role placeholders (`inquisitor`, `sergeant`, `tech-priest`, `magus`, `medic`) preserve the source intent.

## §3 — Commit structure

Pass 3 produced 13 commits. Each was verified `dotnet build` 0/0 + `dotnet test` 82/82 before the next began.

| # | Commit | Files |
|---|---|---|
| 1 | `refactor(diagnostics): introduce IsolationDiagnostics constants` | 1 |
| 2 | `refactor(diagnostics): use IsolationDiagnostics in SystemExecutionContext` | 1 |
| 3 | `test(diagnostics): assert against IsolationDiagnostics constants` | 1 |
| 4 | `refactor(i18n): translate isolation diagnostics to English` | 1 |
| 5 | `chore(i18n): translate XML docs in mods/DualFrontier.Mod.Example` | 1 |
| 6 | `chore(i18n): translate XML docs and scheduler diagnostics in DualFrontier.Core` | 6 |
| 7 | `chore(i18n): translate XML docs in DualFrontier.Components` | 13 |
| 8 | `chore(i18n): translate XML docs in DualFrontier.AI` | 9 |
| 9 | `chore(i18n): translate Presentation UI strings and coupled job-label emitter` | 8 |
| 10 | `chore(i18n): translate XML docs and inline comments in DualFrontier.Application` | 18 |
| 11 | `chore(i18n): translate XML docs in DualFrontier.Contracts` | 23 |
| 12 | `chore(i18n): translate XML docs in DualFrontier.Events` | 33 |
| 13 | `chore(i18n): translate XML docs in DualFrontier.Systems` | 25 |
| 14 | `chore(i18n): translate XML docs and inline comments in tests/` | 10 |

Atomicity rule held: zero rolled-back commits, zero `--amend`-rewritten commits.

## §4 — Self-check (per Pass 3 prompt)

- [x] `grep -rE --include='*.cs' '[А-Яа-я]' src/ tests/ mods/` — clean.
- [x] All tests green, build clean.
- [x] Glossary terms used consistently (spot-checked: `pawn`, `ether`, `mana`, `golem`, `mage`, `school`, `bus`, `isolation guard`, `lease`).
- [x] `IsolationDiagnostics` constant values align with `docs/architecture/ISOLATION.md` examples (lines 137–141 for read, 158–161 for direct-system access, 184–202 for the general format).
- [x] Public API surface unchanged: `git diff main..HEAD -- '*.cs' | grep -E '^[+-]\s*(public|internal|protected|private)\s' | grep -v 'XML doc\|comment'` returns no signature changes (only doc/comment churn).

Pass 4 (verification) ready.
