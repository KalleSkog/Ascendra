# Unity Project Shell

This repository contains the game-authored portion of the Unity project. Create or open the Unity project in this folder using Unity Hub; Unity will create and manage its own `Packages` and `ProjectSettings` folders.

## Folder map

```text
Assets/
  _Project/                 # All game-specific content lives here.
    Art/
      Materials/            # Material assets and material variants.
      Models/               # 3D models, rigs, and imported model source files.
      Textures/             # Image textures used by materials and UI.
    Audio/
      Music/                # Background music and music mixers.
      SFX/                  # Sound effects, such as footsteps and UI clicks.
    Prefabs/
      Characters/           # Reusable player, NPC, and enemy GameObjects.
      Environment/          # Reusable props, terrain pieces, and level objects.
    Scenes/                 # Unity scene files, beginning with MainMenu and Game.
    Scripts/
      Core/                 # App-wide setup: game state, services, and bootstrapping.
      Editor/               # Editor-only tools that create or validate game assets.
      Player/               # Input, movement, abilities, and player-specific logic.
      World/                # Level generation, interactables, AI, and world systems.
    Settings/               # Game-owned ScriptableObject settings and configuration.
    UI/                     # UI documents, sprites, prefabs, and UI scripts.
```

## Why use `_Project`?

`Assets` can become crowded because Unity packages and imported assets may add their own folders. Keeping all assets created for this game under `Assets/_Project` makes ownership obvious and makes moving or reviewing game content easier.

## Starting in Unity Hub

1. Open Unity Hub and select **New project**.
2. Choose a current **Universal 3D** template for a 3D game, or **Universal 2D** for a 2D game.
3. Set the location to `C:\Ascendra`. Unity Hub may require an empty folder; if it does, create the project in a temporary empty folder and then copy this `Assets` folder into it.
4. Open the project. Unity detects `Assets` and creates `.meta` files beside folders and assets. Commit those `.meta` files to Git because they preserve Unity asset references.
5. Wait for Unity to finish importing assets. It will recognize `Scripts/Editor` as editor-only code and add the `Tools/Ascendra/Create Flat Green World` command, shown in Unity as **Tools > Ascendra > Create Flat Green World**, to the top menu.
6. Select **Tools > Ascendra > Create Flat Green World**. The tool creates and saves `Assets/_Project/Scenes/FlatWorld.unity`, then creates `Assets/_Project/Art/Materials/FlatGround.mat`.
7. Open **File > Build Profiles** and add `FlatWorld` to the **Scene List** so Unity includes it in a game build.

## First world

The Flat Green World command builds a deliberately small first scene:

- A `Ground` plane, scaled to 100 by 100 Unity units and assigned a green Lit material.
- A directional `Sunlight` object so the material is visibly lit.
- A `Stick Person` made from Unity primitive shapes, with one invisible `CharacterController` for collision.
- A `Main Camera` that follows behind the player during Play mode.

It is safe to run the command again while learning; it replaces the ground material and re-saves the `FlatWorld` scene. Open the scene from `Scenes` and press the Play button to enter it.

## Player controls

In `FlatWorld`, use `W`, `A`, `S`, and `D` or the arrow keys to move the stick person. The player turns toward the direction of travel, and the camera follows behind at a fixed height and distance.

The behavior is split between two runtime scripts:

- `Scripts/Player/ThirdPersonStickController.cs` reads movement input, turns the player, and moves its `CharacterController`.
- `Scripts/CameraRig/ThirdPersonCameraFollow.cs` updates after movement and places the camera behind the player.

The editor-only creator in `Scripts/Editor` connects these components when it builds the scene. This separation matters: code in `Editor` helps make assets inside Unity, while code in `Player` and `CameraRig` runs in the actual game.

## Placement rule

When adding something, put it with the feature it serves: player movement code goes in `Scripts/Player`, a reusable player object goes in `Prefabs/Characters`, and its meshes and materials go in `Art`. Create a new subfolder only when a category starts to hold several related assets.
