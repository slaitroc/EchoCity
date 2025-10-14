# Contributing Guidelines

## Commit Message Convention

We use the **Conventional Commits** style:

```git
<type>(<scope>): <short summary>
```

### Common Types

| Type | Purpose |
|------|----------|
| `feat` | New feature |
| `fix` | Bug fix |
| `docs` | Documentation or comments |
| `style` | Formatting or style-only changes |
| `refactor` | Code refactoring |
| `chore` | Maintenance or configuration updates |
| `test` | Adding or modifying tests |

### Examples

```git
feat(input): add sprint action
fix(camera): correct follow target reference
docs(gitignore): clarify intention of local packages folder
chore: ignore .vscode directory
```

## Code Style

- KISS!
- Meaningful names
- Consistent formatting

### Variable Naming

#### Names

- use **nouns** for variables and properties (e.g. `playerScore`)
- use **verbs** for functions and methods (e.g. `CalculateScore()`)
- prefix boolean with a **verb** (e.g. `isGameOver`, `hasKey`, `canJump`)
- events start with **On** + **subject** + **Action** (e.g. `OnPlayerDeath`)
- interfaces start with a capital **I** (e.g. `IInteractable`)
- ScriptableObjects end with **SO** (e.g. `GameSettingsSO`)

#### Casing Schemes

- use `PascalCase` for public variables, properties and functions (e.g. `PlayerScore`, `CalculateScore()`)
- use `camelCase` for [SerializeField] (e.g. `playerScore`)
- use `_camelCase` for private variables (e.g. `_playerScore`)
- use `UPPER_SNAKE_CASE` for constants (e.g. `MAX_HEALTH`)

#### Ordering

In general, follow this order inside your scripts:

  1. Fields and properties
  2. Unity lifecycle methods (`Awake`, `OnEnable`, `Start`, `Update`, `OnDisable`, `OnDestroy`)
  3. Public methods
  4. Private methods
  5. Event handlers

If you have a lot of code in one script, consider using `#region` blocks to organize related portions of code together.

## Pull Requests

Before opening a pull request:

1. Make sure your branch is up to date with `main` or `develop`.
2. Test the project in Unity to verify your changes.
3. Use a clear PR title following the same commit convention.
4. Add a short description of what the PR does.
5. Link related issues if any (e.g. `Closes #42`).
6. Request reviews from relevant team members.