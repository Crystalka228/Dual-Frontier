<#
.SYNOPSIS
    DualFrontier Document Control Register — read-side query interface.

.DESCRIPTION
    Returns structured query results from REGISTER.yaml without manual parsing.
    Agent + human consumer interface per FRAMEWORK §6.5 agent self-check pattern.

.PARAMETER Tier
    Filter by tier (1-5).

.PARAMETER Lifecycle
    Filter by lifecycle state (Draft/Live/LOCKED/EXECUTED/AUTHORED/DEPRECATED/SUPERSEDED/STALE).

.PARAMETER Category
    Filter by category (A-J).

.PARAMETER Requirement
    Look up requirement entry by ID; returns full record + verification artifacts.

.PARAMETER RisksAffecting
    Document ID; returns risks affecting this document.

.PARAMETER Stale
    Documents with STALE lifecycle OR next_review_due < today.

.PARAMETER CapaOpen
    CAPA entries with closure_status OPEN.

.PARAMETER ContentLanguage
    Filter by content_language (en / ru / mixed).

.PARAMETER AffectedByMilestone
    Heuristic; returns documents register tracks as typically affected by milestone type.

.PARAMETER Json
    Output as JSON (default: table).

.PARAMETER PathsOnly
    Output only file paths (one per line).

.NOTES
    Specification: docs/governance/FRAMEWORK.md §6.5
    Requires: powershell-yaml module
#>

[CmdletBinding()]
param(
    [int]$Tier,
    [string]$Lifecycle,
    [string]$Category,
    [string]$Requirement,
    [string]$RisksAffecting,
    [switch]$Stale,
    [switch]$CapaOpen,
    [string]$ContentLanguage,
    [string]$AffectedByMilestone,
    [switch]$Json,
    [switch]$PathsOnly
)

$ErrorActionPreference = 'Stop'

if (-not (Get-Module -ListAvailable -Name powershell-yaml)) {
    Write-Error 'powershell-yaml module not installed. Run: Install-Module powershell-yaml -Scope CurrentUser -Force'
    exit 2
}
Import-Module powershell-yaml -ErrorAction Stop

$REPO_ROOT = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$REGISTER_PATH = Join-Path $REPO_ROOT 'docs/governance/REGISTER.yaml'

if (-not (Test-Path $REGISTER_PATH)) {
    Write-Error "REGISTER.yaml not found at $REGISTER_PATH"
    exit 2
}

$register = ConvertFrom-Yaml (Get-Content $REGISTER_PATH -Raw)

function Format-Output($items, $columns) {
    if ($PathsOnly) {
        $items | ForEach-Object { $_.path } | Where-Object { $_ }
        return
    }
    if ($Json) {
        $items | ConvertTo-Json -Depth 6
        return
    }
    if ($columns) {
        $items | Select-Object $columns | Format-Table -AutoSize
    } else {
        $items | Format-Table -AutoSize
    }
}

# --- Lookup: Requirement ---
if ($Requirement) {
    $req = $register.requirements | Where-Object { $_.id -eq $Requirement }
    if (-not $req) { Write-Host "Requirement not found: $Requirement"; exit 1 }
    Format-Output @($req) $null
    exit 0
}

# --- Filter: documents ---
$docs = $register.documents
if ($Tier)              { $docs = $docs | Where-Object { [int]$_.tier -eq $Tier } }
if ($Lifecycle)         { $docs = $docs | Where-Object { $_.lifecycle -eq $Lifecycle } }
if ($Category)          { $docs = $docs | Where-Object { $_.category -eq $Category } }
if ($ContentLanguage)   { $docs = $docs | Where-Object { $_.content_language -eq $ContentLanguage } }

if ($Stale) {
    $today = (Get-Date).ToString('yyyy-MM-dd')
    $docs = $docs | Where-Object {
        $_.lifecycle -eq 'STALE' -or
        ($_.next_review_due -and ([string]$_.next_review_due) -lt $today)
    }
}

# --- Risks affecting document ---
if ($RisksAffecting) {
    $affecting = $register.risks | Where-Object { $_.affected_documents -contains $RisksAffecting }
    Format-Output $affecting @('id','title','status','risk_type','impact','likelihood')
    exit 0
}

# --- Open CAPA ---
if ($CapaOpen) {
    $open = $register.capa_entries | Where-Object { $_.closure_status -eq 'OPEN' }
    Format-Output $open @('id','opened_date','trigger')
    exit 0
}

# --- Affected by milestone (heuristic) ---
if ($AffectedByMilestone) {
    # Search audit_trail for events whose 'event' or 'id' contains the milestone identifier.
    # Then aggregate documents_affected lists.
    $events = $register.audit_trail | Where-Object {
        $_.event -match $AffectedByMilestone -or $_.id -match $AffectedByMilestone
    }
    $affectedDocs = $events | ForEach-Object { $_.documents_affected } | Where-Object { $_ -is [string] } | Sort-Object -Unique
    if (-not $affectedDocs) {
        Write-Host "No historical events match milestone identifier '$AffectedByMilestone'. Heuristic returned nothing."
        exit 0
    }
    Write-Host "Documents typically affected by milestones matching '$AffectedByMilestone':"
    $affectedDocs | ForEach-Object { Write-Host "  - $_" }
    exit 0
}

# --- Default: filtered documents ---
if (-not $docs) { Write-Host 'No documents match filters.'; exit 0 }
Format-Output $docs @('id','category','tier','lifecycle','path','version')
exit 0
