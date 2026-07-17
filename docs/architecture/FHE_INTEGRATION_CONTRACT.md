---
register_id: DOC-A-FHE_INTEGRATION_CONTRACT_V2
project: Dual Frontier
category: A
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: 1.0.1
first_authored: 2026-07-15
last_modified: 2026-07-17
content_language: en
next_review_due: 2027-Q3
title: FHE Integration Contract (authored rework; dormant-contract role, D1-D8 preserved)
supersedes:
- DOC-A-FHE_INTEGRATION_CONTRACT
last_modified_commit: d66e02e
review_cadence: on-change+annual
last_review_date: 2026-07-17
last_review_event: 'DRAFTS_RATIFICATION MC-1 (C5): candidate-banner class retired - banner to ratified-successor note (EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION carried), checklist line removed, Role to normative (ratified successor) where the candidate token was present, pending-amendment sentence to LOCKED form (ARCHITECTURE, CONTRACTS). Changelog status cells left as authored-session history per HALT-1 OD-2. PATCH 1.0.0 to 1.0.1.'
reviewer: Crystalka
special_case_rationale: Ratified LOCKED v1.0.0 2026-07-17 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION (checklist item [1]). Successor of DOC-A-FHE_INTEGRATION_CONTRACT per EVT-2026-07-15-CORPUS_REWORK_R3_SUBSTRATE; dormant-contract role, D1-D8 preserved ('The dormant period is unbounded.').
---

# FHE Integration Contract

The ratified, currently-dormant contract governing how fully homomorphic encryption would integrate with Dual Frontier's deterministic simulation core, if and when the three conditions in §D1 are ever simultaneously met.

> **Ratified successor (LOCKED v1.0.0 per EVT-2026-07-17-CORPUS_CLOSURE_RATIFICATION, 2026-07-17).** Successor of `docs/architecture/historical/FHE_INTEGRATION_CONTRACT.md` (DOC-A-FHE_INTEGRATION_CONTRACT, now SUPERSEDED). Produced by the corpus rework of 2026-07-15 (session report: [ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715](../reports/ARCHITECTURE_DECOMPOSITION_CONTRACTS_SESSION_20260715.md)); content verified against code at HEAD `35364c2`. (The contract itself was first ratified LOCKED v1.0 on 2026-05-06 and remained in force through v1.0.1; the 2026-07-17 event ratifies this reworked successor text — the dormant-contract Role below is unchanged law.)

## Status block

| Field | Value |
|---|---|
| Role | **dormant-contract.** Ratified law, currently binding, currently inert. Every decision below is in force; none has an implementation yet — the contract's designed resting state, not an execution gap. |
| Successor of | `docs/architecture/historical/FHE_INTEGRATION_CONTRACT.md` (DOC-A-FHE_INTEGRATION_CONTRACT) |
| Scope | The boundary between the deterministic simulation core and any future FHE-based multiplayer compute layer: what crosses it, in what form, under what failure discipline. |
| Non-goals | Save-file encryption (Persistence), transport encryption (TLS), mod signing (MOD_OS_ARCHITECTURE.md capability system). Not a schedule — activation timing is [ROADMAP](../ROADMAP.md)'s domain, gated on §D1. |
| Authority domains | FHE integration decisions D1–D8: activation gate, determinism class, boundary placement, operation-set floor, failure mode, mod-capability interaction, versioning, export-control posture. |
| Defers to | MOD_OS_ARCHITECTURE.md — capability model (§D6), SemVer model (§D7), structural-boundary discipline §D3/§D5 mirror. [METHODOLOGY](../methodology/METHODOLOGY.md) — ratification process. Determinism commitment: this contract's own §1 scope (§D2); class vocabulary: TIME_AND_CONSISTENCY_MODEL.md (AUTHORED draft). |

> **Ratified and dormant.** Binding law today; activation is gated on §D1, which depends on external industry conditions Dual Frontier does not control — not on anything this project schedules. Current runtime footprint, verified this pass at HEAD `35364c2`: `DualFrontier.Crypto.Future` (`src/DualFrontier.Crypto.Future/IHomomorphicComputeProvider.cs`) contains exactly two reserved interfaces, `IHomomorphicComputeProvider` and `IFheBoundaryParticipant`, both empty, **zero consumers** anywhere in the solution — no `ProjectReference` to `DualFrontier.Crypto.Future.csproj` exists outside its own project file. Dormant is not a defect; it is this contract doing exactly what it was ratified to do.

---

## 1. Scope

This contract governs the boundary between Dual Frontier's deterministic simulation core and any future homomorphic-encryption layer responsible for:

