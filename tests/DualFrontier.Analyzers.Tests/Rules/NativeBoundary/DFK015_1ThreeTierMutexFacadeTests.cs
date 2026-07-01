using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.NativeBoundary.DFK015_1ThreeTierMutexFacadeAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.NativeBoundary;

/// <summary>
/// DFK015_1 — a raw heavyweight OS sync primitive (new Mutex / new Semaphore) bypasses
/// the three-tier mutex facade. Coverage anchor: К-L15.1 / K_CLOSURE §7.2 (DF015.1).
/// Real = 0 (no such sites on disk). SemaphoreSlim / lock are not flagged.
/// </summary>
public sealed class DFK015_1ThreeTierMutexFacadeTests
{
    [Fact]
    public async Task DFK015_1_Fires_On_Raw_Mutex()
    {
        const string source = """
            using System.Threading;

            namespace App
            {
                internal static class Sync
                {
                    public static Mutex Make() => {|DFK015_1:new Mutex()|};
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DFK015_1_Silent_On_SemaphoreSlim()
    {
        const string source = """
            using System.Threading;

            namespace App
            {
                internal static class Sync
                {
                    public static SemaphoreSlim Make() => new SemaphoreSlim(1);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(source);
    }
}
