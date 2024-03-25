using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// InventoryManager用来保存玩家的物品
/// </summary>

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>, ISavable
{
    //保存物品的数据容器
    [SerializeReference] private SO_ItemList itemList = null;

    //创建一个字典遍历SO_ItemList中的所有物品详细信息，并将它们添加到字典中，以后直接使用字典即可
    private Dictionary<int, ItemDetails> itemDetailsDictionary;

    //玩家背包
    public List<InventoryItem> playerInventoryList;
    [HideInInspector] public int playerInventoryCapacity;


    //当前选中的物品的id
    private int selectedInventoryItem;
    
    private UIInventoryBar inventoryBar;

    
    
    protected override void Awake()
    {
        base.Awake();

        //初始化
        InitializeAll();
        
    }

    
    private void Start()
    {
        inventoryBar = FindObjectOfType<UIInventoryBar>();
    }


    private void OnEnable()
    {
        IRegister();
    }


    private void OnDisable()
    {
        IDeregister();
    }




    #region Awake初始化相关函数
    
    private void InitializeAll()
    {
        playerInventoryList = new List<InventoryItem>();
        
        playerInventoryCapacity = Settings.PlayerInventoryCapacity;
        
        //创建一个字典，其中存储了物品代码和物品详细信息之间的映射。遍历了SO_ItemList中的所有物品详细信息，并将它们添加到字典中
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();
        foreach (ItemDetails itemDetails in itemList.itemDetails)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
        
        //初始化
        selectedInventoryItem = -1;
    }

    
    #endregion

    

    #region 添加物品的相关函数

    //拾取物品时调用的AddItem函数：添加物体 + 删除场景中的物体
    public void AddItem(Item item, GameObject gameObjectToDelete)
    {
        AddItem(item);

        Destroy(gameObjectToDelete);
    }


    //传入item，添加到玩家背包
    public void AddItem(Item item)
    {
        int itemCode = item.ItemCode;

        //检查物品是否已经存在，不在则添加新的，在则在对应的itemPosition处数量+1
        int itemPosition = FindItemInInventory(itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(itemCode);
        }

        EventHandler.CallInventoryUpdatedEvent(playerInventoryList);
    }

    //传入itemCode，添加到对应的inventoryList里面
    public void AddItem(int itemCode)
    {
        int itemPosition = FindItemInInventory(itemCode);

        if (itemPosition != -1)
        {
            AddItemAtPosition(itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(itemCode);
        }

        EventHandler.CallInventoryUpdatedEvent(playerInventoryList);
    }

    //添加新的物品，声明一个新的数据列表元素添加到最后
    private void AddItemAtPosition(int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        
        playerInventoryList.Add(inventoryItem);
        
    }


    //在position处添加物品数量，声明一个新的数据列表元素替换掉原来的
    private void AddItemAtPosition(int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = playerInventoryList[position].itemQuantity + 1;
        
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        
        playerInventoryList[position] = inventoryItem;
    }


    //寻找给定itemCode的东西的位置，不存在则返回-1
    public int FindItemInInventory(int itemCode)
    {
        for (int i = 0; i < playerInventoryList.Count; i++)
        {
            if (playerInventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    
    
    
    #region 交换两个物品的位置
    
    //交换两个物品的位置
    public void SwapInventoryItems(int fromItem, int toItem)
    {
        if (fromItem < playerInventoryList.Count &&
            toItem < playerInventoryList.Count && 
            fromItem != toItem && fromItem >= 0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = playerInventoryList[fromItem];
            InventoryItem toInventoryItem = playerInventoryList[toItem];

            playerInventoryList[toItem] = fromInventoryItem;
            playerInventoryList[fromItem] = toInventoryItem;

            EventHandler.CallInventoryUpdatedEvent(playerInventoryList);
        }

    }
    
    
    #endregion
    
    
    
    #region 查找的相关函数
    
    
    //该方法接受物品代码，返回在_itemDetailsDictionary字典中的，与该代码对应的物品详细信息
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }
        else
        {
            return null;
        }
    }
    
    #endregion

    
    
    #region 删除物品的相关函数

    public void RemoveItem(int itemCode)
    {
        int itemPosition = FindItemInInventory(itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(playerInventoryList, itemCode, itemPosition);
        }

        //更新UI显示
        EventHandler.CallInventoryUpdatedEvent(playerInventoryList);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position);
        }

    }
    
    #endregion
    
    
    
    
    #region 当前选中的物体，selectedInventoryItem相关函数

    
    //传入物品的ID，保存到selectedInventoryItem
    public void SetSelectedInventoryItem(int itemCode)
    {
        selectedInventoryItem = itemCode;
    }
    
    public int GetSelectedInventoryItem()
    {
        return selectedInventoryItem;
    }

    
    //将selectedInventoryItem里，当前选中的物品的ID清除
    public void CancelSelectedInventoryItem()
    {
        selectedInventoryItem = -1;
    }
    
    //获取当前选中的物体的itemDetails
    public ItemDetails GetSelectedInventoryItemDetails()
    {
        if (selectedInventoryItem == -1)
        {
            return null;
        }
        else
        {
            return GetItemDetails(selectedInventoryItem);
        }
    }
    
    //获取当前选中物体的位置，未选中则为-1
    public int GetSelectedInventoryItemPosition()
    {
        if (selectedInventoryItem == - 1)
            return -1;
        
        
        for(int i=0;i<12;i++)
        {
            if (playerInventoryList[i].itemCode == selectedInventoryItem)
                return i;
        }

        return -1;
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
        gameSave.playerInventory = playerInventoryList;
    }


    public void ILoad(GameSave gameSave)
    {
        //清除玩家动作
        Player.Instance.ClearCarriedItem();
        
        playerInventoryList = gameSave.playerInventory;
        
        //更新UI，不能调用EventHandler.CallInventoryUpdatedEvent(playerInventoryList);
        inventoryBar.InventoryUpdated(playerInventoryList);
    }
    
    #endregion
    
}
    
    