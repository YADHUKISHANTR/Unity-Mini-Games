using UnityEngine;
using UnityEngine.InputSystem;

public class FrogPlayer : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;

    [SerializeField] float JumpHeight = 8f;
    [SerializeField] float MoveSpeed = 5f;
    [SerializeField] Camera mainCam;

    [SerializeField] DJGameController dJGameController;
    [SerializeField] DJCanvasScript djc;
    float highestY; // to store the highest point reached
    //int moveVal = 0;

    float toppest;

    float score = 0;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        highestY = mainCam.transform.position.y;
    }
    private void Start()
    {
        toppest = mainCam.transform.position.y + mainCam.orthographicSize;

    }
    public float GetScore()
    {
        return score * 10;
    }
    private void Update()
    {
       /* if (Keyboard.current.leftArrowKey.isPressed)
            moveVal = -1;
        else if (Keyboard.current.rightArrowKey.isPressed)
            moveVal = 1;
        else
            moveVal = 0;*/

        if(Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }
        else
        {
            Debug.Log("Accelerometer not avaliable");
        }

            HandleCameraFollow();
        ChangeDir();

        if(transform.position.y > score)
        {
            score = transform.position.y;
            
        }
        
       
    }

    void FixedUpdate()
    {
        // bool isOnGround = HitCollider();
        // anim.SetBool("OnAir", !isOnGround);

        var acc = Accelerometer.current;

        if (acc != null && acc.enabled)
        {
            Vector3 val = acc.acceleration.ReadValue();
            Vector2 tilt = new Vector2(val.x, val.y);
            Move(tilt.x);
        }

        

       
    }

    private void Move(float mov)
    {
        rb.linearVelocity = new Vector2(mov * MoveSpeed * Time.deltaTime, rb.linearVelocity.y);
        if (mov != 0)
            transform.localScale = new Vector3(Mathf.Sign(mov) * 0.6f, transform.localScale.y, transform.localScale.z);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpHeight);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            if(rb.linearVelocity.y <= 0.01f)
            {
                Jump();
                anim.SetTrigger("OnAir");
            }
            
            
            //Debug.Log("Is On Ground");
        }
       
        
       
    }
    void HandleCameraFollow()
    {
       
        // Track player’s highest Y position
        if (transform.position.y > highestY)
        {
            /*float top = mainCam.transform.position.y + mainCam.orthographicSize;
            if(top > toppest + 1.8f)
            {
                dJGameController.SpawnPlatform();
                toppest = top;
            }*/
            highestY = transform.position.y;
            
        }
        mainCam.transform.position = new Vector3(mainCam.transform.position.x, highestY, mainCam.transform.position.z);
           
    }
    
    void ChangeDir()
    {
        float height = mainCam.orthographicSize * 2f;
        float width = height * mainCam.aspect;
        float left = mainCam.transform.position.x - width / 2f;
        float right = mainCam.transform.position.x + width / 2f;

        if(transform.position.x < left)
        {
            transform.position = new Vector2(right,transform.position.y);
        }
        else if(transform.position.x > right)
        {
            transform.position = new Vector2(left, transform.position.y);
        }

        if(transform.position.y < mainCam.transform.position.y - height/2)
        {
            //Debug.Log("GameEnded");
            Destroy(gameObject);
        }
    }
    public void DoubleSpeed()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, JumpHeight * 1.5f);
    }
    private void OnDestroy()
    {
        djc.gameEnded();
    }
}
