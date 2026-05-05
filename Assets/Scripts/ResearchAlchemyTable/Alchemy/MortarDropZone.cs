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
            bool isCrushable = draggedItem.staticData.isCrushable;
            
            // ZMĚNA ZDE: Už nekontrolujeme Data, ale Static! To je ta pravá pojistka.
            bool isMortarEmpty = alchemyUI.currentTable.mortarItemStatic == null;

            if (isCrushable && isMortarEmpty)
            {
                Debug.Log("ÚSPĚCH! Item vložen do hmoždíře.");
                alchemyUI.DropItemIntoMortar(draggedItem.saveData, draggedItem.staticData);
                Destroy(draggedItem.gameObject);
            }
            else
            {
                Debug.LogWarning("Zadrž, alchymisto!");
                if (!isCrushable) Debug.LogWarning("- Tento předmět se nedá drtit.");
                if (!isMortarEmpty) Debug.LogWarning("- Hmoždíř už je plný!");
            }
        }
    }
}