# Game Design Document: Backrooms

## Concept

First-person horror in Level 0: endless yellow rooms, buzzing lights, damp carpet.
Something in the dark hears noise and hunts by sound.

## Player goal

Find the exit in 10–15 minutes without being caught.

## Core loop

1. Explore a procedurally generated chunk of Level 0.
2. Manage flashlight battery and sanity.
3. Stay quiet or create noise deliberately to bait / distract the entity.
4. Reach the exit door.

## Mechanics

- FPS movement: walk, sprint, crouch
- Flashlight with limited battery
- Sanity drops in darkness and near the entity
- Entity AI: Wander → Investigate → Chase → Search
- Noise events (sprint, interact) attract the entity
- One interactable exit

## Scope (in)

- One polished level (Level 0 aesthetic)
- Procedural maze (Kruskal + connectivity check)
- One entity with FSM + A*
- WebGL build on itch.io
- Clean architecture: pure C# core + thin MonoBehaviour views

## Out of scope

- Multiplayer
- Multiple Backrooms levels / lore encyclopedia
- Inventory crafting
- Complex narrative / cutscenes
- Mobile controls
