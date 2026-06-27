using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Canvases (Drag these in manually!)")]
    public GameObject mainMenuCanvas;
    public GameObject arenaMenuCanvas;
    public GameObject levelSelectorCanvas;
    public GameObject shopMenuCanvas;
    public GameObject settingsCanvas;

<<<<<<< Updated upstream
    [Header("Panels")]
    public CanvasGroup optionsPanel;
    public CanvasGroup storePanel;
    public CanvasGroup loginPanel;
=======
    private enum MenuState { Main, Arena, Level, Shop }
    private MenuState currentState = MenuState.Main;
>>>>>>> Stashed changes

    private void Start()
    {
        // Fallback: If you didn't drag them in, try to find them by name
        // (But this only finds ACTIVE objects. For hidden ones, you MUST drag them in!)
        if (mainMenuCanvas == null) mainMenuCanvas = GameObject.Find("MainMenu");
        if (arenaMenuCanvas == null) arenaMenuCanvas = GameObject.Find("ArenaMenu");
        if (levelSelectorCanvas == null) levelSelectorCanvas = GameObject.Find("Level");
        if (shopMenuCanvas == null) shopMenuCanvas = GameObject.Find("ShopMenu");
        if (settingsCanvas == null) settingsCanvas = GameObject.Find("Settings");

<<<<<<< Updated upstream
        // Initialize panels state
        HideAllPanelsImmediate();
=======
        // Hook up buttons ONLY inside our known canvases!
        if (mainMenuCanvas != null) HookUpButtons(mainMenuCanvas.transform);
        if (arenaMenuCanvas != null) HookUpButtons(arenaMenuCanvas.transform);
        if (levelSelectorCanvas != null) HookUpButtons(levelSelectorCanvas.transform);
        if (shopMenuCanvas != null) HookUpButtons(shopMenuCanvas.transform);
        if (settingsCanvas != null) HookUpButtons(settingsCanvas.transform);

        // Set initial state
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(false);
        if (levelSelectorCanvas != null) levelSelectorCanvas.SetActive(false);
        if (shopMenuCanvas != null) shopMenuCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        currentState = MenuState.Main;
>>>>>>> Stashed changes
    }

    private void HookUpButtons(Transform canvasRoot)
    {
<<<<<<< Updated upstream
        isPlayExpanded = !isPlayExpanded;
        float duration = 0.3f;

        if (isPlayExpanded)
        {
            // Tween sub-buttons in
            arenaButton.gameObject.SetActive(true);
            trainingButton.gameObject.SetActive(true);
            
            // Move out and scale up
            arenaButton.DOAnchorPos(new Vector2(150, 0), duration).SetEase(Ease.OutBack);
            arenaButton.DOScale(1f, duration).SetEase(Ease.OutBack);

            trainingButton.DOAnchorPos(new Vector2(300, 0), duration).SetEase(Ease.OutBack);
            trainingButton.DOScale(1f, duration).SetEase(Ease.OutBack);
        }
        else
        {
            // Tween sub-buttons out
            arenaButton.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.InBack);
            arenaButton.DOScale(0f, duration).SetEase(Ease.InBack).OnComplete(() => arenaButton.gameObject.SetActive(false));

            trainingButton.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.InBack);
            trainingButton.DOScale(0f, duration).SetEase(Ease.InBack).OnComplete(() => trainingButton.gameObject.SetActive(false));
        }
