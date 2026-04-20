# Modding — Загрузчик модов

## Назначение
Инфраструктура загрузки, выгрузки и изоляции модов. Каждый мод
живёт в отдельном `AssemblyLoadContext` (`ModLoadContext`) с
`isCollectible: true` — это физически препятствует моду дотянуться
до внутренностей ядра и позволяет горячую перезагрузку
(см. TechArch 11.8).

## Зависимости
- `DualFrontier.Contracts` — `IMod`, `IModApi`, `IModContract`, `ModManifest`
- `DualFrontier.Core` — `GameServices` (через прокси)

## Что внутри
- `ModLoader.cs` — API загрузки/выгрузки и реестр активных модов.
- `ModLoadContext.cs` — свой `AssemblyLoadContext` на каждый мод.
- `RestrictedModApi.cs` — реализация `IModApi`, проксирует вызовы в ядро
  с дополнительными проверками прав и квот.
- `ModIsolationException.cs` — бросается, если мод попытался достучаться
  до внутренностей ядра (обход `IModApi`).

## Правила
- Мод **видит** только сборку `DualFrontier.Contracts`. Всё остальное —
  через `IModApi`.
- Кастить `IModApi` к `RestrictedModApi` запрещено, детектируется.
- Ни одной ссылки на `DualFrontier.Core` из сборки мода напрямую —
  `AssemblyLoadContext` это гарантирует физически.
- `ModLoader.Unload` обязан дождаться завершения всех колбэков мода
  перед выгрузкой его контекста.

## Примеры использования
```csharp
var loader = new ModLoader(services);
loader.LoadMod("mods/DualFrontier.Mod.Example/bin/Debug/net8.0/");
foreach (var id in loader.GetLoaded())
{
    Console.WriteLine($"Loaded mod: {id}");
}
loader.UnloadMod("dualfrontier.example");
```

## TODO
- [ ] Фаза 2 — `ModLoader.LoadMod` (Manifest → Assembly → reflection `IMod`).
- [ ] Фаза 2 — `RestrictedModApi` проксирует в `GameServices`.
- [ ] Фаза 2 — тесты изоляции (`tests/DualFrontier.Modding.Tests`).
- [ ] Фаза 3 — горячая перезагрузка (Unload + повторный LoadMod).
