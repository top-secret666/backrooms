# День 5.10 — движение игрока

День 0 закрыт. Сегодня: WASD + мышь.

## Настройка Player в Unity

1. Stop (выйти из Play).
2. Выдели `Player`.
3. `Add Component` → `Character Controller`  
   - Height ≈ `2`, Radius ≈ `0.4`, Center ≈ `(0, 1, 0)` если pivot у ног;  
     если Player — Capsule с центром в середине: Center `(0, 0, 0)`, Height `2`.
4. `Add Component` → `Player Controller`.
5. В поле **Camera Transform** перетащи дочернюю `Main Camera`.
6. Play → кликни в Game view → WASD ходить, мышь смотреть, Esc — отпустить курсор.

Если Player — пустой объект без Capsule: Hierarchy → ПКМ на Player → `3D Object` → `Capsule` как ребёнок (или Add Component Capsule Collider не нужен — CharacterController сам коллизия).

## Что понять в коде

Открой `PlayerController.cs` и объясни вслух:

- `Keyboard.current` / `Mouse.current` — New Input System
- `CharacterController.Move` — движение без физики Rigidbody
- `Time.deltaTime` — скорость не зависит от FPS
- yaw на теле игрока, pitch только на камере — почему так

## Параллельно: госы (1 час)

В `csharp-fundamentals` билеты:

- ОАиП 5–6: переменные, область видимости, время жизни; ввод/вывод
- ООП 2–3: класс и объект; члены, конструкторы, static

Завтра разберём C# vs Java глубже (struct, events, LINQ) и усилим контроллер (бег, присед).
