using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
        
        
        // SetScreenMode(true);
        

    }

    private void Start()
    {
        
    }

    // public void SetScreenMode(bool isFullScreen)
// {
//     if (isFullScreen)
//     {
//         Screen.SetResolution(1920,1080,FullScreenMode.ExclusiveFullScreen,0);
//     }
//     else
//     {
//         Screen.SetResolution(1920,1080,FullScreenMode.Windowed,0);
//     }
// }
}


    