- Multiplayer state synchronization with cheat-resistant compute.
- Server-side validation of client state transitions without disclosure of raw client state.
- Cross-instance computation on encrypted simulation data where determinism MUST be preserved.

It does NOT govern: single-player save-file encryption (standard symmetric cryptography in the Persistence layer), network transport encryption (TLS at the protocol layer), or mod signing/verification (the capability system in MOD_OS_ARCHITECTURE.md).

The reading discipline mirrors MOD_OS_ARCHITECTURE.md's own preamble: a decision marked LOCKED binds every implementation pass; interpretations are footnoted at point of use; conflicts with another LOCKED spec resolve through escalation, not improvisation. If a decision below proves infeasible at integration time, the contract is reopened formally through the [METHODOLOGY](../methodology/METHODOLOGY.md) ratification process — not implicitly violated.

## 2. Decisions

### D1. Activation gate (three independent conditions)

FHE integration activates when ALL of the following hold simultaneously:

- A production-grade FHE library exists with overhead ≤100× over native compute for the operation set in §D4.
- The library exposes a stable C# binding or P/Invoke surface compatible with the .NET runtime `DualFrontier.Core` currently targets.
- At least one shipped game has demonstrated practical FHE integration in a multiplayer context.

Until all three hold, the contract remains active but dormant — the specified resting state, not a defect. Single-condition activation gates have historically produced premature integration of immature cryptographic primitives industry-wide; three independent conditions are structural protection against wishful activation.

### D2. Determinism preservation

The FHE layer MUST preserve simulation determinism. Where homomorphic operations introduce floating-point ambiguity — notably CKKS-family schemes — the integration MUST use exact-arithmetic schemes (BGV, BFV, or successor) for any operation participating in the deterministic core. Approximate schemes are permissible only for operations explicitly marked non-deterministic in the simulation contract; at v1.0 ratification, no such operations exist.

Determinism is non-negotiable per this contract's own §1 scope commitment ("determinism MUST be preserved"); determinism-class vocabulary: TIME_AND_CONSISTENCY_MODEL.md (AUTHORED draft). CKKS approximation compounds over simulation ticks; over a long session two clients on identical input would diverge indistinguishably from a desync bug. The contract treats this as disqualifying.

### D3. Boundary placement (structural, not optional)

The FHE boundary sits between the **state diff producer** (client-side simulation) and the **state diff validator** (server-side homomorphic compute). State diffs cross in encrypted form; raw simulation state never crosses in either direction.

The boundary is structural, not optional — optimizations that bypass it "for performance" violate the contract regardless of measured benefit. Optional boundaries decay under performance pressure; structural boundaries do not. The same discipline governs mod isolation elsewhere in the project — see MOD_OS_ARCHITECTURE.md's structural-boundary/isolation-enforcement discussion (the `RestrictedModApi` cast-prevention barrier is the concrete precedent) — and this contract carries it forward to the encryption layer unmodified.

### D4. Operation set (minimum, not maximum)

The FHE layer MUST support, at minimum:

- Integer addition and multiplication (resource accounting).
- Comparison primitives (capability validation).
- Bounded iteration (tick-bounded validation loops).

Operations outside this set MUST NOT depend on FHE compute; a feature that needs them is redesigned, not the contract amended. A minimum prevents feature creep at integration time; a maximum would invite scope expansion instead.

### D5. Failure mode (no silent plaintext fallback)

When FHE compute fails — timeout, library error, version mismatch — the affected multiplayer session MUST degrade to a documented fallback, and MUST NOT silently fall back to plaintext compute. Silent plaintext fallback is the worst possible failure mode: it preserves apparent function while defeating the entire purpose of the integration.

The fallback is read-only spectator state for the affected client until FHE compute is restored or the session ends, with a `FheComputeUnavailableEvent` (specified at v1.1, §5) so the menu UI surfaces the degradation rather than letting it pass unobserved. This mirrors the "crash with diagnostics rather than silent state corruption" discipline documented for mod isolation in MOD_OS_ARCHITECTURE.md. Silent fallback is the industry's standard failure mode for cryptographic integrations; explicit prohibition is required.

### D6. Mod system interaction

Mods MAY declare a capability requirement for FHE-protected operations through the MOD_OS_ARCHITECTURE.md capability model (verified current: `ManifestCapabilities.Parse` and the manifest `capabilities.required`/`.provided` grammar, see [MODDING](./MODDING.md) §7). Mods declaring this capability get additional review during manifest validation — specifically, that the declared operation set fits within §D4.

