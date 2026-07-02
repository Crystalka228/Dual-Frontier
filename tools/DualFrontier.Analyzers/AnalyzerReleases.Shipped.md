; Shipped analyzer releases — DualFrontier.Analyzers
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md
;
; Release 1.0 — A'.9.1 Phase γ severity promotion (2026-07-01; F-12 ratified by
; Crystalka 2026-07-01). The 17-rule first batch (S-LOCK-4 set) ships at the
; ratified severities: 11 Error + 5 Warning + 1 Info (DFL025_B — .editorconfig
; `suggestion`, IDE-only). Rows transitioned from AnalyzerReleases.Unshipped.md
; (β1/β2/β3 provenance) with the Severity column updated Info → shipped values.

## Release 1.0

### New Rules

Rule ID  | Category                    | Severity | Notes
---------|-----------------------------|----------|-------
DFK003   | DualFrontier.Architecture   | Error    | К-L3 storage ownership — native owns ECS storage. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk003)
DFK003_1 | DualFrontier.Architecture   | Error    | К-L3.1 storage bridge — managed bridge preserves native ownership. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk003_1)
DFK004   | DualFrontier.Architecture   | Error    | К-L4 ComponentTypeRegistry SSoT. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk004)
DFK005   | DualFrontier.Architecture   | Error    | К-L5 declarative bootstrap — bootstrap_graph.h source. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk005)
DFK007   | DualFrontier.Architecture   | Error    | К-L7 Span<T> protocol. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk007)
DFK011   | DualFrontier.Architecture   | Error    | К-L11 NativeWorld SSoT. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk011)
DFK013   | DualFrontier.Architecture   | Warning  | К-L13 wake_type discipline (efficiency). [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk013)
DFK016   | DualFrontier.Architecture   | Warning  | К-L16 pipeline depth — reference PipelineSlotInterop constants. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk016)
DFK017   | DualFrontier.Architecture   | Error    | К-L17 display composition multi-layer. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk017)
DFK001   | DualFrontier.NativeBoundary | Error    | К-L1 native language discipline (managed-side bridge). [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk001)
DFK002   | DualFrontier.NativeBoundary | Error    | К-L2 P/Invoke bindings — canonical Core.Interop surface only. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk002)
DFK007_1 | DualFrontier.NativeBoundary | Error    | К-L7.1 GPU pipeline slot — managed ReadSlotTail access. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk007_1)
DFK015_1 | DualFrontier.NativeBoundary | Error    | К-L15.1 three-tier mutex managed facade. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk015_1)
DFK019_A | DualFrontier.NativeBoundary | Warning  | К-L19 static Vulkan API surface (Q-L-8 split; Warning per K_CLOSURE §7.2, F-12 ratified). [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfk019_a)
DFL025_A | DualFrontier.Discipline     | Warning  | Lesson #25 — [ReservedStub] behavior invocation needs [Trait]. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfl025_a)
DFL025_B | DualFrontier.Discipline     | Info     | Lesson #25 — standalone tests against reserved stubs should use [Fact(Skip=...)] (.editorconfig `suggestion`, IDE-only). [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#dfl025_b)
DF999    | DualFrontier.Discipline     | Warning  | Solution-wide GlobalSuppressions / [assembly: SuppressMessage] banned. [Documentation](https://github.com/Crystalka228/Dual-Frontier/blob/main/docs/architecture/ANALYZER_RULES.md#df999)
