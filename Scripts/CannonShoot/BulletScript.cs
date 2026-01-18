using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] private float Speed;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    private void Start()
    {
        rb.linearVelocity = transform.right * Speed;

        
        Destroy(gameObject, 20f);
    }
   private void FixedUpdate()
    {
       
        if (rb.linearVelocityY >= -0.14f && rb.linearVelocityY <= 0.14f && rb.linearVelocityX != 0)
        {
            rb.AddForce(new Vector2(0, -0.02f));
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {


            rb.linearVelocity = Vector2.zero;
            //CannonScript.bulletList.Remove(gameObject);
            Destroy(gameObject, 0.2f);
            /*if (CannonScript.bulletList.Count <= 0)
            {

                CannonGameController.MoveTheBlocks = true;
                //Vector3 worldpos = Camera.main.ScreenToWorldPoint(transform.position);
                CannonScript.xPos = transform.position.x;
                CannonScript.stopshooting = true;
            }*/
            //Debug.Log(CannonScript.bulletList.Count);
            //Debug.Log(CannonScript.bulletList);
            //transform.parent = null;

        }
    }
    private void OnDestroy()
    {
        CannonScript.bulletList.Remove(gameObject);
        if (CannonScript.bulletList.Count <= 0)
        {

            CannonGameController.MoveTheBlocks = true;
            //Vector3 worldpos = Camera.main.ScreenToWorldPoint(transform.position);
            CannonScript.xPos = transform.position.x;
            CannonScript.stopshooting = true;
        }
    }
}
