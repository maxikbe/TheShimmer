using UnityEngine;
using UnityEngine.EventSystems;

public class CauldronDropZone : MonoBehaviour, IDropHandler
{
    public AlchemyUI alchemyUI; 

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggedItem != null)
        {
            alchemyUI.DropItemIntoCauldron(draggedItem.saveData, draggedItem.staticData);
            Destroy(draggedItem.gameObject);
        }
    }
}