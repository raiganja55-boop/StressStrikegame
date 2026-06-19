using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public float transitionSpeed = 2f;

    [System.Serializable]
    public class MenuStation
    {
        public string stationName;
        public Transform cameraAnchor; // The empty GameObject marking where the camera should go
        public GameObject[] uiPanelsToActivate; // The UI panels to turn ON at this station
    }

    [Header("Menu Stations")]
    public MenuStation[] stations;
    
    [Header("All UI Panels")]
    [Tooltip("Drag EVERY UI panel here so the script can turn them all off while moving.")]
    public GameObject[] allUiPanels;

    private void Start()
    {
        // Go to the first station (Home) when the game starts
        if (stations != null && stations.Length > 0)
            GoToStation(0);
    }

    public void GoToStation(int stationIndex)
    {
        if (stations == null || stationIndex < 0 || stationIndex >= stations.Length) return;
        StopAllCoroutines();
        StartCoroutine(MoveCameraRoutine(stations[stationIndex]));
    }

    private IEnumerator MoveCameraRoutine(MenuStation targetStation)
    {
        // 1. Turn OFF all UI panels before the camera starts moving
        if (allUiPanels != null)
        {
            foreach(var panel in allUiPanels)
            {
                if (panel != null) panel.SetActive(false);
            }
        }

        if (mainCamera == null || targetStation == null || targetStation.cameraAnchor == null) yield break;

        // 2. Prepare for camera movement
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        
        float timeElapsed = 0;

        // 3. Smoothly move and rotate the camera to the new anchor
        while (timeElapsed < 1)
        {
            timeElapsed += Time.deltaTime * transitionSpeed;
            
            float smoothCurve = Mathf.SmoothStep(0, 1, timeElapsed);

            mainCamera.transform.position = Vector3.Lerp(startPos, targetStation.cameraAnchor.position, smoothCurve);
            mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetStation.cameraAnchor.rotation, smoothCurve);
            
            yield return null;
        }

        // 4. We have arrived! Turn ON the specific UI panels for this station
        if (targetStation.uiPanelsToActivate != null)
        {
            foreach(var panel in targetStation.uiPanelsToActivate)
            {
                if (panel != null) panel.SetActive(true);
            }
        }
    }
}
