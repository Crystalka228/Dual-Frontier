using DualFrontier.Governance;

namespace DualFrontier.Governance.Tests;

/// <summary>
/// Builds a <see cref="Frontmatter"/> that passes every schema check and every
/// gate (the NIH validDoc(overrides) idiom). Tests spread overrides to construct
/// a single violation; a null override value removes that key (the missing-field
/// case). This is the shared green baseline for the failability suite.
/// </summary>
internal static class Fixture
{
    public static Frontmatter Doc(params (string Key, object? Value)[] overrides)
    {
        var fields = new Dictionary<string, object?>
        {
            ["register_id"] = "DOC-D-EXAMPLE_CASCADE_1",
            ["project"] = "Dual Frontier",
            ["category"] = "D",
            ["tier"] = 3,
            ["lifecycle"] = "EXECUTED",
            ["owner"] = "Crystalka",
            ["version"] = "1.0",
            ["first_authored"] = "2026-07-15",
            ["last_modified"] = "2026-07-15",
            ["content_language"] = "en",
            ["next_review_due"] = "null",
        };

        foreach (var (key, value) in overrides)
        {
            if (value is null)
            {
                fields.Remove(key);
            }
            else
            {
                fields[key] = value;
            }
        }

        string id = fields.TryGetValue("register_id", out object? v) ? v?.ToString() ?? "x" : "x";
        return new Frontmatter(fields, $"fixtures/{id}.md");
    }
}
