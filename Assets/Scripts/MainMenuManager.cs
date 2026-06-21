using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sub Buttons for Play")]
    public RectTransform playButton;
    public RectTransform arenaButton;
    public RectTransform trainingButton;
    private bool isPlayExpanded = false;

    [Header("Panels")]
    public CanvasGroup settingsPanel;
    public CanvasGroup optionsPanel;
    public CanvasGroup storePanel;
    public CanvasGroup loginPanel;
    private bool isSettingsOpen = false;

    [Header("Arena / Next Screen")]
    public CanvasGroup mainMenuCanvasGroup;
    public CanvasGroup arenaCanvasGroup;

    [Header("Camera Animation")]
    public Animator cameraAnimator;
    public string cameraAnimationTrigger = "MoveToArena";

    private void Start()
    {
        // Initialize sub-buttons state
        if (arenaButton != null) arenaButton.localScale = Vector3.zero;
        if (trainingButton != null) trainingButton.localScale = Vector3.zero;

        // Initialize panels state
        HideSettingsImmediate();
        HideAllPanelsImmediate();
    }

    public void TogglePlayOptions()
    {
        ToggleSettingsPanel();
    }

    public void ToggleSettingsPanel()
    {
        isSettingsOpen = !isSettingsOpen;

        if (isSettingsOpen)
        {
            ShowPanel(settingsPanel);
        }
        else
        {
            HidePanel(settingsPanel);
        }
    }

    public void OpenArenaMenu()
    {
        Debug.Log("Transitioning to Arena UI with Camera Animation...");
        
        // 1. Play the Camera Animation
        if (cameraAnimator != null)
        {
            cameraAnimator.SetTrigger(cameraAnimationTrigger);
        }

        // 2. Fade OUT the Main Menu Canvas
        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.blocksRaycasts = false;
            mainMenuCanvasGroup.DOFade(0f, 0.5f);
        }

        // 3. Fade IN the Arena Canvas
        if (arenaCanvasGroup != null)
        {
            arenaCanvasGroup.gameObject.SetActive(true);
            arenaCanvasGroup.DOFade(1f, 0.5f).SetDelay(0.3f); // Slight delay so camera moves first
            arenaCanvasGroup.interactable = true;
            arenaCanvasGroup.blocksRaycasts = true;
        }
    }

    public void StartArenaMatch()
    {
        Debug.Log("Starting Arena Match! (Add your scene name here)");
        // SceneManager.LoadScene("ArenaScene");
    }

    public void LoadArena()
    {
        StartArenaMatch();
    }

    public void StartTrainingMatch()
    {
        Debug.Log("Starting Training Match! (Add your scene name here)");
        // SceneManager.LoadScene("TrainingScene");
    }

    public void LoadTraining()
    {
        StartTrainingMatch();
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
        HidePanel(settingsPanel);
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
        if (settingsPanel) { settingsPanel.alpha = 0; settingsPanel.gameObject.SetActive(false); }
        if (optionsPanel) { optionsPanel.alpha = 0; optionsPanel.gameObject.SetActive(false); }
        if (storePanel) { storePanel.alpha = 0; storePanel.gameObject.SetActive(false); }
        if (loginPanel) { loginPanel.alpha = 0; loginPanel.gameObject.SetActive(false); }
    }

    private void HideSettingsImmediate()
    {
        if (!settingsPanel) return;
        settingsPanel.alpha = 0;
        settingsPanel.interactable = false;
        settingsPanel.blocksRaycasts = false;
        settingsPanel.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}
