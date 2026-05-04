using UnityEngine;
using UnityEngine.EventSystems;

public class MortarDropZone : MonoBehaviour, IDropHandler
{
    public AlchemyUI alchemyUI; 

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        
        if (draggedItem != null)
        {
            if (draggedItem.staticData.isCrushable && alchemyUI.currentTable.mortarItemData == null)
            {
                alchemyUI.DropItemIntoMortar(draggedItem.saveData, draggedItem.staticData);
                Destroy(draggedItem.gameObject);
            }
        }
    }
}