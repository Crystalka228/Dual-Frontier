---
register_id: DOC-D-BOUNDARY_BANNER_PATCH_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
last_review_event: 'Authored + EXECUTED in one commit 2026-07-18 (BOUNDARY_BANNER_PATCH, doc-only micro-patch on main; operator chat ratification 2026-07-18). Self-enrolled via the sync scan (Draft) and flipped EXECUTED in the same commit; no EVT (PR #45 post-merge-PATCH precedent -- provenance lives in the patched docs last_review_event). Patches: law DOC-A-GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY 1.0.0 -> 1.0.1, plan DOC-A-VANILLA_SEPARATION_MIGRATION_PLAN 1.0.1 -> 1.1.0.'
title: BOUNDARY_BANNER_PATCH micro-brief -- stale-banner fixes on the two boundary docs + the ratified scaffolding ruling folded into the migration plan (MINOR)
---

# BOUNDARY_BANNER_PATCH -- Micro Execution Brief

One-commit doc-only patch on `main` (operator merges/pushes as usual). Two jobs: (1) fix the stale
body banners the W0 flips left behind; (2) record the operator's SCAFFOLDING RULING (ratified in chat
2026-07-18) in the migration plan, since it materially relaxes wave posture. No code, no lifecycle
transitions (law stays LOCKED, plan stays Live), no EVT (precedent: the PR #45 post-merge PATCH --
provenance lives in `last_review_event`).

## The scaffolding ruling (ratified, transcribe faithfully)

The current gameplay logic is NOT a product to preserve: it is a minimal conditional test harness. The
separation program may therefore DELETE and REIMPLEMENT rather than migrate-preserve. Consequences:
equivalence obligations bind ENGINE behavior only, never harness gameplay behavior; no
save-compatibility obligation exists toward pre-separation saves of the harness; vanilla content is
grown CLEAN inside mods, not rescued out of src/. The law (Definition of Done, B-1..B-6) is unchanged
-- only the migration MECHANICS get cheaper.

## Commit C1 (single commit; sync folded; validate --armed exit 0)

1. `docs/architecture/GAME_DISTRIBUTION_AND_VANILLA_BOUNDARY.md` -- PATCH 1.0.0 -> 1.0.1. Replace the
   body banner paragraph ("> Document class: AUTHORED target-axis law, pending ratification. ...") with
   a ratified-successor form: LOCKED law of the game-vs-engine composition axis, ratified 2026-07-18
   (EVT-2026-07-18-BOUNDARY_W0); keep the no-conflict framing sentence (current LOCKED corpus stays
   current-truth until waves amend it) and the evidence-base sentence verbatim. `last_review_event`
   records this PATCH.
2. `docs/architecture/VANILLA_SEPARATION_MIGRATION_PLAN.md` -- MINOR 1.0.1 -> 1.1.0. (a) Banner: replace
   "> Document class: Draft program document. ... Flips Live at ratification;" with the Live-program
   form (Live since 2026-07-18, same EVT; rows move to DONE with hashes; ends SUPERSEDED into ROADMAP at
   W8). (b) NEW section "1.1 The scaffolding ruling (2026-07-18)" directly after section 1, carrying the
   ruling above verbatim-in-substance, plus its wave consequences: W3 vertical slice is WRITTEN FRESH in
   the mod (the src/ Weather code is reference material, not a migration source); W5 becomes SLICE
   REPLACEMENT -- implement clean in the owning mod, then DELETE the src/ originals in the same closure
   (no equivalence proof against harness behavior; the ratchet census still shrinks per slice); W7
   carries NO backward-compatibility obligation toward harness-era saves (PSC schema starts clean). (c)
   Touch the three wave paragraphs (W3, W5, W7) minimally so their wording matches (b) -- e.g. W5 heading
   "Atomic slice moves" -> "Slice replacement (clean rebuild + delete)". Do not restructure anything else.
3. Run sync; `validate --armed` exit 0; commit with a body citing the chat ratification date and this
   brief. Subject: `docs(boundary): banner fixes + scaffolding ruling (law 1.0.1, plan 1.1.0 MINOR)`.

## Rails

Doc-only; no other file may change; derived artifacts regenerate via sync only; AUDIT_TRAIL untouched;
brief self-enrolls via the sync scan (Draft) and flips EXECUTED in the same commit's frontmatter --
record both in the commit body. HALT if any banner text on disk differs from what W0 landed (quote both).
