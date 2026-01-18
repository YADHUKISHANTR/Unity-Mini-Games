
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;


public class WormGameController : MonoBehaviour
{
    [SerializeField] GameObject ApplePrefab;
    [SerializeField] Tilemap tilemap;
    [SerializeField] GameObject wormHead;

    WormHead wormheadscript;
    [HideInInspector] public Vector3Int spawnedpos;
    GameObject apple;
    public List<Vector3> allPos;
    [SerializeField]float minY,maxY,minX,maxX;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI scoreText2;
    [SerializeField] TextMeshProUGUI hs;
    [SerializeField] TextMeshProUGUI hs2;

    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject GameoverPanel;
    int score = 0;
    private void Awake()
    {
        allPos = new List<Vector3>();
       

    }
    private void Start()
    {
        Time.timeScale = 1;
        
        hs.text = "HS: " + PlayerPrefs.GetInt("WormHS").ToString();
        scoreText.text = "Score: " + score.ToString();
        wormheadscript = wormHead.GetComponent<WormHead>();

        wormheadscript.TestMove();
        spawnApple();
        
    }
    bool inside = false;
    private void Update()
    {
        for(int i = 0; i <  allPos.Count; i++)
        {
            inside = false;
            if(wormHead.transform.position == allPos[i])
            {
                inside = true;
                break;
            }
            
        }
        if(!inside)
        {
            Gameover();
        }
        if (apple != null) {

            if (wormHead.transform.position == apple.transform.position) {
               Destroy(apple);
                wormheadscript.createWormBody();
                score++;
                
                scoreText.text = "Score: " + score.ToString();
                if (score >= 351)
                {
                    //Here gameComplete
                    Gameover();
                }
                if (PlayerPrefs.GetInt("WormHS") < score)
                {
                    scoreText.color = Color.green;
                    PlayerPrefs.SetInt("WormHS", score);
                    hs.text = "Hs: " + score.ToString();
                }

             
                
                 spawnApple();

                
            }
        }
    }
    public void Gameover()
    {
        
        Debug.Log("GameEnded");
        
        gamePanel.SetActive(false);
        GameoverPanel.SetActive(true);
        scoreText2.text = scoreText.text;
        hs2.text = hs.text;
        Time.timeScale = 0;
    }
    public void spawnApple()
    {
        apple = Instantiate(ApplePrefab);
        List<Vector3> allowedItems = new List<Vector3>();
        bool exist = false;
        foreach(Vector3 item in allPos)
        {
            exist = false;
            foreach (GameObject i in wormheadscript.bodylist)
            {
                if (item == i.transform.position)
                {
                    //Debug.Log(item == i.transform.position);
                    exist = true;
                }
            }
            if (exist) continue;

            allowedItems.Add(item);
        }


        int applepos = Random.Range(0,allowedItems.Count);
        apple.transform.position = (allowedItems[applepos]);
       

    }
    
}
