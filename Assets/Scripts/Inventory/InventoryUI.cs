using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory; 
    public GameObject inventorySlotPrefab; 
    public Transform slotsParent; 

    private List<GameObject> uiSlots = new List<GameObject>(); 

    void Start()
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // Smazání starých slotů
        foreach (GameObject slot in uiSlots)
        {
            Destroy(slot);
        }
        uiSlots.Clear();
        
        // Počítání předmětů podle ID
        Dictionary<int, (Item itemData, int count)> itemCounts = new Dictionary<int, (Item, int)>();
        
        foreach (Item currentItem in inventory.items)
        {
            if (itemCounts.ContainsKey(currentItem.id))
            {
                var existing = itemCounts[currentItem.id];
                itemCounts[currentItem.id] = (existing.itemData, existing.count + 1);
            }
            else
            {
                itemCounts.Add(currentItem.id, (currentItem, 1));
            }
        }

        // Vytvoření UI slotů
        foreach (var pair in itemCounts.Values)
        {
            Item itemData = pair.itemData; 
            int count = pair.count;   

            GameObject uiSlot = Instantiate(inventorySlotPrefab, slotsParent);
            uiSlots.Add(uiSlot);
            
            // Nastavení ikony
            Image itemIcon = uiSlot.transform.Find("ItemIcon").GetComponent<Image>();
            if (itemIcon != null && itemData.icon != null)
            {
                itemIcon.sprite = itemData.icon;
                itemIcon.enabled = true;
            }
            else if (itemIcon != null)
            {
                itemIcon.enabled = false;
            }

            // Nastavení počtu předmětů
            /*Text countText = uiSlot.transform.Find("CountText").GetComponent<Text>();
            if (countText != null)
            {
                countText.text = count > 1 ? count.ToString() : "";
            }*/
        }
    }
}