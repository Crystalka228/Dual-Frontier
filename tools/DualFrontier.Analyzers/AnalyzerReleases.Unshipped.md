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
DFK001   | DualFrontier.NativeBoundary | Info     | К-L1 native language discipline (managed-side bridge). [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk001)
DFK002   | DualFrontier.NativeBoundary | Info     | К-L2 P/Invoke bindings — canonical Core.Interop surface only. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk002)
DFK007.1 | DualFrontier.NativeBoundary | Info     | К-L7.1 GPU pipeline slot — managed ReadSlotTail access. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk007-1)
DFK015.1 | DualFrontier.NativeBoundary | Info     | К-L15.1 three-tier mutex managed facade. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk015-1)
DFK019.A | DualFrontier.NativeBoundary | Info     | К-L19 static Vulkan API surface (Q-L-8 split). [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk019-a)
