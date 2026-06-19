using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuSetupEditor : EditorWindow
{
    [MenuItem("Tools/Setup Main Menu UI")]
    public static void SetupUI()
    {
        GameObject mainMenu = GameObject.Find("MainMenu");
        if (mainMenu == null)
        {
            Debug.LogError("Could not find a GameObject named 'MainMenu' in the scene.");
            return;
        }

        MainMenuManager manager = mainMenu.GetComponent<MainMenuManager>();
        if (manager == null) manager = mainMenu.AddComponent<MainMenuManager>();

        // Find the original play button
        Transform playTransform = mainMenu.transform.Find("play");
        if (playTransform != null)
        {
            manager.playButton = playTransform.GetComponent<RectTransform>();

            // Create Arena and Training as children of Play
            manager.arenaButton = CreateSubButton(playTransform, "arena", "ARENA", manager, "LoadArena");
            manager.trainingButton = CreateSubButton(playTransform, "training", "TRAINING", manager, "LoadTraining");
            
            // Set Play button to TogglePlayOptions
            ConvertObjectToButton(mainMenu.transform, "play", manager, "TogglePlayOptions");
        }
        else
        {
            Debug.LogWarning("play button not found!");
        }

        // Convert the others
        ConvertObjectToButton(mainMenu.transform, "options", manager, "OpenOptions");
        ConvertObjectToButton(mainMenu.transform, "quit", manager, "QuitGame");
        ConvertObjectToButton(mainMenu.transform, "store", manager, "OpenStore");
        ConvertObjectToButton(mainMenu.transform, "login", manager, "OpenLogin");

        // Create Panels
        manager.optionsPanel = CreatePanel(mainMenu.transform, "OptionsPanel", "OPTIONS MENU");
        manager.storePanel = CreatePanel(mainMenu.transform, "StorePanel", "STORE MENU");
        manager.loginPanel = CreatePanel(mainMenu.transform, "LoginPanel", "LOGIN MENU");

        // Ensure EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // Apply changes to the object so they are saved
        EditorUtility.SetDirty(manager);

        Debug.Log("UI Transitions and Panels setup successfully!");
    }

    private static RectTransform CreateSubButton(Transform parent, string name, string label, MainMenuManager manager, string methodName)
    {
        Transform existing = parent.Find(name);
        GameObject btnObj;
        
        if (existing == null)
        {
            btnObj = DefaultControls.CreateButton(new DefaultControls.Resources());
            btnObj.name = name;
            btnObj.transform.SetParent(parent, false);
            btnObj.GetComponentInChildren<Text>().text = label;
        }
        else
        {
            btnObj = existing.gameObject;
        }

        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero; // Start at center
        rect.sizeDelta = new Vector2(120, 50); // Smaller than main buttons

        Button btn = btnObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));

        return rect;
    }

    private static CanvasGroup CreatePanel(Transform parent, string name, string title)
    {
        Transform existing = parent.Find(name);
        if (existing != null) return existing.GetComponent<CanvasGroup>();

        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.1f);
        rect.anchorMax = new Vector2(0.9f, 0.9f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.9f); // Dark background

        CanvasGroup cg = panel.AddComponent<CanvasGroup>();

        // Add Title Text
        GameObject textObj = new GameObject("Title");
        textObj.transform.SetParent(panel.transform, false);
        Text text = textObj.AddComponent<Text>();
        text.text = title;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 40;
        text.alignment = TextAnchor.UpperCenter;
        text.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0.8f);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Add Close Button
        GameObject closeBtn = DefaultControls.CreateButton(new DefaultControls.Resources());
        closeBtn.name = "CloseButton";
        closeBtn.transform.SetParent(panel.transform, false);
        closeBtn.GetComponentInChildren<Text>().text = "X";
        
        RectTransform closeRect = closeBtn.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 1);
        closeRect.anchorMax = new Vector2(1, 1);
        closeRect.anchoredPosition = new Vector2(-40, -40);
        closeRect.sizeDelta = new Vector2(50, 50);

        MainMenuManager manager = parent.GetComponent<MainMenuManager>();
        Button btn = closeBtn.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, "CloseAllPanels"));

        panel.SetActive(false);
        return cg;
    }

    private static void ConvertObjectToButton(Transform parent, string childName, MainMenuManager manager, string methodName)
    {
        Transform childTransform = parent.Find(childName);
        if (childTransform == null) return;

        GameObject childObj = childTransform.gameObject;
        Image img = childObj.GetComponent<Image>();
        if (img == null)
        {
            img = childObj.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0f);
        }

        Button btn = childObj.GetComponent<Button>();
        if (btn == null) btn = childObj.AddComponent<Button>();

        btn.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
        colors.selectedColor = Color.white;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        
        img.raycastTarget = true;
    }
}
