using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [Header("Transition Settings")]
    [Tooltip("How long the fade takes in seconds.")]
    public float fadeDuration = 1f;
    [Tooltip("The color of the fade screen.")]
    public Color fadeColor = Color.black;

    private Canvas transitionCanvas;
    private Image fadeImage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateTransitionUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateTransitionUI()
    {
        // Create Canvas dynamically
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform);
        transitionCanvas = canvasObj.AddComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.sortingOrder = 999; // Render on top of everything else

        // Add CanvasScaler to make it fit any screen
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Create Image dynamically
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);

        // Stretch image to fill canvas completely
        RectTransform rectTransform = fadeImage.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        // Disable raycasts by default so it doesn't block UI when invisible
        fadeImage.raycastTarget = false;
    }

    /// <summary>
    /// Call this method to load a scene by name with a fade transition.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    /// <summary>
    /// Call this method to load a scene by build index with a fade transition.
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(TransitionToScene(sceneIndex));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        // Fade to black (or chosen color)
        yield return StartCoroutine(Fade(1f));
        
        // Load the new scene
        SceneManager.LoadScene(sceneName);
        
        // Fade back to transparent
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator TransitionToScene(int sceneIndex)
    {
        // Fade to black (or chosen color)
        yield return StartCoroutine(Fade(1f));
        
        // Load the new scene
        SceneManager.LoadScene(sceneIndex);
        
        // Fade back to transparent
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        fadeImage.raycastTarget = true; // Block UI interactions while fading
        
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        // Ensure it ends perfectly at the target alpha
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, targetAlpha);
        
        if (targetAlpha == 0f)
        {
            fadeImage.raycastTarget = false; // Allow UI interactions again once faded out
        }
    }
}
