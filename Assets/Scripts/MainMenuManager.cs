using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private GameObject mainMenuCanvas;
    private GameObject arenaMenuCanvas;
    private GameObject levelSelectorCanvas;
    private GameObject shopMenuCanvas;

    private void Start()
    {
        Debug.Log("--- ULTIMATE MENU MANAGER INITIALIZING ---");
        
        // 1. Find all Canvases and Buttons automatically!
        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        
        foreach (Transform t in allTransforms)
        {
            // Skip prefabs that aren't actually in the scene
            if (t.gameObject.scene != this.gameObject.scene) continue;
            
            string objName = t.gameObject.name;
            string btnName = objName.ToLower().Trim();

            // --- FIND CANVASES ---
            if (objName == "MainMenu") mainMenuCanvas = t.gameObject;
            else if (objName == "ArenaMenu") arenaMenuCanvas = t.gameObject;
            else if (objName == "Level") levelSelectorCanvas = t.gameObject;
            else if (objName == "ShopMenu") shopMenuCanvas = t.gameObject;

            // --- FIND BUTTONS ---
            bool isButton = btnName == "play" || btnName == "options" || btnName == "store" || btnName == "shop" || btnName == "quit" ||
                            btnName == "arena" || btnName == "training" || btnName == "exit" || btnName == "back" || btnName == "close" ||
                            btnName == "level 1" || btnName == "arena mode (1)" || btnName == "level 2" || btnName == "arena mode (2)" ||
                            btnName == "level 3" || btnName == "arena mode (3)";

            if (isButton)
            {
                Button btn = t.GetComponent<Button>();
                if (btn == null) btn = t.gameObject.AddComponent<Button>(); // Auto-add Button if missing
                
                // Clear out any old, broken Unity inspector events!
                btn.onClick.RemoveAllListeners();

                // Add the new listener directly via C# code
                if (btnName == "play") btn.onClick.AddListener(OnPlayClicked);
                else if (btnName == "store" || btnName == "shop") btn.onClick.AddListener(OnStoreClicked);
                else if (btnName == "quit") btn.onClick.AddListener(OnQuitClicked);
                
                else if (btnName == "arena") btn.onClick.AddListener(OnArenaClicked);
                else if (btnName == "training") btn.onClick.AddListener(OnTrainingClicked);
                
                else if (btnName == "level 1" || btnName == "arena mode (1)") btn.onClick.AddListener(() => LoadScene("Level 1"));
                else if (btnName == "level 2" || btnName == "arena mode (2)") btn.onClick.AddListener(() => LoadScene("Level 2"));
                else if (btnName == "level 3" || btnName == "arena mode (3)") btn.onClick.AddListener(() => LoadScene("Level 3"));
                
                else if (btnName == "exit" || btnName == "back" || btnName == "close") 
                {
                    btn.onClick.AddListener(() => OnBackOrExitClicked(t));
                }
            }
        }

        // 2. Set the exact starting state
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(false);
        if (levelSelectorCanvas != null) levelSelectorCanvas.SetActive(false);
        if (shopMenuCanvas != null) shopMenuCanvas.SetActive(false);
        
        Debug.Log("--- ULTIMATE MENU MANAGER READY! ---");
    }

    #region --- Button Click Logic ---
    private void OnPlayClicked()
    {
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(true);
    }

    private void OnStoreClicked()
    {
        if (shopMenuCanvas != null) shopMenuCanvas.SetActive(true);
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit game requested.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnArenaClicked()
    {
        if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(false);
        if (levelSelectorCanvas != null) levelSelectorCanvas.SetActive(true);
    }

    private void OnTrainingClicked()
    {
        // I am setting this to "idlee" since you mentioned it before!
        // If your training scene is named something else, just change this text.
        LoadScene("idlee"); 
    }

    private void OnBackOrExitClicked(Transform clickedButtonTransform)
    {
        // Smart back button: detects which menu it lives inside to know where to go back to!
        if (arenaMenuCanvas != null && clickedButtonTransform.IsChildOf(arenaMenuCanvas.transform))
        {
            arenaMenuCanvas.SetActive(false);
            if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        }
        else if (levelSelectorCanvas != null && clickedButtonTransform.IsChildOf(levelSelectorCanvas.transform))
        {
            levelSelectorCanvas.SetActive(false);
            if (arenaMenuCanvas != null) arenaMenuCanvas.SetActive(true);
        }
        else if (shopMenuCanvas != null && clickedButtonTransform.IsChildOf(shopMenuCanvas.transform))
        {
            shopMenuCanvas.SetActive(false);
            // Shop just closes its overlay
        }
    }
    #endregion

    private void LoadScene(string sceneName)
    {
        Debug.Log($"Loading Scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
