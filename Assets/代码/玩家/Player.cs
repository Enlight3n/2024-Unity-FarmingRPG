using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>, ISavable
{
    #region 移动相关

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




    [SerializeField] public SpriteRenderer equippedItemSpriteRenderer = null;

    private AnimationOverrides animationOverrides;

    private Camera mainCamera;

    private Rigidbody2D _rigidbody2D;


    private GridCursor gridCursor;
    private Cursor cursor;

    //属性
    private bool playerInputIsDisabled;
    public bool PlayerInputIsDisabled { get; set; }

    private bool playerToolUseDisabled = false;


    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds pickAnimationPause;
    private WaitForSeconds afterPickAnimationPause;




    #region 主要函数

    protected override void Awake()
    {
        base.Awake();

        _rigidbody2D = GetComponent<Rigidbody2D>();

        mainCamera = Camera.main;

        animationOverrides = GetComponentInChildren<AnimationOverrides>();
    }


    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();

        cursor = FindObjectOfType<Cursor>();

        //从settings中赋值
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);
    }


    private void OnDisable()
    {
        IDeregister();
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;
    }


    private void OnEnable()
    {
        IRegister();
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
    }


    private void Update()
    {
        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers(); //重置动画中使用工具部分的参数

            PlayerMovementInput(); //移动输入，根据键盘输入的情况设定动画参数

            PlayerWalkInput(); //检查是否行走，若按住shift，则改变人物移动速度和动画参数


            PlayerClickInput();

            EventHandler.CallMovementEvent(xInput, yInput,
                isWalking, isRunning, isIdle, isCarrying,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft,
                isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
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
                playerDirection = Direction.up;
            }
        }
        else if (yInput == 0 && xInput == 0)
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
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime,
            yInput * movementSpeed * Time.deltaTime);
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
            false, false, false, false);
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
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
    }


    public Vector3 GetPlayerViewportPosition()
    {
        //Camera.main.WorldToScreenPoint()函数接收一个世界空间下的位置，返回其所在的屏幕空间位置，以及其相对于摄像机的深度信息
        return mainCamera.WorldToViewportPoint(transform.position);
    }




    #region 处理动画覆盖控制器的两个函数

    //清除举起物体的动画
    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        animationOverrides.ApplyCharacterCustomisationParameters(角色动画类型.举起, false);
        isCarrying = false;
    }


    //显示举起物体的动画
    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

        if (itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            animationOverrides.ApplyCharacterCustomisationParameters(角色动画类型.举起, true);

            isCarrying = true;
        }
    }

    #endregion




    #region 点击放置、使用物体的函数

    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
                {
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();

                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            }
        }
    }


    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement(); //玩家放置东西时会停一下

        //传入玩家的光标网格位置和玩家网格位置的相对关系，获得动作动画的释放方位
        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);

        //获取光标网格位置的详细信息
        GridPropertyDetails gridPropertyDetails =
            GridManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        //获取当前选择的物体的详细信息
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails();

        if (itemDetails != null)
        {
            //看当前选择的物品的类型，对不同的类型实现不同的方法（考虑是否可被释放，光标是否有效）
            switch (itemDetails.itemType)
            {
                case ItemType.种子:
                    if (Input.GetMouseButtonDown(0))
                    {
                        ProcessPlayerClickInputSeed(gridPropertyDetails, itemDetails);
                    }

                    break;
                case ItemType.果实:
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (gridCursor.CursorPositionIsValid)
                        {
                            EventHandler.CallDropSelectedItemEvent();
                        }
                    }

                    break;
                case ItemType.锄头:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                    }

                    break;
                case ItemType.水壶:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                    }

                    break;
                case ItemType.镰刀:
                    if (cursor.CursorPositionIsValid)
                    {
                        playerDirection =
                            GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                        ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                    }

                    break;
                case ItemType.斧头:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        ChopInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                    }

                    break;
                case ItemType.镐子:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        BreakInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                    }

                    break;
                case ItemType.菜篮:
                    if (gridCursor.CursorPositionIsValid)
                    {
                        CollectInPlayerDirection(gridPropertyDetails, itemDetails, playerDirection);
                    }

                    break;
                default:
                    break;
            }
        }
    }




    #region 种地

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //判断是否可以种地的
        if (itemDetails.itemType == ItemType.种子 && gridCursor.CursorPositionIsValid &&
            gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
    }


    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        if (GridManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            //更新gridPropertyDetails
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;

            //显示作物
            GridManager.Instance.DisplayPlantedCrop(gridPropertyDetails);

            //从物品栏移除
            EventHandler.CallRemoveSelectedItemFromInventoryEvent();


            AudioManager.Instance.PlaySound(SoundName.种植种子声);
        }
    }

    #endregion




    #region 锄头

    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.使用锄头);

        StartCoroutine(HoeGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }


    private IEnumerator HoeGroundAtCursorRoutine(Vector3Int _playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;


        if (_playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }
        else if (_playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }
        else if (_playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (_playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;

        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        GridManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,
            gridPropertyDetails);

        GridManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    #endregion




    #region 水壶

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.浇水声);

        StartCoroutine(WaterGroundAtCursorRoutine(playerDirection, gridPropertyDetails));
    }


    private IEnumerator WaterGroundAtCursorRoutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;


        toolEffect = ToolEffect.watering;

        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }
        else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }
        else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        // 设置为已经浇过水
        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        /*
         即使禁用下面的SetGridPropertyDetails方法，程序同样正常运行，
         本来以为会出现，场景切换后未能正常保存浇水网格的GridPropertyDetails的情况
         究其原因，应该是这个gridPropertyDetails是引用类型，在赋值的之后虽然经过反复传递，但还是会直接改变原来字典中的值
         因此，下面这个方法或许没有执行的必要
         */
        GridManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,
            gridPropertyDetails);

        GridManager.Instance.DisplayWateredGround(gridPropertyDetails);

        yield return afterLiftToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    #endregion




    #region 镰刀

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }


    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }


    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        if (Input.GetMouseButton(0))
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.镰刀:
                case ItemType.菜篮:
                    if (playerDirection == Vector3Int.right)
                    {
                        isSwingingToolRight = true;
                    }
                    else if (playerDirection == Vector3Int.left)
                    {
                        isSwingingToolLeft = true;
                    }
                    else if (playerDirection == Vector3Int.up)
                    {
                        isSwingingToolUp = true;
                    }
                    else if (playerDirection == Vector3Int.down)
                    {
                        isSwingingToolDown = true;
                    }

                    break;
            }

            // 定义用于碰撞检测的正方形的中心点
            Vector2 point =
                new Vector2(
                    GetPlayerCentrePosition().x + (playerDirection.x * (equippedItemDetails.itemUseRadius / 2f)),
                    GetPlayerCentrePosition().y + playerDirection.y * (equippedItemDetails.itemUseRadius / 2f));

            // 定义用于碰撞检测的正方形的大小
            Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);

            //获取位于给定正方形中的，具有2D碰撞体的Item组件
            Item[] itemArray = GetComponentsAtBoxLocationNonAlloc<Item>(15, point, size, 0f);

            int reapableItemCount = 0;

            // 遍历接受到的物体
            for (int i = itemArray.Length - 1; i >= 0; i--)
            {
                if (itemArray[i] != null)
                {
                    // 如果可收获则摧毁物体
                    if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.植物)
                    {
                        // 粒子位置
                        Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x,
                            itemArray[i].transform.position.y + Settings.GridCellSize / 2f,
                            itemArray[i].transform.position.z);

                        //显示收获后的粒子特效
                        EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.收获);

                        AudioManager.Instance.PlaySound(SoundName.使用镰刀);

                        Destroy(itemArray[i].gameObject);

                        reapableItemCount++;

                        //一次最多只能收获两个
                        if (reapableItemCount >= 2)
                            break;
                    }
                }
            }
        }
    }

    #endregion




    #region 斧头

    private void ChopInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails,
        Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.使用斧头);

        //触发动画
        StartCoroutine(ChopInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }


    private IEnumerator ChopInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails,
        ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        // 使用动画以后
        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }

    #endregion




    #region 镐子

    private void BreakInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails,
        Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.使用镐子);

        StartCoroutine(BreakInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }


    private IEnumerator BreakInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails,
        ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;


        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return useToolAnimationPause;

        yield return afterUseToolAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }


    private void ProcessCropWithEquippedItemInPlayerDirection(Vector3Int playerDirection,
        ItemDetails equippedItemDetails, GridPropertyDetails gridPropertyDetails)
    {
        //确定方向动画参数
        switch (equippedItemDetails.itemType)
        {
            case ItemType.菜篮:
            {
                if (playerDirection == Vector3Int.right)
                {
                    isPickingRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isPickingLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isPickingUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isPickingDown = true;
                }
                break;
            }
            default:
            {
                if (playerDirection == Vector3Int.right)
                {
                    isUsingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isUsingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isUsingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isUsingToolDown = true;
                }
                break;
            }
        }
        


        //获取这个网格中的作物
        Crop crop = GridManager.Instance.GetCropObjectAtGridLocation(gridPropertyDetails);

        //如果crop脚本不空，且拿着的是采集工具，执行作物的corp脚本处理采集的方法
        //（包括累计采集次数判断是否采集完成，决定生成果实的数量和位置，重置网格属性，销毁作物，生成果实）
        if (crop != null)
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.镐子:
                case ItemType.斧头:
                    crop.ProcessToolAction(equippedItemDetails, isUsingToolRight, isUsingToolLeft, isUsingToolDown,
                        isUsingToolUp);
                    break;

                case ItemType.菜篮:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingDown,
                        isPickingUp);
                    break;
            }
        }
    }

    #endregion




    #region 菜篮

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails,
        Vector3Int playerDirection)
    {
        AudioManager.Instance.PlaySound(SoundName.采摘声);

        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails, equippedItemDetails, playerDirection));
    }
    
    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails,
        ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        ProcessCropWithEquippedItemInPlayerDirection(playerDirection, equippedItemDetails, gridPropertyDetails);

        yield return pickAnimationPause;

        
        yield return afterPickAnimationPause;

        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }
    
    #endregion

    #endregion




    #region get方法

    //传入玩家的光标网格位置和玩家网格位置的相对关系，获得动作动画的释放方位
    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if (cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }


    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (cursorPosition.x > playerPosition.x && cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2f)
                                                && cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2f))
        {
            return Vector3Int.right;
        }
        else if (cursorPosition.x < playerPosition.x && cursorPosition.y <
                                                     (playerPosition.y + cursor.ItemUseRadius / 2f)
                                                     && cursorPosition.y >
                                                     (playerPosition.y - cursor.ItemUseRadius / 2f)
                )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }


    //返回玩家的中心位置，因为玩家的轴心在脚底
    public Vector3 GetPlayerCentrePosition()
    {
        Vector3 tempP = transform.position;
        return new Vector3(tempP.x, tempP.y + Settings.PlayerCentreYOffset, tempP.z);
    }


    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfCollidersToTest, Vector2 point, Vector2 size,
        float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfCollidersToTest];

        //避免了在每次调用时都分配新的数组，减少GC
        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);

        T tComponent = default(T);

        T[] componentArray = new T[collider2DArray.Length];

        for (int i = collider2DArray.Length - 1; i >= 0; i--)
        {
            if (collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                if (tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }

        return componentArray;
    }

    #endregion




    #region 接口
    public void IRegister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Add(this);
    }

    public void IDeregister()
    {
        SaveLoadManager.Instance.iSavableObjectList.Remove(this);
    }

    public void ISave(ref GameSave gameSave)
    {
        var temp = transform.position;
        
        gameSave.playerPosition = new Vector3Serializable(temp.x, temp.y, temp.z);
    }
    
    public void ILoad(GameSave gameSave)
    {
        var temp = gameSave.playerPosition;

        transform.position = new Vector3(temp.x, temp.y, temp.z);
    }
    
    #endregion
}