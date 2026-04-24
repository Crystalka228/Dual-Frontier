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

Если ты открыл свежую ветку от main и хочешь, чтобы Claude-сессия выстроила всю гигиену за один раз — скопируй промт ниже в новую сессию (она должна стартовать в корне репозитория).

Промт самодостаточный: в свежей сессии нет контекста нашей работы, поэтому в нём дублируются все нужные детали — состояние репозитория, список файлов, точная нарезка правок.

```
Задача: привести документацию этой ветки в соответствие с гигиеной разработки
DualFrontier / Colony_Simulator. Ветка только что создана от main и в ней
ещё никаких правок сделано. Цель — перенести в main четыре документационных
блока, которые фиксируют границу движок/игра, scope-префиксы коммитов, план
пост-релизного форка движка и практический чек-лист для каждого PR.

Контекст проекта:
* Dual Frontier — колониальный симулятор на Godot 4 + C#, строится как
  многофазный проект (Phase 0–7 в docs/ROADMAP.md). Сейчас завершены
  Phase 0–3.5, идёт Phase 4.
* Архитектура уже разделена на движковые и игровые сборки — проверь
  grep-ом перед началом работы:
    grep -rn "using DualFrontier\.\(Components\|Systems\|Events\|AI\)" \
        src/DualFrontier.Contracts/ \
        src/DualFrontier.Core/ \
        src/DualFrontier.Core.Interop/ \
        src/DualFrontier.Presentation.Native/ \
        src/DualFrontier.Application/Modding/
  Ожидаемый вывод: пусто. Если не пусто — остановись и сообщи, архитектура
  уже поехала.
* Если src/DualFrontier.Core.Interop/ и native/ отсутствуют — это нормально,
  они живут на экспериментальной ветке и в этот перенос не входят.

Сделай ровно четыре правки, ничего больше.

ПРАВКА 1 — docs/ARCHITECTURE.md
Найди секцию "## Правила зависимостей" и сразу после её последнего пункта
(строка с "Моды зависят **только** от Contracts") вставь новую секцию
"## Граница движок / игра". Её содержимое должно:
- Перечислить движковые сборки: DualFrontier.Contracts, DualFrontier.Core,
  DualFrontier.Core.Interop + native/DualFrontier.Core.Native/,
  DualFrontier.Presentation.Native, модинг-часть DualFrontier.Application
  (ModIntegrationPipeline, ContractValidator, ModRegistry, ModLoader,
  RestrictedModApi, PresentationBridge).
- Перечислить игровые сборки: DualFrontier.Components, DualFrontier.Events,
  DualFrontier.Systems, DualFrontier.AI, игровая часть
  DualFrontier.Application (GameLoop, FrameClock, ScenarioLoader),
  DualFrontier.Presentation (Godot DevKit).
- Зафиксировать правило: движковые сборки никогда не получают
  ProjectReference на игровые; ни одного using DualFrontier.Components,
  DualFrontier.Systems, DualFrontier.Events, DualFrontier.AI в движковых
  сборках. Обратное направление (игра → движок) — норма. Нарушение на
  PR — блокер ревью.
- Упомянуть, что правило — задел под пост-релизный форк движка в отдельный
  продукт (ссылка вперёд на ROADMAP §«Пост-релиз — развилка на движок»).

ПРАВКА 2 — docs/CODING_STANDARDS.md
Перед финальной секцией "## См. также" вставь новую секцию "## Сообщения
коммитов". Её содержимое:
- Формат: "<scope>: <imperative summary>" в первой строке; опционально
  развёрнутое тело через пустую строку; 70 символов максимум на summary.
- Движковые scope-префиксы: contracts:, core:, interop:, native:, modding:,
  presentation-native:, experiment:.
- Игровые scope-префиксы: feat(pawn|combat|magic|world|inventory|ai):,
  fix(те же области):, feat(application):, feat(presentation):,
  feat(bootstrap):.
- Нейтральные: docs:, test:, chore:, build:, refactor:.
- Правила:
  * Если коммит затрагивает и движок, и игру — обязательно разбить на два.
  * Scope выбирается по самой высокой затронутой сборке.
  * Summary на английском (as in: add, fix, update) — чтобы проходил через
    стандартные git-инструменты. Доменные термины в теле — на русском, как
    и комментарии в коде.
- Дай 5–6 примеров реальных коммит-сообщений в кодовом блоке.
- Обоснуй цель правила: по scope одной командой
  (git log --grep="^\(contracts\|core\|interop\|native\|modding\): ")
  вытаскивается движковая история для форка.

ПРАВКА 3 — docs/ROADMAP.md
Перед финальной секцией "## См. также" вставь новую секцию
"## Пост-релиз — развилка на движок". Её содержимое:
- Мотивация: после Phase 7 и релиза игры движковая часть форкается в
  отдельный продукт. Развилка — не "возможно, когда-нибудь", а
  запланированное следствие того, как проект строился с Phase 0.
- Что уходит в движковый форк: DualFrontier.Contracts, DualFrontier.Core,
  DualFrontier.Core.Interop + native/DualFrontier.Core.Native/,
  DualFrontier.Presentation.Native, модинг-секция DualFrontier.Application,
  тесты на эти сборки.
- Что остаётся в игровой ветке: DualFrontier.Components,
  DualFrontier.Events, DualFrontier.Systems, DualFrontier.AI, игровая
  часть Application, DualFrontier.Presentation (Godot DevKit), контент
  Phase 4–7, ассеты, моды.
- Пробелы, которые закрываются только после форка (не во время игры):
  asset pipeline, build & packaging для Win/Linux/macOS, hot-reload модов,
  replay/determinism tools, cross-platform CI для native,
  документация "как сделать свою игру на этом движке".
- Критерий готовности к форку:
  * Phase 7 закрыта, игра в релизе, пройдена стадия первых багфиксов
    (1–3 месяца).
  * Движковая часть выдержала все Phase 4–7 без нарушений границы.
  * Моддинг-инфра обкатана хотя бы на 2–3 реальных модах (не только
    пример).

ПРАВКА 4 — создать docs/DEVELOPMENT_HYGIENE.md
Новый документ-чек-лист на каждый PR. Структура:
1. Краткое введение: два трека (игра, движок), ссылки на ROADMAP §«Пост-релиз»,
   ARCHITECTURE, CODING_STANDARDS. Этот документ — не теория, а прикладной
   чек-лист.
2. Секция "## Главный инвариант" — движковые сборки не ссылаются на игровые.
   Таблица (движковые | игровые). Ссылка на ARCHITECTURE §«Граница движок /
   игра».
3. Секция "## Чек-лист перед каждым PR" — пять проверок с bash-командами:
   (1) grep на отсутствие using DualFrontier.{Components,Systems,Events,AI}
       в движковых сборках;
   (2) dotnet build зелёный;
   (3) dotnet test без регрессий по числу тестов;
   (4) все коммиты имеют scope-префикс (регулярка grep через
       git log origin/main..HEAD --no-merges; обязательно --no-merges
       и origin/main после git fetch, иначе merge-коммиты и устаревший
       локальный main дают ложные срабатывания);
   (5) ни один коммит не смешивает движок и игру.
4. Секция "## Типичные ситуации" — как правильно добавить:
   новую игровую систему, расширение ECS API, новый Intent/Granted/Refused
   в шину, новый компонент. Плюс "соблазн: импортну Components в Core" и
   что с этим делать.
5. Секция "## Красные флаги" — таблица симптомов деградации архитектуры
   (SystemBase обрастает specific методами, switch по Type в Core, Contracts
   растёт без потребителей, моды-примеры не обновляются, native отстаёт,
   смешанные коммиты, бизнес-логика в Presentation) и что делать.
6. Секция "## Quick reference — scope-префиксы коммитов" — списки движковых
   и игровых префиксов со ссылкой на CODING_STANDARDS §«Сообщения коммитов».
7. "## См. также" — ссылки на ARCHITECTURE §«Граница движок / игра»,
   CODING_STANDARDS §«Сообщения коммитов», ROADMAP §«Пост-релиз», ISOLATION,
   MODDING.

Добавь ссылку на новый документ в docs/README.md в раздел "## Разработка",
одной строкой:
  - [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md) — чек-лист гигиены на
    каждый PR, граница движок/игра, красные флаги.

Коммиты:
Работай в своей ветке (не в main). Разбей работу на два коммита с
движковыми scope-префиксами — это будет первое применение конвенции,
которую вносишь:
  1. "docs: fix engine/game boundary for post-release fork"
     (три правки в ARCHITECTURE, CODING_STANDARDS, ROADMAP)
  2. "docs: add development hygiene checklist"
     (новый DEVELOPMENT_HYGIENE.md + ссылка в README.md)

Проверка:
* dotnet build DualFrontier.sln -c Release — зелёный (ничего не трогал в коде).
* grep движка на отсутствие игровых using — пусто.
* git log --oneline -5 — два новых коммита с правильными префиксами.

Ограничения:
* Трогать только файлы в docs/.
* Никаких изменений в src/, tests/, native/, DualFrontier.sln.
* Никаких новых InternalsVisibleTo, csproj-правок, ничего кроме докуменатции.
* Если какая-то из секций уже частично существует в целевом документе —
  остановись и спроси, не переписывай вслепую.
```

## См. также

- [ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра)
- [CODING_STANDARDS §«Сообщения коммитов»](./CODING_STANDARDS.md#сообщения-коммитов)
- [ROADMAP §«Пост-релиз — развилка на движок»](./ROADMAP.md#пост-релиз--развилка-на-движок)
- [ISOLATION](./ISOLATION.md)
- [MODDING](./MODDING.md)
