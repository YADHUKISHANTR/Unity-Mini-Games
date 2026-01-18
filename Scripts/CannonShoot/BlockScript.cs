using TMPro;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    string[] blockColors = new string[]
    {
    // Easy (greenish, calming)
    "#C9F786", // light lime green
    "#A8E6CF", // mint green
    "#B5EAD7", // pale turquoise
    "#06D6A0", // aqua green
    "#43AA8B", // jade green
    "#D0F4DE", // pastel greenish white
    "#86C2F7", // soft sky blue
    "#118AB2", // teal blue

    // Medium (transition tones)
    "#CDB4DB", // lavender purple
    "#9B5DE5", // violet purple
    "#FFDAC1", // soft apricot
    "#FFD166", // warm yellow
    "#F9844A", // orange
    "#FFB997", // peachy orange

    // Hard (reddish, alerting)
    "#FF874D",
    "#EF476F", // pinkish red
    "#FF6F61", // coral red
    "#E63946", // crimson rose
    "#D00000", // deep scarlet red
    "#F77F00"  // deep amber orange

    };

    public int Number;

    private bool isExtraBullet = false;
    private bool isMultyBullet = false;
    [SerializeField] TextMeshPro NumberText;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] Sprite addbulletSprite;
    [SerializeField] Sprite multibulletSprite;

    SpriteRenderer BlockRd;
    private BoxCollider2D bd;
    private Animator anim;
    void Start()
    {
        bd = GetComponent<BoxCollider2D>();
        
        BlockRd = GetComponent<SpriteRenderer>();

        anim = GetComponent<Animator>();

        BlockNumberSelect();

    }
    void BlockNumberSelect()
    {
        int rd = Random.Range(0, 100);
        //Debug.Log(rd);
        if (rd > 90)
        {
            NumberText.enabled = false;


            isExtraBullet = true;
            BlockRd.sprite = addbulletSprite;
            BlockRd.color = Color.white;
        }
        else if (rd <= 90 && rd > 78)
        {
            NumberText.enabled = false;

            isMultyBullet = true;
            BlockRd.sprite = multibulletSprite;
            BlockRd.color = Color.white;
        }
        else
        {
            ColorSelect();
               
        }


    }
    void ColorSelect()
    {
        string colorSelect;
        if (Number <= 99 && Number >= 0)
        {
            colorSelect = blockColors[(Number /5)];

        }
        else
        {
            colorSelect = blockColors[19];
        }
        Color newCol;
        if (ColorUtility.TryParseHtmlString(colorSelect, out newCol))
        {
            BlockRd.color = newCol;
        }

        NumberText.text = Number.ToString();
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.collider.tag);
        if(collision.collider.tag == "Enemy")
        {
            if(isExtraBullet)
            {

                // Start shooting
                CannonScript cannon = Object.FindFirstObjectByType<CannonScript>();
                if (cannon != null)
                {
                    if(cannon.bulletNum < 50)
                    {
                        cannon.bulletNum++;

                    }
                    else
                    {
                        CannonGameController.Instance.AddScore(5);
                    }
                        cannon.StartCoroutine(cannon.shoot(1));
                    
                }
                CannonGameController.blocks.Remove(gameObject);
                Destroy(gameObject);
            }
            else if(isMultyBullet)
            {
                Destroy(bd);
                
                for(int i = 1; i < 7; i++)
                {
                   Quaternion rotation = Quaternion.Euler(0, 0, 25*i);
                   GameObject bullet = Instantiate(bulletPrefab, transform.position, rotation);
                   CannonScript.bulletList.Add(bullet);

                }
                
                CannonGameController.blocks.Remove(gameObject);
                
                Destroy(gameObject,0.1f);
            }
            else
            {
                Number--;
                CannonGameController.Instance.AddScore(1);
                ColorSelect();
                if (Number <= 0)
                {
                    CannonGameController.Instance.AddScore(5);
                    CannonGameController.blocks.Remove(gameObject);
                    Destroy(gameObject);
                }
                NumberText.text = Number.ToString();
                //transform.SetAsFirstSibling();
                //NumberText.sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
                anim.SetTrigger("Size");
                


            }

        }
        
       
    }
    
}
