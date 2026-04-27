namespace DualFrontier.Core.ECS;

/// <summary>
/// Constant pool for user-facing diagnostic substrings emitted by
/// <see cref="SystemExecutionContext"/>. Centralises every literal that
/// <see cref="SystemExecutionContext"/> composes into violation messages
/// and that tests assert against, so the wording can be evolved (e.g.
/// translated) in exactly one place without breaking call sites or tests.
///
/// Members are pure substring tokens. Dynamic fragments — system name,
/// component type name — stay in the StringBuilder construction at the
/// emission site; they are not encoded here.
///
/// Constraint: this type is a constant pool. No methods, no logic, no
/// instances. Adding behaviour here is a code-review-blocking violation.
/// </summary>
internal static class IsolationDiagnostics
{
    // ── Emission constants — read-violation message ──────────────────────
    public const string ViolationHeader = "[ISOLATION VIOLATED]";
    public const string SystemPrefix = "System '";
    public const string SystemSuffix = "'";
    public const string ReadVerb = "accessed '";
    public const string ComponentSuffix = "'";
    public const string ReadReason = "without an access declaration.";
    public const string HintPrefix = "Add: [SystemAccess(";
    public const string ReadHintArgPrefix = "reads: new[]{typeof(";
    public const string HintArgSuffix = ")})]";

    // ── Emission constants — write-violation message ─────────────────────
    public const string WriteVerb = "modified '";
    public const string WriteReason = "without a write declaration.";
    public const string WriteHintArgPrefix = "writes: new[]{typeof(";

    // ── Emission constants — direct-system-access message ────────────────
    public const string GetSystemHeader = "[IsolationViolationException]";
    public const string GetSystemBody = "Direct access to systems is forbidden.";
    public const string GetSystemHint = "Use the EventBus instead of a direct system reference.";

    // ── Test-contract tokens — substrings tests assert against ───────────
    public const string UndeclaredAccessToken = "without an access declaration";
    public const string WriteVerbToken = "modified";
    public const string HintToken = "Add: [SystemAccess";
    public const string DirectSystemAccessToken = "Direct access to systems is forbidden";
}
