using System.Globalization;
using System.Text.RegularExpressions;
using YamlDotNet.Core;

namespace DualFrontier.Governance;

/// <summary>
/// G-ROWFORM -- the F-ledger table keeps its own row form (FRAMEWORK 14.5).
/// STRUCTURAL checks are <see cref="Severity.Report"/> (exit-affecting under
/// <c>--armed</c>): every data row carries the header's cell count counted on
/// unescaped pipes, ends with a pipe, has an id of the form <c>F-&lt;n&gt;</c>,
/// and the ids are unique and strictly increasing, and no line shaped like a
/// row follows the end of the table (a line that does not start with a pipe
/// ends it, so a later row would be read by nothing). VOCABULARY checks are
/// <see cref="Severity.Monitor"/> (printed, exit-neutral): the severity cell is
/// in the legend set and the state cell's leading member is in the legend set.
/// The gate reads one file named by <see cref="LedgerFormConfig"/> and is inert
/// when the config or the file is absent, so a scratch root without a ledger
/// stays clean.
/// </summary>
public static class LedgerRowForm
{
    private static readonly Regex UnescapedPipe = new(@"(?<!\\)\|", RegexOptions.Compiled);
    private static readonly Regex IdForm = new(@"^F-(\d+)$", RegexOptions.Compiled);
    private static readonly Regex DetachedRow = new(@"^\s*\|\s*(F-\d+)\s*\|", RegexOptions.Compiled);
    private static readonly Regex LeadingMember = new(@"^[^A-Za-z0-9]*([A-Za-z][A-Za-z0-9-]*)", RegexOptions.Compiled);

    /// <summary>Runs the gate over the ledger a root's config names; empty when the config or the file is absent.</summary>
    public static IReadOnlyList<Finding> Validate(string repoRoot)
    {
        string configPath = RepoPaths.LedgerFormPath(repoRoot);
        LedgerFormConfig? config;
        try
        {
            config = LedgerFormConfig.Load(configPath);
        }
        catch (Exception ex) when (ex is InvalidOperationException or YamlException)
        {
            return new[]
            {
                new Finding(GateCatalog.RowForm, Severity.Report, "tools/governance/LEDGER_FORM.yaml",
                    $"ledger form config is unreadable -- {ex.Message}"),
            };
        }

        if (config is null)
        {
            return Array.Empty<Finding>();
        }

        string ledgerFile = Path.Combine(repoRoot, config.LedgerPath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(ledgerFile))
        {
            return Array.Empty<Finding>();
        }

        string[] lines = File.ReadAllText(ledgerFile).Replace("\r\n", "\n").Split('\n');
        return ValidateTable(lines, config);
    }

    /// <summary>A Markdown table separator row: pipe-delimited cells of dashes, optionally colon-aligned.</summary>
    private static readonly Regex SeparatorRow = new(@"^\|(\s*:?-{3,}:?\s*\|)+\s*$", RegexOptions.CultureInvariant);

