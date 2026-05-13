<#
.SYNOPSIS
    DualFrontier Document Control Register — human-readable derivative renderer.

.DESCRIPTION
    Generates docs/governance/REGISTER_RENDER.md from REGISTER.yaml as
    browsable view: statistics + per-category sections + global collections.

.NOTES
    Specification: docs/governance/FRAMEWORK.md §4 (sections), Pass 3 §2.5
    Output: docs/governance/REGISTER_RENDER.md (Tier 2 Live; meta-entry)
    Requires: powershell-yaml module
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

if (-not (Get-Module -ListAvailable -Name powershell-yaml)) {
    Write-Error 'powershell-yaml module not installed. Run: Install-Module powershell-yaml -Scope CurrentUser -Force'
    exit 2
}
Import-Module powershell-yaml -ErrorAction Stop

$REPO_ROOT = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$REGISTER_PATH = Join-Path $REPO_ROOT 'docs/governance/REGISTER.yaml'
$OUTPUT_PATH = Join-Path $REPO_ROOT 'docs/governance/REGISTER_RENDER.md'

$register = ConvertFrom-Yaml (Get-Content $REGISTER_PATH -Raw)

$today = (Get-Date).ToString('yyyy-MM-dd')

# --- Aggregate statistics ---
$totalDocs = if ($register.documents) { $register.documents.Count } else { 0 }
$byTier = @{}
1..5 | ForEach-Object { $byTier[$_] = 0 }
$byCategory = @{}
@('A','B','C','D','E','F','G','H','I','J') | ForEach-Object { $byCategory[$_] = 0 }

if ($register.documents) {
    foreach ($d in $register.documents) {
        $t = [int]$d.tier
        if ($byTier.ContainsKey($t)) { $byTier[$t]++ }
        if ($d.category -and $byCategory.ContainsKey([string]$d.category)) { $byCategory[[string]$d.category]++ }
    }
}

$openCapa = if ($register.capa_entries) { @($register.capa_entries | Where-Object { $_.closure_status -eq 'OPEN' }).Count } else { 0 }
$activeRisks = if ($register.risks) { @($register.risks | Where-Object { $_.status -in @('ACTIVE','RESIDUAL','REALIZED') }).Count } else { 0 }
$staleDocs = if ($register.documents) { @($register.documents | Where-Object {
    $_.lifecycle -eq 'STALE' -or ($_.next_review_due -and ([string]$_.next_review_due) -lt $today)
}).Count } else { 0 }

# --- Render header + statistics ---
$sb = [System.Text.StringBuilder]::new()
[void]$sb.AppendLine('# DualFrontier Document Control Register — Rendered View')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('*Auto-generated from [REGISTER.yaml](./REGISTER.yaml) by `tools/governance/render_register.ps1`. Do not edit — edit REGISTER.yaml instead.*')
[void]$sb.AppendLine('')
[void]$sb.AppendLine("*Last generated: $today  |  Schema version: $($register.schema_version)  |  Register version: $($register.register_version)*")
[void]$sb.AppendLine('')
[void]$sb.AppendLine('---')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('## Statistics')
[void]$sb.AppendLine('')
[void]$sb.AppendLine("- Total documents: $totalDocs")
[void]$sb.AppendLine("- Tier 1: $($byTier[1])  |  Tier 2: $($byTier[2])  |  Tier 3: $($byTier[3])  |  Tier 4: $($byTier[4])  |  Tier 5: $($byTier[5])")
$catLine = ($byCategory.GetEnumerator() | Sort-Object Name | ForEach-Object { "$($_.Key)=$($_.Value)" }) -join '  |  '
[void]$sb.AppendLine("- Per category: $catLine")
[void]$sb.AppendLine("- Open CAPA: $openCapa  |  Active risks: $activeRisks  |  Stale documents: $staleDocs")
[void]$sb.AppendLine('')
[void]$sb.AppendLine('---')
[void]$sb.AppendLine('')

# --- Table of contents ---
[void]$sb.AppendLine('## Table of contents')
[void]$sb.AppendLine('')
foreach ($cat in @('A','B','C','D','E','F','G','H','I','J')) {
    $count = $byCategory[$cat]
    if ($count -eq 0) { continue }
    [void]$sb.AppendLine("- [Category $cat (${count} documents)](#category-$cat)")
}
[void]$sb.AppendLine('- [Global: Requirements](#global-requirements)')
[void]$sb.AppendLine('- [Global: Risks](#global-risks)')
[void]$sb.AppendLine('- [Global: CAPA log](#global-capa-log)')
[void]$sb.AppendLine('- [Global: Audit trail](#global-audit-trail)')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('---')
[void]$sb.AppendLine('')

