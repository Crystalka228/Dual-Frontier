---
register_id: DOC-D-K8_DECISION
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: 1.0
first_authored: 2026-05-09
last_modified: 2026-05-09
content_language: en
next_review_due: null
title: K8 — Decision Brief
review_cadence: on-status-transition
reviewer: Crystalka
---

# K8 — Decision step + production cutover

**Status**: SKELETON
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K8
**Prerequisite**: K7 complete

## Goal

Apply §8 rule к K7 numbers. Decide path forward (Outcome 1/2/3). Execute decision (cutover, optional, или park).

## Time estimate

1 week (depending on outcome)

## Deliverables (high-level)

**Outcome 1 — native wins**: replace managed `World` с `NativeWorld`, migrate Application bootstrap, deprecate managed World, update modding API к v3.

**Outcome 2 — managed-with-structs wins**: native kernel optional optimization, components remain structs, document «native available но не required».

**Outcome 3 — equivalent**: park kernel work, document lessons learned.

## TODO

- [ ] Author full brief (after K7 closes — outcome-dependent content)
- [ ] Include decision matrix (which metrics → which outcome)
- [ ] Include cutover protocol (Outcome 1)
- [ ] Include rollback protocol if cutover fails
- [ ] Include acceptance criteria per outcome

**Brief authoring trigger**: after K7 results available.