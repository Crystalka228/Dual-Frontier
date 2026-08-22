using System.Threading.Tasks;
using Xunit;
using Verify = DualFrontier.Analyzers.Tests.Verifiers.CSharpAnalyzerVerifier<
    DualFrontier.Analyzers.Rules.NativeBoundary.DFK022EntityIdentityAnalyzer>;

namespace DualFrontier.Analyzers.Tests.Rules.NativeBoundary;

/// <summary>
/// DFK022 — a literal in the Version position of <c>new EntityId(...)</c> is a
/// fabricated generation. Coverage anchor: К-L22 / IDENTITY_AND_ABI_CONTRACT §2
/// (F-59, cascade ID_B_ENTITY_VERSIONS). Real violations on disk = 1, waived:
/// <c>EntityEncoder.cs</c> decodes persisted index ranges and defers version
/// truth to the A7 persistence contract.
/// </summary>
public sealed class DFK022EntityIdentityTests
{
    /// <summary>
    /// A stand-in for the real record struct. It is the shape the rule keys on:
    /// positional parameters named Index and Version, so the Version half is
    /// identified by parameter name rather than by ordinal.
    /// </summary>
    private const string Contracts = """
        namespace DualFrontier.Contracts.Core
        {
            public readonly record struct EntityId(int Index, int Version)
            {
                public static readonly EntityId Invalid = default;
            }
        }
        """;

    [Fact]
    public async Task DFK022_Fires_On_Fabricated_Version_Zero()
    {
        // Version 0 is the fabrication the whole cascade exists to remove: it
        // matches a never-recycled slot and nothing else.
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object Go(int index)
                        => new DualFrontier.Contracts.Core.EntityId(index, {|DFK022:0|});
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Fires_On_Any_Integer_Literal_Not_Just_Zero()
    {
        // The pre-W3 fabrication was Version = 1, which matched NO entity at all.
        // The rule flags any literal, because the defect is inventing the value,
        // not the particular value invented.
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object One(int index)
                        => new DualFrontier.Contracts.Core.EntityId(index, {|DFK022:1|});

                    public static object Seven(int index)
                        => new DualFrontier.Contracts.Core.EntityId(index, {|DFK022:7|});
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Fires_On_Constants_That_Are_Not_Literals()
    {
        // PR #51 review R4. A literal-only check let these through: -1 is a unary
        // operation wrapping a literal, and a named const is a field reference.
        // Both are compile-time constants, so both are fabrications by
        // definition — the world cannot have handed back a value fixed at build
        // time. Detection is on the constant VALUE, not the operation kind.
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    private const int CachedVersion = 0;

                    public static object Negated(int index)
                        => new DualFrontier.Contracts.Core.EntityId(index, {|DFK022:-1|});

                    public static object NamedConstant(int index)
                        => new DualFrontier.Contracts.Core.EntityId(index, {|DFK022:CachedVersion|});
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Silent_On_A_Foreign_Type_Named_EntityId()
    {
        // PR #51 review R5. The invariant governs Contracts' EntityId. Matching
        // on the short type name would hand an unrelated type carrying a Version
        // parameter a build-breaking Error it has no way to satisfy — it does not
        // even have a world to ask.
        const string test = """


            namespace ThirdParty.Physics
            {
                public readonly record struct EntityId(int Index, int Version);

                internal static class Caller
                {
                    public static object Go(int index) => new EntityId(index, 0);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Fires_On_Named_Argument_Out_Of_Order()
    {
        // Detection is by PARAMETER NAME, so reordering the arguments does not
        // hide the fabrication — and does not make the honest Index literal fire.
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object Go(int index)
                        => new DualFrontier.Contracts.Core.EntityId(Version: {|DFK022:0|}, Index: index);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Silent_On_The_Versions_Idiom()
    {
        // The shipped form: the version comes from the world's own table, read at
        // the ENTITY INDEX (versions is not parallel to the dense span).
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object Go(System.ReadOnlySpan<int> indices,
                                            System.ReadOnlySpan<int> versions,
                                            int i)
                        => new DualFrontier.Contracts.Core.EntityId(
                               indices[i], versions[indices[i]]);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Silent_On_Invalid_And_Default()
    {
        // Naming the sentinel is not fabricating a generation, and neither form
        // is a constructor invocation, so neither is even reachable by the rule.
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object Sentinel() => DualFrontier.Contracts.Core.EntityId.Invalid;
                    public static object Defaulted() => default(DualFrontier.Contracts.Core.EntityId);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Silent_On_Index_Literal_With_Real_Version()
    {
        // A literal INDEX is not the defect — the world's index space is knowable
        // managed-side. Only the Version half is the world's secret.
        const string test = """


            namespace DualFrontier.Systems
            {
                internal static class Caller
                {
                    public static object Go(int version)
                        => new DualFrontier.Contracts.Core.EntityId(1, version);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Silent_Inside_Core_Interop_Internals()
    {
        // The interop layer reconstitutes an id from the ABI's packed halves; it
        // is the one place entitled to assemble a pair from raw values.
        const string test = """


            namespace DualFrontier.Core.Interop.Marshalling
            {
                internal static class Packing
                {
                    public static object Go(int index)
                        => new DualFrontier.Contracts.Core.EntityId(index, 0);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Silent_In_Test_Namespaces()
    {
        // Fixtures legitimately name specific (index, version) pairs to assert
        // behaviour at a corner — the DFK011 precedent.
        const string test = """


            namespace DualFrontier.Core.Tests.ECS
            {
                internal static class Caller
                {
                    public static object Go() => new DualFrontier.Contracts.Core.EntityId(0, 1);
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }

    [Fact]
    public async Task DFK022_Waiver_Suppresses_The_Persistence_Site()
    {
        // The EntityEncoder shape: a pragma pair around the decode loop, carrying
        // its CODING_STANDARDS §5.3 authority citation. Pinned by the DFK-WAIVER
        // census (CensusMetaTests), which this cascade moves 2 -> 3.
        const string test = """


            namespace DualFrontier.Persistence.Compression
            {
                internal static class Decoder
                {
                    public static object Go(int start)
                    {
            // DFK-WAIVER(DFK022): decodes persisted index ranges; version truth is A7's.
            #pragma warning disable DFK022
                        return new DualFrontier.Contracts.Core.EntityId(start, 0);
            #pragma warning restore DFK022
                    }
                }
            }
            """;
        await Verify.VerifyAnalyzerAsync(Contracts + test);
    }
}
