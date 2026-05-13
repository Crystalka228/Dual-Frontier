# docs/ideas — Ideas Bank index

*This folder contains Category I idea documents (Tier 5 Speculative per A'.4.5 governance framework).*

*Individual ideas live as separate `.md` files in this directory; each is a Category I Tier 5 entry in `docs/governance/REGISTER.yaml`.*

*Top-level [IDEAS_RESERVOIR.md](../IDEAS_RESERVOIR.md) remains the project-wide index pointing into this folder.*

---

## Purpose

The Ideas Bank captures speculative proposals, brainstorming output, shelved alternatives, and future-feature what-ifs. Friction-free brainstorming surface; nothing here is authoritative.

**Tier 5 properties** (per FRAMEWORK.md §3.3):
- No scheduled review cadence — ideas don't go STALE
- Allowed lifecycle: Draft, Live, DEPRECATED, SUPERSEDED (no STALE state)
- Approval gate for entry: none
- Approval gate for promotion out: architectural deliberation milestone

## Promotion path

When an idea matures into an architectural commitment:
1. Deliberation milestone surfaces (analog A'.0.7, K-L3.1)
2. If accepted: Category A spec drafted in `docs/architecture/`; idea entry transitions to SUPERSEDED with cross-reference to resulting spec
3. If rejected: idea transitions to DEPRECATED with rationale

## Authoring

New idea: `docs/ideas/<slug>.md` (kebab-case slug). Register ID `DOC-I-<SLUG_UPPER>`. Frontmatter mirror auto-applied by `tools/governance/sync_register.ps1`.

## See also

- [docs/governance/FRAMEWORK.md](../governance/FRAMEWORK.md) §3 classification model
- [docs/IDEAS_RESERVOIR.md](../IDEAS_RESERVOIR.md) project-wide index (Category C Tier 2)
- [docs/governance/REGISTER.yaml](../governance/REGISTER.yaml) operational SoT
