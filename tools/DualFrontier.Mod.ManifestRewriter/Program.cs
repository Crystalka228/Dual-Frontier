using System;

namespace DualFrontier.Mod.ManifestRewriter;

internal static class Program
{
    private static int Main(string[] args)
    {
        string? path = null;
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--path")
            {
                path = args[i + 1];
                break;
            }
        }

        if (path is null)
        {
            Console.Error.WriteLine("Usage: manifest-rewriter --path <manifest.json>");
            return 1;
        }

        ManifestRewriter.Result result = ManifestRewriter.Rewrite(path);
        return result switch
        {
            ManifestRewriter.Result.Rewritten => 0,
            ManifestRewriter.Result.AlreadyFalse => 0,
            ManifestRewriter.Result.FieldAbsent => 0,
            ManifestRewriter.Result.NotFound => Fail(1, $"Manifest not found: {path}"),
            ManifestRewriter.Result.ParseError => Fail(2, $"Invalid JSON: {path}"),
            ManifestRewriter.Result.WriteError => Fail(3, $"Cannot write: {path}"),
            _ => Fail(99, "Unknown result"),
        };
    }

    private static int Fail(int code, string message)
    {
        Console.Error.WriteLine(message);
        return code;
    }
}
