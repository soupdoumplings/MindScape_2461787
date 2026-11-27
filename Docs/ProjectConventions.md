# MindScape project conventions

## Folder placement

- Shared runtime systems go in `Assets/Scripts/Core`.
- Game-specific behaviour goes in `Assets/Scripts/Gameplay`.
- UI behaviour goes in `Assets/Scripts/UI`.
- AI behaviour goes in `Assets/Scripts/AI`.
- Reusable pathfinding experiments go in `Assets/Scripts/Pathfinding`.
- Gameplay scenes go in `Assets/Scenes/Gameplay`.
- Isolated learning and test scenes go in `Assets/Scenes/Sample`.
- Input and render-pipeline configuration belongs in `Assets/Settings`.
- Project prefabs belong in the relevant `Assets/Prefabs` subfolder.
- `Assets/Starter Assets` and its contents are Unity-provided third-party assets and retain their original structure.

## Asset names

| Asset type | Convention | Example |
| --- | --- | --- |
| C# scripts | PascalCase, named for their responsibility | `AStarPathfinder.cs` |
| Scenes | `Sample` plus a PascalCase purpose | `SamplePathfinding.unity`, `SampleGameplay.unity` |
| Prefabs | `PF_` plus a PascalCase purpose | `PF_Player.prefab` |
| Materials | `MAT_` plus a PascalCase purpose | `MAT_Obstacle.mat` |
| Textures | `TEX_` plus a PascalCase purpose | `TEX_Grid.png` |
| Sound effects | `SFX_` plus a PascalCase purpose | `SFX_Click.wav` |
| Music | `MUS_` plus a PascalCase purpose | `MUS_MainTheme.wav` |

## Current examples

- `Bootstrap.unity` is the first scene in Build Settings. It owns the one persistent `GameManager`.
- `SamplePathfinding.unity` is the isolated A* and hide-and-seek learning scene.
- `SampleGameplay.unity` is the gameplay scene.
- `Grid3D.cs`, `GridNode.cs`, and `AStarPathfinder.cs` follow the script naming convention.

## Runtime scene management

- `GameManager` is the sole persistent scene-management service. It is created in `Bootstrap.unity`; duplicate instances log a warning and are destroyed.
- Request transitions with `GameManager.Instance.LoadScene("SceneName")`. The call returns `false` and logs an error if the name is empty, missing from Build Settings, or another transition is in progress.
- A successful request uses a single-mode async load. Subscribe to `SceneLoadStarted`, `SceneLoadCompleted`, or `SceneLoadFailed` when another system needs to respond to a transition.
- `startupScene` currently defaults to `SampleGameplay`. Any future startup or destination scene must be enabled in Build Settings and use its exact scene name.

## Player input

- `InputSystem_Actions` is the one place that lists the game's controls. Do not add new keyboard checks directly in gameplay scripts.
- The sample scenes use the working Starter Assets player prefab. Keep it in place until the project input actions are wired to a tested player prefab.
- Move with WASD or the arrow keys. Look with the mouse or the right gamepad stick. Jump with Space or the gamepad south button. Sprint with Left Shift or the left-stick button.

## Basic menu

- The menu appears in the top-left corner and stays visible while scenes change.
- It is made for a 1920 by 1080 screen and scales up or down to fit other screens.
- Use **Play Gameplay** to open the gameplay scene and **Pathfinding Test** to open the grid test scene.

## Settings

- Open **Settings** in the game menu to change the options.
- **Volume** changes the game sound level. **Sensitivity** changes how quickly the first-person camera turns.
- **FPS Cap** switches between 30, 60, and 120 frames per second.
- Choices are saved on this computer and are used again the next time the game starts.

## Grid data

- `Grid3D` stores one `GridNode` for every X, Y, Z cell. Each node records its grid position, world position, and whether it can be walked on.
- Use `GetNode(position)` to look up a cell and `GetNeighbours(position)` to inspect the cells sharing one of its six sides.
- The current sample scenes are flat grids (`height = 1`), but the data structure supports multiple height levels.
- Blocked cells are listed on `Grid3D` in the Inspector. Invalid or repeated entries are ignored, and `IsWalkable(position)` is the single check used by movement and pathfinding.
