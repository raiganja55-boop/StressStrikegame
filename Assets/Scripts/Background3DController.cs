using System.Collections;
using UnityEngine;

public class Background3DController : MonoBehaviour
{
    [Header("3D Layer Assignments")]
    [Tooltip("Assign the background stadium arena root here.")]
    public Transform background3DPlane;
    [Tooltip("Assign the target punch bag or bot model here.")]
    public Transform targetPunchBag;

    [Header("Stadium Visual Effects")]
    public StadiumArenaGenerator stadium;
    public Color hitFlashColor = new Color(1f, 0.2f, 0.1f);
    public float hitFlashDuration = 0.15f;

    [Header("Kinetic Camera Shake Modifiers")]
    public float standardReturnSpeed = 8f;
    public float shiftDampingFactor = 0.15f;

    private Vector3 originalCameraPos;
    private Quaternion originalCameraRot;
    private Vector3 cameraTargetPos;
    private float slowMoTimer = 0f;
    private bool isSlowMoActive = false;
    private Material stadiumMat;
    private Color originalStadiumColor;

    private void Start()
    {
        originalCameraPos = transform.position;
        originalCameraRot = transform.rotation;
        cameraTargetPos = originalCameraPos;

        if (stadium != null)
        {
            MeshRenderer mr = stadium.GetComponent<MeshRenderer>();
            if (mr != null && mr.sharedMaterial != null)
            {
                stadiumMat = mr.material;
                originalStadiumColor = stadiumMat.color;
            }
        }
    }

    private void Update()
    {
        // Smoothly interpolate camera back to rest state using frame-rate independent snapping
        transform.position = Vector3.Lerp(transform.position, cameraTargetPos, Time.unscaledDeltaTime * standardReturnSpeed);
        cameraTargetPos = Vector3.Lerp(cameraTargetPos, originalCameraPos, Time.unscaledDeltaTime * (standardReturnSpeed * 0.5f));

        // Manage Slow Motion duration execution window
        if (isSlowMoActive)
        {
            slowMoTimer -= Time.unscaledDeltaTime;
            if (slowMoTimer <= 0f)
            {
                TerminateSlowMotionMode();
            }
        }
    }

    /// <summary>
    /// Computes explosive spatial kickbacks when an input signature matches.
    /// </summary>
    public void ApplyImpactForce(float forceX, float forceY, float forceZ)
    {
        Vector3 randomKick = new Vector3(forceX, forceY, -forceZ);
        cameraTargetPos = originalCameraPos + randomKick;

        if (stadiumMat != null)
        {
            StopCoroutine(nameof(StadiumHitFlash));
            StartCoroutine(StadiumHitFlash());
        }

        if (targetPunchBag != null)
        {
            // Simulate 3D physics wobble on target asset container
            targetPunchBag.localScale = new Vector3(1f + (forceX * 0.2f), 1f - (forceY * 0.2f), 1f);
            StartCoroutine(RestoreTargetBagScale());
        }
    }

    private IEnumerator StadiumHitFlash()
    {
        if (stadiumMat == null) yield break;
        stadiumMat.color = hitFlashColor;
        float elapsed = 0f;
        while (elapsed < hitFlashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            stadiumMat.color = Color.Lerp(hitFlashColor, originalStadiumColor, elapsed / hitFlashDuration);
            yield return null;
        }
        stadiumMat.color = originalStadiumColor;
    }

    private IEnumerator RestoreTargetBagScale()
    {
        float elapsed = 0f;
        while (elapsed < 0.2f)
        {
            elapsed += Time.unscaledDeltaTime;
            targetPunchBag.localScale = Vector3.Lerp(targetPunchBag.localScale, Vector3.one, elapsed / 0.2f);
            yield return null;
        }
        targetPunchBag.localScale = Vector3.one;
    }

    /// <summary>
    /// Scales time processing vectors down dynamically to freeze animations.
    /// </summary>
    public void TriggerSlowMotionMode(float duration)
    {
        isSlowMoActive = true;
        slowMoTimer = duration;
        Time.timeScale = 0.254f; // Drop game tick rates down heavily
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Retain physics calculation precision
    }

    private void TerminateSlowMotionMode()
    {
        isSlowMoActive = false;
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
        Debug.Log("SYSTEM LOG: SLOW MOTION MATRIX RESOLVED - RESUMING NORMAL METRICS");
    }
}