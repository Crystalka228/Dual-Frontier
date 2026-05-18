# V0.A scaffolding generator per VULKAN_SUBSTRATE.md §2.1 + V0.A brief §10.
# Idempotent: subsequent runs produce no git diff. Materializes V0.A directory
# structure only. V0.B + V0.C extensions append here when those briefs land.
#
# Run from repo root: ./tools/scaffold-runtime.ps1

[CmdletBinding()]
param(
    [string]$RepoRoot = (Resolve-Path "$PSScriptRoot/..").Path
)

$ErrorActionPreference = 'Stop'

$runtimeRoot = Join-Path $RepoRoot 'src\DualFrontier.Runtime'

if (-not (Test-Path $runtimeRoot)) {
    throw "Runtime project not found: $runtimeRoot. Run Commit 2 of V0.A cascade first."
}

# V0.A directory structure per VULKAN_SUBSTRATE.md §2.1 (foundation subset).
# Subsequent V0.B adds Compute/, Assets/. V0.C adds Sprite/, Text/, full Diagnostic/.
$directories = @(
    'Native',
    'Native\Win32',
    'Native\Vulkan',
    'Window',
    'Input',
    'Graphics',
    'Diagnostic'
)

$created = 0
foreach ($dir in $directories) {
    $path = Join-Path $runtimeRoot $dir
    if (-not (Test-Path $path)) {
        New-Item -ItemType Directory -Path $path | Out-Null
        Write-Host "Created: $dir"
        $created++
    }
}

if ($created -eq 0) {
    Write-Host "V0.A scaffold idempotent: all directories already exist (no changes)."
} else {
    Write-Host "V0.A scaffolding complete: $created new director(ies) under $runtimeRoot."
}
