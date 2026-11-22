using YamlDotNet.Serialization;

namespace DualFrontier.Governance;

/// <summary>
/// Where the G-ROWFORM gate finds the F-ledger and what its legend admits,
/// loaded from <c>tools/governance/LEDGER_FORM.yaml</c>. The ledger location is
/// one value (<see cref="LedgerPath"/>), so moving the ledger is a one-line edit
/// of the config. An absent config file means the gate is inert.
/// </summary>
public sealed class LedgerFormConfig
{
    /// <summary>Project-relative POSIX path of the file holding the ledger table.</summary>
    public string LedgerPath { get; }

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
        string ledgerPath,
        string idHeader,
        string severityHeader,
        string stateHeader,
        IReadOnlyList<string> severitySet,
        IReadOnlyList<string> stateSet)
    {
        LedgerPath = ledgerPath;
        IdHeader = idHeader;
        SeverityHeader = severityHeader;
        StateHeader = stateHeader;
        SeveritySet = severitySet;
        StateSet = stateSet;
    }

    /// <summary>Loads the config from a YAML file; returns null when the file is absent.</summary>
    public static LedgerFormConfig? Load(string yamlPath)
    {
        if (!File.Exists(yamlPath))
        {
            return null;
        }

        var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        LedgerFormFile? raw = deserializer.Deserialize<LedgerFormFile>(File.ReadAllText(yamlPath));
        if (raw is null || string.IsNullOrWhiteSpace(raw.ledger_path))
        {
            throw new InvalidOperationException($"{yamlPath}: ledger_path is required");
        }

        return new LedgerFormConfig(
            raw.ledger_path,
            raw.id_header ?? "F-#",
            raw.severity_header ?? "Severity",
            raw.state_header ?? "State",
            raw.severity_set ?? new List<string>(),
            raw.state_set ?? new List<string>());
    }

    // YamlDotNet DTO (snake_case field names match the file).
    private sealed class LedgerFormFile
    {
        public string? ledger_path { get; set; }
        public string? id_header { get; set; }
        public string? severity_header { get; set; }
        public string? state_header { get; set; }
        public List<string>? severity_set { get; set; }
        public List<string>? state_set { get; set; }
    }
}
