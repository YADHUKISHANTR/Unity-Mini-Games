
using UnityEngine;

public class RopePlayerScript : MonoBehaviour
{
    [SerializeField] Transform GroundTr;
    float ground;

    [SerializeField] Vector2 Force;


    [SerializeField] Transform[] joints;

     Vector2[] CurrentPos;
     Vector2[] previousPos;

    float[] maxDistance = new float[10];


    [Header("LimbsSprite")]
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform body;
    [SerializeField] Transform leftLeg;
    [SerializeField] Transform rightLeg;
    [SerializeField] Transform ankhorPoint;

    //cam stuff
    Camera maincam;
    float hieght;
    float bottom;
    private void Awake()
    {
        CurrentPos = new Vector2[joints.Length];
        previousPos = new Vector2[joints.Length];
        //ankhorPoint.position = joints[3].position;
        maincam = Camera.main;
        
    }

    private void Start()
    {
        ground = GroundTr.position.y;
        hieght = maincam.orthographicSize;
        for(int i = 0; i < joints.Length; i++)
        {
            CurrentPos[i] = joints[i].position;
            previousPos[i] = CurrentPos[i];
        }
        setMaxd();
        
    }
    void setMaxd()
    {
        maxDistance[0] = (CurrentPos[0] - CurrentPos[1]).magnitude; //hand Left
        maxDistance[1] = (CurrentPos[2] - CurrentPos[3]).magnitude; //hand right
        maxDistance[2] = (CurrentPos[4] - CurrentPos[6]).magnitude; //leg Left
        maxDistance[3] = (CurrentPos[5] - CurrentPos[7]).magnitude; //leg right
        maxDistance[4] = (CurrentPos[0] - CurrentPos[2]).magnitude; // top chest
        maxDistance[5] = (CurrentPos[4] - CurrentPos[5]).magnitude; //bottom belly
        maxDistance[6] = (CurrentPos[0] - CurrentPos[4]).magnitude; // left side
        maxDistance[7] = (CurrentPos[3] - CurrentPos[5]).magnitude; //right side
        maxDistance[8] = (CurrentPos[0] - CurrentPos[5]).magnitude; // middle1
        maxDistance[9] = (CurrentPos[2] - CurrentPos[4]).magnitude; //middle2

    }
    void FixD()
    {
        fixDistence(ref CurrentPos[1], ref CurrentPos[0], maxDistance[0],false);
        fixDistence(ref CurrentPos[3], ref CurrentPos[2], maxDistance[1],true);
        fixDistence(ref CurrentPos[4], ref CurrentPos[6], maxDistance[2], false);
        fixDistence(ref CurrentPos[5], ref CurrentPos[7], maxDistance[3], false);
        fixDistence(ref CurrentPos[2], ref CurrentPos[0], maxDistance[4], false);
        fixDistence(ref CurrentPos[4], ref CurrentPos[5], maxDistance[5], false);
        fixDistence(ref CurrentPos[0], ref CurrentPos[4], maxDistance[6], false);
        fixDistence(ref CurrentPos[2], ref CurrentPos[4], maxDistance[7] - 0.2f, false);
        fixDistence(ref CurrentPos[0], ref CurrentPos[5], maxDistance[8] + 0.05f, false);
        fixDistence(ref CurrentPos[2], ref CurrentPos[5], maxDistance[9], false);

    }
    void setLimbs()
    {
        leftHand.position = CurrentPos[0];
        rightHand.position = CurrentPos[2];
        leftLeg.position = CurrentPos[4];
        rightLeg.position = CurrentPos[5];
        body.position = CurrentPos[2];

        LookAtJoint(CurrentPos[1],leftHand, 90f);
        LookAtJoint(CurrentPos[3], rightHand, 90f);

        LookAtJoint(CurrentPos[6], leftLeg, 90f);

        LookAtJoint(CurrentPos[7], rightLeg, 90f);

        LookAtJoint(CurrentPos[5], body, 90f);


    }

    private void Update()
    {
        bottom = maincam.transform.position.y - hieght;

        if(bottom >= ground - 1.8f || CurrentPos[3].y > maincam.transform.position.y)
        {
            maincam.transform.position = new Vector3(CurrentPos[3].x, CurrentPos[3].y, maincam.transform.position.z);

        }
        else
        {
            maincam.transform.position = new Vector3(CurrentPos[3].x, maincam.transform.position.y, maincam.transform.position.z);

        }

    }
    private void FixedUpdate()
    {
        CurrentPos[3] = ankhorPoint.position;
        for (int i = 0; i < joints.Length; i++)
        {
            VerletSimlulation(ref CurrentPos[i], ref previousPos[i]);
            constraints(ref CurrentPos[i],ref previousPos[i]);
            joints[i].position = CurrentPos[i];
        }
        
        for(int v = 0; v < 5; v++)
        {
           //if(!TouchedGround) => to unlimb limb
            FixD();
        }
        setLimbs();
    }

    void constraints(ref Vector2 currentpos,ref Vector2 previouspos)
    {
        if(currentpos.y < ground)
        {
            float velocity = currentpos.y - previouspos.y;
            float bounce = 1f;
            currentpos.y = ground;

            previouspos.y =currentpos.y + velocity * bounce;

        }
    }
    void fixDistence(ref Vector2 pos0,  ref Vector2 pos1,float maxd,bool isAnchor)
    {
        Vector2 difference = pos1 - pos0;
        float distance  = difference.magnitude;

        if (distance == 0) return;

        float error = distance - maxd;

        float percentage = (error / distance) / 2;

        Vector2 offset = difference * percentage;
       
        if (!isAnchor)
            pos0 += offset;
        //else pos0 = ankhorPoint.position;
        
        pos1 -= offset;
    }
    void VerletSimlulation(ref Vector2 currentpos, ref Vector2 previouspos)
    {
        Vector2 acceleration = Force;//* mass
        Vector2 velocity = currentpos - previouspos;
        previouspos = currentpos;
        currentpos += velocity + (acceleration * Time.fixedDeltaTime * Time.fixedDeltaTime);
    }

    private void LookAtJoint(Vector2 lookat,Transform obj,float rotationPercent)
    {
        Vector3 direction = lookat - (Vector2)obj.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationPercent);
    }

}
