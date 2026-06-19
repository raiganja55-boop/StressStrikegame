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
    public CanvasGroup optionsPanel;
    public CanvasGroup storePanel;
    public CanvasGroup loginPanel;

    private void Start()
    {
        // Initialize sub-buttons state
        if (arenaButton != null) arenaButton.localScale = Vector3.zero;
        if (trainingButton != null) trainingButton.localScale = Vector3.zero;

        // Initialize panels state
        HideAllPanelsImmediate();
    }

    public void TogglePlayOptions()
    {
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
    }

    public void LoadArena()
    {
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
        Application.Quit();
    }
}
