using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///一个数据容器包含了一个替换后的动画Clip
/// </summary>
[CreateAssetMenu(fileName = "so_AnimationType",menuName = "Scriptable Objects/Animation/Animation Type")]
public class SO_AnimationOverrides : ScriptableObject
{
    public List<AnimationOverridesType> AnimationOverridesList;
}


[System.Serializable]
public class AnimationOverridesType
{
    //实际animation clip的引用
    public AnimationClip animationClipBefore; 
    
    //实际animation clip的引用
    public AnimationClip animationClipAfter; 
    
    //变量类型，指定这个动画类型所指的变量
    public 角色动画类型 playerAnimType; 
}