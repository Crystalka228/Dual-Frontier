---
register_id: DOC-D-EQ_A4_RENDER_TAIL_BRIEF
project: Dual Frontier
category: D
tier: 3
lifecycle: EXECUTED
owner: Volodymyr (Crystalka)
version: '1.0'
first_authored: '2026-07-18'
last_modified: '2026-07-18'
content_language: en
next_review_due: null
title: EQ_A4_RENDER_TAIL execution brief (Cascade D, the EQ-a family CLOSER -- M6 swapchain prepare-before-reclaim + M9 device-lost v1 under ratified D1 fail-fast+diagnostic)
---

# EQ_A4_RENDER_TAIL -- Execution Brief

Cascade D, the LAST of the EQ-a shutdown-law family: the render/device tail. Two members:
M6 (swapchain recreation becomes prepare-before-reclaim to ELT section 2.5's letter) and
M9 (device-lost v1 under the ratified D1 policy). Closing this cascade closes the EQ-a
row. GPU/render scope, managed render stack expected -- if a native (C++/ABI) change
appears required, HALT H-ABI (nothing in this family's remaining scope should need one).
Anti-pattern rule: brief-vs-LOCKED-law conflict means THE BRIEF IS WRONG -- H-LAW, quote
both.

## 1. Ratified decisions (operator, chat, 2026-07-18)

- **D1 (device-lost v1): FAIL-FAST + DIAGNOSTIC.** A detected device loss produces a
  STRUCTURED diagnostic (which Vulkan call failed, frame/tick context, swapchain state)
  and terminates through a clean deliberate path -- never the current generic
  `InvalidOperationException` crash. NO recovery attempt in v1: device recreation is
  future work with its own design; a half-recovery here would be a kostyl. The
  diagnostic route must not use console output (census pin: `Environment.FailFast`
  carries the message, matching the EngineSession Abort precedent).
- **Closure semantics:** this cascade completes the EQ-a row. The ROADMAP closure text
  marks the family DONE end-to-end (M1-M9 with per-cascade hashes).

## 2. Established facts (EQ_A recon @ da63a93; counts re-verified at later closures --
re-verify ALL at Phase 0 and pin each figure to the measured HEAD)

- F1. Swapchain recreation ALREADY wires `oldSwapchain` (recon A5): the remaining gap is
  ONLY image views + framebuffers -- they are destroyed before their replacements exist.
  The member set SHRINKS relative to the original EQ-a work order; if Phase 0 finds the
  gap larger than views+framebuffers, H-SCOPE: report the measured surface, stop.
- F2. Device-lost today = an unhandled generic exception from the render path (recon
  A6); no structured detection at submit/present/wait sites.
- F3. A throwing-seam test precedent exists in the render test surface (recon M7.2
  reference) -- mirror it for injection rather than inventing a new seam style.
- F4. ELT section 2.5 defines the prepare-before-reclaim transaction law; ELT OQ-3 and
  IAC section 7 item 7-7 hold the device-lost open question that D1 now answers.
- F5. Gates baseline (at EQ_A3 closure, main 15f4ed0): builds 0W/0E both configs;
  full-sln 1191/0/5; native selftest 104 ALL PASSED both configs; validate --armed exit
  0; DFK-WAIVER 2=2; Console.WriteLine src=2; BoundaryRatchet 4+1; register 355.
- F6. Real-GPU verification precedent (EQ_A2/EQ_A3): operator machine RX 7600S; the
  closure includes a real Launcher run.

## 3. Phase 0 (stop on any failure)

1. HEAD recorded (15f4ed0 or descendant); clean tree; F5 gates re-run and pinned.
2. MANDATORY LAW READS: ELT sections 2.5 + 4 + OQ-3 IN FULL; IAC section 7 (item 7-7);
   VULKAN_SUBSTRATE's swapchain + device sections; K-L19 (or the LOCKED GPU invariant
   family) as applicable; CODING_STANDARDS render sections. The ELT 2.5 letter is the M6
   work order.
