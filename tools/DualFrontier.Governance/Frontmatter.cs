using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DualFrontier.Governance;

/// <summary>
/// A parsed document frontmatter block: the inverted register's source of truth
/// for one document (FRAMEWORK 14.1). Wraps the raw YAML mapping with typed
/// accessors the gates read. Scalars are coerced to strings deterministically --
/// including any <see cref="DateTime"/> YamlDotNet may infer from an unquoted
/// ISO date -- so <c>next_review_due: 2027-05-25</c> reads as the string
/// "2027-05-25" regardless of the deserializer's scalar-typing.
/// </summary>
public sealed class Frontmatter
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder().Build();

    private readonly IReadOnlyDictionary<string, object?> _fields;

    /// <summary>Project-relative POSIX path of the document this frontmatter belongs to.</summary>
    public string RelPath { get; }

    public Frontmatter(IReadOnlyDictionary<string, object?> fields, string relPath)
    {
        _fields = fields;
        RelPath = relPath;
    }

    /// <summary>
    /// Parses a frontmatter YAML body. Returns the document, or an error message
    /// when the body is not a YAML mapping (a G-SCHEMA parse error).
    /// </summary>
    public static (Frontmatter? Doc, string? Error) TryParse(string yamlBody, string relPath)
    {
        try
        {
            var raw = Deserializer.Deserialize<Dictionary<string, object?>>(yamlBody);
            if (raw is null)
            {
                return (null, "frontmatter is empty");
            }

            return (new Frontmatter(raw, relPath), null);
        }
        catch (YamlException ex)
        {
            return (null, $"YAML parse error -- {ex.Message}");
        }
        catch (InvalidCastException)
        {
            return (null, "frontmatter is not a YAML mapping");
        }
    }

    /// <summary>True when the key is present (regardless of value) -- the required-field test.</summary>
    public bool ContainsKey(string key) => _fields.ContainsKey(key);

    /// <summary>The raw deserialized value for a key, or null when absent / YAML-null.</summary>
    public object? Raw(string key) => _fields.TryGetValue(key, out var v) ? v : null;

    /// <summary>Scalar as an invariant string (ISO for dates), or null when absent / YAML-null.</summary>
    public string? Str(string key) => Raw(key) switch
    {
        null => null,
        DateTime dt => dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        var v => v.ToString(),
    };

    /// <summary>Scalar parsed as an int, or null when absent / non-numeric.</summary>
    public int? Int(string key) =>
        Str(key) is { } s && int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n)
            ? n
            : null;

    /// <summary>A YAML sequence as strings (empty when absent or not a sequence).</summary>
    public IReadOnlyList<string> List(string key) =>
        Raw(key) is IEnumerable<object?> seq
            ? seq.Where(x => x is not null).Select(x => x!.ToString()!).ToList()
            : Array.Empty<string>();

    // --- schema v2 typed shortcuts (FRAMEWORK 14.3) ---

    public string? RegisterId => Str("register_id");
    public string? Category => Str("category");
    public int? Tier => Int("tier");
    public string? Lifecycle => Str("lifecycle");
    public string? Version => Str("version");
    public string? ContentLanguage => Str("content_language");
    public string? SpecialCaseRationale => Str("special_case_rationale");
    public string? LastModifiedCommit => Str("last_modified_commit");
    public string? SupersededBy => Str("superseded_by");
    public string? DeprecatedBy => Str("deprecated_by");
    public IReadOnlyList<string> Supersedes => List("supersedes");

    /// <summary>The next_review_due value coerced to its sentinel string ('null' when YAML-null/absent).</summary>
    public string NextReviewDue => Str("next_review_due") ?? "null";

    /// <summary>A non-empty special_case_rationale is a FRAMEWORK 3.4.2 sanctioned deviation (exempts G-CATLIFE).</summary>
    public bool HasSanctionedDeviation => !string.IsNullOrWhiteSpace(SpecialCaseRationale);

    /// <summary>Identifier used as the subject of a finding (id when present, else the path).</summary>
    public string Subject => RegisterId ?? RelPath;

    /// <summary>The raw parsed fields, in parse order -- used to re-emit the entry into the derived archive.</summary>
    public IReadOnlyDictionary<string, object?> Fields => _fields;
}
