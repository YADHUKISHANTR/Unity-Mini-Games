using System.Collections;
using UnityEngine;

public class OrientationScript : MonoBehaviour
{
    [SerializeField] private bool isPortrait = true;
    void Awake()
    {
        if (isPortrait)
        { 
           Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }

        // Optional: Prevent user rotation gestures
        Screen.autorotateToPortrait = isPortrait;
        Screen.autorotateToPortraitUpsideDown = isPortrait;
        Screen.autorotateToLandscapeLeft = !isPortrait;
        Screen.autorotateToLandscapeRight = !isPortrait;
    }
    
  
}
