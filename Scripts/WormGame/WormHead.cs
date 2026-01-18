using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;



public class WormHead : MonoBehaviour
{
    [SerializeField] WormGameController wgc;
    [SerializeField] Tilemap tilemap;
    [SerializeField] float TimeDelay;
    [SerializeField] GameObject wormBody;
    //[SerializeField] WormGameController gameController;
    Vector3Int cellPos,cellPos2;
    Vector3Int moveCell;
    [HideInInspector] public Vector3 prevpos;
    
    float TimePassed = 0;

    [HideInInspector] public List<GameObject> bodylist = new List<GameObject>();
    List<Vector3> preposlist = new List<Vector3>();


    private Vector2 startPos;
    private Vector2 endPos;
    private bool isSwiping = false;

    [SerializeField] private float minSwipeDistance = 45f;
    
    private void Start()
    {
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        cellPos = tilemap.WorldToCell(transform.position);
        cellPos2 = cellPos;
        bodylist.Add(gameObject);
        //transform.position = tilemap.GetCellCenterWorld(cellPos);
        
    }

    private void Update()
    {
        
        movement();
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            // Touch began
            if (touch.press.wasPressedThisFrame)
            {
                startPos = touch.position.ReadValue();
                isSwiping = true;
            }

            // Touch moved
            if (isSwiping && touch.press.isPressed)
            {
                endPos = touch.position.ReadValue();
            }

            // Touch ended  detect swipe
            if (touch.press.wasReleasedThisFrame)
            {
                DetectSwipe();
                isSwiping = false;
            }

            return;
        }

       
    }

    void DetectSwipe()
    {
        Vector2 swipe = endPos - startPos;

        if (swipe.magnitude < minSwipeDistance)
            return;

        if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
        {
            if (swipe.x > 0 && moveCell != new Vector3Int(-1, 0, 0))
            {
                moveCell = new Vector3Int(1, 0, 0);
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            else if(swipe.x < 0 && moveCell != new Vector3Int(1, 0, 0))
            {
                moveCell = new Vector3Int(-1, 0, 0);
                transform.rotation = Quaternion.Euler(0, 0, 90);

            }
        }
        else
        {
            if (swipe.y > 0 && moveCell != new Vector3Int(0, -1, 0))
            {
                moveCell = new Vector3Int(0, 1, 0);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if(swipe.y < 0 && moveCell != new Vector3Int(0, 1, 0))
            {
                moveCell = new Vector3Int(0, -1, 0);
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
    }
    void movement()
    {
        
        if (Keyboard.current.upArrowKey.wasPressedThisFrame && moveCell != new Vector3Int(0, -1, 0))
        {
            moveCell = new Vector3Int(0, 1, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Keyboard.current.downArrowKey.wasPressedThisFrame && moveCell != new Vector3Int(0, 1, 0))
        {
            moveCell = new Vector3Int(0, -1, 0);
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame && moveCell != new Vector3Int(1, 0, 0))
        {
            moveCell = new Vector3Int(-1, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame && moveCell != new Vector3Int(-1, 0, 0))
        {
            moveCell = new Vector3Int(1, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }
    private void LateUpdate()
    {
       
        TimePassed += Time.deltaTime;

        if (TimePassed >= TimeDelay)
        {
            TimePassed = 0f;
            Move(moveCell);
            MoveBody();
             
        }
    }
    public void TestMove()
    {
        Vector3Int newCellPos; ;
        for (int i = 0; i < 27; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                newCellPos = cellPos + new Vector3Int(1, 0, 0);
                Vector3 worldPos = tilemap.GetCellCenterWorld(newCellPos);
                transform.position = worldPos;
                //allPos.Add(worldPos);
                //Debug.Log(worldPos + ",,");

                cellPos = newCellPos;
                wgc.allPos.Add(worldPos);
            }
            cellPos = cellPos2;
            newCellPos = cellPos + new Vector3Int(0, -1, 0);
            cellPos = newCellPos;
            cellPos2 = cellPos;

        }
        cellPos = new Vector3Int(0, 1, 0);
    }

     void Move(Vector3Int direction)
    {
        Vector3Int newCellPos = cellPos + direction;
        prevpos = tilemap.GetCellCenterWorld(cellPos);

        Vector3 worldPos = tilemap.GetCellCenterWorld(newCellPos);
        transform.position = worldPos;
        prevpos = tilemap.GetCellCenterWorld(cellPos);
        if(preposlist.Count == 0) preposlist.Add(prevpos);
        preposlist[0] = prevpos;
        cellPos = newCellPos;

        for(int i = 1; i < bodylist.Count;i++)
        {

            if (bodylist[i].transform.position == transform.position)
            {
                Debug.Log("GameEnded");
                wgc.Gameover();
            }
        }
        
     }
  
    public void createWormBody()
    {
        GameObject gb = Instantiate(wormBody);
        bodylist.Add(gb);
        gb.transform.position = prevpos;
        preposlist.Add(new Vector3());
    }

    private void MoveBody()
    {
        for (int i = 0; i < bodylist.Count - 1; i++)
        {
            preposlist[i + 1] = bodylist[i + 1].transform.position;
            bodylist[i+1].transform.position = preposlist[i];
        }
        
        
    }
}
