using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 整个类就是为了实现光标的显示，绝大部分方法都是为了DisplayCursor这个方法服务
/// </summary>
public class GridCursor : MonoBehaviour
{
    //代码赋值
    private Canvas canvas;
    private Grid grid; //场景中的TilemapGrid物体的grid组件
    private Camera mainCamera;

    //拖拽赋值
    [SerializeField] private Image cursorImage = null; //存放红绿图标的地方
    [SerializeField] private RectTransform cursorRectTransform = null; //图标的位置
    [SerializeField] private Sprite greenCursorSprite = null; //绿色图标的图片
    [SerializeField] private Sprite redCursorSprite = null; //红色图标的图片

    [SerializeField] private SO_CropDetailsList so_CropDetailsList;

    #region 属性

    //光标位置是否有效，有效是绿色，无效是红色
    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid
    {
        get => _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;
    }

    //物体的使用的网格半径
    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius
    {
        get => _itemUseGridRadius;
        set => _itemUseGridRadius = value;
    }           

    //选择的物品类型
    private ItemType _selectedItemType;
    public ItemType SelectedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }

    //光标是否被禁用，光标的启用与禁用只在于物品栏是否有物品被选中，选中就启用，未选中就禁用
    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }
    
    #endregion
    
    
    
    
    #region 获取场景中的grid组件
    
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }
    
    #endregion
    
    
    
    
    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        Application.targetFrameRate = 60;
    }

    //每一帧询问：如果光标启用，则显示
    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    
    private Vector3Int DisplayCursor()
    {
        //场景中是否存在网格
        if (grid != null)
        {
            
            //获取光标对应网格的坐标
            Vector3Int gridPosition = GetGridPositionForCursor();

            //获取玩家对应的网格的坐标
            Vector3Int playerGridPosition = GetGridPositionForPlayer();
            
            //根据各种判断（光标和玩家的距离，物品是否可以被扔下），来决定光标的颜色
            SetCursorValidity(gridPosition, playerGridPosition);

            //传入网格坐标，将其返回值设置为光标位置
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

            return gridPosition;
        } 
        else
        {
            return Vector3Int.zero;
        }
    }

    
    
    
    #region 私有函数

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid(); //设置光标为绿色

        //判断光标和玩家距离，太远了设置为红色，如果是红色后面就不用看了
        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius ||
            Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }
        
        //获取当前选择物体的详细信息
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails();
        
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        //寻找当前场景中的xy坐标对应网格的详细信息
        GridPropertyDetails gridPropertyDetails =
            GridManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
               case ItemType.种子:
                   if (!gridPropertyDetails.bCanDropItem)
                   {
                       SetCursorToInvalid();
                   }
                   break;
               case ItemType.果实:
                   if (!gridPropertyDetails.bCanDropItem)
                   {
                       SetCursorToInvalid();
                   } 
                   break;
               case ItemType.树:
               case ItemType.材料:
               case ItemType.植物:
               case ItemType.矿:
                   break;
               
               default:
               {
                   if (!IsCursorValidForTool(gridPropertyDetails, itemDetails))
                   {
                       SetCursorToInvalid();
                   } 
                   break;
               }
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }
    

    //无效时，设置光标为红色
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        _cursorPositionIsValid = false;
    }

    //有效时，设置光标为绿色
    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }


    
    //这个函数是为了判断光标是有效还是无效的，是上面SetCursorValidity函数的续写，上面判断了一次之后，都挪到这里来了
    //按理来说，这里拆成多个函数写会更加规整
    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.锄头:
                if (gridPropertyDetails.bDiggable && gridPropertyDetails.daysSinceDug == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ItemType.水壶:
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case ItemType.镐子:
            case ItemType.斧头:
            case ItemType.镰刀:
            case ItemType.菜篮:
                //检查物体是否可以被所选的物体收获，检查物体是否成熟
                
                // 检查这块地是否有种子
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    // 获取种子的详情
                    CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                    
                    if (cropDetails != null)
                    {
                        // 检查是否成熟
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            if (cropDetails.CanUseToolToHarvestCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                return false;
            default:
                return false;
        }
    }
    
    #endregion

    
    
    
    #region 启用和禁用光标

    public void DisableCursor()
    {
        cursorImage.color=Color.clear;
        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }
    
    #endregion
    
    
    
    
    #region 获取网格坐标
    
    /*--------使用grid.WorldToCell方法可以将世界坐标转换为瓦片坐标--------*/
    
    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            -mainCamera.transform.position.z));
        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }
    
    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        //将 gridPosition 转换为世界坐标系中的位置
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        
        //将世界坐标系中的位置转换为屏幕坐标系中的位置
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        
        //使用 RectTransformUtility.PixelAdjustPoint 方法调整屏幕坐标系中的位置以适应画布，并返回调整后的位置
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    //获取鼠标的世界坐标，将其转换为网格坐标，然后再转化为规则化的世界坐标
    public Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }
    #endregion
}
