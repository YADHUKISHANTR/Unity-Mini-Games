using UnityEngine;

public class GRmissile : MonoBehaviour
{
    Rigidbody2D rb;
    float width;
    float height;
    Camera cam;
    [SerializeField] float speed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    private void Start()
    {
        height = cam.orthographicSize;
        width = height * cam.aspect;
        rb.linearVelocity = new Vector2(-speed,rb.linearVelocityY);
    }
    private void Update()
    {
        
        float left = cam.transform.position.x - width;
        if(transform.position.x < left - 2)
        {
            Destroy(gameObject);
        }
    }
}
