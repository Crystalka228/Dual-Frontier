---
register_id: DOC-F-TESTS-RUNTIME-VULKAN
project: Dual Frontier
category: F
tier: 4
lifecycle: Live
owner: Crystalka
version: Live
first_authored: 2026-05-18
last_modified: 2026-05-18
content_language: en
next_review_due: 2026-Q4
title: DualFrontier.Runtime.Tests / Vulkan — module doc
last_modified_commit: a9e5ebb
review_cadence: phase-led
reviewer: Crystalka
special_case_rationale: Enrolled at CORPUS_CLOSURE_INVERSION_B CD2 per the ratified Cascade-B orphan triage (enroll F/4); real git provenance.
---

# DualFrontier.Runtime.Tests / Vulkan

P/Invoke ABI alignment audit tests per Lesson #7 strengthening (V0.A executor finding).

## Discipline

Every new Vulkan struct that crosses the managed → native boundary via P/Invoke
gets a `Marshal.SizeOf<T>()` verification test against the Vulkan 1.3 spec sizeof
on x64 (MSVC ABI). Catches alignment regressions before they corrupt Vulkan
driver state at runtime.

The pattern surfaced organically when V0.A executor landed VkPhysicalDeviceProperties
824-byte fix (was 816 bytes — missing `_padBeforeLimits[4]` to align VkPhysicalDeviceLimits
(which contains VkDeviceSize 8-byte fields) at offset 296, then trailing 4-byte pad).

Each test is one assertion:
```csharp
[Fact]
public void VkXxxCreateInfo_Size_Matches_Spec()
{
    Marshal.SizeOf<VkXxxCreateInfo>().Should().Be(<spec_size>);
}
```

## V0.A baseline coverage (regression)

- VkApplicationInfo (48)
- VkPhysicalDeviceProperties (824)

## V0.B additions (per-commit)

Each commit landing new Vulkan structs extends this test file. Cf.
`tools/briefs/V0_B_EXECUTION_BRIEF.md` §1.8 (S-LOCK-8) and §3 per-commit task lists.
