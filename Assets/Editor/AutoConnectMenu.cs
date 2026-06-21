using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AutoConnectMenu : EditorWindow
{
    [MenuItem("Tools/Auto-Link Menu Controller")]
    public static void LinkMenu()
    {
        // 1. Find or create MenuManager
        GameObject menuManagerObj = GameObject.Find("MenuManager");
        if (menuManagerObj == null) menuManagerObj = new GameObject("MenuManager");

        MenuController controller = menuManagerObj.GetComponent<MenuController>();
        if (controller == null) controller = menuManagerObj.AddComponent<MenuController>();

        // 2. Set Main Camera
        if (controller.mainCamera == null) controller.mainCamera = Camera.main;

        // 3. Find Panels
        GameObject mainMenuPanel = GameObject.Find("MainMenu");
        GameObject settingsPanel = GameObject.Find("Settings");

        // 4. Fill All UI Panels array
        controller.allUiPanels = new GameObject[2];
        controller.allUiPanels[0] = mainMenuPanel;
        controller.allUiPanels[1] = settingsPanel;

        // 5. Find Anchors
        GameObject anchorHome = GameObject.Find("Anchor_Home") ?? GameObject.Find("CamPos_Home");
        GameObject anchorSettings = GameObject.Find("Anchor_Options") ?? GameObject.Find("CamPos_Settings") ?? GameObject.Find("Anchor_Settings");

        // 6. Setup Stations
        controller.stations = new MenuController.MenuStation[2];
        
        // Station 0: Home
        controller.stations[0] = new MenuController.MenuStation();
        controller.stations[0].stationName = "Home";
        if (anchorHome != null) controller.stations[0].cameraAnchor = anchorHome.transform;
        controller.stations[0].uiPanelsToActivate = new GameObject[1] { mainMenuPanel };

        // Station 1: Settings
        controller.stations[1] = new MenuController.MenuStation();
        controller.stations[1].stationName = "Settings";
        if (anchorSettings != null) controller.stations[1].cameraAnchor = anchorSettings.transform;
        controller.stations[1].uiPanelsToActivate = new GameObject[1] { settingsPanel };
        controller.stations[1].keepPreviousPanelsOpen = true; // DO NOT CLOSE MAIN MENU

        // 7. Auto-hook the "options" button in MainMenu
        if (mainMenuPanel != null)
        {
            Transform optionsBtnTrans = mainMenuPanel.transform.Find("options");
            if (optionsBtnTrans != null)
            {
                Button optBtn = optionsBtnTrans.GetComponent<Button>();
                if (optBtn != null)
                {
                    // Clear old listeners
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(optBtn.onClick, 
                        (UnityEngine.Events.UnityAction<int>)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction<int>), controller, "GoToStation"));
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener(optBtn.onClick, 
                        (UnityEngine.Events.UnityAction<int>)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction<int>), controller, "ToggleStation"));

                    // Add new ToggleStation listener
                    UnityEditor.Events.UnityEventTools.AddIntPersistentListener(optBtn.onClick, 
                        (UnityEngine.Events.UnityAction<int>)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction<int>), controller, "ToggleStation"), 1);
                }
            }
        }

        EditorUtility.SetDirty(controller);
        Debug.Log("MCP automatically linked the MenuController and setup ToggleStation!");
    }
}
