param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("architecture", "methodology", "reports", "all")]
    [string]$Group
)

$ErrorActionPreference = "Stop"
$repoRoot = (Get-Location).Path

$movedToArch = @(
    "ARCHITECTURE_TYPE_SYSTEM.md", "KERNEL_ARCHITECTURE.md",
    "MAXIMUM_ENGINEERING_REFACTOR_TRACK_B_ACTIVATION.md", "MOD_OS_ARCHITECTURE.md",
    "MOD_PIPELINE.md", "RUNTIME_ARCHITECTURE.md", "VISUAL_ENGINE.md",
    "GPU_COMPUTE.md", "MIGRATION_PLAN_KERNEL_TO_VANILLA.md", "K_L3_1_AMENDMENT_PLAN.md",
    "CONTRACTS.md", "EVENT_BUS.md", "FIELDS.md", "GODOT_INTEGRATION.md",
    "ISOLATION.md", "MODDING.md", "OWNERSHIP_TRANSITION.md", "RESOURCE_MODELS.md",
    "THREADING.md", "FHE_INTEGRATION_CONTRACT.md", "COMBO_RESOLUTION.md",
    "COMPOSITE_REQUESTS.md", "PERFORMANCE.md", "ARCHITECTURE.md", "ECS.md"
)
$movedToMethodology = @(
    "METHODOLOGY.md", "PIPELINE_METRICS.md", "MAXIMUM_ENGINEERING_REFACTOR.md",
    "CODING_STANDARDS.md", "DEVELOPMENT_HYGIENE.md", "TESTING_STRATEGY.md"
)
$movedToReports = @(
    "PERFORMANCE_REPORT_K3.md", "PERFORMANCE_REPORT_K7.md", "CPP_KERNEL_BRANCH_REPORT.md",
    "NATIVE_CORE_EXPERIMENT.md", "NORMALIZATION_REPORT.md"
)

# Pick groups
$activeNames = @()
$activeDestDir = @{}
$activeNewPath = @{}
if ($Group -eq "architecture" -or $Group -eq "all") {
    foreach ($n in $movedToArch) {
        $activeNames += $n
        $activeDestDir[$n] = "docs\architecture"
        $activeNewPath[$n] = "/docs/architecture/$n"
    }
}
if ($Group -eq "methodology" -or $Group -eq "all") {
    foreach ($n in $movedToMethodology) {
        $activeNames += $n
        $activeDestDir[$n] = "docs\methodology"
        $activeNewPath[$n] = "/docs/methodology/$n"
    }
}
if ($Group -eq "reports" -or $Group -eq "all") {
    foreach ($n in $movedToReports) {
        $activeNames += $n
        $activeDestDir[$n] = "docs\reports"
        $activeNewPath[$n] = "/docs/reports/$n"
    }
}

# Sort by length DESC so longer names match before shorter substrings
$activeNamesSorted = $activeNames | Sort-Object -Property Length -Descending

# Skip A'.0.5's own audit-trail files (they describe pre-move state)
$skipPathPrefixes = @(
    "tools\scratch\A_05",
    "tools\briefs\A_PRIME_0_5_REORG_AND_REFRESH_BRIEF.md",
    "BenchmarkDotNet.Artifacts",
    "tests\DualFrontier.Core.Benchmarks\BenchmarkDotNet.Artifacts"
)

$mdFiles = Get-ChildItem -Path $repoRoot -Filter "*.md" -Recurse |
    Where-Object {
        $rel = $_.FullName.Substring($repoRoot.Length + 1)
        $skip = $false
        foreach ($s in $skipPathPrefixes) {
            if ($rel -like "$s*") { $skip = $true; break }
        }
        -not $skip
    }

$filesUpdated = @()

foreach ($file in $mdFiles) {
    $rel = $file.FullName.Substring($repoRoot.Length + 1)
    $fileDir = Split-Path $rel -Parent

    $content = [System.IO.File]::ReadAllText($file.FullName)
    $original = $content

    foreach ($name in $activeNamesSorted) {
        $newPath = $activeNewPath[$name]
        $destDir = $activeDestDir[$name]
        $escName = [regex]::Escape($name)

        # Markdown link patterns: skip intra-destDir refs (sibling links still resolve)
        if ($fileDir -ne $destDir) {
            $linkPatterns = @(
                "]\(\./$escName\)",
                "]\(\.\./$escName\)",
                "]\(\./docs/$escName\)",
                "]\(\.\./docs/$escName\)",
                "]\(docs/$escName\)",
                "]\(\.\./\.\./docs/$escName\)",
                "]\(\.\./\.\./\.\./docs/$escName\)"
            )
            foreach ($pattern in $linkPatterns) {
                $replacement = "]($newPath)"
                $content = [regex]::Replace($content, $pattern, $replacement)
            }

            # Anchor refs
            $anchorPatterns = @(
                "]\(\./$escName(#[A-Za-z0-9_-]+)\)",
                "]\(\.\./$escName(#[A-Za-z0-9_-]+)\)",
                "]\(\./docs/$escName(#[A-Za-z0-9_-]+)\)",
                "]\(\.\./docs/$escName(#[A-Za-z0-9_-]+)\)",
                "]\(docs/$escName(#[A-Za-z0-9_-]+)\)",
                "]\(\.\./\.\./docs/$escName(#[A-Za-z0-9_-]+)\)"
            )
            foreach ($pattern in $anchorPatterns) {
                $replacement = "]($newPath" + '$1' + ")"
                $content = [regex]::Replace($content, $pattern, $replacement)
            }
        }

        # Inline path forms (in backticks, prose mentions): `docs/X.md` → `docs/architecture/X.md`
        # ALWAYS apply — inline path-form refs are interpreted from repo root,
        # so the referencing file's location does not matter.
        $newRelPath = $destDir -replace "\\", "/"
        $inlinePatterns = @(
            "(?<![/A-Za-z])docs/$escName",
            "(?<![/A-Za-z])\./docs/$escName",
            "(?<![/A-Za-z])\.\./docs/$escName"
        )
        foreach ($pattern in $inlinePatterns) {
            $replacement = "$newRelPath/$name"
            $content = [regex]::Replace($content, $pattern, $replacement)
        }
    }

    if ($content -ne $original) {
        [System.IO.File]::WriteAllText($file.FullName, $content)
        Write-Host "Updated: $rel"
        $filesUpdated += $rel
    }
}

Write-Host ""
Write-Host "=== Summary ($Group) ==="
Write-Host "Files updated: $($filesUpdated.Count)"
