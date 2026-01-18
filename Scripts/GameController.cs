using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    
    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GoHome();
        }
    }
    public static void Restart()
    {
        BirdScript.gamestarted = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoHome()
    {
        SceneManager.LoadScene(0);
    }
    public void Pause()
    {
        
        Time.timeScale = 0;
    }
    public void resume()
    {
        Time.timeScale = 1;

    }
}
