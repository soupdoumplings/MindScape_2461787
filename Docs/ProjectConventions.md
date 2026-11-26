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
- `PF_Player` is the player prefab used by the sample scenes. It uses `InputSystem_Actions`.
- Move with WASD or the arrow keys. Look with the mouse or the right gamepad stick. Jump with Space or the gamepad south button. Sprint with Left Shift or the left-stick button.
