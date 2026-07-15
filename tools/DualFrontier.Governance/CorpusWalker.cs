namespace DualFrontier.Governance;

/// <summary>
/// Enumerates governed Markdown documents under a root. Directory-level skips
/// (bin/obj/.git/node_modules/.vs) keep the walk fast; file-level scoping is the
/// caller's <see cref="ExclusionConfig"/>. Paths are returned project-relative
/// with POSIX separators so the derived register is stable across operating
/// systems (parity with sync_register.ps1 / the NIH sync walk).
///
/// A filesystem walk -- rather than <c>git ls-files</c> -- is deliberate: the
/// same code must run against the non-git scratch copy the migrate dry-run
/// operates on, where <c>git ls-files</c> would return nothing.
/// </summary>
public static class CorpusWalker
{
    private static readonly HashSet<string> SkipDirs = new(StringComparer.OrdinalIgnoreCase)
    {
        ".git", "bin", "obj", "node_modules", ".vs", ".pnpm-store",
    };

    /// <summary>Yields every <c>*.md</c> under <paramref name="root"/> as a POSIX relative path.</summary>
    public static IEnumerable<string> MarkdownRelPaths(string root)
    {
        foreach (string abs in WalkMarkdown(root))
        {
            yield return ToPosixRelative(root, abs);
        }
    }

    private static IEnumerable<string> WalkMarkdown(string dir)
    {
        string[] files;
        string[] subdirs;
        try
        {
            files = Directory.GetFiles(dir, "*.md");
            subdirs = Directory.GetDirectories(dir);
        }
        catch (Exception ex) when (ex is DirectoryNotFoundException or UnauthorizedAccessException)
        {
            files = Array.Empty<string>();
            subdirs = Array.Empty<string>();
        }

        foreach (string file in files)
        {
            yield return file;
        }

        foreach (string sub in subdirs)
        {
            if (SkipDirs.Contains(Path.GetFileName(sub)))
            {
                continue;
            }

            foreach (string nested in WalkMarkdown(sub))
            {
                yield return nested;
            }
        }
    }

    /// <summary>Project-relative POSIX path (forward slashes) for a file under a root.</summary>
    public static string ToPosixRelative(string root, string absPath) =>
        Path.GetRelativePath(root, absPath).Replace('\\', '/');
}
