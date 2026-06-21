using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;

public class AutoSetupUI : EditorWindow
{
    [MenuItem("Tools/Auto Setup My UI")]
    public static void SetupUI()
    {
        MainMenuManager manager = FindObjectOfType<MainMenuManager>();
        if (manager == null)
        {
            Debug.LogError("MainMenuManager not found in the active scene!");
            return;
        }

        CanvasGroup arenaMenu = null;
        CanvasGroup levelMenu = null;
        CanvasGroup mainMenu = null;

        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (var t in allTransforms)
        {
            if (!t.gameObject.scene.isLoaded) continue;

            if (t.gameObject.name.Contains("ArenaMenu"))
            {
                arenaMenu = t.GetComponent<CanvasGroup>();
                if (arenaMenu == null) arenaMenu = t.gameObject.AddComponent<CanvasGroup>();
            }
            if (t.gameObject.name == "Level")
            {
                levelMenu = t.GetComponent<CanvasGroup>();
                if (levelMenu == null) levelMenu = t.gameObject.AddComponent<CanvasGroup>();
            }
            if (t.gameObject.name.Contains("MainMenu (important)"))
            {
                mainMenu = t.GetComponent<CanvasGroup>();
                if (mainMenu == null) mainMenu = t.gameObject.AddComponent<CanvasGroup>();
            }
        }

        // Apply references using SerializedObject since the fields might be private
        SerializedObject serializedManager = new SerializedObject(manager);
        if (mainMenu != null) serializedManager.FindProperty("mainMenuCanvas").objectReferenceValue = mainMenu;
        if (arenaMenu != null) serializedManager.FindProperty("arenaMenuCanvas").objectReferenceValue = arenaMenu;
        if (levelMenu != null) serializedManager.FindProperty("levelSelectorCanvas").objectReferenceValue = levelMenu;
        serializedManager.ApplyModifiedProperties();

        // Hook up Main Menu Buttons
        if (mainMenu != null)
        {
            Button[] mainButtons = mainMenu.GetComponentsInChildren<Button>(true);
            foreach (var btn in mainButtons)
            {
                string btnName = btn.gameObject.name.ToLower().Trim();
                
                if (btnName == "play")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnPlayClicked);
                }
                else if (btnName == "options")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnOptionsClicked);
                }
                else if (btnName == "store")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnStoreClicked);
                }
                else if (btnName == "quit")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnQuitClicked);
                }
            }
            EditorUtility.SetDirty(mainMenu.gameObject);
        }

        // Hook up ArenaMenu Buttons
        if (arenaMenu != null)
        {
            Button[] arenaButtons = arenaMenu.GetComponentsInChildren<Button>(true);
            foreach (var btn in arenaButtons)
            {
                string btnName = btn.gameObject.name.ToLower().Trim();
                if (btnName == "arena")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnArenaClicked);
                }
                else if (btnName == "training")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnTrainingClicked);
                }
                else if (btnName == "back" || btnName == "close")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnArenaMenuBackClicked);
                }
            }
            EditorUtility.SetDirty(arenaMenu.gameObject);
        }

        // Hook up Level Buttons
        if (levelMenu != null)
        {
            Button[] levelButtons = levelMenu.GetComponentsInChildren<Button>(true);
            foreach (var btn in levelButtons)
            {
                string btnName = btn.gameObject.name.ToLower().Trim();
                
                if (btnName == "level 1" || btnName == "arena mode (1)")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnLevel1Clicked);
                }
                else if (btnName == "level 2" || btnName == "arena mode (2)")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnLevel2Clicked);
                }
                else if (btnName == "level 3" || btnName == "arena mode (3)")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnLevel3Clicked);
                }
                else if (btnName == "back" || btnName == "close")
                {
                    ClearEvents(btn);
                    UnityEventTools.AddPersistentListener(btn.onClick, manager.OnLevelSelectorBackClicked);
                }
            }
            EditorUtility.SetDirty(levelMenu.gameObject);
        }

        EditorUtility.SetDirty(manager);
        
        Debug.Log("--- AUTOMATIC UI SETUP COMPLETE! ---");
        Debug.Log("Successfully wired up all buttons to the NEW DG.Tweening script format!");
    }

    private static void ClearEvents(Button btn)
    {
        while (btn.onClick.GetPersistentEventCount() > 0)
        {
            UnityEventTools.RemovePersistentListener(btn.onClick, 0);
        }
    }
}
