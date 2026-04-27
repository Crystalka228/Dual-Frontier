# mods/

Example mods. Each is a separate assembly that sees ONLY `DualFrontier.Contracts`.
See [docs/MODDING.md](../docs/MODDING.md).

## Contents

- `DualFrontier.Mod.Example/` — reference minimal mod.
  Demonstrates the correct pattern: `IMod` + `mod.manifest.json`,
  with a dependency only on `DualFrontier.Contracts`.

## Rules

- A mod assembly MUST NOT reference `Core`, `Systems`, `Components`,
  `Events`, `AI`, or `Application`. Only `Contracts`.
- The core's `AssemblyLoadContext` enforces this physically — any
  additional reference produces a load error.
- Every mod MUST ship with `mod.manifest.json` next to the dll.
