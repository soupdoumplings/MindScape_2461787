# Mindscape Git workflow

This repository uses branches to keep unfinished experiments away from the stable game.

## Branches

| Branch | Purpose | Rule |
| --- | --- | --- |
| `main` | Stable version of the game | Only merge code that has been tested in Unity. |
| `develop` | Integration branch | Completed feature branches are merged here first. |
| `feature/min-XX-name` | One planned feature or task | Build and test the feature here. |
| `fix/name` | A bug fix | Keep the fix small and focused. |
| `docs/name` | Documentation only | Do not include code or scene changes. |
| `archive/legacy-main` | Old project history | Never delete or develop from this branch. |

## Before starting work

Run these commands from the project folder:

```powershell
git switch develop
git pull origin develop
git status
```

`git status` should show no unexpected changes before switching branches. Do not use `git reset --hard` to clear changes; ask first or use a new branch/stash.

## Start a feature

Replace the name with the MIN task you are working on:

```powershell
git switch develop
git pull origin develop
git switch -c feature/min-15-player-movement
git push --set-upstream origin feature/min-15-player-movement
```

## Save a completed checkbox

One acceptance-criteria checkbox should normally equal one clear commit.

```powershell
git status
git add Assets/Scripts/Pathfinding/Grid3D.cs
git commit -m "feat(grid): explain the completed change"
git push
```

Always run `git status` before `git add`. Stage exact files instead of using `git add .`; Unity can create scene or editor changes you did not intend to commit.

## Finish and merge a feature

1. Test the feature in Unity.
2. Push the feature branch.
3. Create a GitHub pull request from the feature branch into `develop`.
4. Merge it into `develop` only when it works.
5. Test `develop` in Unity.
6. Create a pull request from `develop` into `main` when the work is stable enough to release.

Command-line version after review:

```powershell
git switch develop
git pull origin develop
git merge --no-ff feature/min-15-player-movement
git push origin develop
```

Do not merge a feature into `main` directly unless it is a small, urgent hotfix.

## Fix a bug

```powershell
git switch develop
git pull origin develop
git switch -c fix/settings-menu-sliders
```

Test the fix, make one focused commit, push it, then merge it into `develop` first.

## Keep work safe when switching tasks

If work is unfinished, save it temporarily:

```powershell
git stash push -m "wip/short-description"
```

Return it later:

```powershell
git stash list
git stash pop
```

## Recover from a mistake

Do not delete or reset files in panic. First inspect the state:

```powershell
git status
git log --oneline --decorate -10
git reflog -10
```

The `archive/legacy-main` branch preserves the project history from before the branch workflow was introduced.

## What not to commit

- `Library`, `Temp`, `Logs`, and other Unity-generated folders.
- Play Mode scene changes unless they were intentionally saved and reviewed.
- Unrelated `.vscode` setting changes.
- A broken experiment just because a commit is needed. Fix or revert it first.

## Daily quick version

```powershell
git switch develop
git pull origin develop
git switch feature/your-current-branch
git status
```

When finished for the day:

```powershell
git status
git add path/to/the/files-you-mean-to-save
git commit -m "type(area): short description"
git push
```
