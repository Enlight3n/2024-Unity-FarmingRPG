using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static readonly int PlayerInventoryCapacity = 20;
    
    
    public static readonly float SecondsPerGameSecond = 0.7f/60f;

    public static readonly float GridCellSize = 1.0f;
    
    public static readonly float PlayerCentreYOffset = 0.875f;
    
    //动画持续时间
    public static float useToolAnimationPause = 0.25f;
    public static float afterUseToolAnimationPause = 0.2f;
    
    public static float liftToolAnimationPause = 0.4f;
    public static float afterLiftToolAnimationPause = 0.4f;

    public static float pickAnimationPause = 1f;
    public static float afterPickAnimationPause = 0.2f;
}
