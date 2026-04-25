# Гигиена разработки

Проект сознательно развивается двумя параллельными треками: **игра** Dual Frontier (Phase 0–7, основная ветка) и **движок** — обобщённое ECS-ядро, которое после релиза игры форкается в отдельный продукт (см. [ROADMAP §«Пост-релиз»](./ROADMAP.md#пост-релиз--развилка-на-движок)). Чтобы форк прошёл дёшево, граница движок/игра должна оставаться чистой на протяжении Phase 4–7, когда соблазн «срезать угол» будет максимальным.

Этот документ — не архитектурная теория (она в [ARCHITECTURE](./ARCHITECTURE.md) и [CODING_STANDARDS](./CODING_STANDARDS.md)), а **прикладной чек-лист на каждый PR**. Если все пункты зелёные — граница не деградирует. Если что-то красное — это останавливает merge, а не «поправим потом».

## Главный инвариант

Движковые сборки никогда не ссылаются на игровые. Всё остальное в этом документе — способы проверить, что инвариант не нарушен случайно.

| Движковые (generic, переиспользуемые) | Игровые (специфичны для Dual Frontier) |
|---------------------------------------|----------------------------------------|
| `DualFrontier.Contracts` | `DualFrontier.Components` |
| `DualFrontier.Core` | `DualFrontier.Events` |
| `DualFrontier.Core.Interop` | `DualFrontier.Systems` |
| `native/DualFrontier.Core.Native/` | `DualFrontier.AI` |
| `DualFrontier.Presentation.Native` | `DualFrontier.Presentation` (Godot DevKit) |
| Модинг-секция `DualFrontier.Application` | Игровой цикл `DualFrontier.Application` |

Полный список и обоснование — в [ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра).

## Чек-лист перед каждым PR

Пять проверок. Выполняются последовательно, одна красная — PR не уходит.

### 1. Движок не импортирует игру

```bash
grep -rn "using DualFrontier\.\(Components\|Systems\|Events\|AI\)" \
    src/DualFrontier.Contracts/ \
    src/DualFrontier.Core/ \
    src/DualFrontier.Core.Interop/ \
    src/DualFrontier.Presentation.Native/ \
    src/DualFrontier.Application/Modding/
```

Ожидаемый вывод: пусто. Одна строка — PR не мержится.

### 2. `dotnet build` зелёный

```bash
dotnet build DualFrontier.sln -c Release
```

`TreatWarningsAsErrors=true` в [Directory.Build.props](../Directory.Build.props). Любое предупреждение = блокер. Nullable warnings, CS-предупреждения — всё считается.

### 3. Все тесты проходят

```bash
dotnet test DualFrontier.sln -c Release
```

Числа растут, не падают. Если на main было 61/61, на ветке ожидается ≥61/61. Регрессия — блокер.

### 4. Коммиты имеют scope-префиксы

```bash
git fetch origin main
git log origin/main..HEAD --no-merges --format='%s' \
    | grep -v '^\(contracts\|core\|interop\|native\|modding\|presentation-native\|experiment\|feat\|fix\|docs\|test\|chore\|build\|refactor\)[(:]'
```

Ожидаемый вывод: пусто. Любая строка — коммит без scope, его нужно переписать через `git rebase -i`. Префиксы описаны в [CODING_STANDARDS §«Сообщения коммитов»](./CODING_STANDARDS.md#сообщения-коммитов).

`--no-merges` нужен, чтобы merge-коммиты (`Merge pull request #…`) не считались нарушением. `origin/main..HEAD` после `git fetch` гарантирует, что сравнение идёт с актуальным main, а не со стейлом локального бранча.

### 5. Коммиты не смешивают движок и игру

Просмотр руками: каждый коммит в ветке должен трогать либо только движковые сборки (префикс `core:`, `contracts:`, `interop:`, `native:`, `modding:`, `presentation-native:`), либо только игровые (`feat(pawn):`, `feat(combat):`, `feat(presentation):`, …). Смешанный коммит — разбить на два:

```bash
git log origin/main..HEAD --no-merges --stat | less
# визуально проверить, что каждый commit трогает одну сторону границы
```

Это стоит минуты. После форка эта минута возвращается как возможность `git log --grep "^\(core\|contracts\|interop\|native\|modding\): "` вытащить всю движковую историю одной командой.

## Типичные ситуации

### Добавляю новую игровую систему (Phase 4–7)

Пример: `HaulSystem` в Phase 4.

- Файлы создаются **только** в игровых сборках: `DualFrontier.Systems/`, при необходимости — новые компоненты в `DualFrontier.Components/`, события в `DualFrontier.Events/`.
- Система наследуется от `SystemBase` (движковый API). Никаких изменений в `SystemBase` ради этой системы быть не должно. Если SystemBase чего-то не хватает — это сигнал из §«Красные флаги» ниже.
- Декларация доступа через `[SystemAccess(reads: …, writes: …, buses: …)]` (движковый атрибут, в `Contracts.Attributes`).
- Коммит: `feat(inventory): add HaulSystem`.

### Расширяю ECS API

Пример: понадобился новый метод на `SystemExecutionContext`.

- Метод должен быть **generic** — никаких упоминаний `HealthComponent`, `PawnComponent` и т.д.
- Добавлять в `DualFrontier.Core/ECS/SystemExecutionContext.cs`.
- Если нужен новый атрибут — он идёт в `DualFrontier.Contracts/Attributes/`.
- Коммит: `core: add <что-то>` или `contracts: add <что-то>`.

### Добавляю Intent/Granted/Refused в существующую шину

- Записи (`record struct`) идут в `DualFrontier.Events/` (игровая сборка). Это нормально — события уже игровые.
- Шина в `DualFrontier.Contracts/Bus/` **не** расширяется, если только это не generic инфраструктура (например, `IDeferredBus<T>`). Конкретные Intent-ы не попадают в Contracts.
- Коммит: `feat(combat): add ShieldRefillIntent to combat bus`.

### Нужно добавить новый компонент

- Файл идёт в `DualFrontier.Components/<Area>/<Name>Component.cs`.
- POCO, никакой логики.
- Если компонент потенциально generic (условно — `PositionComponent`, `FactionComponent`) — всё равно остаётся в `Components`. Движку компоненты не принадлежат; движку принадлежит `IComponent` маркер.

### Соблазн: «просто один раз импортну `Components` в `Core`»

Это **всегда** блокер. Примеры того, как правильно решить задачу, которая толкает к такому импорту:

- **«`Core` должен знать формат `HealthComponent`, чтобы X».** Нет, не должен. Если нужна операция над произвольным компонентом — она выражается через `IComponent` + reflection/generic. Если нужна операция только над конкретным — она живёт в системе, не в `Core`.
- **«Мне нужен `DamageEvent` в `Core` для логирования».** Логирование идёт через движковый `IEventBus.Subscribe<T>` — `T` параметризируется в Systems, а не в Core.
- **«Производительность: generic — медленно, хочу конкретный тип».** Измерить. В 95% случаев JIT снимает overhead. Если оставшиеся 5% реально критичны — это повод для обсуждения, а не для нарушения границы.

## Красные флаги

Ранние симптомы, что архитектура начинает деградировать. Каждый сам по себе — не катастрофа, но сигнал остановиться и обсудить.

| Симптом | Что значит | Что делать |
|---------|------------|------------|
| `SystemBase` обрастает методами под конкретные сценарии | Игровые паттерны переезжают в движок | Вытащить паттерн в helper в `Systems/Shared/`, оставить `SystemBase` generic |
| В `Core` появляется `switch` по `Type` конкретного компонента | Движок начал знать доменные типы | Перевести логику в систему, или обобщить через `IComponent` + атрибут-маркер |
| `Contracts` растёт быстрее, чем `Systems` | Контракты публикуются под задачи, которых ещё нет | Откатить, добавить контракт только когда он реально нужен второму потребителю |
| Моды-примеры не обновляются месяцами | Модинг-инфра эволюционирует в вакууме | Либо добавить хотя бы один реальный мод как second customer, либо заморозить развитие модинга |
| `native/` отстаёт от `Core` | C++ эксперимент мёртв, но всё ещё в репо | Либо догнать, либо закрыть эксперимент и вынести в отдельную ветку-артефакт |
| Коммиты «core: fix X, also feat(pawn): Y» в одном | Граница стирается в голове автора | Требовать разделения на ревью, даже если это стоит `git rebase -i` |
| `DualFrontier.Presentation` содержит бизнес-логику | Игра лезет в Godot-слой с геймплейными решениями | Бизнес-логика идёт в `Systems`, `Presentation` только переводит команды в Godot-вызовы |

## Quick reference — scope-префиксы коммитов

Полный список и примеры — в [CODING_STANDARDS §«Сообщения коммитов»](./CODING_STANDARDS.md#сообщения-коммитов).

**Движковые** (уйдут в форк после релиза):
- `contracts:`, `core:`, `interop:`, `native:`, `modding:`, `presentation-native:`
- `experiment:` — исследовательские ветки до мёржа

**Игровые** (останутся в игре после форка):
- `feat(pawn):`, `feat(combat):`, `feat(magic):`, `feat(world):`, `feat(inventory):`, `feat(ai):`
- `fix(…):` — баг-фиксы в тех же областях
- `feat(application):` — игровой цикл, `feat(presentation):` — Godot DevKit
- `feat(bootstrap):` — проводка при появлении новой системы

**Нейтральные**: `docs:`, `test:`, `chore:`, `build:`, `refactor:`.

## Применение в чистую ветку

## См. также

- [ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра)
- [CODING_STANDARDS §«Сообщения коммитов»](./CODING_STANDARDS.md#сообщения-коммитов)
- [ROADMAP §«Пост-релиз — развилка на движок»](./ROADMAP.md#пост-релиз--развилка-на-движок)
- [ISOLATION](./ISOLATION.md)
- [MODDING](./MODDING.md)
