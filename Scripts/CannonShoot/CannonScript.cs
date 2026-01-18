using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CannonScript : MonoBehaviour
{
    [SerializeField] private Transform canonTransform;
    [SerializeField] private  GameObject bulletPrefab;
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private float TimeToWait;
    [SerializeField] private LineRenderer DrawLine;

    public static List<GameObject> bulletList = new List<GameObject>();
    public static bool stopshooting = true;
    [HideInInspector] public int bulletNum = 1;
    Vector3 worldPos;
    public static float xPos = 0;
    [SerializeField] TextMeshProUGUI touchTheScreenText;
    [SerializeField] private float maxLineDistance;
    //public static CannonScript Instance { get; private set; }
    void Start()
    {
        touchTheScreenText.enabled = true;
        xPos = 0;
    }

    void Update()
    {
        /*if(Touchscreen.current == null)
            return;*/
        
        if (CannonGameController.pauseGame)
            return;

        HitTheRay();
       
        if (stopshooting)
        {
            if(xPos != 0)
            {
                transform.position = new Vector2(xPos,transform.position.y);
            }
            return;
        }
        var touch = Touchscreen.current.primaryTouch;

        
        
        
            if (touch.press.isPressed)
            {
                

                
                 Vector2 TouchPos  = touch.position.ReadValue();

                  
                  worldPos = Camera.main.ScreenToWorldPoint(TouchPos);
                   worldPos.z = 0;
                if (worldPos.y > -3 && worldPos.x < 2f && worldPos.x > -2.1)
                {
                      drawLine();
                        LookAtFuction(worldPos);

                }
            }
            if (touch.press.wasReleasedThisFrame)
            {
                if(touchTheScreenText.enabled)
                {
                touchTheScreenText.enabled = false;
                }

                if (worldPos.y > -3 && worldPos.x < 2f && worldPos.x > -2.1)
                {
                StartCoroutine(shoot(bulletNum));
                DrawLine.positionCount = 0;
            }

            }
      



    }
    
    void LookAtFuction(Vector3 touchpos)
    {
        
        Vector3 pos = canonTransform.position - touchpos;
        float angle = Mathf.Atan2(pos.y,pos.x) * Mathf.Rad2Deg;
        canonTransform.rotation = Quaternion.Euler(0f,0f,angle + 180f);

    }
    
    public  IEnumerator shoot(int BulletNum)
    {
        stopshooting = true;
        int temp = BulletNum;
        while (temp > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, SpawnPoint.position, SpawnPoint.rotation);
            bulletList.Add(bullet);
            //Debug.Log(bulletList.Count);
            yield return new WaitForSeconds(TimeToWait);
            temp--;
        }

    }
    // LineRenderer
   public void drawLine()
    {
        DrawLine.positionCount = 2;
        DrawLine.SetPosition(0, SpawnPoint.position); // Z = 0 already
        // Force Z = 0 for 2D
        DrawLine.SetPosition(1, HitTheRay());
        //Debug.Log("Line Drawn");
        CancelInvoke("HideLine"); // Optional if you plan to hide later
    }

    void HideLine()
    {
        DrawLine.positionCount = 0; // Remove line
    }
    Vector2 HitTheRay()
    {
        Debug.DrawRay(SpawnPoint.position, SpawnPoint.right * maxLineDistance, Color.red);

        // Raycast with max distance
        RaycastHit2D hit = Physics2D.Raycast(SpawnPoint.position, SpawnPoint.right, maxLineDistance);

        Vector3 pos;

        if (hit.collider != null)
        {
            pos = hit.point; // hit something
        }
        else
        {
            // didn't hit, go max distance
            pos = SpawnPoint.position + SpawnPoint.right * maxLineDistance;
        }
        //Debug.Log("Ray Working");

        pos.z = 0;
        return pos;
    }
}
