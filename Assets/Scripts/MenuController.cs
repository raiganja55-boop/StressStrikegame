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
        public Transform cameraAnchor; 
        public GameObject[] uiPanelsToActivate; 
        public bool keepPreviousPanelsOpen = false;
    }

    [Header("Menu Stations")]
    public MenuStation[] stations;

    [Header("All UI Panels")]
    public GameObject[] allUiPanels;

    private int currentStationIndex = 0;
    private bool isMoving = false;

    private void Start()
    {
        // Instantly snap to the first station on start, no animation!
        if (stations != null && stations.Length > 0)
        {
            SnapToStation(0);
        }
    }

    public void ToggleStation(int stationIndex)
    {
        if (isMoving) return;

        if (currentStationIndex == stationIndex && stationIndex != 0)
        {
            // Already here, go back Home
            StartCoroutine(MoveCameraRoutine(0));
        }
        else
        {
            // Go to new station
            StartCoroutine(MoveCameraRoutine(stationIndex));
        }
    }

    public void GoToStation(int stationIndex)
    {
        if (isMoving) return;
        StartCoroutine(MoveCameraRoutine(stationIndex));
    }

    // Instantly snaps everything without moving/animating (used for Start)
    private void SnapToStation(int targetIndex)
    {
        if (stations == null || targetIndex >= stations.Length) return;
        MenuStation targetStation = stations[targetIndex];

        // Turn off all panels
        if (allUiPanels != null)
        {
            foreach(var panel in allUiPanels)
            {
                if (panel != null) panel.SetActive(false);
            }
        }

        // Snap camera instantly
        if (targetStation.cameraAnchor != null && mainCamera != null)
        {
            mainCamera.transform.position = targetStation.cameraAnchor.position;
            mainCamera.transform.rotation = targetStation.cameraAnchor.rotation;
        }

        // Turn on correct panels instantly
        if (targetStation.uiPanelsToActivate != null)
        {
            foreach(var panel in targetStation.uiPanelsToActivate)
            {
                if (panel != null) panel.SetActive(true);
            }
        }

        currentStationIndex = targetIndex;
    }

    private IEnumerator MoveCameraRoutine(int targetIndex)
    {
        if (stations == null || targetIndex >= stations.Length) yield break;
        
        isMoving = true;
        MenuStation targetStation = stations[targetIndex];

        if (!targetStation.keepPreviousPanelsOpen && allUiPanels != null)
        {
            foreach(var panel in allUiPanels)
            {
                if (panel != null) panel.SetActive(false);
            }
        }

        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;

        float timeElapsed = 0;

        while (timeElapsed < 1)
        {
            timeElapsed += Time.deltaTime * transitionSpeed;
            float smoothCurve = Mathf.SmoothStep(0, 1, timeElapsed);

            if (targetStation.cameraAnchor != null && mainCamera != null)
            {
                mainCamera.transform.position = Vector3.Lerp(startPos, targetStation.cameraAnchor.position, smoothCurve);
                mainCamera.transform.rotation = Quaternion.Lerp(startRot, targetStation.cameraAnchor.rotation, smoothCurve);
            }

            yield return null;
        }

        if (targetStation.uiPanelsToActivate != null)
        {
            foreach(var panel in targetStation.uiPanelsToActivate)
            {
                if (panel != null) panel.SetActive(true);
            }
        }

        currentStationIndex = targetIndex;
        isMoving = false;
    }
}
