namespace DualFrontier.Governance;

/// <summary>
/// Extracts the YAML frontmatter block from a Markdown document's full text.
///
/// Non-README documents carry the block at the TOP: the first non-blank line is a
/// <c>---</c> fence, terminated by the next <c>---</c> (the NIH extractFrontmatter
/// semantic). README.md documents carry the register mirror at END-of-file -- the
/// documented GitHub front-page policy (sync_register.ps1) keeps the H1 title
/// visible -- so the extractor reads the LAST <c>---</c>-delimited block. A leading
/// UTF-8 BOM is tolerated (the FRAMEWORK.md mirror is written with one). Returns
/// <c>null</c> when no frontmatter block is present.
/// </summary>
public static class FrontmatterExtractor
{
    /// <summary>True when the path's leaf is <c>README.md</c> (end-of-file placement class).</summary>
    public static bool IsReadmeClass(string path) =>
        string.Equals(Path.GetFileName(path), "README.md", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the frontmatter body (the text between the fences), or <c>null</c>
    /// when absent. <paramref name="endOfFile"/> selects the README EOF block.
    /// </summary>
    public static string? Extract(string text, bool endOfFile)
    {
        // Normalize EOL and strip a leading BOM so the fence comparison is exact.
        string normalized = text.Replace("\r\n", "\n").TrimStart('﻿');
        string[] lines = normalized.Split('\n');
        return endOfFile ? ExtractAtEnd(lines) : ExtractAtStart(lines);
    }

    private static string? ExtractAtStart(string[] lines)
    {
        int start = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim().Length == 0)
            {
                continue;
            }

            if (lines[i].Trim() == "---")
            {
                start = i;
            }

            break; // only the first non-blank line may open the block
        }

        if (start == -1)
        {
            return null;
        }

        for (int i = start + 1; i < lines.Length; i++)
        {
            if (lines[i].Trim() == "---")
            {
                return string.Join("\n", lines[(start + 1)..i]);
            }
        }

        return null;
    }

    private static string? ExtractAtEnd(string[] lines)
    {
        int end = -1;
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            if (lines[i].Trim().Length == 0)
            {
                continue;
            }

            if (lines[i].Trim() == "---")
            {
                end = i;
            }

            break; // only the last non-blank line may close the block
        }

        if (end == -1)
        {
            return null;
        }

        for (int i = end - 1; i >= 0; i--)
        {
            if (lines[i].Trim() == "---")
            {
                return string.Join("\n", lines[(i + 1)..end]);
            }
        }

        return null;
    }
}
