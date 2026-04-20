# mods/

Примеры модов. Каждый — отдельная сборка, видит ТОЛЬКО `DualFrontier.Contracts`.
См. [docs/MODDING.md](../docs/MODDING.md).

## Что внутри

- `DualFrontier.Mod.Example/` — референсный минимальный мод.
  Демонстрирует правильный паттерн: `IMod` + `mod.manifest.json`,
  зависимость только от `DualFrontier.Contracts`.

## Правила

- Сборка мода НЕ должна ссылаться на `Core`, `Systems`, `Components`,
  `Events`, `AI` или `Application`. Только `Contracts`.
- `AssemblyLoadContext` ядра гарантирует это физически — любой
  дополнительный reference приведёт к ошибке загрузки.
- Каждый мод обязан поставляться с `mod.manifest.json` рядом с dll.
