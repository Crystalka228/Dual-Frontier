<#
.SYNOPSIS
    DualFrontier Document Control Register — write-side sync + validation.

.DESCRIPTION
    Synchronizes docs/governance/REGISTER.yaml with per-document frontmatter mirrors;
    validates schema and cross-reference integrity. Primary closure-gate per Q-A45-X5
    post-session protocol.

.PARAMETER Validate
    Validate only; no writes. Use as commit-time gate.

.PARAMETER Sync
    Sync REGISTER.yaml → frontmatter mirrors (no validation).

.PARAMETER Default behavior (no switches)
    Both --sync and --validate.

.OUTPUTS
    Exit codes:
      0 — all green
      1 — validation errors (commit blocked)
      2 — tooling failure (file lock, permission, missing dependency)
    Structured report to stdout + docs/governance/VALIDATION_REPORT.md.

.NOTES
    Specification: docs/governance/FRAMEWORK.md §6
    Schema version: 1.0 (lock at A'.4.5)
    Requires: powershell-yaml module (Install-Module powershell-yaml -Scope CurrentUser)
#>

[CmdletBinding()]
param(
    [switch]$Validate,
    [switch]$Sync
)

$ErrorActionPreference = 'Stop'

# Default: both operations
if (-not $Validate -and -not $Sync) {
    $Validate = $true
    $Sync = $true
}

$EXPECTED_SCHEMA = '1.0'
$REPO_ROOT = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$REGISTER_PATH = Join-Path $REPO_ROOT 'docs/governance/REGISTER.yaml'
$EXCLUSIONS_PATH = Join-Path $REPO_ROOT 'tools/governance/SCOPE_EXCLUSIONS.yaml'
$REPORT_PATH = Join-Path $REPO_ROOT 'docs/governance/VALIDATION_REPORT.md'

$errors = [System.Collections.ArrayList]::new()
$warnings = [System.Collections.ArrayList]::new()
$infos = [System.Collections.ArrayList]::new()

function Write-Status($msg, $level = 'INFO') {
    $prefix = switch ($level) {
        'ERROR'   { '[ERROR]' }
        'WARN'    { '[WARN] ' }
        'INFO'    { '[INFO] ' }
        'OK'      { '[OK]   ' }
    }
    Write-Host "$prefix $msg"
}

# --- Dependency check ---
if (-not (Get-Module -ListAvailable -Name powershell-yaml)) {
    Write-Status 'powershell-yaml module not installed.' 'ERROR'
    Write-Status 'Install via: Install-Module powershell-yaml -Scope CurrentUser -Force' 'ERROR'
    exit 2
}
Import-Module powershell-yaml -ErrorAction Stop

# --- Load REGISTER.yaml ---
if (-not (Test-Path $REGISTER_PATH)) {
    Write-Status "REGISTER.yaml not found at $REGISTER_PATH" 'ERROR'
    exit 2
}
$registerRaw = Get-Content $REGISTER_PATH -Raw
$register = ConvertFrom-Yaml $registerRaw

# --- Schema version compatibility ---
$yamlSchema = [string]$register.schema_version
$cmp = [version]::new()
if (-not [version]::TryParse($yamlSchema, [ref]$cmp)) {
    [void]$errors.Add("Schema version '$yamlSchema' is not parseable as semantic version")
} else {
    $expected = [version]$EXPECTED_SCHEMA
    if ($cmp.Major -ne $expected.Major) {
        [void]$errors.Add("MAJOR version mismatch: register=$yamlSchema, tooling=$EXPECTED_SCHEMA. Migration required.")
    } elseif ($cmp.Minor -gt $expected.Minor) {
        [void]$errors.Add("Register schema v$yamlSchema requires tooling v$($cmp.Major).$($cmp.Minor)+. Update tooling.")
    } elseif ($cmp.Minor -lt $expected.Minor) {
        [void]$warnings.Add("Tooling forward-compatible to register v$yamlSchema (tooling expects v$EXPECTED_SCHEMA).")
    }
}

# --- Load SCOPE_EXCLUSIONS.yaml ---
$exclusions = @{ excluded_paths = @(); included_extensions = @('.md') }
if (Test-Path $EXCLUSIONS_PATH) {
    $exclusions = ConvertFrom-Yaml (Get-Content $EXCLUSIONS_PATH -Raw)
}

function Test-PathExcluded($relPath) {
    foreach ($entry in $exclusions.excluded_paths) {
        $pattern = $entry.pattern -replace '\*\*', '.*' -replace '\*', '[^/]*'
        if ($relPath -match "^$pattern$") { return $true }
    }
    return $false
}

# --- Enum definitions per FRAMEWORK §3 ---
$VALID_CATEGORIES = @('A','B','C','D','E','F','G','H','I','J')
$VALID_TIERS = @(1,2,3,4,5)
$VALID_LIFECYCLES = @('Draft','Live','LOCKED','EXECUTED','AUTHORED','DEPRECATED','SUPERSEDED','STALE')
$VALID_RISK_TYPES = @('Technical','Architectural','Methodological','Operational','External')
$VALID_RISK_STATUS = @('ACTIVE','RESIDUAL','CLOSED','REALIZED','ACCEPTED')
$VALID_LIKELIHOODS = @('Low','Medium-Low','Medium','Medium-High','High')
$VALID_IMPACTS = @('Low','Medium','High','Critical')
$VALID_VERIFICATION_STATUS = @('PENDING','PARTIAL','VERIFIED','FAILED')
$VALID_CAPA_STATUS = @('OPEN','CLOSED')
$VALID_EVENT_TYPES = @('deliberation_milestone','execution_milestone','amendment_landing','lifecycle_transition','governance_event')

# Forbidden Category × Tier combinations (per FRAMEWORK §3.4.1)
$FORBIDDEN_CT = @{
    'D' = @(1,2,4,5)
    'E' = @(1,2,4,5)
    'F' = @(1,2,3,5)
}

# --- Validate documents collection ---
$docIds = @{}
$docPaths = @{}
$docsByCategory = @{}
foreach ($cat in $VALID_CATEGORIES) { $docsByCategory[$cat] = 0 }

if ($register.documents -and $register.documents.Count -gt 0) {
    foreach ($doc in $register.documents) {
        $id = $doc.id
        $path = $doc.path
        $cat = $doc.category
        $tier = [int]$doc.tier
        $lifecycle = $doc.lifecycle

        # Required fields
        if ([string]::IsNullOrEmpty($id))         { [void]$errors.Add("Document entry missing 'id': $($doc | ConvertTo-Json -Depth 2)"); continue }
        if ([string]::IsNullOrEmpty($path))       { [void]$errors.Add("Document entry $id missing 'path'") }
        if ([string]::IsNullOrEmpty($cat))        { [void]$errors.Add("Document entry $id missing 'category'") }
        if ([string]::IsNullOrEmpty($lifecycle))  { [void]$errors.Add("Document entry $id missing 'lifecycle'") }
        if (-not $tier)                           { [void]$errors.Add("Document entry $id missing 'tier'") }

        # Unique ID
        if ($docIds.ContainsKey($id)) {
            [void]$errors.Add("Duplicate document id: $id")
        } else {
            $docIds[$id] = $doc
        }
        if ($path) { $docPaths[$path] = $id }

        # Enum value checks
        if ($cat -and $VALID_CATEGORIES -notcontains $cat)        { [void]$errors.Add("$id : invalid category '$cat'") }
        if ($tier -and $VALID_TIERS -notcontains $tier)           { [void]$errors.Add("$id : invalid tier '$tier'") }
        if ($lifecycle -and $VALID_LIFECYCLES -notcontains $lifecycle) { [void]$errors.Add("$id : invalid lifecycle '$lifecycle'") }
        if ($cat) { $docsByCategory[$cat]++ }

        # Forbidden Category × Tier
        if ($FORBIDDEN_CT.ContainsKey($cat) -and $FORBIDDEN_CT[$cat] -contains $tier) {
            if (-not $doc.special_case_rationale) {
                [void]$errors.Add("$id : forbidden combination category=$cat + tier=$tier without special_case_rationale")
            }
        }

        # Tier × Lifecycle forbidden combinations
        $combo = "$tier+$lifecycle"
        if ($combo -in @('1+AUTHORED','3+LOCKED','5+STALE','4+LOCKED')) {
            if (-not $doc.special_case_rationale) {
                [void]$errors.Add("$id : forbidden combination tier=$tier + lifecycle=$lifecycle without special_case_rationale")
            }
        }

        # Terminal state cross-references
        if ($lifecycle -eq 'DEPRECATED' -and -not $doc.deprecated_by) {
            [void]$errors.Add("$id : DEPRECATED entry missing 'deprecated_by'")
        }
        if ($lifecycle -eq 'SUPERSEDED' -and -not $doc.superseded_by) {
            [void]$errors.Add("$id : SUPERSEDED entry missing 'superseded_by'")
        }

        # File existence (skip meta-entries with PENDING-INITIAL exemption handled elsewhere)
        if ($path) {
            $fullPath = Join-Path $REPO_ROOT $path
            if (-not (Test-Path $fullPath)) {
                [void]$errors.Add("$id : file not found at '$path'")
            }
        }
    }
}

# --- Orphan check: every .md file in scope has a register entry ---
Push-Location $REPO_ROOT
try {
    $allMd = git ls-files '*.md' 2>$null
    foreach ($mdRel in $allMd) {
        $mdRel = $mdRel -replace '\\','/'
        if (Test-PathExcluded $mdRel) { continue }
        if (-not $docPaths.ContainsKey($mdRel)) {
            [void]$warnings.Add("Orphan file (no register entry): $mdRel")
        }
    }
} finally {
    Pop-Location
}

# --- Validate global collections cross-references ---
$reqIds = @{}
if ($register.requirements) {
    foreach ($req in $register.requirements) {
        $rid = $req.id
        if (-not $rid) { [void]$errors.Add("Requirement entry missing 'id'"); continue }
        if ($reqIds.ContainsKey($rid)) { [void]$errors.Add("Duplicate requirement id: $rid") }
        $reqIds[$rid] = $req
        if ($req.verification_status -and $VALID_VERIFICATION_STATUS -notcontains $req.verification_status) {
            [void]$errors.Add("$rid : invalid verification_status '$($req.verification_status)'")
        }
    }
}

$riskIds = @{}
if ($register.risks) {
    foreach ($risk in $register.risks) {
        $rid = $risk.id
        if (-not $rid) { [void]$errors.Add("Risk entry missing 'id'"); continue }
        if ($riskIds.ContainsKey($rid)) { [void]$errors.Add("Duplicate risk id: $rid") }
        $riskIds[$rid] = $risk
        if ($risk.likelihood -and $VALID_LIKELIHOODS -notcontains $risk.likelihood) {
            [void]$errors.Add("$rid : invalid likelihood '$($risk.likelihood)'")
        }
        if ($risk.impact -and $VALID_IMPACTS -notcontains $risk.impact) {
            [void]$errors.Add("$rid : invalid impact '$($risk.impact)'")
        }
        if ($risk.risk_type -and $VALID_RISK_TYPES -notcontains $risk.risk_type) {
            [void]$errors.Add("$rid : invalid risk_type '$($risk.risk_type)'")
        }
        if ($risk.status -and $VALID_RISK_STATUS -notcontains $risk.status) {
            [void]$errors.Add("$rid : invalid status '$($risk.status)'")
        }
    }
}

$capaIds = @{}
if ($register.capa_entries) {
    foreach ($capa in $register.capa_entries) {
        $cid = $capa.id
        if (-not $cid) { [void]$errors.Add("CAPA entry missing 'id'"); continue }
        if ($capaIds.ContainsKey($cid)) { [void]$errors.Add("Duplicate CAPA id: $cid") }
        $capaIds[$cid] = $capa
        if ($capa.closure_status -and $VALID_CAPA_STATUS -notcontains $capa.closure_status) {
            [void]$errors.Add("$cid : invalid closure_status '$($capa.closure_status)'")
        }
    }
}

$evtIds = @{}
if ($register.audit_trail) {
    foreach ($evt in $register.audit_trail) {
        $eid = $evt.id
        if (-not $eid) { [void]$errors.Add("Audit trail entry missing 'id'"); continue }
        if ($evtIds.ContainsKey($eid)) { [void]$errors.Add("Duplicate audit_trail id: $eid") }
        $evtIds[$eid] = $evt
        if ($evt.event_type -and $VALID_EVENT_TYPES -notcontains $evt.event_type) {
            [void]$errors.Add("$eid : invalid event_type '$($evt.event_type)'")
        }
    }
}

[void]$infos.Add("Counts: documents=$($docIds.Count), requirements=$($reqIds.Count), risks=$($riskIds.Count), capa=$($capaIds.Count), audit_trail=$($evtIds.Count)")
[void]$infos.Add("Per-category documents: $(($docsByCategory.GetEnumerator() | ForEach-Object { "$($_.Key)=$($_.Value)" }) -join ', ')")

# --- Sync frontmatter mirrors ---
$syncedCount = 0
if ($Sync -and $errors.Count -eq 0) {
    foreach ($doc in $register.documents) {
        $path = $doc.path
        if (-not $path) { continue }
        $fullPath = Join-Path $REPO_ROOT $path
        if (-not (Test-Path $fullPath)) { continue }

        $content = Get-Content $fullPath -Raw -Encoding UTF8
        $frontmatter = @"
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: $($doc.id)
category: $($doc.category)
tier: $($doc.tier)
lifecycle: $($doc.lifecycle)
owner: $($doc.owner)
version: "$($doc.version)"
next_review_due: $(if ($doc.next_review_due) { $doc.next_review_due } else { '"null"' })
register_view_url: docs/governance/REGISTER_RENDER.md#$($doc.id)
---

"@
        # Strip existing frontmatter if present
        if ($content -match '^---[\r\n]+(.*?)[\r\n]+---[\r\n]+(.*)$') {
            $existingFm = $Matches[1]
            $body = $Matches[2]
            if ($existingFm -match 'register_id:') {
                # Replace existing register frontmatter
                $newContent = $frontmatter + $body
            } else {
                # Existing non-register frontmatter; preserve as-is, prepend register
                $newContent = $frontmatter + $content
            }
        } else {
            $newContent = $frontmatter + $content
        }

        Set-Content -Path $fullPath -Value $newContent -Encoding UTF8 -NoNewline
        $syncedCount++
    }
    [void]$infos.Add("Synced frontmatter mirrors: $syncedCount documents")
}

# --- Write VALIDATION_REPORT.md ---
$report = @"
# Register Validation Report

*Auto-generated by ``tools/governance/sync_register.ps1``. Do not edit; re-run tooling to refresh.*

*Last run: $(Get-Date -Format 'yyyy-MM-ddTHH:mm:ssZ' -AsUTC)*
*Schema version: $yamlSchema (tooling expects $EXPECTED_SCHEMA)*

---

## Summary

- Errors: $($errors.Count)
- Warnings: $($warnings.Count)
- Documents enrolled: $($docIds.Count)
- Documents synced: $syncedCount

$($infos | ForEach-Object { "- $_" } | Out-String)

## Errors

$(if ($errors.Count -eq 0) { '*None.*' } else { ($errors | ForEach-Object { "- $_" }) -join "`n" })

## Warnings

$(if ($warnings.Count -eq 0) { '*None.*' } else { ($warnings | ForEach-Object { "- $_" }) -join "`n" })
"@

Set-Content -Path $REPORT_PATH -Value $report -Encoding UTF8

# --- Print summary + exit ---
Write-Host ""
foreach ($e in $errors) { Write-Status $e 'ERROR' }
foreach ($w in $warnings) { Write-Status $w 'WARN' }
foreach ($i in $infos) { Write-Status $i 'INFO' }

if ($errors.Count -gt 0) {
    Write-Status "Validation FAILED with $($errors.Count) errors. See VALIDATION_REPORT.md." 'ERROR'
    exit 1
}

Write-Status "Validation passed. $($warnings.Count) advisory warnings." 'OK'
exit 0
