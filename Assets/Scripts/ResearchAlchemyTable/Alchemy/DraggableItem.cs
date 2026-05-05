using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemSaveData saveData;
    public Item staticData;
    
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private AlchemyUI alchemyUI; // Odkaz na náš hlavní alchymistický skript

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Nyní přijímáme i referenci na UI, ať víme, komu poslat data pro Tooltip
    public void Setup(ItemSaveData save, Item stat, AlchemyUI ui)
    {
        saveData = save;
        staticData = stat;
        alchemyUI = ui;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root); 
        transform.SetAsLastSibling();
        
        canvasGroup.blocksRaycasts = false; 
        
        // Když item vezmeme do ruky, skryjeme info, ať nám nesvítí
        if (alchemyUI != null) alchemyUI.HideTooltip();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent); 
        canvasGroup.blocksRaycasts = true;
    }

    // ==========================================
    // HOVER LOGIKA (Myš najela / odjela)
    // ==========================================
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Ukážeme info jen když item zrovna netaháme vzduchem
        if (alchemyUI != null && canvasGroup.blocksRaycasts) 
        {
            alchemyUI.ShowTooltip(staticData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (alchemyUI != null)
        {
            alchemyUI.HideTooltip();
        }
    }
}