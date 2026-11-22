using YamlDotNet.Serialization;

namespace DualFrontier.Governance;

/// <summary>
/// One ledger file the G-ROWFORM gate reads: its project-relative POSIX path and the
/// severity its vocabulary findings carry. Structural findings are always
/// <see cref="Severity.Report"/>; only the vocabulary severity is per file, so a file that
/// keeps legacy values (the closed-rows archive) can be watched without reddening the
/// armed run while the live file is held to the legend.
/// </summary>
/// <param name="Path">Project-relative POSIX path of the file holding a ledger table.</param>
/// <param name="VocabularySeverity"><see cref="Severity.Report"/> or <see cref="Severity.Monitor"/>.</param>
public sealed record LedgerFileConfig(string Path, Severity VocabularySeverity);

/// <summary>
/// Where the G-ROWFORM gate finds the F-ledger and what its legend admits, loaded
/// from <c>tools/governance/LEDGER_FORM.yaml</c>. The ledger is one or more files
/// (<see cref="Ledgers"/>, in the order the config lists them); the header names and the
/// legend sets are shared by all of them, because the legend is declared once. An absent
/// config file means the gate is inert.
/// </summary>
public sealed class LedgerFormConfig
{
    /// <summary>The ledger files, in config order; each holds one table that opens with <see cref="IdHeader"/>.</summary>
    public IReadOnlyList<LedgerFileConfig> Ledgers { get; }

    /// <summary>Text of the header cell that opens the ledger table (the id column).</summary>
    public string IdHeader { get; }

    /// <summary>Header text of the severity column.</summary>
    public string SeverityHeader { get; }

    /// <summary>Header text of the state column.</summary>
    public string StateHeader { get; }

    /// <summary>The severity values the ledger legend admits.</summary>
    public IReadOnlyList<string> SeveritySet { get; }

    /// <summary>The state values the ledger legend admits (matched on a state cell's leading member).</summary>
    public IReadOnlyList<string> StateSet { get; }

    public LedgerFormConfig(
        IReadOnlyList<LedgerFileConfig> ledgers,
        string idHeader,
        string severityHeader,
        string stateHeader,
        IReadOnlyList<string> severitySet,
        IReadOnlyList<string> stateSet)
    {
        Ledgers = ledgers;
        IdHeader = idHeader;
        SeverityHeader = severityHeader;
        StateHeader = stateHeader;
        SeveritySet = severitySet;
        StateSet = stateSet;
    }

    /// <summary>
    /// Loads the config from a YAML file; returns null when the file is absent. Throws
    /// <see cref="InvalidOperationException"/> when <c>ledgers</c> is missing or empty, an entry
    /// lacks its <c>path</c>, an entry's <c>vocabulary</c> is not <c>report</c> or <c>monitor</c>,
    /// or two entries name the same path.
    /// </summary>
    public static LedgerFormConfig? Load(string yamlPath)
    {
        if (!File.Exists(yamlPath))
        {
            return null;
        }

        var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        LedgerFormFile? raw = deserializer.Deserialize<LedgerFormFile>(File.ReadAllText(yamlPath));
        if (raw?.ledgers is not { Count: > 0 })
        {
            throw new InvalidOperationException($"{yamlPath}: ledgers is required and must list at least one ledger file");
        }

        var ledgers = new List<LedgerFileConfig>();
        var paths = new HashSet<string>(StringComparer.Ordinal);
        foreach (LedgerEntry entry in raw.ledgers)
        {
            if (string.IsNullOrWhiteSpace(entry.path))
            {
                throw new InvalidOperationException($"{yamlPath}: every ledgers entry needs a path");
            }

            if (!paths.Add(entry.path))
            {
                throw new InvalidOperationException($"{yamlPath}: the ledger path {entry.path} is listed twice");
            }

            Severity vocabulary = entry.vocabulary?.Trim().ToLowerInvariant() switch
            {
                "report" => Severity.Report,
                "monitor" => Severity.Monitor,
                _ => throw new InvalidOperationException(
                    $"{yamlPath}: the vocabulary of {entry.path} must be \"report\" or \"monitor\""),
            };
            ledgers.Add(new LedgerFileConfig(entry.path, vocabulary));
        }

        return new LedgerFormConfig(
            ledgers,
            raw.id_header ?? "F-#",
            raw.severity_header ?? "Severity",
            raw.state_header ?? "State",
            raw.severity_set ?? new List<string>(),
            raw.state_set ?? new List<string>());
    }

    // YamlDotNet DTOs (snake_case field names match the file).
    private sealed class LedgerFormFile
    {
        public List<LedgerEntry>? ledgers { get; set; }
        public string? id_header { get; set; }
        public string? severity_header { get; set; }
        public string? state_header { get; set; }
        public List<string>? severity_set { get; set; }
        public List<string>? state_set { get; set; }
    }

    private sealed class LedgerEntry
    {
        public string? path { get; set; }
        public string? vocabulary { get; set; }
    }
}
