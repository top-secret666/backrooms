# Day 0 checklist (open project in Unity Hub)

## Open

1. Unity Hub → Projects → Add → Add project from disk → `D:\HUH\backrooms`
2. Open with **Unity 6.6 (6000.6.4f1)**
3. If URP wizard appears: create a URP Asset and assign it (Edit → Project Settings → Graphics)
4. External Script Editor: Rider (Edit → Preferences → External Tools)

## Build a greybox room (15–20 min)

1. Hierarchy → 3D Object → Plane (floor). Scale ~(2, 1, 2), colour yellowish
2. Four Cubes as walls
3. Cube as a short column
4. Light → Point Light near ceiling; add component `FlickeringLight`
5. Capsule as placeholder player (camera child looking forward)
6. Press Play: light should flicker

## Learn (say out loud)

- GameObject = container; Component = behaviour (`Transform`, `Light`, `FlickeringLight`)
- `Awake` vs `Start` vs `Update`
- `[SerializeField]` shows private fields in Inspector
- Prefab: drag a lamp GameObject into `Assets/_Project/Prefabs`

## Done when

- [ ] Project opens without errors
- [ ] Room exists, Play mode shows flickering light
- [ ] You can explain `FlickeringLight` line by line
