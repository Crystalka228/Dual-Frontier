; Unshipped analyzer release — DualFrontier.Analyzers
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md
;
; Was empty from Release 1.0 (A'.9.1 Phase γ severity promotion, 2026-07-01) — all 17
; first-batch rules transitioned to AnalyzerReleases.Shipped.md. Future rule
; additions/changes accumulate here ahead of their release transition.
;
; DFK022 is the first addition since. It enforces К-L22 (entity identity honesty),
; seated AUTHORED by the ID_B_ENTITY_VERSIONS cascade (2026-08-22, F-59), and ships
; enforcing at Error from the moment it lands — the rule exists precisely because
; the shipped tree had been fabricating versions, so a non-enforcing grace period
; would defeat its purpose. It transitions to Shipped.md at the next release roll.

### New Rules

Rule ID | Category                    | Severity | Notes
--------|-----------------------------|----------|-------
DFK022  | DualFrontier.NativeBoundary | Error    | К-L22 entity identity — no fabricated EntityId Version. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk022)
