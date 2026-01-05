using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventorySlot slot;
    public Canvas canvas;

    private CanvasGroup canvasGroup;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false; // aby OnDrop na cílovém slotu fungoval
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // UI se po dropu obnoví přes UpdateInventoryUI
    }
}