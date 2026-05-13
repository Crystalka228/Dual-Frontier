---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-FHE_INTEGRATION_CONTRACT
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-FHE_INTEGRATION_CONTRACT
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-A-FHE_INTEGRATION_CONTRACT
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-A-FHE_INTEGRATION_CONTRACT
---
# FHE Integration Contract

*Architectural contract specifying the integration of fully homomorphic encryption between Dual Frontier's deterministic simulation core and a future cheat-resistant multiplayer compute layer. This document follows the same discipline as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md): decisions are enumerated, ratified, and locked. Implementation is deferred pending technological maturity of the underlying libraries; the contract itself is binding from v1.0 onward.*

*Version: 1.0 (2026-05-06). RATIFIED. Activation pending §D1 conditions.*

---

## 0. Preamble

This document specifies the integration contract for fully homomorphic encryption (FHE) within Dual Frontier. The contract is LOCKED at v1.0 as of the date above. Implementation activation is conditional on §D1; the contract decisions themselves are not.

The contract follows the same ratification discipline as other LOCKED specifications in this project. Decisions enumerated below are not subject to revision through implementation pressure, schedule constraints, or library-specific limitations encountered at integration time. If circumstances at integration render a decision infeasible, the contract is reopened formally — through the ratification process described in [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — not implicitly violated.

The reading discipline mirrors [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §0: a decision marked LOCKED is binding for every implementation pass; deliberate interpretations are footnoted at point of use; conflicts between this document and another LOCKED spec resolve through escalation, not improvisation.

---

## 1. Scope

This contract governs the boundary between Dual Frontier's deterministic simulation core and any future homomorphic encryption layer responsible for:

- Multiplayer state synchronization with cheat-resistant compute.
- Server-side validation of client state transitions without disclosure of raw client state.
- Cross-instance computation on encrypted simulation data where determinism MUST be preserved.

The contract does NOT govern: single-player save file encryption (handled by standard symmetric cryptography in the Persistence layer), network transport encryption (handled by TLS at the protocol layer), or mod signing and verification (handled by the capability system specified in [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §3).

---

## 2. Decisions

### D1. Activation gate (three independent conditions)

FHE integration activates when ALL of the following conditions hold simultaneously:

- A production-grade FHE library exists with overhead ≤100× over native compute for the operation set defined in §D4.
- The library exposes a stable C# binding or P/Invoke surface compatible with the .NET runtime currently targeted by `DualFrontier.Core`.
- At least one shipped game in the industry has demonstrated practical FHE integration in a multiplayer context.

Until all three conditions hold, the contract remains active but dormant. The dormant state is not a defect; it is the contract's specified resting state. Single-condition activation gates have historically led to premature integration of immature cryptographic primitives across the industry; three independent conditions provide structural protection against wishful activation.

### D2. Determinism preservation

The FHE layer MUST preserve simulation determinism. Where homomorphic operations introduce floating-point ambiguity — notably in CKKS-family schemes — the integration MUST use exact-arithmetic schemes (BGV, BFV, or successor) for any operation participating in the deterministic core. Approximate schemes are permissible only for operations explicitly marked as non-deterministic in the simulation contract. At v1.0 ratification, no such operations exist.

Determinism is not negotiable per [METHODOLOGY](/docs/methodology/METHODOLOGY.md) §7.1. CKKS approximation introduces non-determinism that compounds over simulation ticks; over a long session, two clients running the same input would observe state divergence indistinguishable from a desync bug. The contract treats this as disqualifying.

### D3. Boundary placement (structural, not optional)

The FHE boundary sits between the **state diff producer** (client-side simulation) and the **state diff validator** (server-side homomorphic compute). State diffs cross the boundary in encrypted form. Raw simulation state never crosses the boundary in either direction.

The boundary is structural, not optional. Optimizations that bypass the boundary "for performance" violate the contract regardless of measured benefit. Optional boundaries decay under performance pressure; structural boundaries do not. The same discipline applies elsewhere in the project — see the isolation guard in [ISOLATION](./ISOLATION.md) — and the contract carries it forward to the encryption layer without modification.

### D4. Operation set (minimum, not maximum)

The FHE layer MUST support, at minimum, the following operations on encrypted state:

- Integer addition and multiplication (for resource accounting).
- Comparison primitives (for capability validation).
- Bounded iteration (for tick-bounded validation loops).

Operations outside this set MUST NOT depend on FHE compute. If a future feature requires operations outside this set, the feature is to be redesigned, not the contract amended. Specifying a minimum operation set prevents feature creep at integration time; specifying a maximum would invite scope expansion.

### D5. Failure mode (no silent plaintext fallback)

When FHE compute fails — timeout, library error, version mismatch — the affected multiplayer session MUST degrade to a documented fallback mode. The session MUST NOT silently fall back to plaintext compute. Silent plaintext fallback is the worst possible failure mode: it preserves apparent function while violating the entire purpose of the integration.

The fallback mode is: the session enters read-only spectator state for the affected client until FHE compute is restored or the session ends. The fallback emits a `FheComputeUnavailableEvent` (specified at v1.1, see §6) so the menu UI can surface the degradation to the player rather than letting it pass unobserved.

This decision mirrors the isolation guard's "crash with diagnostics rather than silent state corruption" discipline from [ISOLATION](./ISOLATION.md). Silent fallback is the standard failure mode of cryptographic integrations across the industry; explicit prohibition is required.

### D6. Mod system interaction

Mods MAY declare a capability requirement for FHE-protected operations through the [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) capability model. Mods that declare this capability are subject to additional review during the manifest validation pass — specifically, that the mod's declared operation set fits within §D4.

Mods MUST NOT bypass the FHE layer through raw network access. Bypass is a capability violation regardless of mod intent and is enforced by the same `RestrictedModApi` runtime check that enforces other capability boundaries. The FHE layer adds no new mod-facing API surface beyond capability declaration; mod authors interact with the layer through existing `IModApi` operations whose backing implementation is silently routed through homomorphic compute when the session is FHE-active.

### D7. Versioning

FHE library upgrades require contract review. The contract version is bumped (1.0 → 1.1, etc.) when integration parameters change. The library version is recorded in shipped game build metadata for forensic reproduction of historical sessions — without this record, a session save from v1.0 of an external library cannot be replayed deterministically against v1.1 of the same library, breaking the determinism guarantee in §D2.

The recording lives in the same Persistence layer that already pins `ContractsVersion.Current` for save-game compatibility (see [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) §8 on the three-tier SemVer model). FHE library version is appended as a fourth field; existing save-game logic ignores the field if the library is absent.

### D8. Cross-jurisdiction compliance

Some jurisdictions classify FHE as dual-use cryptography. The shipped product MUST include export-control documentation for any jurisdiction where Dual Frontier is distributed. This is a release-time concern rather than an architectural one, but is noted here so it is not forgotten when integration activates. Compliance review is the responsibility of the project's release pipeline, not of `DualFrontier.Crypto.Future` itself.

---

## 3. Interface specification

The reserved interface lives in `DualFrontier.Crypto.Future`. At contract version 1.0 the interface is empty — method signatures are deferred to v1.1, when a candidate library is identified and evaluated against §D1.

```csharp
namespace DualFrontier.Crypto.Future
{
    /// <summary>
    /// Provider abstraction for fully homomorphic encryption operations.
    ///
    /// Implementations of this interface bind to a specific FHE library.
    /// At contract version 1.0, no implementation exists. The interface
    /// is reserved for future binding when conditions in §D1 of the
    /// FHE Integration Contract are met.
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
    /// </summary>
    public interface IFheBoundaryParticipant
    {
        // Reserved.
    }
}
```

The empty-interface form is deliberate. It commits the namespace, names the participant types, and reserves the binding surface — without committing to any specific library's API shape. A populated interface at v1.0 would lock the project to whichever library shape happened to dominate at the time of writing; the empty form leaves the implementation free to bind against whatever shape the industry settles on.

---

## 4. Decision log

| ID | Decision | Rationale |
|---|---|---|
| D1 | Three-condition activation gate | Single-condition gates have historically led to premature integration of immature cryptographic primitives. Three independent conditions provide structural protection against wishful activation. |
| D2 | Exact-arithmetic schemes only for the deterministic core | CKKS-family approximation introduces non-determinism that compounds over simulation ticks. Determinism is non-negotiable per [METHODOLOGY](/docs/methodology/METHODOLOGY.md) §7.1. |
| D3 | Structural boundary, not optional | Optional boundaries decay under performance pressure. Structural boundaries do not. Mirrors the isolation-guard discipline from [ISOLATION](./ISOLATION.md). |
| D4 | Minimum operation set, not maximum | Specifying minimums prevents feature creep at integration time; specifying maximums would invite scope expansion. |
| D5 | No silent plaintext fallback | Silent fallback is the standard failure mode of cryptographic integrations across the industry. Explicit prohibition is required. Read-only spectator state is the documented degradation mode. |
| D6 | Capability system integration | FHE-aware mods are subject to the same scrutiny as any other capability-elevated mod. No special treatment, no new mod-facing API surface beyond capability declaration. |
| D7 | Library version pinning | Forensic reproduction of historical sessions requires the exact library version. This is invariant preservation, not optimization. Lives in the existing Persistence layer's version metadata. |
| D8 | Export-control awareness at release time | Architecturally adjacent rather than architecturally central. Noted to prevent surprise at release time when integration activates. |

---

## 5. Open questions deferred to v1.1

The following questions are deliberately not answered in v1.0. They are addressed in v1.1 when a candidate library is identified and evaluated against §D1:

- Specific method signatures for `IHomomorphicComputeProvider`.
- Memory budget for encrypted state per session.
- Tick budget allocation for homomorphic compute (how much of the per-tick scheduler budget is consumable by the encryption layer before the session degrades per §D5).
- Concrete fallback latency thresholds before §D5 activation.
- Key management strategy (likely distributed; specifics deferred until library candidate is identified).
- Specification of `FheComputeUnavailableEvent` referenced in §D5.

These are not gaps in v1.0. They are appropriately deferred questions that cannot be answered without a concrete library to bind against. Answering them speculatively at v1.0 would lock the project to assumptions that may not hold at integration time.

---

## 6. Notes for the future architect

When this document is read at integration time, the following will likely be true:

- The library being bound against did not exist when this contract was written.
- Performance characteristics will differ from current expectations.
- At least one decision in §D will feel inconvenient under the constraints of the chosen library.

The third point is intentional. Inconvenient decisions in LOCKED specs are not bugs; they are the spec doing its job. If a decision feels inconvenient enough to warrant violation, the correct path is contract revision (v1.0 → v1.1) through the [METHODOLOGY](/docs/methodology/METHODOLOGY.md) ratification process, not implicit non-compliance.

The contract was written precisely because the technology did not exist at v1.0. Specifications written under the pressure of an existing implementation tend to legitimize that implementation's compromises. This contract is not subject to that pressure. Treat it accordingly.

---

## 7. Ratification

This contract is ratified at v1.0 under the same discipline as [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) v1.0. It joins the LOCKED spec corpus.

Activation pending §D1 conditions. Dormant period is unbounded.

---

## See also

- [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) — capability model (§3) and three-tier SemVer model (§8) referenced by §D6 and §D7.
- [ISOLATION](./ISOLATION.md) — structural-boundary discipline referenced by §D3 and §D5.
- [METHODOLOGY](/docs/methodology/METHODOLOGY.md) — determinism invariant referenced by §D2; ratification process referenced by §6.
- [IDEAS_RESERVOIR](./IDEAS_RESERVOIR.md) — post-release ideas reservoir; this contract is referenced from there as the formal architectural commitment behind one of its entries.
