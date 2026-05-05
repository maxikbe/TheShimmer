using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class FlaskInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
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
        // POUZE ZMĚNA ZDE: Ptáme se na flaskItemStatic místo mortar!
        if (alchemyUI != null && alchemyUI.currentTable != null && alchemyUI.currentTable.flaskItemStatic != null && canvasGroup.blocksRaycasts)
        {
            alchemyUI.ShowTooltip(alchemyUI.currentTable.flaskItemStatic);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (alchemyUI != null) alchemyUI.HideTooltip();
    }

    // 2. TAHÁNÍ ZPOD KOHOUTKU
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ZMĚNA: Ptáme se na flaskItemData
        if (alchemyUI == null || alchemyUI.currentTable == null || alchemyUI.currentTable.flaskItemData == null) 
        {
            eventData.pointerDrag = null; 
            return;
        }

        startLocalPos = transform.localPosition;
        originalParent = transform.parent;
        
        transform.SetParent(transform.root);
        transform.SetAsLastSibling(); 
        
        canvasGroup.blocksRaycasts = false; 
        alchemyUI.HideTooltip(); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);
        transform.localPosition = startLocalPos; 
        canvasGroup.blocksRaycasts = true;
    }
}