using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GroundScript : MonoBehaviour
{
    private BoxCollider2D bx;
    [SerializeField] LayerMask blockLayer;
    [SerializeField] GameObject gameOverPanel;

    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI HsText;

    public static GroundScript Instance {  get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        bx = GetComponent<BoxCollider2D>();
    }
   
    public void GameEnded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(bx.bounds.center, bx.bounds.size, 0, Vector2.up,0.65f, blockLayer);
        if(hit.collider != null)
        {
            //CannonScript.stopshooting = true;
            CannonGameController.pauseGame = true;
            
            ScoreText.text = "Score : " + CannonGameController.Instance.getScore().ToString();
            HsText.text = "Hs" + PlayerPrefs.GetInt("CannonHS").ToString();
            //Time.timeScale = 0;
            gameOverPanel.SetActive(true);
        }
        
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);    
    }

}
