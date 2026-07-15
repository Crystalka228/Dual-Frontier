using YamlDotNet.Serialization;

namespace DualFrontier.Governance;

/// <summary>
/// The four hand-edited global collections (FRAMEWORK 14.6): requirements, risks,
/// CAPA entries, and the append-only audit trail. Their sources of truth are the
/// files docs/governance/{REQUIREMENTS,RISKS,CAPA,AUDIT_TRAIL}.yaml; the tool
/// validates them against the FRAMEWORK 4.3-4.7 enums (G-GLOBALS) and merges them
/// into the derived archive. Entries are kept as raw maps so their full content
/// round-trips into the archive without a lossy typed model.
/// </summary>
public sealed class GlobalsCollections
{
    public List<Dictionary<string, object?>> Requirements { get; init; } = new();
    public List<Dictionary<string, object?>> Risks { get; init; } = new();
    public List<Dictionary<string, object?>> Capa { get; init; } = new();
    public List<Dictionary<string, object?>> AuditTrail { get; init; } = new();

    /// <summary>True when at least one SoT file existed (they are created at migration).</summary>
    public bool AnyFilePresent { get; init; }

    private static readonly IDeserializer Deserializer =
        new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

    public static GlobalsCollections Load(string governanceDir)
    {
        var (req, p1) = LoadCollection<RequirementsFile>(
            Path.Combine(governanceDir, "REQUIREMENTS.yaml"), f => f.requirements);
        var (risk, p2) = LoadCollection<RisksFile>(
            Path.Combine(governanceDir, "RISKS.yaml"), f => f.risks);
        var (capa, p3) = LoadCollection<CapaFile>(
            Path.Combine(governanceDir, "CAPA.yaml"), f => f.capa_entries);
        var (evt, p4) = LoadCollection<AuditTrailFile>(
            Path.Combine(governanceDir, "AUDIT_TRAIL.yaml"), f => f.audit_trail);

        return new GlobalsCollections
        {
            Requirements = req,
            Risks = risk,
            Capa = capa,
            AuditTrail = evt,
            AnyFilePresent = p1 || p2 || p3 || p4,
        };
    }

    /// <summary>G-GLOBALS -- enum membership + id uniqueness within each collection (Report severity).</summary>
    public IEnumerable<Finding> Validate()
    {
        foreach (Finding f in ValidateCollection(Requirements, "requirement",
                     ("verification_status", Validators.ValidVerificationStatus)))
        {
            yield return f;
        }

        foreach (Finding f in ValidateCollection(Risks, "risk",
                     ("likelihood", Validators.ValidLikelihood),
                     ("impact", Validators.ValidImpact),
                     ("risk_type", Validators.ValidRiskType),
                     ("status", Validators.ValidRiskStatus)))
        {
            yield return f;
        }

        foreach (Finding f in ValidateCollection(Capa, "capa",
                     ("closure_status", Validators.ValidCapaStatus)))
        {
            yield return f;
        }

        foreach (Finding f in ValidateCollection(AuditTrail, "audit_trail",
                     ("event_type", Validators.ValidEventType)))
        {
            yield return f;
        }
    }

    private static IEnumerable<Finding> ValidateCollection(
        List<Dictionary<string, object?>> collection,
        string kind,
        params (string Field, IReadOnlySet<string> Allowed)[] enums)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (Dictionary<string, object?> entry in collection)
        {
            string id = entry.GetValueOrDefault("id")?.ToString() ?? "(no id)";
            if (id != "(no id)" && !seen.Add(id))
            {
                yield return new Finding("G-GLOBALS", Severity.Report, id, $"duplicate {kind} id \"{id}\"");
            }

            foreach (var (field, allowed) in enums)
            {
                if (entry.GetValueOrDefault(field)?.ToString() is { } value && !allowed.Contains(value))
                {
                    yield return new Finding("G-GLOBALS", Severity.Report, id,
                        $"{kind} {field} \"{value}\" is not in the sanctioned set");
                }
            }
        }
    }

    private static (List<Dictionary<string, object?>> List, bool Present) LoadCollection<TFile>(
        string path, Func<TFile, List<Dictionary<string, object?>>?> select)
    {
        if (!File.Exists(path))
        {
            return (new List<Dictionary<string, object?>>(), false);
        }

        TFile file = Deserializer.Deserialize<TFile>(File.ReadAllText(path));
        return (select(file) ?? new List<Dictionary<string, object?>>(), true);
    }

    private sealed class RequirementsFile { public List<Dictionary<string, object?>>? requirements { get; set; } }
    private sealed class RisksFile { public List<Dictionary<string, object?>>? risks { get; set; } }
    private sealed class CapaFile { public List<Dictionary<string, object?>>? capa_entries { get; set; } }
    private sealed class AuditTrailFile { public List<Dictionary<string, object?>>? audit_trail { get; set; } }
}
