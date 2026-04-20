# Modding — API модификаций

## Назначение
Публичный API модов. Каждый мод реализует `IMod`, получает `IModApi` и
работает с ядром только через него. AssemblyLoadContext физически блокирует
моду доступ к внутренностям `DualFrontier.Core` — единственная ссылка, которую
мод имеет — эта сборка (Contracts).

## Зависимости
- `DualFrontier.Contracts.Core` (маркеры `IEvent`, `IComponent`).

## Что внутри
- `IMod.cs` — точка входа мода: `Initialize(api)` и `Unload()`.
- `IModApi.cs` — методы, которые мод может вызвать: регистрация компонентов
  и систем, публикация/подписка на события, публикация и получение
  межмодовых контрактов.
- `IModContract.cs` — маркер-интерфейс публичного контракта между модами.
- `ModManifest.cs` — метаданные мода: id, имя, версия, автор, зависимости.

## Правила
- Мод НЕ имеет права приводить `IModApi` к конкретному типу — это попытка
  обойти изоляцию.
- Мод взаимодействует с другими модами только через `IModContract`.
- Жёсткие зависимости между модами запрещены: используй
  `TryGetContract<T>` и gracefully degrade если контракт не найден.

## Примеры использования
```csharp
public sealed class ExampleMod : IMod
{
    public void Initialize(IModApi api)
    {
        api.RegisterComponent<MyComponent>();
        api.Subscribe<SpellCastEvent>(OnSpellCast);
    }

    public void Unload() { /* TODO: cleanup */ }

    private void OnSpellCast(SpellCastEvent e) { /* ... */ }
}
```

## TODO
- [ ] Фаза 2 — описать структуру `mod.manifest.json` и маппинг на `ModManifest`.
- [ ] Фаза 2 — определить политику версий `ModManifest.Version` (SemVer).
- [ ] Фаза 2 — реализовать `RestrictedModApi` в `DualFrontier.Application`.
