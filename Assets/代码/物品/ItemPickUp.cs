using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            if (itemDetails.itemType != ItemType.植物 && itemDetails.itemType != ItemType.树 && 
                itemDetails.itemType != ItemType.矿)
            {
                InventoryManager.Instance.AddItem(item, collision.gameObject);
            
                AudioManager.Instance.PlaySound(SoundName.拾取声);
            }
        }
    }
}