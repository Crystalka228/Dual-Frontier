---
register_id: DOC-G-BYPASS_LOG
project: Dual Frontier
category: G
tier: 2
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-12
last_modified: 2026-11-22
content_language: en
next_review_due: 2027-Q1
title: Register Validation Bypass Tracking
last_modified_commit: b655d4b
review_cadence: on-closure+quarterly
reviewer: Crystalka
---

# BYPASS_LOG — Register Validation Bypass Tracking

*Tier 2 Live document. Tracks instances where the governance gate (`dotnet run --project tools/DualFrontier.Governance -- validate --armed`, FRAMEWORK section 14) was knowingly skipped or overridden for a commit.*

*Each bypass creates an entry below: timestamp + commit hash + bypassing-author + rationale + follow-up plan.*

*No commit hook exists to bypass and the governance tool does not read this log: an entry is a hand-kept record. Bypasses do not block subsequent work, but accumulated unresolved bypasses are governance hygiene debt.*

---

## Entry template

```markdown
### Bypass: <commit-hash>

- **Time**: <ISO 8601 UTC, e.g. 2026-05-12T14:32:00Z>
- **Author**: <session_id> OR Crystalka
- **Rationale**: <why bypass was necessary — what specifically failed validation>
- **Follow-up**: <how register state should be reconciled; expected milestone for cleanup>
- **Status**: PENDING | RESOLVED (link to resolution commit)
```

---

## 2026

*(No bypasses recorded at A'.4.5 closure. First entry will appear if/when any execution session bypasses validation.)*

---

## See also

- [docs/governance/FRAMEWORK.md](./FRAMEWORK.md) section 14 -- the governance law: gates, deployment discipline (14.8)
- [docs/governance/REGISTER.yaml](./REGISTER.yaml) -- derived archive register (entry for this document: DOC-G-BYPASS_LOG, Tier 2 Live)
- [tools/DualFrontier.Governance](../../tools/DualFrontier.Governance) -- the governance tool whose gate a bypass skips
