using UnityEngine;

/// <summary>
/// 这个脚本挂载到Player的各个部位上，用来调用每个部位对应的动画
/// </summary>
public class MovementAnimController : MonoBehaviour
{
    private Animator _animator; 
    
    //初始化
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void OnEnable()
    {
        EventHandler.MovementEvent += SetAnimationParameters;
    }
    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParameters;
    }

    //根据传入的参数更新动画状态
    private void SetAnimationParameters(float xInput, float yInput, bool isWalking, bool isRunning, 
        bool isIdle, bool isCarrying, ToolEffect toolEffect,
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown,
        bool isPickingRight, bool isPickingLeft, bool isPickingUp, bool isPickingDown,
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
        bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        //Animator. SetFloat(string name, float value);
        //name：该参数的名称。value：该参数的新值。
        _animator.SetFloat(MoveSettings.xInput, xInput);
        _animator.SetFloat(MoveSettings.yInput, yInput);
        _animator.SetBool(MoveSettings.isWalking, isWalking);
        _animator.SetBool(MoveSettings.isRunning, isRunning);
        
        _animator.SetInteger(MoveSettings.toolEffect, (int)toolEffect);

        if (isUsingToolRight)
        {
            _animator.SetTrigger(MoveSettings.isUsingToolRight);
        }
        if (isUsingToolLeft)
        {
            _animator.SetTrigger(MoveSettings.isUsingToolLeft);
        }
        if (isUsingToolUp)
        {
            _animator.SetTrigger(MoveSettings.isUsingToolUp);
        }
        if (isUsingToolDown)
        {
            _animator.SetTrigger(MoveSettings.isUsingToolDown);
        }
        
        
        if (isLiftingToolRight)
        {
            _animator.SetTrigger(MoveSettings.isLiftingToolRight);
        }
        if (isLiftingToolLeft)
        {
            _animator.SetTrigger(MoveSettings.isLiftingToolLeft);
        }
        if (isLiftingToolUp)
        {
            _animator.SetTrigger(MoveSettings.isLiftingToolUp);
        }
        if (isLiftingToolDown)
        {
            _animator.SetTrigger(MoveSettings.isLiftingToolDown);
        }
        
        if (isSwingingToolRight)
        {
            _animator.SetTrigger(MoveSettings.isSwingingToolRight);
        }
        if (isSwingingToolLeft)
        {
            _animator.SetTrigger(MoveSettings.isSwingingToolLeft);
        }
        if (isSwingingToolUp)
        {
            _animator.SetTrigger(MoveSettings.isSwingingToolUp);
        }
        if (isSwingingToolDown)
        {
            _animator.SetTrigger(MoveSettings.isSwingingToolDown);
        }
        
        if (isPickingRight)
        {
            _animator.SetTrigger(MoveSettings.isPickingRight);
        }
        if (isPickingLeft)
        {
            _animator.SetTrigger(MoveSettings.isPickingLeft);
        }
        if (isPickingUp)
        {
            _animator.SetTrigger(MoveSettings.isPickingUp);
        }
        if (isPickingDown)
        {
            _animator.SetTrigger(MoveSettings.isPickingDown);
        }
        
        if (idleRight)
        {
            _animator.SetTrigger(MoveSettings.idleRight);
        }
        if (idleLeft)
        {
            _animator.SetTrigger(MoveSettings.idleLeft);
        }
        if (idleUp)
        {
            _animator.SetTrigger(MoveSettings.idleUp);
        }
        if (idleDown)
        {
            _animator.SetTrigger(MoveSettings.idleDown);
        }
    }

    private void AnimationEventPlayFootstepSound()
    {
        AudioManager.Instance.PlaySound(SoundName.脚步声重);
    }
}


