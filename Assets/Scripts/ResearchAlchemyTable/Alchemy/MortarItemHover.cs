using UnityEngine;
using UnityEngine.EventSystems;

public class MortarItemHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AlchemyUI alchemyUI; // Odkaz na hlavní UI

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Když na něj najedeme, zkontrolujeme, jestli ve stole něco je
        if (alchemyUI != null && alchemyUI.currentTable != null && alchemyUI.currentTable.mortarItemStatic != null)
        {
            // A ukážeme tooltip s daty toho itemu!
            alchemyUI.ShowTooltip(alchemyUI.currentTable.mortarItemStatic);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Když myš odjede, tooltip schováme
        if (alchemyUI != null)
        {
            alchemyUI.HideTooltip();
        }
    }
}