Mods MUST NOT bypass the FHE layer through raw network access. Bypass is a capability violation regardless of intent, enforced by the same `RestrictedModApi` runtime check (`src/DualFrontier.Application/Modding/RestrictedModApi.cs`) that enforces every other capability boundary today. The FHE layer adds no new mod-facing API surface beyond capability declaration; mod authors interact with it through existing `IModApi` operations whose backing implementation would route through homomorphic compute when a session is FHE-active.

### D7. Versioning

FHE library upgrades require contract review; the contract version bumps (1.0 → 1.1, etc.) when integration parameters change. The library version is recorded in shipped build metadata for forensic reproduction of historical sessions — without it, a save from library v1.0 cannot be replayed deterministically against v1.1 of the same library, breaking §D2.

This decision specifies where that recording lives: the same versioning layer that already carries `ContractsVersion.Current` for mod/kernel compatibility (`ContractsVersion.cs`, `DualFrontier.Contracts.Modding`), appending the FHE library version as a fourth field, ignored by existing logic when the library is absent. Two things worth being precise about, verified this pass: `ContractsVersion.Current` today is consumed only by mod-manifest validation — no fourth field exists on it, and nothing in `DualFrontier.Persistence` references `ContractsVersion` at all (`grep ContractsVersion` across `src/DualFrontier.Persistence` returns nothing; Persistence is itself orphaned from production per other corpus findings). This clause specifies *where the field should land when both the FHE integration and a real save path exist* — it is forward specification, not a description of wiring that exists today.

### D8. Cross-jurisdiction compliance

Some jurisdictions classify FHE as dual-use cryptography. The shipped product MUST include export-control documentation for any jurisdiction where Dual Frontier is distributed. Release-time concern, not an architectural one — noted here so it is not forgotten when integration activates. Compliance review is the release pipeline's responsibility, not `DualFrontier.Crypto.Future`'s.

## 3. Interface specification

The reserved interfaces live in `DualFrontier.Crypto.Future` (verified against `src/DualFrontier.Crypto.Future/IHomomorphicComputeProvider.cs` at HEAD `35364c2` — interface names and empty bodies reproduced exactly; doc comments abridged in this excerpt, and the file uses a file-scoped namespace):

```csharp
namespace DualFrontier.Crypto.Future
{
    /// Provider abstraction for fully homomorphic encryption operations.
    /// At contract version 1.0, no implementation exists. Reserved for
    /// future binding when §D1 conditions are met. Implementations MUST
    /// satisfy all decisions in this contract's version 1.0 or successor;
    /// partial conformance is not acceptable.
    public interface IHomomorphicComputeProvider
    {
        // Reserved. Method signatures specified in contract v1.1 once a
        // candidate library is identified and evaluated against §D1.
    }

    /// Marker for state diff producers participating in the FHE boundary.
    /// At v1.0 this marker exists but has no enforcement; enforcement
    /// activates concurrently with provider binding.
    public interface IFheBoundaryParticipant
    {
        // Reserved.
    }
}
```

`DualFrontier.Crypto.Future.csproj` is a bare class-library project with no dependencies beyond the SDK (verified). The empty-interface form is deliberate: it commits the namespace and names the participant types without committing to any specific library's API shape. A populated v1.0 interface would lock the project to whichever library shape dominated at time of writing; the empty form leaves the eventual binding free to match whatever shape the industry settles on.

## 4. Decision log

| ID | Decision | Rationale |
|---|---|---|
| D1 | Three-condition activation gate | Single-condition gates have historically produced premature integration of immature primitives; three independent conditions are structural protection against wishful activation. |
| D2 | Exact-arithmetic schemes only for the deterministic core | CKKS-family approximation compounds non-determinism over ticks. Non-negotiable per §1's scope commitment (see §D2). |
| D3 | Structural boundary, not optional | Optional boundaries decay under performance pressure; structural ones do not. Mirrors the mod-isolation structural-barrier discipline (MOD_OS_ARCHITECTURE.md). |
| D4 | Minimum operation set, not maximum | Minimums prevent feature creep at integration time; maximums invite scope expansion. |
| D5 | No silent plaintext fallback | Silent fallback is the industry's standard cryptographic-integration failure mode. Read-only spectator state is the documented degradation. |
| D6 | Capability-system integration | FHE-aware mods face the same scrutiny as any capability-elevated mod; no new mod-facing surface beyond capability declaration. |
| D7 | Library version pinning | Forensic reproduction of historical sessions requires the exact library version — invariant preservation, not optimization. |
| D8 | Export-control awareness at release time | Architecturally adjacent, not central; noted to avoid surprise when integration activates. |

## 5. Open questions deferred to v1.1

