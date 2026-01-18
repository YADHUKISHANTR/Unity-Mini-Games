using UnityEngine;

public class GRcameraScript : MonoBehaviour
{
    [SerializeField] Transform player;

    private void Update()
    {
        transform.position = new Vector3(player.position.x + 3f, transform.position.y,transform.position.z);
    }
}
