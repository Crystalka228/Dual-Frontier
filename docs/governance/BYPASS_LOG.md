---
register_id: DOC-G-BYPASS_LOG
project: Dual Frontier
category: G
tier: 2
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-12
last_modified: 2026-05-12
content_language: en
next_review_due: 2026-Q3
title: Register Validation Bypass Tracking
last_modified_commit: b655d4b
review_cadence: on-closure+quarterly
reviewer: Crystalka
---

# BYPASS_LOG — Register Validation Bypass Tracking

*Tier 2 Live document per A'.4.5 governance framework. Tracks instances where `sync_register.ps1 --validate` was bypassed via `git commit --no-verify`.*

*Each bypass creates an entry below: timestamp + commit hash + bypassing-author + rationale + follow-up plan.*

*`sync_register.ps1 --validate` includes advisory check: «if BYPASS_LOG.md has entries since last validation, flag for follow-up review». Bypasses do not block subsequent work, but accumulated unresolved bypasses surface as governance hygiene debt.*

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

- [docs/governance/FRAMEWORK.md](./FRAMEWORK.md) §6.3 — bypass mechanism specification
- [docs/governance/REGISTER.yaml](./REGISTER.yaml) — register entry for this document (DOC-G-BYPASS_LOG, Tier 2 Live)
- [tools/governance/sync_register.ps1](../../tools/governance/sync_register.ps1) — validation tooling that creates bypass-detection signals