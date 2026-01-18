using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CannonGameController : MonoBehaviour
{
    [Header("Walls")]
    [SerializeField] Transform ground;
    [SerializeField] Transform canon;
    [SerializeField] Transform roof;
    
    [Header("Blocks")]
    [SerializeField] GameObject BlockPrefab;
    [SerializeField] float BlockSpeed;
    [SerializeField] Transform BlockObject;
   
    public static List<GameObject> blocks = new List<GameObject>();
    [SerializeField] float TimePass;
    public static bool MoveTheBlocks = false;

    public static Transform canonpos;

    float minNum = 1;
    float maxNum = 5;
    float DeleteRandom = 50;

     [Header("Canvas")]
    [SerializeField]float TimeScale;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI HsText;
    int Score = 0;
    bool IncreaseTime;
    public static bool pauseGame = false;

    public static CannonGameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        
        resetgame();
        pauseGame = false;
        AllignWalls();
        SpawnBlocks();
        StartCoroutine(moveBlocks());
        HsText.text = "Hs : " + PlayerPrefs.GetInt("CannonHS").ToString();
    }
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            resetgame();
            SceneManager.LoadScene(0);
        }
        if (pauseGame)
            return;
        
        if (MoveTheBlocks)
        {
            SpawnBlocks();
            StartCoroutine(moveBlocks());
            MoveTheBlocks = false;
        }
        if(IncreaseTime)
        {
            Time.timeScale += (Time.deltaTime/10);
            //Debug.Log(Time.timeScale);
        }

    }

    void AllignWalls()
    {
        Renderer GroundRd = ground.GetComponent<Renderer>();
        Renderer CanonRd = canon.GetComponent<Renderer>();
       
        Vector2 Bottompos = new Vector2(0, (Camera.main.transform.position.y - Camera.main.orthographicSize) - (GroundRd.bounds.size.y / 4f) + 1.3f);
        Vector2 Canonpos = new Vector2(0, Bottompos.y + (CanonRd.bounds.size.y /2f));

        ground.position = Bottompos;
        canon.position = Canonpos;
    }    
    //spawningBlocks
    void SpawnBlocks()
    {
        Renderer RoofRd = roof.GetComponent<Renderer>();
        
        Vector2 startpos = new Vector2(roof.position.x - (RoofRd.bounds.size.x/2) - 0.056f, roof.position.y);
        
        for (int i = 0; i < 11; i++)
        {
            GameObject blockClone = Instantiate(BlockPrefab);
            blockClone.transform.parent = BlockObject;
            Renderer BlockRd = blockClone.GetComponent<Renderer>();
            startpos.x += BlockRd.bounds.size.x - 0.01f;// + 0.05f;
            
            //deletingRandomly
            int rand = DifficultyModifier(0, 100);
            if (rand < DeleteRandom)
            {
                blocks.Remove(blockClone);
                Destroy(blockClone);
                continue;
            }
            
            
            blockClone.transform.position = startpos ;
            BlockScript blockscript= blockClone.GetComponent<BlockScript>();
            ControlBlock(blockscript);
            blocks.Add(blockClone);
           
            
        }
        
        
    }
    //Modifiying score
    void resetgame()
    {
        CannonScript.bulletList.Clear();
        blocks.Clear();
        MoveTheBlocks = false;
        CannonScript.stopshooting = true;
        CannonScript.xPos = 0;
        pauseGame = false;


    }
    private void ControlBlock(BlockScript script)
    {

        script.Number = DifficultyModifier(minNum,maxNum);
        
    }
    int DifficultyModifier(float minScoreModifier, float maxScoreModifier)
    {
        return (int)Random.Range(minScoreModifier, maxScoreModifier);
    }

    IEnumerator moveBlocks()
    {
        float currentTime = 0;
        while(currentTime < TimePass)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].transform.Translate(0, -BlockSpeed * Time.deltaTime, 0);
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        GroundScript.Instance.GameEnded();
        CannonScript.stopshooting = false;

        minNum += 0.8f;
        maxNum += 1.5f;

        //modifying if to delete the blocks
        if(DeleteRandom > 5)
        {
            DeleteRandom -= 0.8f;
        }
        //canon.position = canonpos.position;
    }
    public void fastForward()
    {
        Time.timeScale = TimeScale;
    }
    public void AddScore(int num)
    {
        Score+=num;
        if(PlayerPrefs.GetInt("CannonHS") < Score)
        {
            scoreText.color = Color.green;
            PlayerPrefs.SetInt("CannonHS", Score);
        }
        scoreText.text = "Score : " + Score.ToString();
    }
    public int getScore()
    {
        return Score;
    }
    public void Speedup()
    {
        Time.timeScale = 1.5f;
        IncreaseTime = true;
    }
    public void slowDown()
    {
        IncreaseTime = false;
        Time.timeScale = 1f;
       
    }
    
    
}
