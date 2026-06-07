# CyberPink

Мобильная 2D мультиплеерная экшн-игра на Unity. Игроки подключаются через Photon PUN, управляют персонажем с оружием и способностями, сражаются на арене с респавном.

**Движок:** Unity 2020.3.30f1  
**Платформа:** Android (mobile)  
**Сетевой слой:** Photon Unity Networking v1 (PUN1)  
**Билд:** `test.apk` (в корне проекта)

---

## Структура проекта

```
Assets/
├── Scripts/               # Основные игровые скрипты
│   ├── Player.cs          # Логика игрока: здоровье, смерть, PUN RPC
│   ├── PlayerMove.cs      # Движение, прыжки, анимация, flip, джойстики
│   ├── PlayerStats.cs     # Singleton: скорость, макс. здоровье, реген
│   ├── Weapon.cs          # Стрельба, щит, мушка, звук, эффекты
│   ├── ArmRotation.cs     # Вращение руки по джойстику (дубликат логики)
│   ├── GameMaster.cs      # Респавн, жизни, деньги, скины, UI
│   ├── AudioManager.cs    # Singleton для звуков (DontDestroyOnLoad)
│   ├── Camera2DFollow.cs  # Следование камеры за игроком (lookahead)
│   ├── CameraShake.cs     # Тряска камеры при выстреле
│   ├── StatusIndicator.cs # UI полоска здоровья и текст HP
│   ├── MenuManager.cs     # Кнопки главного меню, звуки
│   ├── Parallaxind.cs     # Параллакс фона
│   └── trash/             # Старые неиспользуемые скрипты (см. ниже)
├── DestroyBullet.cs       # Логика пули: урон, эффект попадания
├── DestroyFeed.cs         # Авто-уничтожение UI-фида через N секунд
├── Health.cs              # Вторая система здоровья (FillImage)
├── PlayerControls.cs      # Сетевая интерполяция позиции игрока
├── MenuController.cs      # Подключение к Photon, создание/вход в комнату
├── AstarPathfindingProject/  # A* Pathfinding (сторонний пакет)
├── Joystick Pack/            # Мобильные джойстики (сторонний пакет)
├── Photon Unity Networking/  # PUN v1 (сторонний пакет)
└── Standard Assets/          # Unity Standard Assets
```

### Папка `trash/` — нерабочие скрипты

| Файл | Статус |
|---|---|
| `Enemy.cs` | Полностью закомментирован |
| `EnemyAI.cs` | Не интегрирован |
| `WaveSpawner.cs` | Ссылается на несуществующий `onToggleUpgradeMenu` |
| `UpgradeMenu.cs` | Не подключён |
| `GameOverUI.cs` | Не используется |
| `LivesCounterUI.cs` | Не используется |
| `MoneyCounterUI.cs` | Не используется |
| `WaveUI.cs` | Не используется |
| `MoveTrail.cs` | Не используется |
| `Tiling.cs` | Не используется |

---

## Что работает сейчас

- Подключение к Photon и создание/вход в комнату
- Спавн игрока по сети (`PhotonNetwork.Instantiate`)
- Движение и прыжки через мобильный джойстик
- Стрельба: снаряд (`BulletTrail`) с уроном по RPC
- Щит: активируется вторым джойстиком
- Вращение руки по направлению стрельбы
- Респавн с таймером и кнопкой "Старт"
- Параллакс фона, тряска камеры, следование камеры
- Система звуков через `AudioManager`
- Отображение пинга
- UI полоска здоровья

---

## Найденные проблемы

### Критические

**1. Две конкурирующие системы здоровья**
`Player.cs` использует внутренний класс `PlayerStats` и RPC `DamagePlayer`/`ReduceHealth`.  
`Health.cs` использует `FillImage.fillAmount` и свой RPC `ReduceHealthBar`.  
`DestroyBullet.cs` вызывает `ReduceHealth` (из `Player.cs`), но `Health.cs` имеет свой `ReduceHealthBar`. Системы не синхронизированы, при попадании пули может обновляться только одна из них.

