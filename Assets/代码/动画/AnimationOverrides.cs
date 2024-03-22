using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    //用来获取Player物体
    [SerializeField] private GameObject character = null; 
    
    //数据容器列表，用来存储赋值的数据容器
    [SerializeField] private SO_AnimationOverrides soAnimationTypeArray = null;

    private Animator[] animatorsArray;

    private void Start()
    {
        //获取Player下的子物体的全部Animator
        animatorsArray = character.GetComponentsInChildren<Animator>();
    }

    
    //传入的参数为CharacterAttribute的列表
    public void ApplyCharacterCustomisationParameters(角色动画部位 playerAnimPart, 角色动画类型 playerAnimType)
    {
        //遍历数组，寻找到和部位类型同名的Animator
        Animator currentAnimator = null;
        
        
        if (playerAnimPart == 角色动画部位.手臂)
        {
            foreach (Animator animator in animatorsArray)
            {
                if (animator.name == "arms")
                {
                    currentAnimator = animator;
                    break;
                }
            }
        }
        else if (playerAnimPart == 角色动画部位.工具)
        {
            foreach (Animator animator in animatorsArray)
            {
                if (animator.name == "tool")
                {
                    currentAnimator = animator;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("playerAnimPart错误");
        }
        
        
        //从当前的状态机里取出runtimeAnimatorController
        AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
        
        
        //新建一个键值对列表animsKeyValuePairList，作为参数修改aoc
        List<KeyValuePair<AnimationClip, AnimationClip>> animsKeyValuePairList =
            new List<KeyValuePair<AnimationClip, AnimationClip>>();

        //给animsKeyValuePairList赋值
        foreach (AnimationOverridesType animationOverridesType in soAnimationTypeArray.AnimationOverridesList)
        {
            if(animationOverridesType.playerAnimType == playerAnimType)
            {
                animsKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(
                    animationOverridesType.animationClipBefore, animationOverridesType.animationClipAfter));
                
                Debug.Log((animationOverridesType.animationClipBefore.ToString(),
                    animationOverridesType.animationClipAfter.ToString()));
            }
        }

        Debug.Log(animsKeyValuePairList.Count);
        
        //修改aoc
        aoc.ApplyOverrides(animsKeyValuePairList);
        
        //将oc应用回当前的状态机
        currentAnimator.runtimeAnimatorController = aoc;
        
        Debug.Log("1");
    }
}
