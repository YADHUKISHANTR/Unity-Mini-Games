using UnityEngine;

public class Parallax : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;

    GameObject[] background;
    Material[] mats;

    [Header("Global Parallax Speed")]
    [Range(0.01f, 0.1f)]
    public float parallaxSpeed = 0.02f;

    [Header("Layer Speed Multipliers (Near Far)")]
    public float[] layerSpeedMultiplier = { 1.0f, 0.7f, 0.4f };

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int count = transform.childCount;

        background = new GameObject[count];
        mats = new Material[count];

        for (int i = 0; i < count; i++)
        {
            background[i] = transform.GetChild(i).gameObject;
            mats[i] = background[i].GetComponent<Renderer>().material;
        }
    }

    void LateUpdate()
    {
        float distance = cam.position.x - camStartPos.x;
        transform.position = new Vector3(cam.position.x,transform.position.y,transform.position.z);
        for (int i = 0; i < background.Length; i++)
        {
            float speed = parallaxSpeed * layerSpeedMultiplier[i];
            mats[i].SetTextureOffset("_MainTex", new Vector2(distance * speed, 0f));
        }
    }
}
