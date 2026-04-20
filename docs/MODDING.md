# Написание модов

В RimWorld мод через Harmony патчит любой приватный метод и в какой-то момент ломает его. Dual Frontier грузит каждый мод в отдельный `AssemblyLoadContext`: мод физически не видит `DualFrontier.Core`, у него нет ссылки ни на `World`, ни на конкретную систему. Моды взаимодействуют с ядром и друг с другом через контракты. Это дольше писать, но у модов появляется совместимость между версиями и между собой.

## IMod

Каждый мод реализует `IMod` — единственную точку входа.

```csharp
public interface IMod
{
    /// <summary>
    /// Вызывается при загрузке мода. Регистрация компонентов, систем,
    /// подписки на события. Мод получает IModApi — единственный
    /// способ взаимодействия с ядром.
    /// </summary>
    void Initialize(IModApi api);

    /// <summary>
    /// Вызывается перед выгрузкой мода. Мод обязан отписаться от
    /// всех событий и освободить ресурсы. После возврата метода
    /// AssemblyLoadContext будет выгружен.
    /// </summary>
    void Unload();
}
```

Загрузчик `ModLoader` в `DualFrontier.Application/Modding` создаёт `ModLoadContext` (наследник `AssemblyLoadContext`) под каждый мод, загружает сборку, находит класс с `IMod`, вызывает `Initialize`.

## IModApi — что можно

Мод получает `IModApi` и может только то, что перечислено в контракте:

```csharp
public interface IModApi
{
    void RegisterComponent<T>() where T : IComponent;
    void RegisterSystem<T>()    where T : SystemBase;

    void Publish<T>(T evt) where T : IEvent;
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;

    // Публикует контракт для других модов
    void PublishContract<T>(T contract) where T : IModContract;

    // Получает контракт другого мода (опционально)
    bool TryGetContract<T>(out T contract) where T : IModContract;

    // Логирование в общий лог с префиксом mod-id
    void Log(string message);
    void LogWarning(string message);
    void LogError(string message);
}
```

Всё, что не в `IModApi` — недоступно. `IModApi` реализуется внутри ядра (`RestrictedModApi`) и проксирует вызовы через сторож: регистрация системы проверяет наличие `[SystemAccess]`, регистрация компонента сохраняется в `ComponentRegistry` с привязкой к mod-id.

| Действие                         | Разрешено | Причина                                   |
|----------------------------------|-----------|-------------------------------------------|
| Публиковать события в шину       | Да        | Через IModApi — проксируется              |
| Подписываться на события         | Да        | Через IModApi — проксируется              |
| Регистрировать компоненты        | Да        | Через IModApi                             |
| Регистрировать системы           | Да        | Через IModApi + декларация READ/WRITE     |
| Публиковать контракт для модов   | Да        | IModContract — публичный интерфейс        |
| Получить ссылку на World         | Нет       | AssemblyLoadContext блокирует             |
| Получить ссылку на систему       | Нет       | Сторож изоляции — краш                    |
| Загрузить DualFrontier.Core      | Нет       | AssemblyLoadContext блокирует             |
| Обойти EventBus напрямую         | Нет       | Физически нет ссылки                      |

## AssemblyLoadContext — что физически недоступно

`ModLoadContext` при попытке загрузить сборку проверяет имя:

- `DualFrontier.Contracts` — пропускается из основного контекста (shared).
- `DualFrontier.Core`, `DualFrontier.Systems`, `DualFrontier.Components`, `DualFrontier.Events`, `DualFrontier.Application` — **отказывает**. Моду возвращается `ModIsolationException`.
- `System.*`, `Microsoft.*`, сторонние библиотеки из `BasePath` мода — пропускаются.

Это означает: даже если мод попытается через рефлексию найти тип `World` — он не найдёт сборку. Даже скомпилировать мод с `using DualFrontier.Core;` не получится, потому что при загрузке пакета `ModLoadContext` не отдаст эту сборку.

## IModContract — API между модами

Моды не ссылаются друг на друга напрямую: это создаёт циклы загрузки и жёсткие зависимости. Вместо этого мод публикует интерфейс, реализующий `IModContract`, а другой мод запрашивает его по типу.

