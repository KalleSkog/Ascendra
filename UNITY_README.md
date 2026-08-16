# Ascendra — World Exploration Milestone

This scene is the first playable milestone: a procedurally generated terrain you can walk, run,
jump and look around in with a third-person orbit camera.

## Open it
1. Open the workspace folder in Unity (6000.5.8f1).
2. Let the editor resolve the newly added packages (Input System, Universal Render Pipeline) —
   this needs internet access the first time.
3. Open the scene at `Assets/Scenes/WorldScene.unity`.
4. Press Play.

## One manual step required (Universal Render Pipeline)
Unity can't safely auto-author render pipeline assets from a text edit, so do this once:
1. In the Project window: right-click `Assets` → `Create > Rendering > URP Asset (with Universal Renderer)`.
   Put it in a new `Assets/Settings` folder.
2. `Edit > Project Settings > Graphics` → set the new asset as the **Scriptable Render Pipeline Settings**.
3. `Edit > Project Settings > Quality` → assign the same asset for each quality level you use.

Until this is done the project keeps rendering with the Built-in pipeline (it still runs, just
without URP's lighting/terrain benefits).

## What's in the scene
A single `Bootstrap` GameObject running `Ascendra.Core.GameBootstrap`, which builds everything at
runtime (same pattern as the earlier bouncing-ball test):
- a directional light ("sun"),
- a procedurally generated `Terrain` (Perlin-noise heightmap with a flattened spawn clearing),
- a capsule `Player` with a `CharacterController` (WASD move, Space jump, Left Shift sprint),
- a `Main Camera` with a mouse-driven orbit rig (locks the cursor — press `Esc` in the editor to
  get it back if needed).

## Code structure
```
Assets/Scripts/
  Ascendra.Runtime.asmdef   # single assembly for now, references Unity.InputSystem
  Core/       Ascendra.Core       — scene bootstrap / composition root
  World/      Ascendra.World     — terrain & world generation
  Player/     Ascendra.Player    — character movement
  CameraRig/  Ascendra.CameraRig — third-person camera
```
All input is built directly in code via the Input System's `InputAction` API — no
`.inputactions` asset to keep in sync yet. We'll introduce an `InputActionAsset` with proper
action maps once we add more verbs (attack, interact, inventory, build).
