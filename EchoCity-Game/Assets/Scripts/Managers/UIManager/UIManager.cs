using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private HUD hud;

    void Awake() { }
    public void PauseMenuHandler()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    public void HUDInteractableHandler()
    {
        hud.IsInteractable(!hud.isInteractable);
        Log.D("HUD Interactable Handler", "green", "UI MANAGER");
    }
}
