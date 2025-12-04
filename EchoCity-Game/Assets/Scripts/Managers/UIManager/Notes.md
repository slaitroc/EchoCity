# UI notes

- The UIInput can only enable the PlayerInput action map, not disable it. The Enable methods are public and can be called by the UIManager or other scripts that have access to the UIManager.

  This is a more general design choice: "**every input manager can enable other input managers, but only itself can disable itself**"

  In other words, it is possible to:

  - enable/disable its own action map;
  - enable other action maps via events;

When a player input action needs to trigger the UI and disable itself, it will:

1. Disable its own action map;
2. Raise an event to open the UI (e.g., open pause menu);
3. The UIManager (or the relevant UI controller) will enable the UI action map via the UIInput.

When closing the UI, an UI controller will:

1. Disable the UI action map via UIManager -> UIInput;
2. Raise an event to enable the player input action map via the UIManager -> UIInput;
