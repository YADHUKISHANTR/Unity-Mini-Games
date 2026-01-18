using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BlockPos : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("objects")]
    [SerializeField] private Transform[] positions;
    [SerializeField] private GameObject[] blocks;
    GameObject BlockTouched = null;
    [SerializeField] private float Speed;
    [SerializeField] private GameObject particle;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI hs;
    [SerializeField] private TextMeshProUGUI completed;
    //[Header("Canvas")]
    // [SerializeField] private Button shuffle;
    //[SerializeField] private GameObject pauseMenu;
    
    float count = 0;
    bool gameStarted = false;
    bool gameEnded = false;
    private int emptyIndex;
    private Vector2 touchpos;
    private void Awake()
    {

        for (int i = 0; i < blocks.Length; i++)
        {
            positions[i].position = blocks[i].transform.position;
        }
    }
    void Start()
    {
        //PlayerPrefs.DeleteKey("HighScore");
        if((PlayerPrefs.GetInt("HighScore") == 0))  
        {
            PlayerPrefs.SetInt("HighScore", int.MaxValue);


        }
        if (PlayerPrefs.GetInt("HighScore") != int.MaxValue)
        {
            hs.text = "HS: " + PlayerPrefs.GetInt("HighScore").ToString() + "s";

        }
        else
        {
            hs.text = "HS: ";
        }

        ShufflePuzzle(100);
    }
    void ShufflePuzzle(int shuffleMoves)
    {
        // assume last slot is empty
        emptyIndex = positions.Length - 1;

        System.Random rnd = new System.Random();

        for (int i = 0; i < shuffleMoves; i++)
        {
            List<int> neighbors = GetNeighbors(emptyIndex);
            int swapIndex = neighbors[rnd.Next(neighbors.Count)];

            // swap block position with empty slot
            GameObject blockToMove = null;
            for (int j = 0; j < blocks.Length; j++)
            {
                if (blocks[j].transform.position == positions[swapIndex].position)
                {
                    blockToMove = blocks[j];
                    break; // stops at first match
                }
            }
            if (blockToMove != null)
            {
                blockToMove.transform.position = positions[emptyIndex].position;
                
            }

            emptyIndex = swapIndex;
        }
    }
    

    List<int> GetNeighbors(int index)
    {
        List<int> neighbors = new List<int>();
        int gridSize = 4; // 4x4 puzzle
        int row = index / gridSize;
        int col = index % gridSize;

        if (row > 0) neighbors.Add(index - gridSize); // up
        if (row < gridSize - 1) neighbors.Add(index + gridSize); // down
        if (col > 0) neighbors.Add(index - 1); // left
        if (col < gridSize - 1) neighbors.Add(index + 1); // right

        return neighbors;
    }
    /*void SwapToPos()
    {
       
        List<int> neigbour = GetNeighbors(emptyIndex);
        int swapIndex = emptyIndex;
        if(BlockTouched != null)
        {
            for(int i = 0; i < neigbour.Count;i++)
            {
                if(BlockTouched.transform.position == positions[neigbour[i]].position)
                {
                    emptyIndex = neigbour[i];
                    BlockTouched.transform.position = positions[swapIndex].position;

                }
            }
        }

    }*/

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(0);
        }
        if(gameEnded) return;
        MoveObject();
        if(gameStarted)
           StartTime();
    }
    void MoveObject()
    {
        if(Touchscreen.current == null) return;
        var touch = Touchscreen.current.primaryTouch;

        if(touch.press.wasPressedThisFrame)
        {
            gameStarted = true;
            touchpos = touch.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(touchpos);
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if(hit != null && hit.CompareTag("block"))
            {
                BlockTouched = hit.gameObject;
            }
        }

        if (touch.press.isPressed && BlockTouched != null)
        {
            Vector2 currentPos = touch.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(currentPos);
            worldPos.z = 0;

            BlockTouched.transform.position =
                Vector3.MoveTowards(BlockTouched.transform.position,
                worldPos,
                Time.deltaTime * Speed);
        }
        
        if(touch.press.wasReleasedThisFrame && BlockTouched != null)
        {
            SnapToPos();
            CheckWin();
            BlockTouched = null;
        }
    }
    void EndGame()
    {
        if(PlayerPrefs.GetInt("HighScore") > count)
        {
            PlayerPrefs.SetInt("HighScore", (int)count);
            hs.text = "HS: " + ((int)count).ToString() + " s";
        }
        gameEnded = true;
        completed.enabled = true;
        foreach (var item in blocks)
        {
            item.SetActive(false);
        }
        particle.SetActive(true);


    }
    void SnapToPos()
    {
        for(int i = 0; i < blocks.Count();i++)
        {
        
            Transform nearest = GetNearestSlot(blocks[i].transform.position);
            blocks[i].transform.position = nearest.position;
           
        }
        
    }
    Transform GetNearestSlot(Vector2 blockPos)
    {
        Transform nearest = positions[0];
        float minDist = Vector3.Distance(blockPos, positions[0].position);

        for (int i = 1; i < positions.Length; i++)
        {
            float dist = Vector3.Distance(blockPos, positions[i].position);
            if (dist < minDist)
            {
                nearest = positions[i];
                minDist = dist;
            }
        }
        return nearest;
    }
    void CheckWin()
    {
        bool solved = false;
        for (int i = 0; i < blocks.Count(); i++)
        {
            if (blocks[i].transform.position != positions[i].position)
            {
                solved = false;
                break;
            }
            solved = true;
        }
        if(solved)
        {
            Debug.Log("won");
            gameStarted = false;
            EndGame();
        }
    }
    void StartTime()
    {
       
        count += Time.deltaTime;
        time.text = "Time: " + ((int)count).ToString() + "s";

    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}