**2. `Weapon.cs` — RPC Shield вызывается каждый кадр**
```csharp
// В Update(), каждый кадр:
photonView.RPC("Shield", PhotonTargets.AllBuffered, true/false);
```
Это флудит сеть сотнями RPC в секунду. Нужно вызывать только при изменении состояния.

**3. `Health.cs` — ModifyHealth дублирует код в if/else**
Ветки `if (photonView.isMine)` и `else` выполняют идентичный код — разницы нет, но создаётся иллюзия разной логики.

**4. `PlayerStats.cs` конфликт имён**
`PlayerStats` — синглтон-монобehaviour в `PlayerStats.cs`. Внутри `Player.cs` есть вложенный класс с тем же именем `PlayerStats`. Это создаёт путаницу и потенциальные баги компиляции.

**5. `ArmRotation.cs` — `Debug.Log` в `Update()`**
```csharp
void Update() {
    Debug.Log(AbilityJoystick.Horizontal); // каждый кадр!
    ...
}
```
Сильно бьёт по производительности на мобильных устройствах.

**6. `ArmRotation.cs` — хардкод `if (true)`**
```csharp
void Awake() {
    if (true) { // бессмысленное условие
        FireJoystick = ...
```

### Архитектурные

**7. Логика вращения руки продублирована**
`ArmRotation.cs` и `PlayerMove.cs` оба реализуют вращение руки по джойстику. Это разные версии одной механики, оба скрипта, судя по всему, висят на одном объекте.

**8. Большие блоки закомментированного кода**
`Player.cs`, `GameMaster.cs`, `Weapon.cs`, `Camera2DFollow.cs`, `PlayerMove.cs` содержат закомментированные старые версии кода. Занимают 50–70% файлов. Нужно удалить — история хранится в git.

**9. `Parallaxind.cs` — `Camera.main` в `Update()`**
`Camera.main` вызывает `FindObjectOfType` каждый кадр. Нужно кешировать в `Start()`.

**10. Нет системы врагов**
`WaveSpawner` и `Enemy` / `EnemyAI` находятся в `trash/` и не работают. Игра — только PvP без PvE контента.

**11. `PlayerStats.cs` — реген здоровья объявлен, но не реализован**
```csharp
public float healthRegenRate = 2f; // поле есть, но логики регена нет
```

**12. Отсутствует синхронизация вращения руки по сети**
В `PlayerMove.OnPhotonSerializeView` весь код закомментирован:
```csharp
// stream.SendNext(armRotation.transform.rotation);
```
Другие игроки не видят, куда направлено оружие.

**13. `test.apk` в репозитории**
Бинарный файл билда лежит в корне проекта и трекается git. Нужен `.gitignore`.

**14. `MenuController.cs` — отладочные `Debug.Log` в продакшн коде**
```csharp
void OnConnectedToMaster() {
    Debug.Log("Connected????????????");
    Debug.Log("Connected!!!!!!!!!!!!!!!!!!!!!");
```

**15. Photon PUN v1 — устаревшая версия**
PUN v1 не поддерживается. Актуальная версия — PUN2 (`Photon.Pun` namespace). Миграция требует переработки всех `Photon.MonoBehaviour`, `photonView.isMine`, `PhotonTargets`, `PhotonNetwork.playerName` и т.д.

---

## Предложения по улучшению

### Геймплей и механики

1. **Объединить системы здоровья** — оставить одну (`Health.cs`), убрать дублирование в `Player.cs`. Вся логика урона и смерти должна идти через единый интерфейс.
2. **Активировать систему врагов** — доработать `Enemy.cs`, `EnemyAI.cs`, `WaveSpawner.cs`, подключить к `GameMaster`. Это даст PvE режим или кооп.
3. **Реализовать реген здоровья** — поле `healthRegenRate` уже есть в `PlayerStats`, нужно добавить корутину.
4. **Добавить систему скинов** — структура (`skinsObject`, `skinsImage`) уже есть в `GameMaster`, нужен UI для выбора.
5. **Синхронизировать вращение руки** — раскомментировать и реализовать `OnPhotonSerializeView` в `PlayerMove`.
6. **Улучшить систему щита** — вынести в отдельный компонент с кулдауном, не вызывать RPC каждый кадр.

