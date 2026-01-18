using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class DJGameController : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject platform;
    [SerializeField] Transform platformParent;
    [SerializeField]float maxHight = 2.4f;
    float SpawnNumberPercent = 80;
    float topy = -3.87f;

    //Platfor percentage
    float staticPlatformPercent = 100;
    float CloudPlatformPercent = 80; // not used yet, you can keep 0
    float CrackingPlatformPercent = 0;
    float MovingPlatformPercent = 0;
    float DisappearingPlatformPercent = 0;
    float SpikyPlatformPercent = 8; // not used
    float BouncingPlatformPercent = 0;

    //List<GameObject> platformArr = new List<GameObject>();


    
    Camera cam;

  
    private void Start()
    {
        cam = Camera.main;
        spawnPlatformsatStart();
    }
    private void Update()
    {
        if (topy < cam.transform.position.y + cam.orthographicSize)
        {
            SpawnPlatform();
        }
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(0);
        }
    }
    void spawnPlatformsatStart()
    {
        float top = cam.transform.position.y + cam.orthographicSize;

        do
        {
            SpawnPlatform(3);

        } while (topy > top);
        

    }
    
    public  void SpawnPlatform(int? spawnNumber = null)
    {
        
        float hieght = cam.orthographicSize * 2f;
        float width = hieght * cam.aspect;
        float left = cam.transform.position.x - width / 2f;
        float right = cam.transform.position.x + width / 2f;




        int SpawnNumber = 1;
        if (Random.Range(0, 100) < SpawnNumberPercent)
        {
            SpawnNumber = Random.Range(1, 3);
            
        }
        if(spawnNumber != null)
        {
            SpawnNumber = (int)spawnNumber;
            //Debug.Log("There is parameter");
        }
       /*else
        {
            //Debug.Log("There is no parameter");
        }*/

            GameObject Ps = null;
        topy += maxHight;
        float yval = RandomVal(topy - 1f, topy);
        float xval = RandomVal(left + 0.7f, right - 0.7f);
        for (int i = 0; i <= SpawnNumber;i++)
        {
            Ps = Instantiate(platform);
            Ps.GetComponent<DJgroundScript>().PlatformTypeNumber = GetRandomPlatformType();
            Ps.transform.position = new Vector2(xval, yval);
            xval = RandomVal(left + 0.7f, right - 0.7f);
            yval += RandomVal(0.7f,1.4f);
            Ps.transform.parent = platformParent;
            topy = Ps.transform.position.y;
        }
        int rand2 = Random.Range(0, 100);
        if(rand2 < CloudPlatformPercent)
        {
            spawnSpikeyandCloud(1);
        }
        if(rand2 < SpikyPlatformPercent)
        {
            spawnSpikeyandCloud(5);
        }
        PlatforTypePercent();
    }
    
    float RandomVal(float a, float b)
    {

        float val = Mathf.Round(Random.Range(a, b) * 10f) / 10f;
        return val;
    }


    public void PlatforTypePercent()
    {
        staticPlatformPercent = Mathf.Max(0, staticPlatformPercent - 0.7f);
        CrackingPlatformPercent = Mathf.Max(0, CrackingPlatformPercent + 0.5f);
        MovingPlatformPercent = Mathf.Max(0, MovingPlatformPercent + 0.3f);
        DisappearingPlatformPercent = Mathf.Max(0, DisappearingPlatformPercent + 0.3f);
        BouncingPlatformPercent = Mathf.Max(0, BouncingPlatformPercent + 0.3f);
        CloudPlatformPercent = Mathf.Max(0, CloudPlatformPercent - 0.1f);
        SpikyPlatformPercent = Mathf.Max(0, SpikyPlatformPercent + 0.2f);

        staticPlatformPercent = Mathf.Max(10, staticPlatformPercent);
        CrackingPlatformPercent = Mathf.Min(20, CrackingPlatformPercent);
        MovingPlatformPercent = Mathf.Min(30, MovingPlatformPercent);
        DisappearingPlatformPercent = Mathf.Min(30, DisappearingPlatformPercent);
        BouncingPlatformPercent = Mathf.Min(20, BouncingPlatformPercent);
        CloudPlatformPercent = Mathf.Max(60, CloudPlatformPercent);
        SpikyPlatformPercent = Mathf.Min(28, SpikyPlatformPercent);

        // Normalize so total = 100
        float total = staticPlatformPercent + CrackingPlatformPercent + MovingPlatformPercent
                      + DisappearingPlatformPercent + BouncingPlatformPercent;

        if (total > 0) // avoid division by zero
        {
            float factor = 100f / total;
            staticPlatformPercent *= factor;
            CrackingPlatformPercent *= factor;
            MovingPlatformPercent *= factor;
            DisappearingPlatformPercent *= factor;
            BouncingPlatformPercent *= factor;
        }

        if (SpawnNumberPercent > 10)
            SpawnNumberPercent -= 0.18f;
    }
    void spawnSpikeyandCloud(int PlatNumber)
    {
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        float left = cam.transform.position.x - width / 2f;
        float right = cam.transform.position.x + width / 2f;

        GameObject sp = Instantiate(platform);
        sp.GetComponent<DJgroundScript>().PlatformTypeNumber = PlatNumber;

        float xval = RandomVal(left + 0.4f, right - 0.4f);

        float topEdge = cam.transform.position.y + cam.orthographicSize;
        float yval = topEdge + RandomVal(0.5f, 2.0f);

        sp.transform.position = new Vector2(xval, yval);
    }


    public int GetRandomPlatformType()
    {
        // Weighted random based on normalized percentages
        float total = staticPlatformPercent + CrackingPlatformPercent + MovingPlatformPercent
                      + DisappearingPlatformPercent + BouncingPlatformPercent;

        float rand = Random.Range(0f, total);

        if (rand < staticPlatformPercent)
            return 0;
        else if (rand < staticPlatformPercent + CrackingPlatformPercent)
            return 2;
        else if (rand < staticPlatformPercent + CrackingPlatformPercent + MovingPlatformPercent)
            return 3;
        else if (rand < staticPlatformPercent + CrackingPlatformPercent + MovingPlatformPercent + DisappearingPlatformPercent)
            return 4;
        else
            return 6; // Bouncing
    }

}