```csharp
// В общей сборке контрактов VoidMagic.Contracts:
public interface IVoidMagicContract : IModContract
{
    bool CanCastVoid(EntityId caster);
    void EmitVoidSurge(EntityId source);
}

// Мод A (VoidMagic) публикует реализацию:
public class VoidMagicMod : IMod
{
    public void Initialize(IModApi api)
    {
        api.PublishContract<IVoidMagicContract>(new VoidMagicImpl());
    }
    public void Unload() { }
}

// Мод B (ArtifactMod) подхватывает, если доступно:
public class ArtifactMod : IMod
{
    public void Initialize(IModApi api)
    {
        if (api.TryGetContract<IVoidMagicContract>(out var voidMagic))
        {
            api.Subscribe<VoidSpellCastEvent>(e => voidMagic.EmitVoidSurge(e.CasterId));
        }
        // Мод A не загружен — просто не подписываемся.
        // Нет краша, нет жёсткой зависимости.
    }
    public void Unload() { }
}
```

Контракт — это обычный публичный интерфейс, размещённый в отдельной сборке, которую оба мода знают. Обычно это `ModName.Contracts.dll`, распространяемая автором оригинального мода.

## mod.manifest.json

Каждый мод содержит файл манифеста в корне пакета.

```json
{
  "id": "com.example.voidmagic",
  "name": "Void Magic",
  "description": "Adds the Void school of magic.",
  "author": "Example Modder",
  "version": "1.2.0",
  "requiresContracts": "^1.0.0",
  "dependencies": [],
  "optionalDependencies": [
    { "id": "com.example.artifactmod", "version": "^0.5.0" }
  ],
  "entryAssembly": "VoidMagic.dll",
  "entryType": "VoidMagic.VoidMagicMod"
}
```

- `id` — уникальный идентификатор в стиле reverse-domain.
- `version` — SemVer мода.
- `requiresContracts` — минимальная версия `DualFrontier.Contracts`. Загрузчик откажет, если ядро старше по major.
- `dependencies` — обязательные моды; их отсутствие блокирует загрузку.
- `optionalDependencies` — опциональные; сообщают, что возможна интеграция через контракт, но мод работает и без них.
- `entryAssembly`/`entryType` — имя сборки и FQN класса с `IMod`.

## Горячая загрузка

`ModLoader` поддерживает выгрузку через `ModLoadContext.Unload()`. Последовательность:

1. Планировщик приостанавливается между фазами.
2. У каждой системы, зарегистрированной модом, вызывается `OnDestroy`.
3. У мода вызывается `Unload()`.
4. Реализация `IModApi` снимает все оставшиеся подписки (страховка).
5. `ModLoadContext.Unload()` освобождает сборку (после следующего GC).

Ограничение: `AssemblyLoadContext.Unload` — коллаборативный. Если мод оставил ссылку на свой тип в статическом поле другого мода, выгрузка не завершится. Поэтому `OnDestroy` критически важен.

Горячая перезагрузка (обновление мода без выхода из игры) = Unload + Load с новой версией. Сохранение не требуется — World не меняется, только состав систем и подписок.

## Пошаговое руководство

Создать первый мод:

1. **Создать проект.** `dotnet new classlib -n MyFirstMod -f net8.0`.
2. **Добавить ссылку на контракты.** `<Reference Include="DualFrontier.Contracts.dll" Private="false" />`. Флаг `Private=false` важен: мод не должен таскать контракты рядом.
3. **Написать IMod.**

    ```csharp
    using DualFrontier.Contracts.Modding;

    namespace MyFirstMod;

    public sealed class MyFirstMod : IMod
    {
        public void Initialize(IModApi api)
        {
            api.Log("MyFirstMod initialized");
            api.Subscribe<DeathEvent>(OnDeath);
        }

        public void Unload()
        {
            // Подписка снимется автоматически.
        }

        private void OnDeath(DeathEvent e) { /* ... */ }
    }
    ```

4. **Написать манифест.** `mod.manifest.json` — как в примере выше.
5. **Собрать и положить в `mods/`.**
    ```
    mods/com.example.myfirstmod/
        MyFirstMod.dll
        mod.manifest.json
    ```
6. **Запустить игру.** В логе появится `[com.example.myfirstmod] MyFirstMod initialized`.

## См. также

- [CONTRACTS](./CONTRACTS.md)
- [ISOLATION](./ISOLATION.md)
- [ARCHITECTURE](./ARCHITECTURE.md)
