---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-TOOLS-GOVERNANCE
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-TOOLS-GOVERNANCE
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-F-TOOLS-GOVERNANCE
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-F-TOOLS-GOVERNANCE
---
# tools/governance — Register tooling module

*Module-local index for governance tooling artifacts. Tier 4 Category F per A'.4.5 governance framework.*

---

## Purpose

This module houses operational tooling for the Document Control Register defined by [`docs/governance/FRAMEWORK.md`](../../docs/governance/FRAMEWORK.md). Three PowerShell scripts + two YAML/MD support files.

## Artifacts

### `sync_register.ps1` — write-side sync + validation

Synchronizes [`docs/governance/REGISTER.yaml`](../../docs/governance/REGISTER.yaml) with per-document frontmatter mirrors; validates schema and cross-reference integrity. Primary closure-gate per Q-A45-X5 post-session protocol.

```powershell
./tools/governance/sync_register.ps1 --validate    # validate only (no writes)
./tools/governance/sync_register.ps1 --sync        # sync REGISTER.yaml → frontmatter mirrors
./tools/governance/sync_register.ps1               # default: sync + validate
```

Exit codes: `0` all green / `1` validation errors / `2` tooling failure.

### `query_register.ps1` — read-side queries

Agent and human query interface; returns structured results without manual YAML parsing.

```powershell
./tools/governance/query_register.ps1 --tier 1 --lifecycle LOCKED
./tools/governance/query_register.ps1 --requirement REQ-K-L11
./tools/governance/query_register.ps1 --risks-affecting DOC-A-KERNEL
./tools/governance/query_register.ps1 --stale
./tools/governance/query_register.ps1 --capa-open
./tools/governance/query_register.ps1 --content-language ru
```

Output formats: default table / `--json` / `--paths-only`.

### `render_register.ps1` — human-readable derivative

Generates `docs/governance/REGISTER_RENDER.md` from REGISTER.yaml — browsable view, indexed by category, with statistics + global collection summaries.

```powershell
./tools/governance/render_register.ps1
```

### `SCOPE_EXCLUSIONS.yaml`

Glob patterns for paths excluded from register enrollment (build outputs, Godot internals, BenchmarkDotNet artifacts). Sync_register reads this to suppress orphan-flagging on out-of-scope `.md` files.

### `MODULE.md`

This file. Tier 4 Category F module-local README. Enrolled in register as `DOC-F-TOOLS-GOVERNANCE`.

## Dependencies

PowerShell 5.1 or PowerShell Core (`pwsh`). YAML parsing via [`powershell-yaml`](https://github.com/cloudbase/powershell-yaml) module (`Install-Module powershell-yaml -Scope CurrentUser`). Bundled fallback parser used if module unavailable.

UTF-8 file encoding for YAML and Markdown (BOM stripped on write).

## See also

- [docs/governance/FRAMEWORK.md](../../docs/governance/FRAMEWORK.md) — governance framework specification
- [docs/governance/REGISTER.yaml](../../docs/governance/REGISTER.yaml) — operational SoT
- [docs/governance/SYNTHESIS_RATIONALE.md](../../docs/governance/SYNTHESIS_RATIONALE.md) — source-standards provenance
- [docs/methodology/METHODOLOGY.md](../../docs/methodology/METHODOLOGY.md) §12 — register integration into closure protocol
