using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GRgameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject[] platforms; // expect exactly 8
    [SerializeField] GameObject spikeyBall;
    [SerializeField] GameObject missile;
    [SerializeField] GameObject cautionSign;
    [SerializeField] Transform player;
    [SerializeField] GravityPlayerControl gpc;

    [Header("Background / Hue")]
    [SerializeField] float bgSpeed = 0.02f;
    [SerializeField] GameObject bg1;
    [SerializeField] GameObject bg2;
    [SerializeField] float hueSpeed = 0.1f;

    [Header("Spawn / Positioning")]
    // initial array must match platforms length (8)
    [SerializeField] float[] platformInitialPercent = { 100f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    [SerializeField] int maxBallsPerPlatform = 4;

    [Header("Difficulty (0 -> 1)")]
    [SerializeField][Range(0f, 1f)] float difficulty = 0f;
    [SerializeField] float difficultyIncreasePerSpawn = 0.05f; // increases after each spawn

    [Header("Ball spawn chance (0..1)")]
    [SerializeField] float ballBaseChance = 0.10f;
    [SerializeField] float ballMaxChance = 0.60f;

    [Header("Missile (caution sign) chance (0..1)")]
    [SerializeField] float missileBaseChance = 0.05f;
    [SerializeField] float missileMaxChance = 0.40f;

    // Internal
    Camera maincam;
    float camHeight;
    float camHalfWidth;
    float spawnPos;
    float lastSpawnX;
    float bottompos;
    float hue = 0f;
    List<GameObject> cautionSignList;

    // MIN / MAX percent tables (8 entries)
    // Per your values:
    // min:  15, 0, 0, 0, 0, 0, 0, 0
    // max: 100,15,15,15,15,10,10,10
    float[] minPercent = { 15f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
    float[] maxPercent = { 100f, 15f, 15f, 15f, 15f, 10f, 0f, 0f };



    private void Awake()
    {
        

        maincam = Camera.main;

        if (platforms == null || platforms.Length != 8)
            Debug.LogError("GRgameController: You must assign EXACTLY 8 platforms in the inspector.");

        // Ensure serialized initial array is the correct length; if not, fix it
        if (platformInitialPercent == null || platformInitialPercent.Length != 8)
        {
            platformInitialPercent = new float[8];
            platformInitialPercent[0] = 100f;
            for (int i = 1; i < 8; i++) platformInitialPercent[i] = 0f;
        }

        spawnPos = (platforms != null && platforms.Length > 0 && platforms[0] != null)
            ? platforms[0].transform.position.x
            : 0f;
    }

    IEnumerator Start()
    {

        while (Screen.width < Screen.height)   
            yield return null;
        yield return null;
        cautionSignList = new List<GameObject>();
        if (maincam == null) maincam = Camera.main;
        camHeight = maincam.orthographicSize;
        camHalfWidth = camHeight * maincam.aspect;
        setPositions();
        for (int i = 0; i < 2; i++)
        {
            spawnPlatform(0);
        }
        
    }

    void setPositions()
    {
        if (platforms != null && platforms.Length > 0 && platforms[0] != null)
        {
            SpriteRenderer sr = platforms[0].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                bottompos = (maincam.transform.position.y - camHeight) + sr.bounds.size.y;
                return;
            }
        }
        bottompos = maincam.transform.position.y - camHeight + 1f;
    }

    private void Update()
    {
        if (!gpc.gameStarted)
        {
            return;
        }
        UpdateCautionSigns();
    }

    private void FixedUpdate()
    {
        if(!gpc.gameStarted)
        {
            return;
        }
        // Spawn new platform when camera approaches last spawn (tweak the -10f as needed)
        if (maincam.transform.position.x > lastSpawnX - 10f)
        {
            int choice = SelectPlatformWithDifficulty();
            spawnPlatform(choice);

            // Increase difficulty after each platform spawn
            difficulty = Mathf.Clamp01(difficulty + difficultyIncreasePerSpawn);
        }

        if (gpc != null && gpc.gameStarted)
        {
            parallaxBg();
        }

        changeHue();
    }

    // ---------------------------------------------------------------
    // PLATFORM SELECTION USING DIFFICULTY (8 platforms)
    // ---------------------------------------------------------------
    int SelectPlatformWithDifficulty()
    {
        int n = 8;
        float[] weights = new float[n];

        for (int i = 0; i < n; i++)
        {
            float start = (i < platformInitialPercent.Length) ? platformInitialPercent[i] : 0f;
            float target;

            if (i == 0)
            {
                // Platform 0 decreases from initial -> min as difficulty -> 1
                target = minPercent[0];
                weights[i] = Mathf.Lerp(start, target, difficulty);
            }
            else
            {
                // Others increase from initial -> their max as difficulty -> 1
                target = maxPercent[i];
                weights[i] = Mathf.Lerp(start, target, difficulty);
            }

            // Clamp to min/max
            float clamped = Mathf.Clamp(weights[i], minPercent[i], maxPercent[i]);
            weights[i] = clamped;
        }

        // If sum is approximately zero (shouldn't happen), fallback to platform 0
        float total = 0f;
        for (int i = 0; i < n; i++) total += weights[i];
        if (total <= Mathf.Epsilon) return 0;

        float rnd = Random.Range(0f, total);
        float cumulative = 0f;
        for (int i = 0; i < n; i++)
        {
            cumulative += weights[i];
            if (rnd <= cumulative) return i;
        }

        return n - 1;
    }

    // ---------------------------------------------------------------
    // SPAWNING LOGIC
    // ---------------------------------------------------------------
    void spawnPlatform(int id)
    {
        if (id < 0 || id >= platforms.Length)
        {
            Debug.LogWarning("spawnPlatform: invalid id, using 0");
            id = 0;
        }

        GameObject spawn = Instantiate(platforms[id]);
        spawn.transform.position = new Vector2(spawnPos - 0.01f, bottompos);

        BoxCollider2D bc = spawn.GetComponent<BoxCollider2D>();
        float width = (bc != null) ? bc.bounds.size.x : 4f;
        lastSpawnX = spawnPos;
        spawnPos += width;

        // Attempt to spawn balls (each attempt has a probability)
        for (int i = 0; i < maxBallsPerPlatform; i++)
        {
            if (ShouldSpawnBall())
                spawnBall(spawn.transform);
        }

        // Missile (caution sign) spawn chance once per platform spawn
        if (ShouldSpawnMissile())
            spawnCautionsign();
    }

    bool ShouldSpawnBall()
    {
        float chance = Mathf.Lerp(ballBaseChance, ballMaxChance, difficulty);
        return Random.value < chance;
    }

    bool ShouldSpawnMissile()
    {
        float chance = Mathf.Lerp(missileBaseChance, missileMaxChance, difficulty);
        return Random.value < chance;
    }

    void spawnBall(Transform parent)
    {
        GameObject spawnedBall = Instantiate(spikeyBall);

        // top should be greater than bottom - use bottom..top in Random.Range
        float top = (maincam.transform.position.y + camHeight) - 2f;
        float bottom = (maincam.transform.position.y - camHeight) + 0.8f;
        if (bottom > top) { float t = top; top = bottom; bottom = t; } // safety

        float left = (maincam.transform.position.x + camHalfWidth);
        float right = (maincam.transform.position.x + (camHalfWidth * 3f)); // spawn ahead
        if (left > right) { float t = left; left = right; right = t; } // safety

        float x = Random.Range(left, right);
        float y = Random.Range(bottom, top);

        spawnedBall.transform.position = new Vector2(x, y);
        spawnedBall.transform.SetParent(parent);
    }

    void spawnCautionsign()
    {
        float right = maincam.transform.position.x + camHalfWidth;
        GameObject spawn2 = Instantiate(cautionSign);
        spawn2.transform.position = new Vector2(right, player.position.y);
        cautionSignList.Add(spawn2);
        StartCoroutine(missileCountDown(spawn2));
    }

    void spawnMissile(float ypos)
    {
        float top = (maincam.transform.position.y + camHeight) - 2f;
        float bottom = (maincam.transform.position.y - camHeight) + 0.8f;
        ypos = Mathf.Clamp(ypos, bottom, top);

        GameObject mis = Instantiate(missile);
        mis.transform.position = new Vector2(maincam.transform.position.x + (camHalfWidth * 2f), ypos);
    }

    IEnumerator missileCountDown(GameObject spawn)
    {
        // simple countdown (tweak counts/delay as desired)
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (spawn == null) yield break; // abort if removed
        }

        if (spawn != null)
        {
            spawnMissile(spawn.transform.position.y);
            cautionSignList.Remove(spawn);
            Destroy(spawn);
        }
    }

    // ---------------------------------------------------------------
    // CAUTION SIGN FOLLOWING PLAYER
    // ---------------------------------------------------------------
    void UpdateCautionSigns()
    {
        if (cautionSignList == null || cautionSignList.Count == 0) return;

        float camRightX = maincam.transform.position.x + camHalfWidth;
        float anchorX = camRightX - 0.8f;
        float speed = 5f;

        for (int i = cautionSignList.Count - 1; i >= 0; i--)
        {
            GameObject cs = cautionSignList[i];
            if (cs == null)
            {
                cautionSignList.RemoveAt(i);
                continue;
            }

            Vector2 pos = cs.transform.position;
            float newY = Mathf.Lerp(pos.y, player.position.y, speed * Time.deltaTime);
            cs.transform.position = new Vector2(anchorX, newY);
        }
    }

    // ---------------------------------------------------------------
    // EXTRA VISUAL FUNCTIONS
    // ---------------------------------------------------------------
    void changeHue()
    {
        hue += Time.deltaTime * hueSpeed;
        if (hue > 1f) hue -= 1f;
        Color newColor = Color.HSVToRGB(hue, 1f, 1f);
        if (bg1 != null) bg1.GetComponent<SpriteRenderer>().color = newColor;
        if (bg2 != null) bg2.GetComponent<SpriteRenderer>().color = newColor;
    }

    public void parallaxBg()
    {
        float hieght = maincam.orthographicSize * 2f;
        float width = hieght * maincam.aspect;
        float left = maincam.transform.position.x - (width / 2f);

        if (bg1 != null)
            bg1.transform.position = new Vector3(bg1.transform.position.x - bgSpeed, bg1.transform.position.y, bg1.transform.position.z);
        if (bg2 != null)
            bg2.transform.position = new Vector3(bg2.transform.position.x - bgSpeed, bg2.transform.position.y, bg2.transform.position.z);

        if (bg1 == null || bg2 == null) return;

        SpriteRenderer sr1 = bg1.GetComponent<SpriteRenderer>();
        SpriteRenderer sr2 = bg2.GetComponent<SpriteRenderer>();
        if (sr1 == null || sr2 == null) return;

        if (bg1.transform.position.x + (sr1.bounds.size.x / 2f) < left)
        {
            bg1.transform.position = new Vector3(bg2.transform.position.x + sr2.bounds.size.x, bg1.transform.position.y, bg1.transform.position.z);
        }
        if (bg2.transform.position.x + (sr2.bounds.size.x / 2f) < left)
        {
            bg2.transform.position = new Vector3(bg1.transform.position.x + sr1.bounds.size.x, bg2.transform.position.y, bg2.transform.position.z);
        }
    }

    
}
