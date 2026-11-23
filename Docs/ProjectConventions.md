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

- `SamplePathfinding.unity` is the isolated A* and hide-and-seek learning scene.
- `SampleGameplay.unity` is the gameplay scene.
- `Grid3D.cs`, `GridNode.cs`, and `AStarPathfinder.cs` follow the script naming convention.
