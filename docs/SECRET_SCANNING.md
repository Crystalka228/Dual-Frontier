# Secret scanning

The `Gitleaks` GitHub Actions workflow runs on every push, pull request and
manual dispatch. `Gitleaks secret scan` scans the complete history reachable
from the checked-out commit, including both parents of a pull request merge.
Findings fail the job; secret values are redacted in logs. Scan errors also
fail the job. No scan reports are uploaded.

Gitleaks 8.30.1 is pinned and its release archive is verified against a fixed
SHA-256 checksum. Checkout is pinned to a commit, has read-only repository
permissions and does not persist credentials. No additional secrets or
Gitleaks Action license are required.

Run the same check locally with Gitleaks 8.30.1:

```sh
gitleaks git . --log-opts=HEAD --redact --no-banner --exit-code=1
```

CI runs after a push has reached GitHub. Preventing a push locally requires a
separate pre-push hook; preventing a merge requires making `Gitleaks secret
scan` a required check in branch protection or a ruleset. This workflow does
not change either setting.

If a real secret is detected, revoke or rotate it first. Removing it from the
latest file does not remove it from history. Do not add an exception for a
real credential. Exceptions for verified false positives must be narrowly
scoped and reviewed.

The two existing `.gitleaksignore` fingerprints refer to CAPA incident IDs in
the historical `REGISTER_RENDER.md` diff at commit `53697da`. Their records in
`docs/governance/REGISTER.yaml` describe API surface and transition incidents;
they are document references, not credentials. No rule or directory is excluded.
