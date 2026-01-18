using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
public class Rope_Verlet : MonoBehaviour
{
    Vector2 AnchorPoint;
    [SerializeField] TextMeshProUGUI velocityofplayer;
    [Header("Player Stuff")]
    [SerializeField] Transform player;
   
    [SerializeField] Material mat;
    [SerializeField] Vector2 playerForce;
    [SerializeField]
    [Tooltip("This is to check the min speed of player less than this threshold so that we can add a foce on x dir to swing again")]
    float MinVelocityThreshold;

    Vector2 playerCurrentPos;
    Vector2 playerPrevPos;
    float playerMaxDistance;
    float stationaryTimePassed;
    [Header("Rope stuff")]
    [SerializeField]
    [Tooltip("Maximum Distance between two points of lines")]
    float maxDistance;
    int numberofpoints = 0;
    

    //[SerializeField] Vector2 ExternalForces;
    [SerializeField] Vector2 Force;
    [SerializeField]
    [Tooltip("It is the speed for rendering line ok")]
    float LineCreationSpeed = 20f;

    [SerializeField]
    LayerMask buildingLayer;

    LineRenderer line;
    Vector2[] prevPos;
    Vector2[] currentpos;

    int maxCapacity = 500;
    bool created = false;


    float visiblePoints = 0;

    [Header("Other stuff")]
    [SerializeField] Transform Ground;
    float floorY;
    private void Awake()
    {
        line = new LineRenderer();
        line = gameObject.AddComponent<LineRenderer>();

        prevPos = new Vector2[maxCapacity];
        currentpos = new Vector2[maxCapacity];

    }
    private void Start()
    {
        floorY = Ground.position.y; 

        playerCurrentPos = player.position;
        playerPrevPos = player.position;
        for (int i = 0; i < numberofpoints; i++)
        {
            currentpos[i] = player.position;
            prevPos[i] = player.position;
        }


        line.material = mat;
        line.startWidth = 0.04f;
        line.endWidth = 0.04f;
        line.sortingOrder = 2;
        //createLine();

    }

    void updateLine()
    {
        if (numberofpoints == 0)
            return;

        visiblePoints += Time.deltaTime * LineCreationSpeed;
        int count = Mathf.Clamp((int)visiblePoints, 0, numberofpoints);

        line.positionCount = count;

        // First point (player)
        //line.SetPosition(0, player.position);

        // Remaining visible rope
        for (int i = 0; i < count; i++)
        {
            line.SetPosition(i, currentpos[i]);
        }
    }

    private void FixedUpdate()
    {
        velocityofplayer.text = "Velocity x: " + ((int)((playerCurrentPos.x - playerPrevPos.x)*100)).ToString() + "Velocity y: " + ((int)((playerCurrentPos.y - playerPrevPos.y) * 100)).ToString();
        if ((playerCurrentPos - playerPrevPos).magnitude < MinVelocityThreshold && numberofpoints != 0)
        {
            stationaryTimePassed += Time.deltaTime;
            if(stationaryTimePassed > 2 && playerCurrentPos.y < AnchorPoint.y)
            {
                //Debug.Log("applyforce");
                playerPrevPos.x = playerPrevPos.x - 0.13f;
                stationaryTimePassed = 0;
            }
            
            //Debug.Log(stationaryTimePassed);
        }
        else
        {
            stationaryTimePassed = 0;
        }

            for (int i = 0; i < numberofpoints; i++)
            {
                verletSimulation(ref currentpos[i], ref prevPos[i], Force);

            }
        verletSimulation(ref playerCurrentPos, ref playerPrevPos, playerForce); ///////////////////////// here

        player.position = playerCurrentPos;
        playerconstraint();

        for (int v = 0; v < 5; v++)
        {
            for (int i = 0; i < numberofpoints - 1; i++)
            {
                fixDistance(ref currentpos[i], ref currentpos[i + 1]);
            }

            if (numberofpoints > 1)
            {

                fixPlayerDistance(ref playerCurrentPos, ref AnchorPoint,playerMaxDistance);
                gravityUpdate();
            }


            if (numberofpoints != 0)
            {
                currentpos[0] = player.position;
                currentpos[numberofpoints - 1] = AnchorPoint;
            }

        }
    }
    
