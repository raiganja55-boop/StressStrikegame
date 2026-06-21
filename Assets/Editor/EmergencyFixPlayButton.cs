using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class EmergencyFixPlayButton : EditorWindow
{
    [MenuItem("Tools/EMERGENCY FIX PLAY BUTTON")]
    public static void FixPlay()
    {
        // 1. Find Play button
        GameObject playBtn = GameObject.Find("play");
        if (playBtn == null)
        {
            // Try recursive search from MainMenu
            GameObject mm = GameObject.Find("MainMenu (important)") ?? GameObject.Find("MainMenu");
            if (mm != null) playBtn = FindChildRecursive(mm.transform, "play")?.gameObject;
        }

        if (playBtn == null)
        {
            Debug.LogError("Could not find any GameObject named 'play'!");
            return;
        }

        // 2. Force it visible
        playBtn.SetActive(true);
        playBtn.transform.localScale = Vector3.one;
        
        Image img = playBtn.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = 1f; // Force fully visible
            img.color = c;
        }

        CanvasGroup cg = playBtn.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.gameObject.SetActive(true);
        }

        // 3. Remove it from MenuController if they accidentally put it there
        MenuController mc = Object.FindObjectOfType<MenuController>();
        if (mc != null)
        {
            if (mc.allUiPanels != null)
            {
                for (int i = 0; i < mc.allUiPanels.Length; i++)
                {
                    if (mc.allUiPanels[i] == playBtn) mc.allUiPanels[i] = null;
                }
            }
            if (mc.stations != null)
            {
                foreach (var station in mc.stations)
                {
                    if (station != null && station.uiPanelsToActivate != null)
                    {
                        for (int i = 0; i < station.uiPanelsToActivate.Length; i++)
                        {
                            // if they put the play button in the panels to activate, leave it or remove it?
                            // better remove it so it's not treated as a whole canvas panel
                            if (station.uiPanelsToActivate[i] == playBtn) station.uiPanelsToActivate[i] = null;
                        }
                    }
                }
            }
            EditorUtility.SetDirty(mc);
        }

        // 4. Fix MainMenuManager if they accidentally slotted 'play' into arena
        MainMenuManager mmm = Object.FindObjectOfType<MainMenuManager>();
        if (mmm != null)
        {
            if (mmm.arenaButton != null && mmm.arenaButton.gameObject == playBtn) mmm.arenaButton = null;
            if (mmm.trainingButton != null && mmm.trainingButton.gameObject == playBtn) mmm.trainingButton = null;
            EditorUtility.SetDirty(mmm);
        }

        EditorUtility.SetDirty(playBtn);
        Debug.Log("EMERGENCY FIX APPLIED! Play button forced visible and removed from incorrect script slots.");
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
