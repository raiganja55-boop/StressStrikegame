using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UltimatePlayButtonFix : EditorWindow
{
    [MenuItem("Tools/ULTIMATE PLAY BUTTON FIX")]
    public static void FixIt()
    {
        // 1. Get the Play button
        GameObject playBtn = GameObject.Find("play");
        if (playBtn == null)
        {
            GameObject mm = GameObject.Find("MainMenu (important)") ?? GameObject.Find("MainMenu");
            if (mm != null) playBtn = FindChildRecursive(mm.transform, "play")?.gameObject;
        }

        if (playBtn == null)
        {
            Debug.LogError("Play button STILL not found!");
            return;
        }

        // 2. Get the MenuController
        MenuController mc = Object.FindObjectOfType<MenuController>();
        if (mc == null)
        {
            Debug.LogError("MenuController not found in the scene!");
            return;
        }

        // 3. Ensure Station 2 exists
        if (mc.stations == null || mc.stations.Length < 3)
        {
            var oldStations = mc.stations;
            mc.stations = new MenuController.MenuStation[3];
            if (oldStations != null)
            {
                for (int i = 0; i < oldStations.Length; i++) mc.stations[i] = oldStations[i];
            }
        }
        if (mc.stations[2] == null) mc.stations[2] = new MenuController.MenuStation();
        mc.stations[2].stationName = "Play";
        mc.stations[2].keepPreviousPanelsOpen = true;

        // 4. Force Button component and Raycast
        Button btn = playBtn.GetComponent<Button>();
        if (btn == null) btn = playBtn.AddComponent<Button>();
        
        Image img = playBtn.GetComponent<Image>();
        if (img != null) img.raycastTarget = true;

        // 5. WIRE THE BUTTON TO TOGGLESTATION(2)
        // First, clear any broken old listeners
        while (btn.onClick.GetPersistentEventCount() > 0)
        {
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(btn.onClick, 0);
        }

        // Add the new listener
        UnityEditor.Events.UnityEventTools.AddIntPersistentListener(btn.onClick, 
            (UnityEngine.Events.UnityAction<int>)System.Delegate.CreateDelegate(typeof(UnityEngine.Events.UnityAction<int>), mc, "ToggleStation"), 2);

        EditorUtility.SetDirty(mc);
        EditorUtility.SetDirty(playBtn);
        Debug.Log("ULTIMATE FIX COMPLETE! Play button is now perfectly wired to MenuController.ToggleStation(2)!");
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
}