=======
        Transform[] allChildren = canvasRoot.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allChildren)
        {
            string btnName = t.gameObject.name.ToLower().Trim();

            bool isButton = btnName == "play" || btnName == "options" || btnName == "store" || btnName == "shop" || btnName == "quit" ||
                            btnName == "arena" || btnName == "training" || btnName == "exit" || btnName == "back" || btnName == "close" ||
                            btnName == "level 1" || btnName == "arena mode (1)" || btnName == "level 2" || btnName == "arena mode (2)" ||
                            btnName == "level 3" || btnName == "arena mode (3)";

            if (isButton)
            {
                Button btn = t.GetComponent<Button>();
                if (btn == null) btn = t.gameObject.AddComponent<Button>();

                // CRITICAL: RemoveAllListeners() doesn't delete Inspector events.
                // We MUST wipe the entire onClick event clean to destroy any broken inspector references!
                btn.onClick = new Button.ButtonClickedEvent();

                if (btnName == "play") btn.onClick.AddListener(OnPlayClicked);
                else if (btnName == "options") btn.onClick.AddListener(OnOptionsClicked);
                else if (btnName == "store" || btnName == "shop") btn.onClick.AddListener(OnStoreClicked);
                else if (btnName == "quit") btn.onClick.AddListener(OnQuitClicked);
                else if (btnName == "arena") btn.onClick.AddListener(OnArenaClicked);
                else if (btnName == "training") btn.onClick.AddListener(OnTrainingClicked);
                
                else if (btnName == "level 1" || btnName == "arena mode (1)") btn.onClick.AddListener(() => LoadScene("Level 1"));
                else if (btnName == "level 2" || btnName == "arena mode (2)") btn.onClick.AddListener(() => LoadScene("Level 2"));
                else if (btnName == "level 3" || btnName == "arena mode (3)") btn.onClick.AddListener(() => LoadScene("Level 3"));
                
                else if (btnName == "exit" || btnName == "back" || btnName == "close") 
                {
                    btn.onClick.AddListener(OnBackOrExitClicked);
                }
            }
        }
    }

    private void OnPlayClicked()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(true);
        currentState = MenuState.Arena;
    }

    private void OnOptionsClicked()
    {
        if (settingsCanvas != null) settingsCanvas.SetActive(true);
>>>>>>> Stashed changes
    }

    private void OnStoreClicked()
    {
<<<<<<< Updated upstream
        Debug.Log("Loading Arena...");
        // SceneManager.LoadScene("ArenaScene");
    }

    public void LoadTraining()
    {
        Debug.Log("Loading Training...");
        // SceneManager.LoadScene("TrainingScene");
    }

    public void OpenOptions()
    {
        ShowPanel(optionsPanel);
    }

    public void OpenStore()
    {
        ShowPanel(storePanel);
    }

    public void OpenLogin()
    {
        ShowPanel(loginPanel);
    }

    public void CloseAllPanels()
    {
        HidePanel(optionsPanel);
        HidePanel(storePanel);
        HidePanel(loginPanel);
    }

    private void ShowPanel(CanvasGroup panel)
    {
        if (panel == null) return;
        
        CloseAllPanels(); // Close others first
        panel.gameObject.SetActive(true);
        panel.alpha = 0f;
        panel.DOFade(1f, 0.4f);
        panel.transform.localScale = Vector3.one * 0.8f;
        panel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }

    private void HidePanel(CanvasGroup panel)
    {
        if (panel == null || !panel.gameObject.activeSelf) return;
        
        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.transform.DOScale(0.8f, 0.3f).SetEase(Ease.InBack);
        panel.DOFade(0f, 0.3f).OnComplete(() => panel.gameObject.SetActive(false));
    }

    private void HideAllPanelsImmediate()
    {
        if (optionsPanel) { optionsPanel.alpha = 0; optionsPanel.gameObject.SetActive(false); }
        if (storePanel) { storePanel.alpha = 0; storePanel.gameObject.SetActive(false); }
        if (loginPanel) { loginPanel.alpha = 0; loginPanel.gameObject.SetActive(false); }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
=======
        if (shopMenuCanvas != null) shopMenuCanvas.SetActive(true);
        // Overlay menu, don't change base state
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
>>>>>>> Stashed changes
        Application.Quit();
#endif
    }

    private void OnArenaClicked()
    {
        if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(false);
        if (levelSelectorCanvas != null) levelSelectorCanvas.SetActive(true);
        currentState = MenuState.Level;
    }

    private void OnTrainingClicked()
    {
        LoadScene("UI Mainmenu"); 
    }

    private void OnBackOrExitClicked()
    {
        // Bulletproof back button using actual state, not hierarchy guessing
        if (shopMenuCanvas != null && shopMenuCanvas.activeSelf)
        {
            shopMenuCanvas.SetActive(false); // Close overlay
        }
        else if (settingsCanvas != null && settingsCanvas.activeSelf)
        {
            settingsCanvas.SetActive(false); // Close overlay
        }
        else if (currentState == MenuState.Level)
        {
            if (levelSelectorCanvas != null) levelSelectorCanvas.SetActive(false);
            if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(true);
            currentState = MenuState.Arena;
        }
        else if (currentState == MenuState.Arena)
        {
            if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(false);
            if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
            currentState = MenuState.Main;
        }
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
