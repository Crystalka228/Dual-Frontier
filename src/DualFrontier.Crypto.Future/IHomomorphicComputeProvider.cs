namespace DualFrontier.Crypto.Future;

/// <summary>
/// Provider abstraction for fully homomorphic encryption operations.
///
/// Implementations of this interface bind to a specific FHE library.
/// At contract version 1.0, no implementation exists. The interface
/// is reserved for future binding when conditions in §D1 of the
/// FHE Integration Contract are met.
///
/// See <c>docs/FHE_INTEGRATION_CONTRACT.md</c> for the full contract.
///
/// Implementations MUST satisfy all decisions in
/// FHE_INTEGRATION_CONTRACT.md version 1.0 or successor.
/// Partial conformance is not acceptable.
/// </summary>
public interface IHomomorphicComputeProvider
{
    // Reserved. Method signatures are specified in contract v1.1
    // when a candidate library is identified and evaluated
    // against §D1.
}

/// <summary>
/// Marker for state diff producers that participate in the
/// FHE boundary. At contract version 1.0, this marker exists
/// but has no enforcement. Enforcement activates concurrently
/// with provider binding.
///
/// See <c>docs/FHE_INTEGRATION_CONTRACT.md</c> §D3 for the
/// boundary placement decision.
/// </summary>
public interface IFheBoundaryParticipant
{
    // Reserved.
}
