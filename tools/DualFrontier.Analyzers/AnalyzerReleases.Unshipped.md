; Unshipped analyzer release — DualFrontier.Analyzers
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md
;
; A'.9.1 Phase β-prep stubs (К-extensions cascade #5). Rules added per category commit:
; - Commit β1: 9 Architecture stubs
; - Commit β2: 5 NativeBoundary stubs (forward)
; - Commit β3: 3 Discipline stubs (forward)

### New Rules

Rule ID  | Category                    | Severity | Notes
---------|-----------------------------|----------|-------
DFK003   | DualFrontier.Architecture   | Info     | К-L3 storage ownership — native owns ECS storage. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk003)
DFK003.1 | DualFrontier.Architecture   | Info     | К-L3.1 storage bridge — managed bridge preserves native ownership. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk003-1)
DFK004   | DualFrontier.Architecture   | Info     | К-L4 ComponentTypeRegistry SSoT. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk004)
DFK005   | DualFrontier.Architecture   | Info     | К-L5 declarative bootstrap — bootstrap_graph.h source. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk005)
DFK007   | DualFrontier.Architecture   | Info     | К-L7 Span<T> protocol. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk007)
DFK011   | DualFrontier.Architecture   | Info     | К-L11 NativeWorld SSoT. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk011)
DFK013   | DualFrontier.Architecture   | Info     | К-L13 wake_type discipline (efficiency). [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk013)
DFK016   | DualFrontier.Architecture   | Info     | К-L16 pipeline depth — reference PipelineSlotInterop constants. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk016)
DFK017   | DualFrontier.Architecture   | Info     | К-L17 display composition multi-layer. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk017)
