using UnityEngine;
using UnityEditor;

public class UltraSafeMenuFix : EditorWindow
{
    [MenuItem("Tools/ULTRA SAFE MENU FIX")]
    public static void FixIt()
    {
        MenuController mc = Object.FindObjectOfType<MenuController>();
        if (mc == null) return;

        if (mc.stations != null)
        {
            foreach (var station in mc.stations)
            {
                if (station != null)
                {
                    if (station.uiPanelsToActivate == null)
                    {
                        station.uiPanelsToActivate = new GameObject[0];
                    }
                }
            }
        }

        EditorUtility.SetDirty(mc);
        Debug.Log("Fixed null arrays in MenuController!");
    }
}
