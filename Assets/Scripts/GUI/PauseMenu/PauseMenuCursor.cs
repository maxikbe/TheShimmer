using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D hoverCursorTexture;
    private readonly Vector2 hotSpot = Vector2.zero;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverCursorTexture != null)
        {
            Cursor.SetCursor(hoverCursorTexture, hotSpot, CursorMode.Auto);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (MenuCursor.Instance != null)
        {
            MenuCursor.Instance.SetDefaultCursor();
        }
        else
        {
            Cursor.SetCursor(null, hotSpot, CursorMode.Auto);
        }
    }
}