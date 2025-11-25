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
| `perf` | Performance improvements |
| `chore` | Maintenance, configuration updates |
| `test` | Adding or modifying tests |
| `meta` | Project management materials: meeting summaries, planning notes, brainstorming docs |
| `add` | New files of every kind (must be relevant) |
| `remove` | Deleting files of every kind (must be relevant) |

### Examples

```git
feat(input): add sprint action
fix(camera): correct follow target reference
docs(gitignore): clarify intention of local packages folder
chore: ignore .vscode directory
```

## Branching Convention

### Main Branches

- `develop`: default unstable branch for ongoing development

Branches should be named using the following pattern:

```git
<type>/<short-description>
```

Optionally include issue IDs:

```git
feat/42-input-sprint-action
```

The target branch is usually `develop`.
For urgent fixes, use `hotfix` branches targeting `main`.

| Type | Purpose |
|------|----------|
| `feat` | New feature |
| `fix` | Bug fix |
| `hotfix` | Urgent fixes. Target branch: `main` |
| `docs` | Documentation or comments |
| `style` | Formatting or style-only changes |
| `refactor` | Code refactoring |
| `chore` | Maintenance or configuration updates |
| `test` | Adding or modifying tests |

Examples:

```git
feat/input-sprint-action
fix/camera-follow-target
docs/readme-update
```

## PR's Title Convention

| Type | Purpose |
|------|----------|
| `feat` | New feature |
| `fix` | Bug fix |
| `hotfix` | Urgent fixes. Target branch: `main` |
| `docs` | Documentation or comments |
| `style` | Formatting or style-only changes |
| `refactor` | Code refactoring |
| `chore` | Maintenance or configuration updates |
| `test` | Adding or modifying tests |

Examples:

```git
feat: add input sprint action
fix: correct camera follow target
docs: update readme
```

## Code Style

- KISS!
- Meaningful names
- Consistent formatting

### Variable Naming

#### Names

- Use **nouns** for variables and properties (e.g., `playerScore`) because they represent data or state.
- Use **verbs** for functions and methods (e.g., `CalculateScore()`) because they perform actions or calculations.
- Prefix boolean variables with a **verb** that expresses condition or ability (e.g., `isGameOver`, `hasKey`, `canJump`) to clearly signify true/false values.
- C# events start with **On** + **subject** + **Action** (e.g., `OnPlayerDeath`) to indicate they notify something happening.
- Methods that are triggered when "something happens," but **are not directly tied to an event subscription**, also start with **On** + **subject** + **Action** (e.g., `OnPlayerDeath()`) to represent internal logic invoked at those moments.
- Variables holding ScriptableObject (SO) events end with **Event** (e.g., `playerDeathEvent`) to clearly identify them as event objects.
- Methods that **raise (trigger)** SO events start with **Raise** (e.g., `RaisePlayerDeathEvent()`) to show their role in firing the event.
- Methods that **handle an event** end with **Handler** (e.g., `PlayerDeathEventHandler()`) indicating they respond as subscribers to the event.
- Scripts that inherit from ScriptableObjects begin with **SO** (e.g., `SOEnemyData`) to help recognize their type quickly.
- Interfaces start with a capital **I** (e.g., `IInteractable`) following .NET conventions to clearly distinguish them.

#### Casing Schemes

- use `PascalCase` for public variables, properties, enums, functions, Scriptable Objects (e.g. `PlayerScore`, `CalculateScore()`)
- use `camelCase` for [SerializeField] (e.g. `playerScore`)
- use `_camelCase` for private and protected variables (e.g. `_playerScore`)
- use `UPPER_SNAKE_CASE` for constants (e.g. `MAX_HEALTH`)

### Ordering

In general, follow this order inside your scripts:

  1. Constants
  2. Serialized fields
  3. Private fields
  4. Unity lifecycle methods (`Awake`, `OnEnable`, `Start`, `Update`, `OnDisable`, `OnDestroy`)
  5. Private methods
  6. Public methods
  7. Event handlers
  
If you have a lot of code in one script, consider using `#region` blocks to organize related portions of code together.

### Patters

#### State Machine Pattern

When implementing the State Machine pattern, follow these guidelines:

- Create an abstract base state class that defines the common methods every state must implement, such as `Enter`, `Update`, and `Exit`.  
  - This base state class should hold a reference to the finite state machine (FSM) controller and any shared data needed by all states.  
  - Methods in the state class ending with `Handler` are event handlers called by the main controller in response to events (for example, `_fsm.CurrentState.PauseGameHandler()` is called by `GameManager` in its homonymous method when the player is hit, as it is subscribed to that event).  
  - Methods in the state class that return a `bool` are used as checks by the main controller to decide whether an action can be performed or an event triggered (for example, `_fsm.CurrentState.OnPlayerHit()` is called by `EnemyAI` to verify if the player was hit).  
  - Each state class should implement all the methods defined in the base class, even if some implementations are empty (to maintain consistency).  
- Each concrete state class inherits from the base state and implements behavior specific to that state.  
- The FSM class is responsible for managing state transitions and holds references to all possible states.

## Pull Requests

Before opening a pull request:

1. Make sure your branch is up to date with the base branch.
2. Test the project in Unity to verify your changes.
3. Use a clear PR title following the conventions above.
4. Add a short description of what the PR does.
5. Link related issues if any (e.g. `Closes #42`).
6. Request reviews from relevant team members.
7. Keep taking care of any feedback until the PR is approved and merged
