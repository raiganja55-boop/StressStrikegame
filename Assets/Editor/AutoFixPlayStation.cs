using UnityEngine;
using UnityEditor;

public class AutoFixPlayStation : EditorWindow
{
    [MenuItem("Tools/Auto-Fix Play Station")]
    public static void FixPlayStation()
    {
        GameObject menuManagerObj = GameObject.Find("MenuManager");
        if (menuManagerObj == null)
        {
            Debug.LogWarning("MenuManager not found!");
            return;
        }

        MenuController controller = menuManagerObj.GetComponent<MenuController>();
        if (controller == null)
        {
            Debug.LogWarning("MenuController not found!");
            return;
        }

        // Check if stations has at least 3 elements (Home, Options, Play)
        if (controller.stations == null || controller.stations.Length < 3)
        {
            var oldStations = controller.stations;
            controller.stations = new MenuController.MenuStation[3];
            if (oldStations != null)
            {
                for (int i = 0; i < oldStations.Length; i++)
                {
                    controller.stations[i] = oldStations[i];
                }
            }
        }

        // Initialize Station 2 if it's null
        if (controller.stations[2] == null)
        {
            controller.stations[2] = new MenuController.MenuStation();
            controller.stations[2].stationName = "Play";
        }

        // Make SURE keepPreviousPanelsOpen is true so MainMenu doesn't disappear!
        controller.stations[2].keepPreviousPanelsOpen = true;

        // Try to find an Anchor for Play and a Panel for Play
        GameObject anchorPlay = GameObject.Find("Anchor_Play");
        if (anchorPlay != null) controller.stations[2].cameraAnchor = anchorPlay.transform;

        GameObject playMenu = GameObject.Find("PlayMenu");
        if (playMenu != null) controller.stations[2].uiPanelsToActivate = new GameObject[] { playMenu };
        else controller.stations[2].uiPanelsToActivate = new GameObject[0];

        EditorUtility.SetDirty(controller);
        Debug.Log("Fixed the Play Station! 'Keep Previous Panels Open' is now checked.");
    }
}
