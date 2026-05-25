namespace DualFrontier.Contracts.Analyzer;

/// <summary>
/// Architectural purpose justifying a <see cref="ReservedStubAttribute"/>
/// marker. Two purposes only per Q-L-10 ratification (Brief A'.9.1 batch 2
/// deliberation 2026-05-24).
/// </summary>
public enum ReservedStubPurpose
{
    /// <summary>
    /// Placeholder structurally required для build composition.
    ///
    /// Example use case: dispatch arm reserved для future cascade activation
    /// (К-extensions cascade #3 deferred dispatch arms HandlePawnState /
    /// HandleItemSpawned / HandleTickAdvanced). The arm exists structurally
    /// к satisfy an interface contract OR enable compilation, не to implement
    /// runtime behavior at the current cascade.
    /// </summary>
    BuildComposition,

    /// <summary>
    /// Structural anchor для forward-design (forward-compatibility sketch).
    ///
    /// Example use case: interface shape committed before consumer materialization
    /// (e.g., К-L20 Mod API surface skeleton at A'.9 closure pre-LOCK cascade).
    /// The structure exists к anchor canonical artifact + freeze architectural
    /// decisions, не to provide functional implementation.
    /// </summary>
    ArchitecturalSketch,
}
