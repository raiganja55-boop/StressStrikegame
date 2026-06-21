using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuSetupWizard
{
    [MenuItem("Tools/Setup Menu Test Scene")]
    public static void SetupTestScene()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera mainCamera = cameraObj.AddComponent<Camera>();
        cameraObj.tag = "MainCamera";
        cameraObj.transform.position = new Vector3(0, 1, -10);

        // 2. Anchors
        GameObject anchorHome = new GameObject("CamAnchor_Home");
        anchorHome.transform.position = new Vector3(0, 1, -10);

        GameObject anchorSettings = new GameObject("CamAnchor_Settings");
        anchorSettings.transform.position = new Vector3(20, 1, -10);
        anchorSettings.transform.rotation = Quaternion.Euler(0, 45, 0);

        // Create some primitives in the world so you can see the camera move relative to them
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0, 1, -5);
        sphere.name = "Home_Reference_Object";

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(20, 1, -5);
        cube.name = "Settings_Reference_Object";

        // 3. UI
        GameObject canvasObj = new GameObject("MenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject homePanel = new GameObject("Panel_Home");
        homePanel.transform.SetParent(canvasObj.transform, false);
        var imgHome = homePanel.AddComponent<Image>();
        imgHome.color = new Color(0, 0, 1, 0.3f); // Blue tint
        RectTransform homeRect = homePanel.GetComponent<RectTransform>();
        homeRect.anchorMin = Vector2.zero;
        homeRect.anchorMax = Vector2.one;
        homeRect.sizeDelta = Vector2.zero;

        GameObject settingsPanel = new GameObject("Panel_Settings");
        settingsPanel.transform.SetParent(canvasObj.transform, false);
        var imgSet = settingsPanel.AddComponent<Image>();
        imgSet.color = new Color(1, 0, 0, 0.3f); // Red tint
        RectTransform settingsRect = settingsPanel.GetComponent<RectTransform>();
        settingsRect.anchorMin = Vector2.zero;
        settingsRect.anchorMax = Vector2.one;
        settingsRect.sizeDelta = Vector2.zero;
        settingsPanel.SetActive(false);

        // 4. Event System
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();

        // 5. Manager
        GameObject managerObj = new GameObject("MenuManager");
        MenuController controller = managerObj.AddComponent<MenuController>();
        
        controller.mainCamera = mainCamera;
        controller.transitionSpeed = 2f;
        
        controller.stations = new MenuController.MenuStation[2];
        
        controller.stations[0] = new MenuController.MenuStation();
        controller.stations[0].stationName = "Home";
        controller.stations[0].cameraAnchor = anchorHome.transform;
        controller.stations[0].uiPanelsToActivate = new GameObject[] { homePanel };

        controller.stations[1] = new MenuController.MenuStation();
        controller.stations[1].stationName = "Settings";
        controller.stations[1].cameraAnchor = anchorSettings.transform;
        controller.stations[1].uiPanelsToActivate = new GameObject[] { settingsPanel };

        controller.allUiPanels = new GameObject[] { homePanel, settingsPanel };

        // Save
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(newScene, "Assets/Scenes/MenuTestScene.unity");
        Debug.Log("MenuTestScene has been created and saved at Assets/Scenes/MenuTestScene.unity");
    }
}
