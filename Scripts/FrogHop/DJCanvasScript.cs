using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DJCanvasScript : MonoBehaviour
{
    [SerializeField] FrogPlayer FP;
    [SerializeField] TextMeshProUGUI Score;
    [SerializeField] TextMeshProUGUI score2;
    [SerializeField] TextMeshProUGUI Hs;
    [SerializeField] TextMeshProUGUI hs2;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] GameObject GamePanel;

    
    private void Start()
    {
        Time.timeScale = 1.0f;
        Score.text = "Score: 0";
        Hs.text = "Hs: " + PlayerPrefs.GetInt("DJhighScore");
    }
    private void Update()
    {
        Score.text = "Score: " + (int)(FP.GetScore());
        if(PlayerPrefs.GetInt("DJhighScore") < (int)FP.GetScore())
        {
            Score.color = Color.green;
            PlayerPrefs.SetInt("DJhighScore", (int)FP.GetScore());
            Hs.text = "Hs: " + PlayerPrefs.GetInt("DJhighScore");


        }

        
    }
    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1.1f;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoHome()
    {
        SceneManager.LoadScene(0);

    }
    public void gameEnded()
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            score2.text = Score.text;
            hs2.text = Hs.text;
            GamePanel.SetActive(false);
        }

    }
}
