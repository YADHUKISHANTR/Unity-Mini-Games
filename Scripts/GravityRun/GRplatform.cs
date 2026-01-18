using UnityEngine;

public class GRplatform : MonoBehaviour
{
    BoxCollider2D bc;
    
    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        
    }
    void Update()
    {
        if(transform.position.x + bc.bounds.size.x <= Camera.main.transform.position.x)
        {
            Destroy(gameObject,0.6f);
        }
    }
    
}
