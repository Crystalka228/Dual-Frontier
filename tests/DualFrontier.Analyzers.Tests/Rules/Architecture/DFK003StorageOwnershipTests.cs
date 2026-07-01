using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.Architecture.DFK003StorageOwnershipAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.Architecture;

/// <summary>
/// DFK003 — Path α/β storage ownership: struct IComponent must NOT carry
/// [ManagedStorage] (Path α); class IComponent MUST carry it (Path β). Coverage
/// anchor: К-L3 / K_CLOSURE §7.2 (DF003).
/// </summary>
public sealed class DFK003StorageOwnershipTests
{
    private const string Contracts = """
        using System;

        namespace DualFrontier.Contracts.Core { public interface IComponent { } }

        namespace DualFrontier.Contracts.Modding
        {
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class ManagedStorageAttribute : Attribute { }
        }
        """;

    [Fact]
    public async Task DFK003_Fires_On_Class_Component_Without_ManagedStorage()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Core;

                public sealed class {|DFK003:BadClass|} : IComponent { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK003_Silent_On_Struct_PathAlpha()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Core;

                public struct GoodStruct : IComponent { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK003_Silent_On_Class_PathBeta()
    {
        const string test = """


            namespace App
            {
                using DualFrontier.Contracts.Core;
                using DualFrontier.Contracts.Modding;

                [ManagedStorage]
                public sealed class GoodClass : IComponent { }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }
}
