using System.Collections.Generic;
using UnityEngine;


public class UIInventoryBar : MonoBehaviour
{
    //用来表明物品栏是空白的图片，清空物品栏时使用
    [SerializeField] private Sprite blank16x16sprite = null;
    
    //声明一个UIInventorySlot数组，用来保存每个UIInventorySlot脚本
    public UIInventorySlot[] inventorySlot = null;
    
    //预先制作好一个用来拖拽的预制体，拖拽赋值给InventoryBarDraggedItem，鼠标拖拽物品到场景中时，用InventoryBarDraggedItem跟随
    public GameObject InventoryBarDraggedItem;
    
    [HideInInspector] public GameObject inventoryTextBoxGameObject;
    
    private RectTransform rectTransform;
    
    public bool isInventoryBarPositionBottom = true;

    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    private void Update()
    {
        SwitchInventoryPosition();

        ScrollCheck();
    }

    #region 鼠标滚轮切换
    
    public void ScrollCheck()
    {
        float t = Input.GetAxisRaw("Mouse ScrollWheel");
        
        if (!Mathf.Approximately(t, 0) && InventoryManager.Instance.GetSelectedInventoryItem()!=-1)
        {
            
            int position = InventoryManager.Instance.GetSelectedInventoryItemPosition();
            
            MoveHighlightedInventorySlotsTo(t, position);
            
        }
    }

    #endregion
   

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }
    
    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }
    

    #region 物品栏UI的更新和清除
    
    //更新物品栏，每次更新物品栏，都会先将物品栏清空再根据inventoryList重新绘制
    public void InventoryUpdated(List<InventoryItem> inventoryList)
    {
        ClearInventorySlots();

        if (inventorySlot.Length > 0 && inventoryList.Count > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                if (i < inventoryList.Count)
                {
                    int itemCode = inventoryList[i].itemCode;

                    ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
                        
                    if (itemDetails != null)
                    {
                        inventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                        inventorySlot[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                        inventorySlot[i].itemDetails = itemDetails;
                        inventorySlot[i].itemQuantity = inventoryList[i].itemQuantity;
                        SetHighlightedInventorySlots(i);
                    }
                }

                else
                {
                    break;
                }
            }
        }
    }
    
    //用来初始化物品栏，将物品栏清空，每次更新物品栏都会清空再重新绘制
    private void ClearInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                inventorySlot[i].inventorySlotImage.sprite = blank16x16sprite;
                inventorySlot[i].textMeshProUGUI.text = "";
                inventorySlot[i].itemDetails = null;
                inventorySlot[i].itemQuantity = 0;
                
                SetHighlightedInventorySlots(i);
            }
        }
    }

    #endregion




    #region 防止玩家遮挡物品栏，每帧调用，检测玩家在屏幕上的位置
    private void SwitchInventoryPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if (playerViewportPosition.y > 0.3f && isInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            isInventoryBarPositionBottom = true;
        }
        else if(playerViewportPosition.y <= 0.3f && isInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);
            
            isInventoryBarPositionBottom = false;
        }
    }
    
    
    #endregion 
    
    
    #region 设置选中红框
    
    
    //清除背包中的选中红框
    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                if (inventorySlot[i].isSelected)
                {
                    inventorySlot[i].isSelected = false;
                    inventorySlot[i].inventorySlotHighlight.color = new Color(0f, 0f, 0f, 0f);
                    InventoryManager.Instance.CancelSelectedInventoryItem();
                }
            }
        }
    }

    //遍历slot是否被选择，执行SetHighlightedInventorySlots(i);
    public void SetHighlightedInventorySlots()
    {
        if (inventorySlot.Length > 0)
        {
            for (int i = 0; i < inventorySlot.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }
    
    public void MoveHighlightedInventorySlotsTo(float t, int itemPosition)
    {
        inventorySlot[itemPosition].isSelected = false;
        inventorySlot[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 0f);
        

        int len = Mathf.Min(InventoryManager.Instance.playerInventoryList.Count, 12);
        if (t>0.05)
        {
            itemPosition = (itemPosition - 1 + len) % len;
        }
        else if(t<-0.05)
        {
            itemPosition = (itemPosition + 1 + len) % len;
        }
        
        inventorySlot[itemPosition].isSelected = true;
        inventorySlot[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);
        
        
        InventoryManager.Instance.SetSelectedInventoryItem(
            InventoryManager.Instance.playerInventoryList[itemPosition].itemCode);

        var itemDetails =
            InventoryManager.Instance.GetItemDetails(InventoryManager.Instance.playerInventoryList[itemPosition]
                .itemCode);
        
        if (itemDetails.itemType == ItemType.材料 || itemDetails.itemType == ItemType.果实 || 
            itemDetails.itemType == ItemType.种子)
        {
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
    }
    
    
    //根据给定位置的slot是否被选中，添加红框
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if (inventorySlot.Length > 0 && inventorySlot[itemPosition].itemDetails != null)
        {
            if (inventorySlot[itemPosition].isSelected)
            {
                inventorySlot[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);

                InventoryManager.Instance.SetSelectedInventoryItem(inventorySlot[itemPosition].itemDetails.itemCode);
            }
        }
    }
    #endregion
    
    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            if (inventorySlot[i].draggedItem != null)
            {
                Destroy(inventorySlot[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            inventorySlot[i].ClearSelectedItem();
        }
    }


    public int GetCurrentSelectedNumber()
    {
        for (int i = 0; i < inventorySlot.Length; i++)
        {
            if (inventorySlot[i].isSelected)
                return i;
        }

        return -1;
    }


}