# --- Per-category document sections ---
foreach ($cat in @('A','B','C','D','E','F','G','H','I','J')) {
    $docsInCat = $register.documents | Where-Object { $_.category -eq $cat } | Sort-Object { $_.id }
    if (-not $docsInCat) { continue }
    [void]$sb.AppendLine("<a name=`"category-$cat`"></a>")
    [void]$sb.AppendLine("## Category $cat")
    [void]$sb.AppendLine('')
    foreach ($d in $docsInCat) {
        [void]$sb.AppendLine("### $($d.id) — $($d.title)")
        [void]$sb.AppendLine('')
        [void]$sb.AppendLine("- **Path**: ``$($d.path)``")
        [void]$sb.AppendLine("- **Tier**: $($d.tier)  |  **Lifecycle**: $($d.lifecycle)  |  **Version**: $($d.version)")
        [void]$sb.AppendLine("- **Owner**: $($d.owner)  |  **Content language**: $($d.content_language)")
        if ($d.last_modified)        { [void]$sb.AppendLine("- **Last modified**: $($d.last_modified) (`$($d.last_modified_commit)`)") }
        if ($d.next_review_due)      { [void]$sb.AppendLine("- **Next review due**: $($d.next_review_due)") }
        if ($d.special_case_rationale) { [void]$sb.AppendLine("- **Special-case rationale**: $($d.special_case_rationale)") }
        if ($d.requirements_authored -and $d.requirements_authored.Count -gt 0) {
            [void]$sb.AppendLine("- **Requirements authored**: $($d.requirements_authored -join ', ')")
        }
        if ($d.risks_referenced -and $d.risks_referenced.Count -gt 0) {
            [void]$sb.AppendLine("- **Risks referenced**: $($d.risks_referenced -join ', ')")
        }
        if ($d.capa_entries_referenced -and $d.capa_entries_referenced.Count -gt 0) {
            [void]$sb.AppendLine("- **CAPA referenced**: $($d.capa_entries_referenced -join ', ')")
        }
        if ($d.is_meta_entry) {
            [void]$sb.AppendLine("- **Meta entry**: yes (role=$($d.meta_role))")
        }
        [void]$sb.AppendLine('')
    }
    [void]$sb.AppendLine('---')
    [void]$sb.AppendLine('')
}

# --- Global: Requirements ---
[void]$sb.AppendLine('<a name="global-requirements"></a>')
[void]$sb.AppendLine('## Global: Requirements')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('| ID | Title | Status | Source document | Milestone |')
[void]$sb.AppendLine('|---|---|---|---|---|')
foreach ($r in ($register.requirements | Sort-Object { $_.id })) {
    [void]$sb.AppendLine("| $($r.id) | $($r.title) | $($r.verification_status) | $($r.source_document) | $($r.verification_milestone) |")
}
[void]$sb.AppendLine('')

# --- Global: Risks ---
[void]$sb.AppendLine('<a name="global-risks"></a>')
[void]$sb.AppendLine('## Global: Risks')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('| ID | Title | Status | Type | Likelihood | Impact |')
[void]$sb.AppendLine('|---|---|---|---|---|---|')
foreach ($r in ($register.risks | Sort-Object { $_.id })) {
    [void]$sb.AppendLine("| $($r.id) | $($r.title) | $($r.status) | $($r.risk_type) | $($r.likelihood) | $($r.impact) |")
}
[void]$sb.AppendLine('')

# --- Global: CAPA log ---
[void]$sb.AppendLine('<a name="global-capa-log"></a>')
[void]$sb.AppendLine('## Global: CAPA log')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('| ID | Opened | Status | Trigger (summary) |')
[void]$sb.AppendLine('|---|---|---|---|')
foreach ($c in ($register.capa_entries | Sort-Object { $_.opened_date })) {
    $triggerLine = ($c.trigger -split "`n")[0]
    [void]$sb.AppendLine("| $($c.id) | $($c.opened_date) | $($c.closure_status) | $triggerLine |")
}
[void]$sb.AppendLine('')

# --- Global: Audit trail ---
[void]$sb.AppendLine('<a name="global-audit-trail"></a>')
[void]$sb.AppendLine('## Global: Audit trail')
[void]$sb.AppendLine('')
[void]$sb.AppendLine('| Date | Event | Type | Commits |')
[void]$sb.AppendLine('|---|---|---|---|')
foreach ($e in ($register.audit_trail | Sort-Object { $_.date })) {
    $commits = if ($e.commits.range) { $e.commits.range } else { '' }
    [void]$sb.AppendLine("| $($e.date) | $($e.event) | $($e.event_type) | $commits |")
}
[void]$sb.AppendLine('')

# --- Write output ---
Set-Content -Path $OUTPUT_PATH -Value $sb.ToString() -Encoding UTF8
Write-Host "REGISTER_RENDER.md generated at $OUTPUT_PATH ($totalDocs documents, $($register.requirements.Count) REQ, $($register.risks.Count) RISK, $($register.capa_entries.Count) CAPA, $($register.audit_trail.Count) EVT)"
exit 0
