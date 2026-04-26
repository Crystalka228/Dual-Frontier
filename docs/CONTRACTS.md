# Контрактная система

Сборка `DualFrontier.Contracts` — единственный модуль, который видят все слои: ядро, системы, моды, внешние инструменты. Контракт — это публичный интерфейс, декларирующий намерение (что можно сделать), без подсказок о реализации. Контракт не меняется ради удобства: либо новое событие, либо новая версия интерфейса.

## Маркер-интерфейсы

Четыре базовых маркер-интерфейса не имеют методов и служат лишь для типизации. Они позволяют планировщику и шине событий работать обобщённо, не завися от конкретных игровых типов.

### IEvent

Маркер для любого сообщения, летящего по шине. События — `record`-ы с `init`-свойствами, неизменяемые. Примеры: `ShootAttemptEvent`, `AmmoIntent`, `ManaGranted`, `DeathEvent`.

### IQuery и IQueryResult

Синхронный запрос в шину с типизированным ответом. Используется редко — только для чтения агрегированных данных (например, `GetPawnsInRadiusQuery`). Большая часть взаимодействия проходит через двухшаговую модель Intent→Granted.

### ICommand

Императивное намерение изменить состояние. В отличие от события `ICommand` адресный — у него есть ожидаемый обработчик. В Dual Frontier команды в основном живут в слое Application для `IRenderCommand` (Domain → Presentation). Доменная логика предпочитает события.

### IComponent

Маркер для чистых данных, прикреплённых к `EntityId`. Никакой логики в компонентах. Пример объявления:

```csharp
public sealed class HealthComponent : IComponent
{
    public float Current;
    public float Maximum;
    public bool IsDead => Current <= 0;
}
```

## Шесть доменных шин

Одна шина на всё — узкое горло под нагрузкой. Lock contention при 100+ системах. Решение: отдельная шина на каждый домен. Меньше contention, проще дебажить, легче профилировать.

```csharp
public interface IGameServices
{
    ICombatBus    Combat    { get; }
    IInventoryBus Inventory { get; }
    IMagicBus     Magic     { get; }
    IWorldBus     World     { get; }
    IPawnBus      Pawns     { get; }
    IPowerBus     Power     { get; } // Введена в v0.3 §13.1
}
```

| Шина          | Кто пишет                        | Кто читает                           | Ключевые события                                  |
|---------------|----------------------------------|--------------------------------------|---------------------------------------------------|
| CombatBus     | CombatSystem, ProjectileSystem   | DamageSystem, StatusEffectSystem     | ShootAttempt, DamageEvent, DeathEvent             |
| InventoryBus  | HaulSystem, CraftSystem          | InventorySystem, JobSystem           | AmmoIntent/Granted, ItemAdded/Removed/Reserved    |
| MagicBus      | SpellSystem, GolemSystem         | ManaSystem, EtherGrowthSystem        | ManaIntent/Granted, SpellCast, EtherSurge         |
| PawnBus       | NeedsSystem, MoodSystem          | JobSystem, SocialSystem              | MoodBreak, DeathReaction, SkillGain               |
| WorldBus      | BiomeSystem, WeatherSystem       | EtherGridSystem, RaidSystem          | EtherNodeChanged, WeatherChanged, RaidIncoming    |
| PowerBus      | ElectricGridSystem, ConverterSystem | ElectricGridSystem, потребители, UI | PowerRequest, PowerGranted, GridOverload, ConverterPowerOutput |

Каждая шина — собственный `ConcurrentDictionary` подписчиков. `CombatSystem` пишет только в `Combat`, `InventorySystem` — только в `Inventory`. Нет общей точки блокировки. Система декларирует в `[SystemAccess]` имя используемой шины, и сторож проверяет, что публикация идёт только туда.

## IModContract — API между модами

Моды не должны ссылаться друг на друга напрямую: это создаёт цикл загрузки и жёсткие зависимости. Вместо этого мод может опубликовать интерфейс-контракт, реализующий `IModContract`. Другой мод запрашивает контракт по типу:

```csharp
public interface IVoidMagicContract : IModContract
{
    bool CanCastVoid(EntityId caster);
    void EmitVoidSurge(EntityId source);
}

public class ArtifactMod : IMod
{
    public void Initialize(IModApi api)
    {
        if (api.TryGetContract<IVoidMagicContract>(out var voidMagic))
        {
            api.Subscribe<VoidSpellCastEvent>(OnVoidMagicDetected);
        }
        // Мод VoidMagic не загружен — просто не подписываемся.
        // Нет краша, нет жёсткой зависимости.
    }
}
```

Контракт — это тот же публичный интерфейс, но размещённый в сборке, которую оба мода знают. Обычно это отдельная сборка `ModName.Contracts`, опубликованная как nuget или лежащая рядом в каталоге `mods/`.

## Эволюция контрактов

Контракты — долгожители. Любое изменение делится на ломающее и не ломающее.

### Не ломающее (разрешено всегда)

- Добавление нового `IEvent`, `IQuery`, `ICommand`, `IComponent`.
- Добавление нового поля с `init` в `record` (старые `record`-ы не упомянут — не сломаются).
- Добавление метода в интерфейс с `default` реализацией (C# 8+).
- Добавление новой доменной шины (новое свойство в `IGameServices`).

### Ломающее (только с мажорной версией)

- Удаление или переименование поля в `IEvent`.
- Удаление метода из интерфейса.
- Изменение типа параметра в существующем методе.
- Изменение семантики фазы (было `[Deferred]`, стало синхронным).

Ломающие изменения требуют поднять мажорную версию сборки и обновить документ миграции.

## Версионирование

`DualFrontier.Contracts` использует семантическое версионирование `MAJOR.MINOR.PATCH`.

- `MAJOR` — ломающие изменения. Старые моды требуют пересборки.
- `MINOR` — добавление контрактов. Старые моды работают.
- `PATCH` — документация, XML-комментарии, внутренние правки.

Манифест мода (`mod.manifest.json`) декларирует минимально требуемую версию контрактов. Загрузчик отказывается грузить мод, построенный против более новой мажорной версии, чем у ядра.

```json
{
  "id": "com.example.voidmagic",
  "name": "Void Magic",
  "version": "1.2.0",
  "requiresContracts": "^1.0.0"
}
```

## См. также

- [EVENT_BUS](./EVENT_BUS.md)
- [MODDING](./MODDING.md)
- [ECS](./ECS.md)
- [ARCHITECTURE](./ARCHITECTURE.md) — состав решения, граница движок / игра.
- [RESOURCE_MODELS](./RESOURCE_MODELS.md) — Intent vs Lease как контрактные модели.
- [COMPOSITE_REQUESTS](./COMPOSITE_REQUESTS.md) — multi-bus запросы поверх контрактов.
- [COMBO_RESOLUTION](./COMBO_RESOLUTION.md) — детерминированная резолюция `DamageIntent`.
- [OWNERSHIP_TRANSITION](./OWNERSHIP_TRANSITION.md) — состояния `GolemBondComponent` и события перехода.
