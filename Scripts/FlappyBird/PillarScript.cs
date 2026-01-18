using Unity.VisualScripting;
using UnityEngine;

public class PillarScript : MonoBehaviour
{
    [SerializeField] private GameObject[] pillars;
    [SerializeField] private float speed;
    private Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }
    private void Start()
    {
        Debug.Log(cam.orthographicSize);
        for (int i = 0; i < pillars.Length; i++)
        {
            float rnd = Random.Range(-cam.orthographicSize + 2.5f, cam.orthographicSize - 2f);
            pillars[i].transform.position = new Vector3(pillars[i].transform.position.x,
                                                    rnd,
                                                    pillars[i].transform.position.z);
        }
    }
    private void Update()
    {
        if(BirdScript.gamestarted)
        {
            Move();
        }
        
    }
    private void Move()
    {
        for (int i = 0; i < pillars.Length; i++)
        {
            
            pillars[i].transform.Translate(-speed * Time.deltaTime, 0, 0);
            if (pillars[i].transform.position.x <= -6.25)
            {
                float rnd = Random.Range(-cam.orthographicSize + 2.5f, cam.orthographicSize - 2f);
                pillars[i].transform.position = new Vector3(8.10f,
                                                    rnd,
                                                    pillars[i].transform.position.z);
            }

        }
    }
}
