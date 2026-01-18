using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private GameObject[] backgrounds; // assign all backgrounds in order
    [SerializeField] private float moveSpeed;

    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        // Cache sprite renderers for width lookup
        spriteRenderers = new SpriteRenderer[backgrounds.Length];
        for (int i = 0; i < backgrounds.Length; i++)
        {
            spriteRenderers[i] = backgrounds[i].GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        float move = -moveSpeed * Time.deltaTime;

        // Move all backgrounds
        foreach (var bg in backgrounds)
        {
            bg.transform.Translate(move, 0, 0);
        }

        // Check each background
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float width = spriteRenderers[i].bounds.size.x;

            // if completely off-screen to the left
            if (backgrounds[i].transform.position.x <= -Camera.main.orthographicSize * 2)
            {
                // Find the rightmost background
                float maxX = float.MinValue;
                int rightMostIndex = 0;

                for (int j = 0; j < backgrounds.Length; j++)
                {
                    if (backgrounds[j].transform.position.x > maxX)
                    {
                        maxX = backgrounds[j].transform.position.x;
                        rightMostIndex = j;
                    }
                }

                float rightEdge = maxX + spriteRenderers[rightMostIndex].bounds.size.x;

                // Move current background right after the last one
                backgrounds[i].transform.position = new Vector3(
                    rightEdge,
                    backgrounds[i].transform.position.y,
                    backgrounds[i].transform.position.z
                );
            }
        }
    }
}
