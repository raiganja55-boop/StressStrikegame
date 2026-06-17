using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _optionsPanel;

    private void Start()
    {
        // Ensure we start with the Main Menu visible and Options hidden
        ShowMainMenu();
    }

    /// <summary>
    /// Called when the PLAY button is clicked.
    /// Loads the next scene (e.g., the combat scene).
    /// </summary>
    public void OnPlayClicked()
    {
        Debug.Log("Play button clicked! Loading game scene...");
        // Ensure you add your game scene to the Build Settings
        // For example, if your game scene is index 1, you can use:
        // SceneManager.LoadScene(1);
        
        // Alternatively, load by scene name:
        // SceneManager.LoadScene("CombatScene");
    }

    /// <summary>
    /// Called when the OPTIONS button is clicked.
    /// Hides the main menu and shows the options panel.
    /// </summary>
    public void OnOptionsClicked()
    {
        Debug.Log("Options button clicked! Opening options panel...");
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(false);
        if (_optionsPanel != null) _optionsPanel.SetActive(true);
    }

    /// <summary>
    /// Called when the Back/Close button inside the OPTIONS panel is clicked.
    /// Hides the options panel and returns to the main menu.
    /// </summary>
    public void ShowMainMenu()
    {
        if (_optionsPanel != null) _optionsPanel.SetActive(false);
        if (_mainMenuPanel != null) _mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Called when the QUIT button is clicked.
    /// Quits the application (or stops playing in the Editor).
    /// </summary>
    public void OnQuitClicked()
    {
        Debug.Log("Quit button clicked! Exiting game...");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
