using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class MortarInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AlchemyUI alchemyUI;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Vector3 startLocalPos;

    void Awake() 
    { 
        canvasGroup = GetComponent<CanvasGroup>(); 
    }

    // 1. UKÁZÁNÍ TOOLTIPU
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (alchemyUI != null && alchemyUI.currentTable != null && alchemyUI.currentTable.mortarItemStatic != null && canvasGroup.blocksRaycasts)
        {
            alchemyUI.ShowTooltip(alchemyUI.currentTable.mortarItemStatic);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (alchemyUI != null) alchemyUI.HideTooltip();
    }

    // 2. TAHÁNÍ Z HMOŽDÍŘE
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Nepůjde tahat, pokud je hmoždíř prázdný
        if (alchemyUI == null || alchemyUI.currentTable == null || alchemyUI.currentTable.mortarItemData == null) 
        {
            eventData.pointerDrag = null; 
            return;
        }

        startLocalPos = transform.localPosition;
        originalParent = transform.parent;
        
        transform.SetParent(transform.root);
        transform.SetAsLastSibling(); // Vykreslí se přes všechno ostatní
        
        canvasGroup.blocksRaycasts = false; // Aby myš prošla skrz na DropZónu
        alchemyUI.HideTooltip(); // Při tahání tooltip schováme
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.localPosition = startLocalPos; // Vrátí se hezky na střed misky
        canvasGroup.blocksRaycasts = true;
    }
}