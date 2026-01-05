using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public Inventory inventory;

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (dragged == null || dragged.slot == null || inventory == null) return;

        int from = dragged.slot.slotIndex;
        int to = slotIndex;

        if (inventory.SwapItems(from, to))
        {
            inventory.inventoryUI?.UpdateInventoryUI();
        }
    }
}