### Код и архитектура

7. **Удалить весь закомментированный код** — git хранит историю.
8. **Убрать `trash/` из Assets** — перенести в ветку или удалить.
9. **Исправить Shield RPC** — хранить текущее состояние, вызывать RPC только при изменении.
10. **Закешировать `Camera.main`** в `Parallaxind.cs` и `Camera2DFollow.cs`.
11. **Удалить `Debug.Log` из `Update()`** в `ArmRotation.cs`.
12. **Добавить `.gitignore`** для Unity — исключить `test.apk`, `Library/`, `Temp/`, `obj/`, `Logs/`, `*.sln`, `*.csproj`.
13. **Мигрировать на PUN2** — при следующем рефакторинге.

### Продакшн

14. **Добавить систему матчмейкинга** — сейчас только ручной ввод кода комнаты. Нужен `PhotonNetwork.JoinRandomRoom`.
15. **Добавить таблицу лидеров / счёт** — `PunPlayerScores` уже есть в пакете Photon UtilityScripts.
16. **Обработка отключения** — `disconnectUI` есть в `GameMaster`, но логика не реализована.
17. **Настроить лимиты комнат** — сейчас `maxPlayers = 5`, нужно проверить баланс.

---

## План запуска в продакшн

### Фаза 1 — Стабилизация (1–2 недели)
- [ ] Исправить Shield RPC (флуд сети)
- [ ] Объединить системы здоровья в одну
- [ ] Убрать `Debug.Log` из Update()
- [ ] Добавить `.gitignore`, удалить `test.apk` и бинарники из git
- [ ] Удалить закомментированный код
- [ ] Убрать `trash/` из сборки (хотя бы перенести вне `Assets/`)

### Фаза 2 — Доработка геймплея (2–3 недели)
- [ ] Синхронизировать вращение руки по сети
- [ ] Реализовать реген здоровья
- [ ] Подключить систему врагов (WaveSpawner + Enemy)
- [ ] Добавить матчмейкинг (случайный поиск комнаты)
- [ ] Реализовать счёт / убийства / смерти
- [ ] Подключить `disconnectUI` при разрыве соединения

### Фаза 3 — Полировка (1–2 недели)
- [ ] Добавить экран выбора скина перед игрой
- [ ] Добавить таблицу результатов в конце раунда
- [ ] Настроить звуки и эффекты (проверить все `audioManager.PlaySound`)
- [ ] Профилировка на реальном устройстве (особенно Parallax + Camera.main)
- [ ] QA: тестирование на 2–5 игроках одновременно

### Фаза 4 — Релиз
- [ ] Настроить Photon AppID для продакшн (отдельно от dev)
- [ ] Собрать релизный APK с keystore
- [ ] Разместить на Google Play (Internal Testing → Closed Testing → Production)
- [ ] Настроить аналитику (Firebase / GameAnalytics)
- [ ] Мониторинг Photon Dashboard (CCU, комнаты, регионы)

---

## Зависимости

| Пакет | Версия | Назначение |
|---|---|---|
| Photon Unity Networking | PUN v1 | Мультиплеер |
| A* Pathfinding Project | Free | Навигация (для врагов) |
| Joystick Pack | — | Мобильные джойстики |
| Unity Standard Assets | 2020 | Утилиты (2D, камеры) |

---

## Быстрый старт (для разработчиков)

1. Открыть проект в Unity 2020.3.30f1
2. Прописать Photon AppID в `Assets/Photon Unity Networking/Resources/PhotonServerSettings`
3. Открыть сцену `MainMenu`, запустить Play
4. Ввести имя, создать или войти в комнату — откроется `MainGame`