    void gravityUpdate()
    {
        if(playerCurrentPos.x < AnchorPoint.x && playerCurrentPos.y < playerPrevPos.y)
        {
            playerForce.y = -18f;
        }
        else
        {
            playerForce.y = -9f;
        }
    }
    private void Update()
    {

        if (Touchscreen.current.press.wasPressedThisFrame)
        {
            if (created)
            {
                RemovePoints();
            }
            else
            {
                var touch = Touchscreen.current.primaryTouch;
                if(isBuilding(touch))
                {
                    
                    spawnpoints();

                }
                
            }

            created = !created;
        }


    }
    bool isBuilding(TouchControl touch)
    {
        AnchorPoint = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(AnchorPoint, Vector2.zero,0f,buildingLayer);
        if(hit.collider !=  null)
        {
            Debug.Log("found building");
            return true;

        }
        return false;
    }
    private void LateUpdate()
    {
        updateLine();

    }


    void spawnpoints()
    {
        Vector2 difference = AnchorPoint - (Vector2)player.position;

        float totalLength = difference.magnitude;

        
        int numberofpointsneeded = (int)(totalLength / maxDistance);// + 1;

        if (numberofpointsneeded <= 1) numberofpointsneeded = 2;

    
        for (int i = 0; i < numberofpointsneeded; i++)
        {
            numberofpoints++;

            float t = (float)i / (numberofpointsneeded - 1);

            Vector2 pos;
            pos.x = player.position.x + t * (AnchorPoint.x - player.position.x);
            pos.y = player.position.y + t * (AnchorPoint.y - player.position.y);

            currentpos[numberofpoints - 1] = pos;
            prevPos[numberofpoints - 1] = pos;
        }

        for (int i = 0; i < numberofpoints; i++)
        {
            prevPos[i] = currentpos[i];
        }

        //playerPrevPos = playerCurrentPos;

        //playerForce.x = 50f;

        line.positionCount = numberofpoints;
        visiblePoints = 1;

        playerMaxDistance = (playerCurrentPos - AnchorPoint).magnitude;

    }

    void RemovePoints()
    {

        line.positionCount = 1;
        for (int i = 0; i < numberofpoints; i++)
        {
            currentpos[i] = player.position;
            prevPos[i] = player.position;
        }
        numberofpoints = 0;
        visiblePoints = 0;
        playerForce.y = -9f;

        //rps.switchHand = !rps.switchHand;
        
        //increase player x velocity
        //playerPrevPos = playerCurrentPos - swingStrength *(playerCurrentPos - playerPrevPos); ;
    }

    void fixDistance(ref Vector2 pos0, ref Vector2 pos1)
    {
        Vector2 currentDistance = pos1 - pos0;
        float distance = currentDistance.magnitude;

        if (distance == 0f) return;

        float error = distance - maxDistance;

        float percentage = (error / distance) / 2;

        Vector2 offset = currentDistance * percentage;

        pos0 += offset;
        pos1 -= offset;

    }

    void fixPlayerDistance(ref Vector2 pos0, ref Vector2 pos1,float maxDist)
    {
        Vector2 currenDistance = pos1 - pos0; 
        float distance = currenDistance.magnitude; 
        if(distance == 0f) return;
        float error = distance - maxDist;

        float percentage = (error / distance) / 2;
        Vector2 offset = currenDistance * percentage;
        pos0 += offset;
    }
    void verletSimulation(ref Vector2 currentPos, ref Vector2 prepos, Vector2 force)
    {
        Vector2 accelaration = force;
        Vector2 velocity = currentPos - prepos;
        prepos = currentPos;
        currentPos += velocity + (accelaration * Time.fixedDeltaTime * Time.fixedDeltaTime);
    }
    void playerconstraint()
    {
        if(playerCurrentPos.y < floorY)
        {
            float velocityY = playerCurrentPos.y - playerPrevPos.y;

            playerCurrentPos.y = floorY;

            float bounce = 0.7f; // 0 = no bounce, 1 = perfect elastic
            playerPrevPos.y = playerCurrentPos.y + velocityY * bounce;
        }
    }
}