> **FENCED (target / planned — not current truth):** deliberately unanswered at v1.0. Not gaps — they cannot be answered without a concrete library to bind against, and answering them speculatively would lock the project to assumptions that may not hold at integration time.

- Specific method signatures for `IHomomorphicComputeProvider`.
- Memory budget for encrypted state per session.
- Tick-budget allocation for homomorphic compute before a session degrades per §D5.
- Concrete fallback-latency thresholds before §D5 activates.
- Key-management strategy (likely distributed; specifics deferred until a library candidate exists).
- Specification of `FheComputeUnavailableEvent` referenced in §D5.

## 6. Notes for the future architect

When this document is read at integration time, expect: the library being bound against did not exist when this contract was written; performance characteristics differ from current expectations; at least one decision in §2 will feel inconvenient under the chosen library's constraints.

That third point is intentional. Inconvenient decisions in LOCKED specs are not bugs — they are the spec doing its job. If a decision feels inconvenient enough to warrant violation, the correct path is contract revision (v1.0 → v1.1) through the [METHODOLOGY](../methodology/METHODOLOGY.md) ratification process, not implicit non-compliance. This contract was written precisely because the technology did not exist at v1.0; specifications authored under the pressure of an existing implementation tend to legitimize that implementation's compromises. This one is not subject to that pressure. Treat it accordingly.

## Cross-references

| Doc | Relation | Note |
|---|---|---|
| [MOD_OS_ARCHITECTURE](./MOD_OS_ARCHITECTURE.md) | defers-to | Capability model (§D6), SemVer/versioning model (§D7), structural-boundary/isolation discipline (§D3, §D5) — successor of the old ISOLATION.md this contract used to cite directly. |
| [MODDING](./MODDING.md) | cites | Worked capability-declaration examples a §D6-compliant mod would follow. |
| [METHODOLOGY](../methodology/METHODOLOGY.md) | defers-to | Ratification process (Amendment protocol below). Determinism commitment lives in this contract's §1 scope (§D2); class vocabulary in TIME_AND_CONSISTENCY_MODEL.md (AUTHORED draft). |
| [ROADMAP](../ROADMAP.md) | cites | Forward scheduling and activation tracking authority — this document is not a roadmap. |

## Amendment protocol

This contract amends only through the [METHODOLOGY](../methodology/METHODOLOGY.md) ratification process — the same discipline MOD_OS_ARCHITECTURE.md uses. A decision D1–D8 does not change by implementation pressure, schedule constraint, or a library-specific limitation discovered at integration time; if a decision proves infeasible, the contract is reopened formally (version bump, new ratification pass) before the implementation proceeds. This document joined the LOCKED spec corpus at v1.0 (2026-05-06); v1.0.1 (2026-06-12) added the reclassification banner and ROADMAP pointer with contract decisions unchanged. Activation itself is never an amendment — it is §D1's conditions becoming true, tracked in [ROADMAP](../ROADMAP.md), not renegotiated here. The dormant period is unbounded.

## Change history

| Version | Date | Change |
|---|---|---|
| 0.1.1 (this doc) | 2026-07-17 | HALT-1-ratified review corrections (CORPUS_CLOSURE_INVERSION_B, D1 R3-22/R3-23): the four "METHODOLOGY §7.1 determinism invariant" citations (Defers-to, §D2 body, §4 D2 row, cross-ref table) re-anchored — no determinism invariant exists in the live METHODOLOGY; the commitment lives in this contract's own §1 scope, class vocabulary in the TCM draft; §3 transcription claim scoped to names+empty-bodies-exact (doc comments abridged; file-scoped namespace), and the 0.1.0 row's "re-verified verbatim" wording aligned below. |
| 0.1.0 (this doc) | 2026-07-15 | Corpus rework: restyled to the passport format (Status block, dormant-contract role, verified-footprint banner); D1–D8 decisions preserved faithfully; ISOLATION.md cross-references redirected to MOD_OS_ARCHITECTURE.md (merge target); §D7 corrected to note `ContractsVersion` currently has no fourth field and Persistence does not reference it — forward specification, not present wiring; `IHomomorphicComputeProvider`/`IFheBoundaryParticipant` re-verified against `src/DualFrontier.Crypto.Future/IHomomorphicComputeProvider.cs` (names + empty bodies exact; excerpt abridged), zero consumers confirmed. |
| 1.0.1 | 2026-06-12 | Predecessor: Architecture Truth Cascade — reclassification banner + ROADMAP pointer added, contract decisions unchanged. |
| 1.0 | 2026-05-06 | Predecessor: RATIFIED. Activation pending §D1 conditions. |