    /// <summary>The pure check over the lines of the ledger file (no I/O).</summary>
    public static IReadOnlyList<Finding> ValidateTable(IReadOnlyList<string> lines, LedgerFormConfig config)
    {
        var findings = new List<Finding>();
        int headerIndex = FindHeader(lines, config.IdHeader);
        if (headerIndex < 0)
        {
            findings.Add(new Finding(GateCatalog.RowForm, Severity.Report, config.LedgerPath,
                $"no ledger table header opening with the \"{config.IdHeader}\" column; the gate cannot read the ledger"));
            return findings;
        }

        List<string> headerCells = Cells(lines[headerIndex]);
        int severityColumn = headerCells.FindIndex(c => c == config.SeverityHeader);
        int stateColumn = headerCells.FindIndex(c => c == config.StateHeader);
        int expectedCells = headerCells.Count;

        int previousNumber = -1;
        var seenIds = new HashSet<string>(StringComparer.Ordinal);

        // The line after the header must be the table's separator row. When it is not,
        // that line is read as a data row, so no row escapes the checks below.
        int i = headerIndex + 1;
        if (i < lines.Count && SeparatorRow.IsMatch(lines[i]))
        {
            i++;
        }
        else
        {
            findings.Add(Structural($"{config.LedgerPath}:{headerIndex + 2}",
                "the ledger table has no separator row after its header"));
        }

        for (; i < lines.Count && lines[i].StartsWith('|'); i++)
        {
            string line = lines[i];
            string subject = $"{config.LedgerPath}:{i + 1}";
            List<string> cells = Cells(line);
            string id = cells.Count > 0 ? cells[0] : string.Empty;
            string label = id.Length > 0 ? id : "(row)";

            if (!line.TrimEnd().EndsWith('|') || line.TrimEnd().EndsWith("\\|", StringComparison.Ordinal))
            {
                findings.Add(Structural(subject, $"{label}: the row does not end with a closing pipe"));
            }

            if (cells.Count != expectedCells)
            {
                findings.Add(Structural(subject,
                    $"{label}: {cells.Count} cells counted on unescaped pipes, the header declares {expectedCells}"));
            }

            Match idMatch = IdForm.Match(id);
            if (!idMatch.Success)
            {
                findings.Add(Structural(subject, $"{label}: the id \"{id}\" is not of the form F-<n>"));
            }
            else
            {
                int number = int.Parse(idMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                if (!seenIds.Add(id))
                {
                    findings.Add(Structural(subject, $"{id}: duplicate ledger id"));
                }
                else if (number <= previousNumber)
                {
                    findings.Add(Structural(subject,
                        $"{id}: ids are not strictly increasing (F-{previousNumber} comes before it)"));
                }

                previousNumber = number;
            }

            if (severityColumn >= 0 && severityColumn < cells.Count &&
                !config.SeveritySet.Contains(cells[severityColumn]))
            {
                findings.Add(Vocabulary(subject,
                    $"{label}: severity \"{cells[severityColumn]}\" is outside the legend set {{{string.Join(", ", config.SeveritySet)}}}"));
            }

            if (stateColumn >= 0 && stateColumn < cells.Count)
            {
                string? member = LeadingMember.Match(cells[stateColumn]) is { Success: true } m ? m.Groups[1].Value : null;
                if (member is null || !config.StateSet.Contains(member))
                {
                    findings.Add(Vocabulary(subject,
                        $"{label}: state leads with \"{member ?? cells[stateColumn]}\", outside the legend set {{{string.Join(", ", config.StateSet)}}}"));
                }
            }
        }

        for (int j = i; j < lines.Count; j++)
        {
            if (DetachedRow.Match(lines[j]) is { Success: true } detached)
            {
                findings.Add(Structural($"{config.LedgerPath}:{j + 1}",
                    $"{detached.Groups[1].Value}: a row-shaped line follows the end of the table, so the gate does not check it"));
            }
        }

        return findings;
    }

    private static int FindHeader(IReadOnlyList<string> lines, string idHeader)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith('|') && Cells(lines[i]) is { Count: > 0 } cells && cells[0] == idHeader)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// The trimmed cells of a table line, split on unescaped pipes. The leading
    /// pipe is dropped; a closing pipe contributes no extra cell, so a row that
    /// lacks its closing pipe and a row that has it count the same cells.
    /// </summary>
    private static List<string> Cells(string line)
    {
        string body = line.TrimEnd();
        if (body.StartsWith('|'))
        {
            body = body[1..];
        }

        List<string> parts = UnescapedPipe.Split(body).Select(p => p.Trim()).ToList();
        if (parts.Count > 0 && parts[^1].Length == 0 && line.TrimEnd().EndsWith('|'))
        {
            parts.RemoveAt(parts.Count - 1);
        }

        return parts;
    }

    private static Finding Structural(string subject, string message) =>
        new(GateCatalog.RowForm, Severity.Report, subject, message);

    private static Finding Vocabulary(string subject, string message) =>
        new(GateCatalog.RowForm, Severity.Monitor, subject, message);
}
