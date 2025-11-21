using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    void Awake()
    {
        
    }

    public void PauseMenuHandler()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
    
}
