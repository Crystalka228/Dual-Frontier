---
register_id: DOC-F-TOOLS-GOVERNANCE
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: '2.0'
first_authored: 2026-05-12
last_modified: 2026-11-22
content_language: en
next_review_due: null
title: Tools governance module (register tooling)
last_modified_commit: 8ee27fe
review_cadence: on-source-commit+quarterly
last_review_date: 2026-11-22
last_review_event: 'MAJOR 1.0 -> 2.0 2026-11-22 (REV_2): the module index describes the .NET governance tool and the config it reads; the three PowerShell scripts it described are gone from the tree and from the law.'
reviewer: Crystalka
---

# tools/governance -- governance tool configuration module

*Module-local index for `tools/governance/`. Tier 4 Category F.*

---

## Purpose

`tools/governance/` holds the configuration the governance tool reads, plus this index. The tool
itself is the .NET project `tools/DualFrontier.Governance` (its tests are
`tests/DualFrontier.Governance.Tests`). The law the tool mirrors is
[`docs/governance/FRAMEWORK.md`](../../docs/governance/FRAMEWORK.md) section 14: the YAML frontmatter
of each document is the source of truth; `docs/governance/REGISTER.yaml` and
`docs/governance/CURRENT_AUTHORITY_SURFACE.yaml` are DERIVED by the tool and never hand-edited. This
module restates neither the gate catalog nor the schema -- section 14 owns them.

## Artifacts

### `SCOPE_EXCLUSIONS.yaml`

The exclusion list and the extension allow-list, read by the tool's `ExclusionConfig` at
`tools/governance/SCOPE_EXCLUSIONS.yaml`. A Markdown file matching an `excluded_paths` pattern is out of
the governed scope (build outputs, tool state, scratch notes, authoring scaffolds); every other `.md`
under the repository root must carry frontmatter, or the walk reports a G-PATH error (FRAMEWORK section
14.2). The file is configuration, not a register entry: only `.md` documents are enrolled.

### `MODULE.md`

This file. Enrolled in the register as `DOC-F-TOOLS-GOVERNANCE`.

## Running the tool

```bash
dotnet run --project tools/DualFrontier.Governance -- validate --armed   # read-only; exit 0 is the gate
dotnet run --project tools/DualFrontier.Governance -- sync               # validate, then regenerate the derived files
dotnet run --project tools/DualFrontier.Governance -- migrate --target <scratch copy>   # migration dry run
```

| Option | Effect |
|---|---|
| `--armed` | semantic gate findings that carry `Report` severity become exit-affecting; without it they are report-only (FRAMEWORK section 14.8) |
| `--repo <path>` | operate on an explicit root instead of the repository that holds `DualFrontier.sln` |
| `--register-version <ver>` | `register_version` stamped into the derived archive (`sync`) |

Exit codes: `0` green, `1` a blocking finding, `2` tooling failure or unknown command. `validate` writes
nothing; `sync` writes a derived file only when its content changed, and a blocking finding stops it
before any write. The environment (the user-local SDK, `DOTNET_ROOT`) and the per-commit duty by tier are
in [`DEVELOPMENT_HYGIENE.md`](../../docs/methodology/DEVELOPMENT_HYGIENE.md) sections 3 and 4.

The walk is the filesystem, not the git index, so a `.md` file anywhere under the root is in scope
unless excluded: run artifacts, patches and reports that are `.md` files live outside the tree.

## Retired

The PowerShell scripts that once lived here (`sync_register.ps1`, `render_register.ps1`,
`query_register.ps1`) are retired. The register is plain YAML: read `REGISTER.yaml` or
`CURRENT_AUTHORITY_SURFACE.yaml` directly, or filter them with any YAML tool.

## See also

- [docs/governance/FRAMEWORK.md](../../docs/governance/FRAMEWORK.md) -- governance framework (section 14: the inverted register)
- [docs/governance/REGISTER.yaml](../../docs/governance/REGISTER.yaml) -- derived archive register
- [docs/governance/SYNTHESIS_RATIONALE.md](../../docs/governance/SYNTHESIS_RATIONALE.md) -- source-standards provenance
- [docs/methodology/DEVELOPMENT_HYGIENE.md](../../docs/methodology/DEVELOPMENT_HYGIENE.md) -- tooling reality and the gates of each commit tier
