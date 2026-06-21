using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    // --- LEGACY VARIABLES ADDED TO PREVENT COMPILATION ERRORS ---
    [Header("Sub Buttons for Play (Legacy)")]
    public RectTransform playButton;
    public RectTransform arenaButton;
    public RectTransform trainingButton;
    
    [Header("Legacy Panels")]
    public CanvasGroup settingsPanel;
    public CanvasGroup loginPanel;
    // -----------------------------------------------------------

    [Header("Menu Canvases (CanvasGroups)")]
    [Tooltip("The main menu canvas containing Play, Options, Store, Quit.")]
    [SerializeField] private CanvasGroup mainMenuCanvas;
    [Tooltip("The arena menu canvas containing Arena, Training.")]
    [SerializeField] private CanvasGroup arenaMenuCanvas;
    [Tooltip("The level selection canvas containing Level 1, 2, and 3.")]
    [SerializeField] private CanvasGroup levelSelectorCanvas;

    [Header("Overlay Panels (CanvasGroups)")]
    [Tooltip("The options popup panel.")]
    public CanvasGroup optionsPanel;
    [Tooltip("The store popup panel.")]
    public CanvasGroup storePanel;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.4f;
    [SerializeField] private Ease transitionEase = Ease.OutQuad;

    private void Start()
    {
        // 1. Initialize screen states (only Main Menu visible on start)
        InitializeMenuState(mainMenuCanvas, true);
        InitializeMenuState(arenaMenuCanvas, false);
        InitializeMenuState(levelSelectorCanvas, false);
        
        // 2. Hide overlay panels instantly
        if (optionsPanel != null) HidePanelImmediate(optionsPanel);
        if (storePanel != null) HidePanelImmediate(storePanel);
    }

    #region --- Main Menu Actions ---
    // Transitions from Main Menu -> Arena Menu
    public void OnPlayClicked()
    {
        TransitionCanvases(mainMenuCanvas, arenaMenuCanvas);
    }

    public void OnOptionsClicked()
    {
        ShowPanelOverlay(optionsPanel);
    }

    public void OnStoreClicked()
    {
        ShowPanelOverlay(storePanel);
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quit game requested.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region --- Arena Menu Actions ---
    // Transitions from Arena Menu -> Level Selector
    public void OnArenaClicked()
    {
        TransitionCanvases(arenaMenuCanvas, levelSelectorCanvas);
    }

    // Loads Training Scene (or preview)
    public void OnTrainingClicked()
    {
        Debug.Log("Loading Training Scene...");
        LoadScene("TrainingScenePreview"); 
    }

    // Back from Arena Menu -> Main Menu
    public void OnArenaMenuBackClicked()
    {
        TransitionCanvases(arenaMenuCanvas, mainMenuCanvas);
    }
    #endregion

    #region --- Level Selection Actions ---
    public void OnLevel1Clicked()
    {
        LoadScene("Level 1");
    }

    public void OnLevel2Clicked()
    {
        LoadScene("Level 2");
    }

    public void OnLevel3Clicked()
    {
        LoadScene("Level 3");
    }

    // Back from Level Selector -> Arena Menu
    public void OnLevelSelectorBackClicked()
    {
        TransitionCanvases(levelSelectorCanvas, arenaMenuCanvas);
    }
    #endregion

    #region --- Panel Overlay Actions ---
    // Close Options Panel and return to Main Menu
    public void CloseOptions()
    {
        HidePanelOverlay(optionsPanel);
    }

    // Close Store Panel and return to Main Menu
    public void CloseStore()
    {
        HidePanelOverlay(storePanel);
    }
    #endregion

    #region --- Helper Animations & Navigation ---
    /// <summary>
    /// Snaps a CanvasGroup to its initial active/inactive state immediately on start.
    /// </summary>
    private void InitializeMenuState(CanvasGroup canvasGroup, bool show)
    {
        if (canvasGroup == null) return;
        canvasGroup.gameObject.SetActive(show);
        canvasGroup.alpha = show ? 1f : 0f;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        canvasGroup.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Smoothly fades and scales out fromCanvas, and fades/scales in toCanvas.
    /// </summary>
    private void TransitionCanvases(CanvasGroup fromCanvas, CanvasGroup toCanvas)
    {
        if (fromCanvas != null)
        {
            fromCanvas.interactable = false;
            fromCanvas.blocksRaycasts = false;
            fromCanvas.DOFade(0f, transitionDuration).SetEase(transitionEase);
            fromCanvas.transform.DOScale(0.95f, transitionDuration).SetEase(transitionEase)
                .OnComplete(() => fromCanvas.gameObject.SetActive(false));
        }

        if (toCanvas != null)
        {
            toCanvas.gameObject.SetActive(true);
            toCanvas.alpha = 0f;
            toCanvas.transform.localScale = Vector3.one * 0.95f;
            
            toCanvas.DOFade(1f, transitionDuration).SetEase(transitionEase);
            toCanvas.transform.DOScale(1f, transitionDuration).SetEase(transitionEase)
                .OnComplete(() =>
                {
                    toCanvas.interactable = true;
                    toCanvas.blocksRaycasts = true;
                });
        }
    }

    /// <summary>
    /// Opens an overlay panel with a premium pop-up elastic scale animation.
    /// </summary>
    private void ShowPanelOverlay(CanvasGroup panel)
    {
        if (panel == null) return;
        panel.gameObject.SetActive(true);
        panel.alpha = 0f;
        panel.transform.localScale = Vector3.one * 0.8f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        
        panel.DOFade(1f, transitionDuration).SetEase(Ease.OutBack);
        panel.transform.DOScale(1f, transitionDuration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                panel.interactable = true;
                panel.blocksRaycasts = true;
            });
    }

    /// <summary>
    /// Closes an overlay panel with a smooth scale-down and fade-out animation.
    /// </summary>
    private void HidePanelOverlay(CanvasGroup panel)
    {
        if (panel == null || !panel.gameObject.activeSelf) return;
        
        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.transform.DOScale(0.8f, transitionDuration).SetEase(Ease.InBack);
        panel.DOFade(0f, transitionDuration).SetEase(Ease.InBack)
            .OnComplete(() => panel.gameObject.SetActive(false));
    }

    private void HidePanelImmediate(CanvasGroup panel)
    {
        if (panel == null) return;
        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Safely loads a scene using the SceneTransitionManager if present in your scene.
    /// </summary>
    private void LoadScene(string sceneName)
    {
        // Reflection check to avoid compiler errors if SceneTransitionManager doesn't exist
        var type = System.Type.GetType("SceneTransitionManager");
        if (type != null)
        {
            var instanceProperty = type.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            if (instanceProperty != null)
            {
                var instance = instanceProperty.GetValue(null);
                if (instance != null)
                {
                    var loadMethod = type.GetMethod("LoadScene", new System.Type[] { typeof(string) });
                    if (loadMethod != null)
                    {
                        loadMethod.Invoke(instance, new object[] { sceneName });
                        return;
                    }
                }
            }
        }
        
        // Fallback to direct scene loading
        SceneManager.LoadScene(sceneName);
    }
    #endregion
}
