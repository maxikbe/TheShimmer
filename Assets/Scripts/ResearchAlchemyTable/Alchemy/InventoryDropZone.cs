using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDropZone : MonoBehaviour, IDropHandler
{
    public AlchemyUI alchemyUI;

    public void OnDrop(PointerEventData eventData)
    {
        // Zkusíme, jestli hráč drží item z hmoždíře
        MortarInteractable draggedMortarItem = eventData.pointerDrag.GetComponent<MortarInteractable>();
        if (draggedMortarItem != null)
        {
            alchemyUI.CollectFromMortar(); 
            return; // Dál už nehledáme
        }

        // Pokud to nebyl hmoždíř, zkusíme, jestli to není lahvička od kotlíku
        FlaskInteractable draggedFlaskItem = eventData.pointerDrag.GetComponent<FlaskInteractable>();
        if (draggedFlaskItem != null)
        {
            alchemyUI.CollectFlaskFromFaucet();
        }
    }
}