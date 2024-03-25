using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PauseMenuInventoryManagementSlot[] inventoryManagementSlot = null;

    public GameObject inventoryManagementDraggedItemPrefab;

    [SerializeField] private Sprite transparent16x16 = null;

    [HideInInspector] public GameObject inventoryTextBoxGameobject;


    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;
        
        //填充
        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryManager.Instance.playerInventoryList);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;

        DestroyInventoryTextBoxGameObject();
    }

    
    public void DestroyInventoryTextBoxGameObject()
    {
        if (inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryTextBoxGameobject);
        }
    }

    
    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < InventoryManager.Instance.playerInventoryList.Count; i++)
        {
            if (inventoryManagementSlot[i].draggedItem != null)
            {
                Destroy(inventoryManagementSlot[i].draggedItem);
            }
        }
    }

    private void PopulatePlayerInventory(List<InventoryItem> playerInventoryList)
    {
        InitialiseInventoryManagementSlots();
        
        for (int i = 0; i < playerInventoryList.Count; i++)
        {
            inventoryManagementSlot[i].itemDetails =
                InventoryManager.Instance.GetItemDetails(playerInventoryList[i].itemCode);
                
            inventoryManagementSlot[i].itemQuantity = playerInventoryList[i].itemQuantity;

            if (inventoryManagementSlot[i].itemDetails != null)
            {
                inventoryManagementSlot[i].inventoryManagementSlotImage.sprite =
                    inventoryManagementSlot[i].itemDetails.itemSprite;
                    
                inventoryManagementSlot[i].textMeshProUGUI.text =
                    inventoryManagementSlot[i].itemQuantity.ToString();
            }
        }
    }

    private void InitialiseInventoryManagementSlots()
    {
        //清空slot
        for (int i = 0; i < inventoryManagementSlot.Length; i++)
        {
            inventoryManagementSlot[i].greyedOutImageGO.SetActive(false);
            inventoryManagementSlot[i].itemDetails = null;
            inventoryManagementSlot[i].itemQuantity = 0;
            inventoryManagementSlot[i].inventoryManagementSlotImage.sprite = transparent16x16;
            inventoryManagementSlot[i].textMeshProUGUI.text = "";
        }
    }
}