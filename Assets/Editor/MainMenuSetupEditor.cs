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

        // Add MainMenuManager if not present
        MainMenuManager manager = mainMenu.GetComponent<MainMenuManager>();
        if (manager == null)
        {
            manager = mainMenu.AddComponent<MainMenuManager>();
        }

        // Convert the 5 objects into interactive buttons
        ConvertObjectToButton(mainMenu.transform, "play", manager, "PlayGame");
        ConvertObjectToButton(mainMenu.transform, "options", manager, "OpenOptions");
        ConvertObjectToButton(mainMenu.transform, "quit", manager, "QuitGame");
        ConvertObjectToButton(mainMenu.transform, "store", manager, "OpenStore");
        ConvertObjectToButton(mainMenu.transform, "login", manager, "OpenLogin");

        // Ensure there is an EventSystem in the scene
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        Debug.Log("Main Menu UI setup successfully! Play, Options, Quit, Store, and Login are now interactive buttons.");
    }

    private static void ConvertObjectToButton(Transform parent, string childName, MainMenuManager manager, string methodName)
    {
        Transform childTransform = parent.Find(childName);
        if (childTransform == null)
        {
            Debug.LogWarning("Could not find child named '" + childName + "'.");
            return;
        }

        GameObject childObj = childTransform.gameObject;

        // Ensure it has an Image component to catch clicks and show color tints
        Image img = childObj.GetComponent<Image>();
        if (img == null)
        {
            img = childObj.AddComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0f); // transparent if it's just a container
        }

        // Add Button component
        Button btn = childObj.GetComponent<Button>();
        if (btn == null)
        {
            btn = childObj.AddComponent<Button>();
        }

        // Set up nice interactive color tinting
        btn.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f); // Slightly darker on hover
        colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f); // Darker on press to feel interactive
        colors.selectedColor = Color.white;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        // Setup the UnityEvent securely
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));

        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        
        // Ensure Raycast Target is on so it can be clicked
        img.raycastTarget = true;
    }
}
