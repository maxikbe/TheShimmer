using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject inventorySlotPrefab;      // prefab slotu s InventorySlot na rootu
    public GameObject inventoryLabelNullPrefab; // prefab pro prázdný/NULL slot
    public Transform slotsParent;

    private readonly List<GameObject> uiSlots = new List<GameObject>();

    void Start()
    {
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        // Smazat staré sloty
        foreach (GameObject slot in uiSlots)
            Destroy(slot);
        uiSlots.Clear();

        // Jeden slot na každý prvek v inventory.items (zachová pořadí)
        for (int i = 0; i < inventory.items.Count; i++)
        {
            Item currentItem = inventory.items[i];

            // NULL / chybějící ikona -> null prefab
            if (currentItem == null || currentItem.icon == null)
            {
                if (inventoryLabelNullPrefab != null)
                {
                    GameObject nullLabel = Instantiate(inventoryLabelNullPrefab, slotsParent);
                    uiSlots.Add(nullLabel);

                    var nullSlot = nullLabel.GetComponent<InventorySlot>();
                    if (nullSlot != null)
                    {
                        nullSlot.slotIndex = i;
                        nullSlot.inventory = inventory;
                    }
                }
                continue;
            }

            // Normální slot
            GameObject uiSlot = Instantiate(inventorySlotPrefab, slotsParent);
            uiSlots.Add(uiSlot);

            var slotComp = uiSlot.GetComponent<InventorySlot>();
            if (slotComp != null)
            {
                slotComp.slotIndex = i;
                slotComp.inventory = inventory;
            }

            Image itemIcon = uiSlot.transform.Find("ItemIcon")?.GetComponent<Image>();
            if (itemIcon != null)
            {
                itemIcon.sprite = currentItem.icon;
                itemIcon.enabled = true;
            }

            var drag = uiSlot.transform.Find("ItemIcon")?.GetComponent<DraggableItem>();
            if (drag != null)
            {
                drag.slot = slotComp;
                drag.canvas = uiSlot.GetComponentInParent<Canvas>();
            }

            // Pokud chcete zobrazovat count, doplňte podle vaší datové struktury
            // Text countText = uiSlot.transform.Find("CountText")?.GetComponent<Text>();
            // if (countText != null) countText.text = "";
        }
    }
}