3. Line-read the swapchain recreation path end-to-end; enumerate EVERY resource
   destroyed/created during recreation with its current order; confirm F1's
   views+framebuffers-only claim (else H-SCOPE).
4. Enumerate the device-lost surfacing sites (submit, present, wait/fence, acquire);
   record how each fails today.
5. Locate the M7.2 throwing-seam precedent and record its pattern.

## 4. Commit plan (atomic; sync folded into frontmatter-touching commits)

- **C1 -- enroll.** Brief (Draft) committed; sync; validate.
- **C2 -- M6 prepare-before-reclaim.** Recreation builds the NEW image views +
  framebuffers first; only on full success are the old ones reclaimed; on any
  mid-prepare failure the OLD set remains intact and the renderer stays presentable
  (ELT 2.5 transaction semantics to the letter). Ordering comments cite ELT 2.5. Both
  configs build 0W/0E.
- **C3 -- M6 failure-injection test.** Using the F3 seam pattern: inject a prepare-step
  failure; assert the old swapchain resources survive, no leak (validation layer clean),
  and a subsequent successful recreation works. If the seam requires redesign beyond the
  precedent's shape, H-SEAM: propose, stop.
- **C4 -- M9 device-lost v1.** Detection at the enumerated sites -> a single typed
  carrier (e.g. `DeviceLostException` or result-object, match the codebase's error
  style) with the structured payload (failed call, frame/tick, swapchain state) ->
  caught at the render-loop boundary -> `Environment.FailFast` with the diagnostic text.
  The generic-exception path dies. Class docs cite D1 + ELT OQ-3 resolution; explicit
  "recovery = future work" note with no half-measures.
- **C5 -- M9 tests.** Seam-injected device-lost at each detection class; assert the
  structured payload content and the fail-fast route (recorder seam, mirroring the
  EngineSession OnAbort pattern -- never a real FailFast in tests). Census pins hold.
- **C6 -- governance.** ELT: OQ-3 CLOSED by D1 (recorded verbatim), 2.5 Realized note
  w/ hashes (PATCH or MINOR per its own change-scale rules); IAC: item 7-7 annotated
  resolved-by-D1 (PATCH); VULKAN_SUBSTRATE: recreation-order + device-lost sections
  aligned to the landed truth (PATCH); ROADMAP: M6+M9 DONE, THE EQ-a ROW CLOSED
  end-to-end (M1-M9 map w/ cascade hashes EQ_A1..EQ_A4); sync; validate.
- **C7 -- closure.** EVT append (prior entries byte-unchanged); brief -> EXECUTED;
  closure report enrolled (schema section 6); REAL-GPU verification recorded: Launcher
  launch-and-close clean exit; if a window-resize path exists, exercise one live
  swapchain recreation and record it.

## 5. Halt catalog

H-LAW / H-ABI (native change appears required) / H-SCOPE (recreation gap exceeds
views+framebuffers) / H-SEAM (injection needs redesign) / H-GATE (validate nonzero;
build/test/selftest regression vs F5; DFK-WAIVER moves; Console.WriteLine census moves;
ratchet moves). Standing rails: never push; no history rewrite; AUDIT_TRAIL append-only;
derived via sync only; historical/ read-only.

## 6. Closure report schema

HEAD before/after; per-commit hashes; the recreation-order table BEFORE -> AFTER; the
device-lost site inventory w/ per-site handling; injection-test evidence; EQ-a family
completion map (M1-M9 -> cascade -> hashes); final gates (F5 shape + deltas); real-GPU
evidence; register/EVT deltas; attestation.

## 7. Out of scope (fenced)

Device recovery/recreation (explicit future work); any W1+ boundary work; F-51 Smoke-F02
(its own session -- if its VertexBufferRing surface overlaps a touched file, do NOT fix
it, note the adjacency); F-43; PSC; render feature work of any kind.
