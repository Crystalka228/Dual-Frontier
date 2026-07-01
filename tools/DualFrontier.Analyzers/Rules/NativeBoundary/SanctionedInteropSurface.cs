using System;
using Microsoft.CodeAnalysis;

namespace DualFrontier.Analyzers.Rules.NativeBoundary;

/// <summary>
/// The federated, domain-bounded interop surface sanctioned for P/Invoke
/// (<c>[DllImport]</c> / <c>[LibraryImport]</c>) — the single §8 definition shared
/// by DFK002 (P/Invoke placement) and DFK001 (managed-bridge boundary).
/// </summary>
/// <remarks>
/// <para>
/// Ratified 2026-07-01 (PHASE_BETA_BRIEF §8). A namespace is sanctioned iff it is,
/// or is nested under, one of:
/// </para>
/// <list type="bullet">
///   <item><c>DualFrontier.Core.Interop</c> — the C++20 kernel boundary (К-L1 / К-L3).</item>
///   <item><c>DualFrontier.Runtime.Native</c> — the native runtime boundary
///   (<c>Runtime.Native.Vulkan</c> GPU substrate К-L19; <c>Runtime.Native.Win32</c>
///   Launcher OS surface).</item>
/// </list>
/// <para>
/// <c>ManagedBusBridge</c> (<c>DualFrontier.Application.Bus</c>) is deliberately NOT
/// sanctioned — it is the one genuine DFK002 violation, triaged at C9 (§8).
/// </para>
/// <para>К-L14 thesis: tooling addition; zero substrate touch.</para>
/// </remarks>
internal static class SanctionedInteropSurface
{
    private static readonly string[] SanctionedRoots =
    {
        "DualFrontier.Core.Interop",
        "DualFrontier.Runtime.Native",
    };

    /// <summary>
    /// True if <paramref name="containingNamespace"/> is, or is nested under, a
    /// sanctioned interop root.
    /// </summary>
    public static bool IsSanctioned(INamespaceSymbol? containingNamespace)
    {
        if (containingNamespace is null || containingNamespace.IsGlobalNamespace)
        {
            return false;
        }

        string ns = containingNamespace.ToDisplayString();
        foreach (string root in SanctionedRoots)
        {
            if (ns == root || ns.StartsWith(root + ".", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
