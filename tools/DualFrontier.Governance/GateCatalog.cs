namespace DualFrontier.Governance;

/// <summary>
/// The registry of machine-gate ids: the single list every emission site names
/// its gate from. FRAMEWORK 14.5 is the law that defines the catalog; the
/// governance test suite asserts this list equals that table, so a gate added on
/// one side and not the other fails a test.
/// </summary>
public static class GateCatalog
{
    public const string Schema = "G-SCHEMA";
    public const string Unique = "G-UNIQUE";
    public const string Xref = "G-XREF";
    public const string CatLife = "G-CATLIFE";
    public const string Terminal = "G-TERMINAL";
    public const string Sentinel = "G-SENTINEL";
    public const string EnrolledPath = "G-PATH";
    public const string Namespace = "G-NAMESPACE";
    public const string Globals = "G-GLOBALS";
    public const string Ratio = "G-RATIO";
    public const string RowForm = "G-ROWFORM";

    /// <summary>Every gate id the tool can emit, in FRAMEWORK 14.5 table order.</summary>
    public static readonly IReadOnlyList<string> All = new[]
    {
        Schema, Unique, Xref, CatLife, Terminal, Sentinel, EnrolledPath, Namespace, Globals, Ratio, RowForm,
    };
}
