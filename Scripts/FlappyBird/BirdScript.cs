
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BirdScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    
    [Header("BirdPhysics")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float rotationSpeed;
    [Header("texts")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI HighScoreText;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI score2;
    [SerializeField] private TextMeshProUGUI hs2;

    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;
   

    private float Score;
    public static bool gamestarted = false;
    bool flying = false;
    bool jump = false;
    bool gameEnded;
    Camera cam;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        jump = false;
        flying = false;
        gamestarted = false;
        gameEnded = false;
    }
    private void Start()
    {
        cam = Camera.main;
        rb.gravityScale = 0;
        scoreText.text = "score: 0";
        HighScoreText.text = "hs: " + PlayerPrefs.GetInt("FlappyBirdHighScore").ToString();
    }

    void Update()
    {
        
        if (gamestarted) 
        {

           transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            if (transform.position.y < -cam.orthographicSize || transform.position.y > cam.orthographicSize)
            {
                GameEnded();
            }
            if (gameEnded) return;
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                jump = true;
            }

            
        }
        else
        {
            startTheGame();
        }
    }
    private void FixedUpdate()
    {
        if (gameEnded) return;
        if (jump)
        {
            DoJump();
            jump = false;
        }
        
    }
    void DoJump()
    {
        animator.SetBool("fly", flying);
        flying = !flying;
        // Debug.Log(animator.GetBool("fly"));
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        transform.rotation = Quaternion.Euler(0, 0, 45);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Score")
        {
            Score++;
            scoreText.text = "score: " + Score.ToString();
            
        }
        else if(collision.tag =="EndGame")
        {
            gameEnded = true;
            //add hit sound here
        }
    }
    
   
    void GameEnded()
    {
        endPanel.SetActive(true);
        startPanel.SetActive(false);
        gameEnded = true;
        if (Score > PlayerPrefs.GetInt("FlappyBirdHighScore"))
        {
            PlayerPrefs.SetInt("FlappyBirdHighScore", (int)Score);
        }
        score2.text = "Score: " + Score.ToString();
        hs2.text = "High Score: " + (PlayerPrefs.GetInt("FlappyBirdHighScore")).ToString();


        Time.timeScale = 0;
        Debug.Log("GameEnded");
    }

    void startTheGame()
    {
       /* if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                rb.gravityScale = 2;
                animator.SetBool("switch", true);
                text.enabled = false;
                jump = true;
                gamestarted = true;
            }
        }*/
        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            rb.gravityScale = 2;
            animator.SetBool("switch", true);
            text.enabled = false;
            jump = true;
            gamestarted = true;
        }
    }
    
}
