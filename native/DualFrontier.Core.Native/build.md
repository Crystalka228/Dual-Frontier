---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-NATIVE_BUILD
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-NATIVE_BUILD
---
---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-E-NATIVE_BUILD
category: E
tier: 3
lifecycle: EXECUTED
owner: Crystalka
version: "1.0"
next_review_due: "null"
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-E-NATIVE_BUILD
---
# DualFrontier.Core.Native — Build Instructions

This is the experimental C++ core. It is **not** part of the `.sln`; CMake
builds it independently and the resulting shared library is copied next to the
C# assembly that hosts the P/Invoke wrappers (`DualFrontier.Core.Interop`).

## Output artifacts

| Platform | File name |
|----------|-----------|
| Windows  | `DualFrontier.Core.Native.dll` |
| Linux    | `libDualFrontier.Core.Native.so` — rename to `DualFrontier.Core.Native.so` or set `DllImport(Name)` accordingly |
| macOS    | `libDualFrontier.Core.Native.dylib` |

The CMake target sets `PREFIX ""` so on Windows the output is exactly
`DualFrontier.Core.Native.dll`. On Linux/macOS the usual `lib` prefix is
suppressed; the default `SUFFIX` (`.so` / `.dylib`) is retained.

## Windows (Visual Studio 2022)

```powershell
cd native/DualFrontier.Core.Native
cmake -S . -B build -A x64
cmake --build build --config Release
# Artifact: build\Release\DualFrontier.Core.Native.dll
```

## Linux / WSL

```bash
cd native/DualFrontier.Core.Native
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build -j
# Artifact: build/DualFrontier.Core.Native.so
```

## macOS

```bash
cd native/DualFrontier.Core.Native
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build -j
# Artifact: build/DualFrontier.Core.Native.dylib
```

## Self-test

The build produces a standalone executable `df_native_selftest` that
exercises the full C ABI (CRUD, deferred destruction, swap-with-last remove,
throughput). Run it after every build to validate the library before the C#
benchmark picks it up:

```bash
./build/df_native_selftest
# -> ALL PASSED
```

## Copying the artifact to the C# project

The C# `DualFrontier.Core.Interop` project expects the native library to sit
next to its own assembly at runtime. Post-build copy example (PowerShell):

```powershell
Copy-Item native\DualFrontier.Core.Native\build\Release\DualFrontier.Core.Native.dll `
         src\DualFrontier.Core.Interop\bin\Release\net8.0\
```

Or automate it via an `AfterBuild` target inside
`DualFrontier.Core.Interop.csproj` once the native build path is stable — this
wasn't added to the csproj because the experiment keeps the two build systems
decoupled while feasibility is being assessed.
