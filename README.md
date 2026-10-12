# Backrooms

First-person horror set in **Level 0** of the Backrooms: yellow corridors, buzzing fluorescents, damp carpet — inspired by Kane Pixels.

Goal (full game): find the exit in 10–15 minutes without being caught.

## Status (plan milestone)

**Level 0 greybox — done** as a playable foundation:

- Unity 6 + URP scene [`Assets/_Project/Level0_Greybox.unity`](Assets/_Project/Level0_Greybox.unity)
- FPS move/look (`PlayerController`) + soft office flicker (`FlickeringLight`)
- Office materials: wallpaper, carpet, ceiling tiles
- Editor tool: **Backrooms → Build Kane-Style T-Corridor** (T-junction, light grid)

Next (not done yet): procedural maze, entity AI/FSM, flashlight/sanity, VContainer bootstrap, WebGL build.

See [docs/GDD.md](docs/GDD.md) and [docs/day-kane-corridor.md](docs/day-kane-corridor.md).

## Tech

- Unity 6 LTS, URP, New Input System
- Game logic (generation, pathfinding, state machines) planned in pure C# + EditMode tests
- DI via VContainer (upcoming), events instead of singletons

## Open in Unity

1. Unity Hub → Add → `D:\HUH\backrooms`
2. Editor **6000.6.x**
3. Open `Assets/_Project/Level0_Greybox.unity`
4. Optional: menu **Backrooms → Build Kane-Style T-Corridor**, hide old greybox cubes, Play
