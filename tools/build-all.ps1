# tools/build-all.ps1
# Full Dual Frontier build: .sln (kernel + tests + tools) + Presentation (Godot).
#
# Usage:
#   .\tools\build-all.ps1                         # Debug build
#   .\tools\build-all.ps1 -Configuration Release  # Release build
#   .\tools\build-all.ps1 -Help

param(
    [string]$Configuration = "Debug",
    [switch]$Help
)

$ErrorActionPreference = 'Stop'

if ($Help) {
    Write-Host @"
Builds the full Dual Frontier solution including DualFrontier.Presentation
(Godot project, deliberately not in .sln due to Godot SDK CI fragility on
machines without Godot installed).

Equivalent to:
    dotnet build DualFrontier.sln -c <Configuration>
    dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj -c <Configuration>

See docs/DEVELOPMENT_HYGIENE.md §2 ('dotnet build' is green) for rationale.
"@
    exit 0
}

# Resolve repo root from script location (CWD-agnostic).
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$RepoRoot  = Split-Path -Parent $ScriptDir

Push-Location $RepoRoot
try {
    Write-Host "==> Building DualFrontier.sln ($Configuration) ..." -ForegroundColor Cyan
    dotnet build DualFrontier.sln -c $Configuration
    if ($LASTEXITCODE -ne 0) { throw ".sln build failed (exit $LASTEXITCODE)" }

    Write-Host ""
    Write-Host "==> Building DualFrontier.Presentation ($Configuration) ..." -ForegroundColor Cyan
    dotnet build src/DualFrontier.Presentation/DualFrontier.Presentation.csproj -c $Configuration
    if ($LASTEXITCODE -ne 0) { throw "Presentation build failed (exit $LASTEXITCODE)" }

    Write-Host ""
    Write-Host "==> Full build successful ($Configuration)" -ForegroundColor Green
}
finally {
    Pop-Location
}
