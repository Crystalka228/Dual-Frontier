---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-A_PRIME_0_5_TIER2_FLAGS
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-A_PRIME_0_5_TIER2_FLAGS
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-A_PRIME_0_5_TIER2_FLAGS
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-A_PRIME_0_5_TIER2_FLAGS
---
# A'.0.5 Phase 8 — Tier 2 surfaced debt (forward-flagged)

Items surfaced during A'.0.5 Phase 1-7 that have **architectural character** or **scope ambiguity** beyond mechanical Phase 8 Tier 1 fixes. Per brief §10.3, these are recorded for Crystalka deliberation in subsequent milestones.

---

## §1 K-L11 framing in kernel-area module-local docs (forward to A'.1)

**Surface**: `src/DualFrontier.Core/README.md` and `src/DualFrontier.Core/ECS/README.md` describe managed-World ECS as production storage. Per K-L11, managed World was retired as production storage; native World (in `DualFrontier.Core.Interop`) is current production; managed World retained as test fixture only.

**Why deferred to A'.1 (not addressed in A'.0.5 Phase 6)**:
- Updating these docs to reflect K-L11 + post-K8.x reality is substantive narrative refresh, not mechanical-deleted-name removal.
- A'.1 amendment plan (`docs/architecture/K_L3_1_AMENDMENT_PLAN.md` Phase 4 deliverable) propagates K-L3.1 locks into 4 LOCKED docs (KERNEL, MOD_OS, MIGRATION_PLAN, MIGRATION_PROGRESS); kernel-area module-local docs are downstream of those propagations and naturally addressed alongside.

**Recommendation**: Bundle these refreshes into A'.1 final commit (or a follow-on commit in the same milestone). Specific files:
- `src/DualFrontier.Core/README.md` Purpose section — add post-K-L11 framing
- `src/DualFrontier.Core/ECS/README.md` — add header note that this README describes managed-World test fixture (not production); production ECS via `DualFrontier.Core.Interop`

---

## §2 mods/DualFrontier.Mod.Vanilla.* placeholder READMEs (forward to A'.1 or M8+)

**Surface**: `mods/DualFrontier.Mod.Vanilla.{Core,Combat,Magic,Inventory,Pawn,World}/README.md` exist as 6 placeholder READMEs for vanilla mod skeletons. These were not refreshed in A'.0.5 Phase 6.

**Why deferred**:
- Vanilla mods are M8 milestone scope (per ROADMAP); content lands during M8 Vanilla skeleton milestone or later.
- Refreshing placeholder READMEs without knowing what content lands is speculative.

**Recommendation**: Address during M8 milestone when actual vanilla mod skeletons are authored. Could also bundle minor mechanical fixes into A'.1 if substantial drift exists at that point.

---

## §3 Module-local doc forward-looking narrative claims (no immediate action)

**Surface**: Several module-local docs contain forward-looking architectural claims (e.g., `src/DualFrontier.Components/Combat/README.md` post-Phase 5 is now thin; ROADMAP backlog mentions «Phase 5 ranged combat»; `src/DualFrontier.Components/Magic/README.md` mentions «Phase 6 ManaRegenModifierComponent»).

**Status**: SKIP — these claims describe ROADMAP backlog items, not stale architectural state. They will naturally close as the corresponding phases land.

**Recommendation**: No action. Phase 6 mechanical refresh in A'.0.5 was sufficient for current state.

---

## §4 IModApi v3 surface mentions in module-local docs (verify post-K8.4)

**Surface**: `K_L3_1_AMENDMENT_PLAN.md` describes IModApi v3 surface (RegisterManagedComponent<T> for Path β). Module-local `src/DualFrontier.Contracts/Modding/README.md` describes IModApi v2. K8.4 milestone lands v3 in code.

**Status**: VERIFY ONLY — current state v2 is correct per current code; v3 is forward-looking. No action needed in A'.0.5.

**Recommendation**: After K8.4 closure, refresh `Contracts/Modding/README.md` to describe v3 surface. Bundle with K8.4 closure commit or A'.1 if K8.4 ships before A'.1.

---

## §5 «Opus aud the result» typo (Tier 1; fixed in Phase 8 commit)

**Surface**: `docs/architecture/MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md` had typo «Opus aud the result».

**Status**: FIXED in Phase 8 Tier 1 commit. Recorded here for completeness; no further action.

---

## §6 Forward-flagged items summary

| # | Item | Forward to | Files |
|---|---|---|---|
| 1 | K-L11 managed-World framing | A'.1 | Core/README.md, Core/ECS/README.md |
| 2 | Vanilla mod placeholder READMEs | A'.1 / M8+ | mods/DualFrontier.Mod.Vanilla.{Core,Combat,Magic,Inventory,Pawn,World}/README.md |
| 3 | Module-local forward-looking claims | SKIP (no action) | Various |
| 4 | IModApi v3 docs propagation | post-K8.4 | Contracts/Modding/README.md |

Items 1-2 forwarded to A'.1 brief. Item 3 is non-action. Item 4 is post-K8.4 follow-up.
