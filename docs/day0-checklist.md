# Чеклист дня 0 (открыть проект в Unity Hub)

## Открыть проект

1. Unity Hub → Projects → Add → Add project from disk → `D:\HUH\backrooms`
2. Открыть **Unity 6.6 (6000.6.4f1)**
3. Если появится мастер URP: создай URP Asset и назначь его (Edit → Project Settings → Graphics)
4. Внешний редактор скриптов: Rider (Edit → Preferences → External Tools)

## Собрать greybox-комнату (15–20 мин)

1. Hierarchy → 3D Object → Plane (пол). Scale примерно `(2, 1, 2)`, цвет желтоватый
2. Четыре Cube — стены
3. Cube — короткая колонна
4. Light → Point Light у потолка; добавь компонент `FlickeringLight`
5. Настройки света (важно, иначе мерцания не видно):
   - Отключи или удали `Directional Light`
   - Point Light Position Y ≈ `3`–`4` (не 16!)
   - Light → Range ≈ `25`–`40`
   - Color — жёлтый
6. Capsule — заглушка игрока (камера — дочерний объект, смотрит вперёд)
7. Нажми Play: свет должен заметно мигать и иногда гаснуть

## Выучи вслух

- GameObject — контейнер; Component — поведение (`Transform`, `Light`, `FlickeringLight`)
- `Awake` vs `Start` vs `Update`
- `[SerializeField]` показывает приватные поля в Inspector
- Prefab: перетащи лампу из Hierarchy в `Assets/_Project/Prefabs`

## Готово, когда

- [ ] Проект открывается без ошибок
- [ ] Комната есть, в Play мерцает свет
- [ ] Можешь объяснить `FlickeringLight` построчно
