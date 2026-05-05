using UnityEngine;
using UnityEngine.EventSystems;

public class FlaskDropZone : MonoBehaviour, IDropHandler
{
    public AlchemyUI alchemyUI; 
    
    [Header("Co sem patří?")]
    public Item emptyFlaskItem; // ZMĚNA: Místo textu sem v Inspektoru přetáhneš přímo tvůj Scriptable Object lahvičky

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        
        if (draggedItem != null && alchemyUI.currentTable.flaskItemStatic == null)
        {
            // ZMĚNA: Porovnáváme, jestli se upuštěný ScriptableObject rovná tomu našemu požadovanému
            // Pro naprostou jistotu porovnáme jejich unikátní ID z tvé databáze
            if (draggedItem.staticData.id == emptyFlaskItem.id)
            {
                alchemyUI.DropFlaskUnderFaucet(draggedItem.saveData, draggedItem.staticData);
                Destroy(draggedItem.gameObject);
            }
            else
            {
                Debug.LogWarning("Sem patří jen prázdná lahvička!");
            }
        }
    }
}