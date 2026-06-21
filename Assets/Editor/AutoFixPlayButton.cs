using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AutoFixPlayButton : EditorWindow
{
    [MenuItem("Tools/Auto-Fix Play Button")]
    public static void FixPlay()
    {
        // Try to find by exact name
        GameObject mainMenu = GameObject.Find("MainMenu (important)");
        if (mainMenu == null) mainMenu = GameObject.Find("MainMenu");

        if (mainMenu == null)
        {
            Debug.LogWarning("Could not find MainMenu (important)!");
            return;
        }

        MainMenuManager manager = mainMenu.GetComponent<MainMenuManager>();
        if (manager == null) manager = mainMenu.AddComponent<MainMenuManager>();

        Transform playTransform = FindChildRecursive(mainMenu.transform, "play");
        if (playTransform == null)
        {
            Debug.LogWarning("Could not find 'play' button anywhere under " + mainMenu.name + "!");
            return;
        }

        manager.playButton = playTransform.GetComponent<RectTransform>();

        Button btn = playTransform.GetComponent<Button>();
        if (btn == null) btn = playTransform.gameObject.AddComponent<Button>();

        Image img = playTransform.GetComponent<Image>();
        if (img == null)
        {
            img = playTransform.gameObject.AddComponent<Image>();
            img.color = new Color(1, 1, 1, 0); 
        }
        img.raycastTarget = true;

        manager.arenaButton = CreateSubButton(playTransform, "arena", "ARENA", manager, "LoadArena", new Vector2(150, 0));
        manager.trainingButton = CreateSubButton(playTransform, "training", "TRAINING", manager, "LoadTraining", new Vector2(300, 0));

        UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, "TogglePlayOptions"));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, "TogglePlayOptions"));

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(playTransform.gameObject);
        Debug.Log("Play button fixed! Found it deeply nested inside: " + playTransform.parent.name);
    }

    private static Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindChildRecursive(child, name);
            if (result != null) return result;
        }
        return null;
    }

    private static RectTransform CreateSubButton(Transform parent, string name, string label, MainMenuManager manager, string methodName, Vector2 defaultPos)
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
        rect.anchoredPosition = Vector2.zero; 
        rect.sizeDelta = new Vector2(120, 50);

        Button btn = btnObj.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction), manager, methodName));

        btnObj.SetActive(false);

        return rect;
    }
}
