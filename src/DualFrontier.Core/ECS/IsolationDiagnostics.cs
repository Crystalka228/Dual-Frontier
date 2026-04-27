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
    public const string ViolationHeader = "[ИЗОЛЯЦИЯ НАРУШЕНА]";
    public const string SystemPrefix = "Система '";
    public const string SystemSuffix = "'";
    public const string ReadVerb = "обратилась к '";
    public const string ComponentSuffix = "'";
    public const string ReadReason = "без декларации доступа.";
    public const string HintPrefix = "Добавь: [SystemAccess(";
    public const string ReadHintArgPrefix = "reads: new[]{typeof(";
    public const string HintArgSuffix = ")})]";

    // ── Emission constants — write-violation message ─────────────────────
    public const string WriteVerb = "модифицирует '";
    public const string WriteReason = "без декларации записи.";
    public const string WriteHintArgPrefix = "writes: new[]{typeof(";

    // ── Emission constants — direct-system-access message ────────────────
    public const string GetSystemHeader = "[IsolationViolationException]";
    public const string GetSystemBody = "Прямой доступ к системам запрещён.";
    public const string GetSystemHint = "Используй EventBus вместо прямой ссылки на систему.";

    // ── Test-contract tokens — substrings tests assert against ───────────
    public const string UndeclaredAccessToken = "без декларации";
    public const string WriteVerbToken = "модифицирует";
    public const string HintToken = "Добавь: [SystemAccess";
    public const string DirectSystemAccessToken = "Прямой доступ к системам запрещён";
}
