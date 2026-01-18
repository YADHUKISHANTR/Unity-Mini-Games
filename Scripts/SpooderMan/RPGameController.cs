using System.Collections;
using UnityEngine;

public class RPGameController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator Start()
    {
        while (Screen.width < Screen.height)
            yield return null;
        yield return null;
    }

 
}
