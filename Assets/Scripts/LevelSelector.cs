using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    /// <summary>
    /// Loads a level by its scene name. Can be called from a UI Button's OnClick event.
    /// </summary>
    /// <param name="levelName">The exact name of the scene to load (e.g., "Level 1")</param>
    public void LoadLevel(string levelName)
    {
        // Try to use the transition manager if it exists for a smooth fade
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(levelName);
        }
        else
        {
            // Fallback to instant loading if transition manager is not in the scene
            SceneManager.LoadScene(levelName);
        }
    }
}
