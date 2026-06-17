using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        Debug.Log("PLAY button pressed!");
        // Example: SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenOptions()
    {
        Debug.Log("OPTIONS button pressed!");
        // Add options menu logic here
    }

    public void OpenStore()
    {
        Debug.Log("STORE button pressed!");
        // Add store menu logic here
    }

    public void OpenLogin()
    {
        Debug.Log("LOGIN button pressed!");
        // Add login menu logic here
    }

    public void QuitGame()
    {
        Debug.Log("QUIT button pressed!");
        Application.Quit();
    }
}
