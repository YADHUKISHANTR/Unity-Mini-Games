using UnityEngine;

public enum Platform {StaticPlatform, CloudPlatform, CrackingPlatform, MovingPlatform,DisappearingPlatform, SpikyPlatform,BouncingPlatform }

public class DJgroundScript : MonoBehaviour
{   
    public Platform platformType;
    [SerializeField] private float movingSpeed;

    Animator anim;
    BoxCollider2D bc;
    
    Vector2 windowSize;
    GameObject player;
    Camera mainCam;
    bool move_right = true;

    [HideInInspector] public int PlatformTypeNumber;
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        
    }
    private void Start()
    {

        platformType = (Platform)PlatformTypeNumber;
        if (anim == null)
        {
            Debug.LogWarning("Animator missing on " + gameObject.name);
            return;
        }
        //platformType = GetRandomEnumValue<Platform>();
       switch (platformType)
       {
            case Platform.StaticPlatform:
                staticPlatformOnStart();
                break;
            case Platform.CloudPlatform:
                cloudPlatformOnStart();

                break;
            case Platform.CrackingPlatform:
                crackingPlatformOnStart();

                break;
            case Platform.MovingPlatform:
                movingPlatformOnStart();

                break;
            case Platform.DisappearingPlatform:
                disappearingPlatformOnStart();

                break;
            case Platform.SpikyPlatform:
                  spikeyPlatformOnStart();    
                break;
            case Platform.BouncingPlatform:
                bouncingPlatformOnStart();
                break;
            default:
                Debug.LogWarning("Invalid choice");
                break;
       }
        player = GameObject.FindGameObjectWithTag("Player");
        
    }
   /* T GetRandomEnumValue<T>()
    {
        System.Array values = System.Enum.GetValues(typeof(T));
        int randomIndex = Random.Range(0, values.Length);
        return (T)values.GetValue(randomIndex);
    }*/
    void staticPlatformOnStart()
    {
         anim.SetInteger("SelectIdle", 0);
    }
    void cloudPlatformOnStart()
    {
        bc.isTrigger = true;
        anim.SetInteger("SelectIdle", 1);
    }
    void crackingPlatformOnStart()
    {
        anim.SetInteger("SelectIdle", 2);

    }
    void movingPlatformOnStart()
    {
        anim.SetInteger("SelectIdle", 3);
        mainCam = Camera.main;

    }
    void disappearingPlatformOnStart()
    {
        anim.SetInteger("SelectIdle", 4);

    }
    void spikeyPlatformOnStart()
    {
        anim.SetInteger("SelectIdle", 5);

    }

    void bouncingPlatformOnStart()
    {
        anim.SetInteger("SelectIdle", 6);
    }

    void Update()
    {
        CheckandDestroy();
        if(platformType == Platform.MovingPlatform)
        {
            MoveBlock();
        }
        else if(platformType == Platform.DisappearingPlatform)
        {
            DisappearingPlatform();
        }
        
    }
    
    private void CheckandDestroy()
    {
        windowSize.y = Camera.main.transform.position.y - Camera.main.orthographicSize;

        if (transform.position.y < windowSize.y)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && platformType == Platform.CloudPlatform && player.GetComponent<Rigidbody2D>().linearVelocity.y <= 0.01f)
        {
            Destroy(bc);
            //Debug.Log("Disappeared");
            anim.SetTrigger("Disappear");
            Destroy(gameObject,1f);
        }
      
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(player == null)
        {
            return;
        }
        if (collision.collider.tag == "Player" && player.GetComponent<Rigidbody2D>().linearVelocity.y <= 0.01f)
        {
            if (platformType == Platform.SpikyPlatform)
            {
                Destroy(collision.collider.gameObject);
                //Debug.Log("GameEnded");
            }
            else if (platformType == Platform.CrackingPlatform)
            {


                anim.SetTrigger("Break");
                Destroy(bc);
                Destroy(gameObject, 1f);


            }
            else if (platformType == Platform.BouncingPlatform)
            {
                player.GetComponent<FrogPlayer>().DoubleSpeed();
                //Debug.Log("This is working");
                anim.SetTrigger("Bounce");

            }
        }
       

    }
    void MoveBlock()
    {
        float height = mainCam.orthographicSize * 2f;
        float width = height * mainCam.aspect;

        float left = mainCam.transform.position.x - width / 2f;
        float right = mainCam.transform.position.x + width / 2f;
        if (transform.position.x <=  left + 0.4f)
        {
            move_right = true;
        }
        else if(transform.position.x >= right - 0.4f)
        {
            move_right = false;
        }
        if (move_right)
            transform.Translate(movingSpeed * Time.deltaTime, 0, 0);
        else transform.Translate(-movingSpeed * Time.deltaTime, 0, 0);
        
    }
    void DisappearingPlatform()
    {
       SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
       if (sprite.color.a < 0.4f)
       {
            bc.enabled = false;
       }
        else
        {
            bc.enabled = true;
        }
    }
    
    
}
