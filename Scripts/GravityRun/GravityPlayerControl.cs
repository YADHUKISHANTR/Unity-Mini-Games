using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GravityPlayerControl : MonoBehaviour
{
    [Header("Player info")]
    [SerializeField] int gravity;
    bool isDown = true;
    Rigidbody2D rb;
    Animator animator;
    CircleCollider2D bc;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject particles;
    [SerializeField] GameObject Fire;
    [SerializeField] float playerSpeed;

    [Header("UI info")]
    [SerializeField] TextMeshProUGUI StartGameText;
    [SerializeField] TextMeshProUGUI Score;
    [HideInInspector]public bool gameStarted = false;
    [SerializeField] Image[] img;
    [SerializeField]float invincibilitySeconds = 2;
    [SerializeField] int total_lives = 3;

    [SerializeField] GameObject GameOverPanel;
    [SerializeField] GameObject GamePanel;


    [SerializeField] GameObject Explosion_prefab;
    bool Collided = false;
    SpriteRenderer sr;
    SpriteRenderer firesr;
    int times;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bc = GetComponent<CircleCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        firesr = Fire.GetComponent<SpriteRenderer>();

    }
    private void Start()
    {
        // changeGravity();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        if (gameStarted == false && Touchscreen.current.press.wasPressedThisFrame)
        {

            startGame();

            return;
        }
        if(!gameStarted)
        {
            return;
        }



        //Debug.Log(rb.linearVelocity.y);
        rb.linearVelocity = new Vector2(playerSpeed, rb.linearVelocityY);

        /*if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            isDown = !isDown;
            particles.SetActive(false);
           particles.SetActive(true);
            changeGravity();
        }*/

        if (Touchscreen.current.press.wasPressedThisFrame)
        {
            if (Time.timeScale == 0) return;
            isDown = !isDown;
            particles.SetActive(false);
            particles.SetActive(true);
            changeGravity();

        }
        animator.SetBool("Jump", CheckGround(isDown));
        Score.text = "Score: " + (int)gameObject.transform.position.x;
    }
    void startGame()
    {
        rb.gravityScale = gravity;
        gameStarted = true;
        StartGameText.enabled = false;
        animator.SetTrigger("Run");
        particles.SetActive(true);
        Fire.transform.rotation = Quaternion.Euler(0, 0, 0);
        Fire.transform.localPosition = new Vector3(-0.073f, 0.2223f, 0);

    }
    void changeGravity()
    {
        rb.linearVelocityY *= 0.43f;
        rb.gravityScale = (isDown ? gravity : -gravity);
        transform.localScale = new Vector3(transform.localScale.x, -(transform.localScale.y), transform.localScale.z);
        //rb.linearVelocity = new Vector2(0, (isDown ? 10 : -10));
        //rb.AddForce(new Vector2(0,(isDown?20 :-20)));
    }

    bool CheckGround(bool isdown)
    {
        Vector3 updown = isdown ? Vector2.down : Vector2.up;
        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center,bc.bounds.size,0,updown,0.1f,groundLayer);

        Debug.DrawLine(bc.bounds.center,
                    bc.bounds.center + updown  * (bc.bounds.extents.y + 0.1f),
                    Color.red);
      /*  if (hit.collider != null)
        {
            rb.gravityScale = 0;
            
        }*/
            return hit.collider != null; 
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if((collision.tag == "EndGame" || collision.tag == "Enemy") && !Collided)
        {
            if(collision.tag == "Enemy")
            {

                GameObject ob = Instantiate(Explosion_prefab);
                ob.transform.position = transform.position;
                Destroy(collision.gameObject);
            }

            times++;
            Collided = true;
            Debug.Log("Collided " + times);
            total_lives--;
            img[total_lives].enabled = false;
            


            if(total_lives <= 0)
            {
                gameStarted = false;
                gameObject.SetActive(false);
                GameOverPanel.SetActive(true);
                GamePanel.SetActive(false);
                return;
            }
            StartCoroutine(Invincibility());

        }
        
    }
    IEnumerator Invincibility()
    {
        StartCoroutine(invincibleAnimation());
        yield return new WaitForSeconds(invincibilitySeconds);
        Collided = false;

    }
    IEnumerator invincibleAnimation()
    {
        while (Collided)
        {
            yield return new WaitForSeconds(0.2f);
            /*Color c = sr.color;
            Color d = firesr.color;
            c.a = 0.5f;
            d.a = 0.5f;*/
            sr.color = Color.red;
            firesr.color = Color.red;   
            yield return new WaitForSeconds(0.2f);
           /* c = sr.color;
            d = firesr.color;
            c.a = 1f;
            d.a = 1f;*/
            sr.color = Color.white;
            firesr.color = Color.white;
        }
    }

}
