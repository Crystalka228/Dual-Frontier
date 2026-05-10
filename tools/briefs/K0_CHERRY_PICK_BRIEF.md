# K0 — Cherry-pick + cleanup от experimental branch

**Status**: SKELETON — full brief authored when ready к execute
**Reference**: `docs/architecture/KERNEL_ARCHITECTURE.md` Part 2 §K0
**Source**: `claude/cpp-core-experiment-cEsyH` per `docs/CPP_KERNEL_BRANCH_REPORT.md` §11.6

## Goal

Cherry-pick 7 substantive C++ commits от experimental branch onto current main. Clean up hygiene issues. Reconcile decision rule conflict.

## Time estimate

1-2 days

## Deliverables (high-level)

- Cherry-pick sequence (7 commits)
- Cleanup commits (`.gitignore`, dead code, `.vscode`, doc reconciliation)
- Single atomic commit chain ending с «native: kernel branch resumption + cleanup»

## Status: EXECUTED

**Date**: 2026-05-07
**Branch**: `feat/k0-kernel-cherry-pick`
**Closure SHA**: `89a4b24` (last K0-substantive commit — SparseSet retention annotation)

Full executable brief was authored separately by Opus and consumed in-session
to drive this milestone. See git log on `feat/k0-kernel-cherry-pick` for the
full cherry-pick + cleanup sequence and `docs/MIGRATION_PROGRESS.md` K0 entry
for the closure record (cherry-pick SHAs, cleanup commits, lessons learned).
