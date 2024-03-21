using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    
    # region 移动相关
    
    //移动参数
    private float xInput;
    private float yInput;
    private bool isWalking;
    private bool isRunning;
    private bool isCarrying = false;
    private bool isIdle;
    private ToolEffect toolEffect = ToolEffect.none;
    private bool isUsingToolRight;
    private bool isUsingToolLeft;
    private bool isUsingToolUp;
    private bool isUsingToolDown;
    private bool isLiftingToolRight;
    private bool isLiftingToolLeft;
    private bool isLiftingToolUp;
    private bool isLiftingToolDown;
    private bool isSwingingToolRight;
    private bool isSwingingToolLeft;
    private bool isSwingingToolUp;
    private bool isSwingingToolDown;
    private bool isPickingRight;
    private bool isPickingLeft;
    private bool isPickingUp;
    private bool isPickingDown;
    
    
    //移动速度
    private float movementSpeed;
    

    
    #endregion

    #region 存档相关

    //玩家方向记录，以便保存
    private Direction playerDirection;

    
    #endregion
    
    
    
    private Camera mainCamera;
    
    private Rigidbody2D _rigidbody2D;
    
    //属性
    private bool playerInputIsDisabled;
    public bool PlayerInputIsDisabled
    {
        get; set;
    }
    
    private string savableUniqueID;
    public string SavableUniqueID { get ; set; }



    #region 主要函数
    
    protected override void Awake()
    {
        base.Awake();

        _rigidbody2D = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main;
        


        // animationOverrides = GetComponentInChildren<AnimationOverrides>();
        //
        // toolCharacterAttribute =
        //     new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColour.none, PartVariantType.hoe);
        //
        // armsCharacterAttribute =
        //     new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);
        //
        // characterAttributeCustomisationList = new List<CharacterAttribute>();
        //
        //
        // /*游戏保存相关*/
        // ISavableUniqueID = GetComponent<GenerateGUID>().GUID;
        // GameObjectSave = new GameObjectSave();
    }
    
    private void Start()
    {
        // gridCursor = FindObjectOfType<GridCursor>();
        //
        // cursor = FindObjectOfType<Cursor>();
        
        // //从settings中赋值
        // afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        // useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        // liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        // afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        // pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        // afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);
    }
    
    // private void OnDisable()
    // {
    //     ISavableDeregister();
    //     
    //     EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
    //     EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
    // }
    //
    //
    // private void OnEnable()
    // {
    //     ISavableRegister();
    //
    //     EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
    //     EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
    // }
    
    
    private void Update()
    {
        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers(); //重置动画中使用工具部分的参数

            PlayerMovementInput(); //移动输入，根据键盘输入的情况设定动画参数

            PlayerWalkInput(); //检查是否行走，若按住shift，则改变人物移动速度和动画参数

            //PlayerClickInput();
            
            EventHandler.CallMovementEvent(xInput, yInput,
                isWalking, isRunning, isIdle, isCarrying,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, 
                isSwingingToolUp, isSwingingToolDown,
                false, false,false, false);
        }

        PlayerTestInput(); //开发者测试功能
    }

    
    private void FixedUpdate()
    {
        PlayerMovement();
    }
    #endregion




    #region 移动相关函数
    private void ResetAnimationTriggers()
    {
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        toolEffect = ToolEffect.none;
    }
    
    
    private void PlayerMovementInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        
        if (yInput != 0 && xInput != 0)
        {
            xInput = xInput * 0.7f;
            yInput = yInput * 0.7f;
        }

        if (yInput != 0 || xInput != 0)
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            
            movementSpeed = 5.333f;
            
            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.right;
            }
        }
        else if(yInput == 0 && xInput == 0)
        {
            isRunning = false;
            isWalking = false;
            isIdle = true;
        }
    }
    
    
    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isRunning = false;
            isWalking = true;
            isIdle = false;
            movementSpeed = 5.333f * 0.4f;
        }
        else
        {
            isRunning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = 5.333f;
        }
    }
    
    
    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput *  movementSpeed * Time.deltaTime, 
            yInput *  movementSpeed * Time.deltaTime);
        _rigidbody2D.MovePosition(_rigidbody2D.position + move);
    }
    
    # endregion
    
    
    
    
    //要禁用玩家移动，只需要调用一次下面的DisablePlayerInputAndResetMovement函数即可
    //要重新启用，只需要设置PlayerInputIsDisabled为false即可
    #region 禁用玩家移动的相关函数

    private void ResetMovement()
    {
        xInput = 0f;
        yInput = 0f;
        isRunning = false;
        isWalking = false;
        isIdle = true;
    }
    
    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();
        
        EventHandler.CallMovementEvent(xInput, yInput,
            isWalking, isRunning, isIdle, isCarrying,
            toolEffect,
            isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
            isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
            isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
            isSwingingToolRight, isSwingingToolLeft, 
            isSwingingToolUp, isSwingingToolDown,
            false, false,false, false);
    }

    private void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }

    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }
    
    #endregion
    
    
    
    
    private void PlayerTestInput()
    {
        // if (Input.GetKey(KeyCode.T))
        // {
        //     TimeManager.Instance.TestAdvanceGameMinute();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     TimeManager.Instance.TestAdvanceGameDay();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(),transform.position);
        // }
    }
}
