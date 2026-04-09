using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

// PŘIDÁNO: IPointerClickHandler, abychom mohli registrovat kliknutí myší
public class ShopItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI Prvek na Tlačítku")]
    public Image iconImage; 
    
    [HideInInspector] public ItemSaveData myItemData;
    [HideInInspector] public Item myItemStaticData; 
    [HideInInspector] public int myPrice;
    [HideInInspector] public bool isOwnedByPlayer;
    [HideInInspector] public bool canSell; 

    public void SetupSlot(ItemSaveData data, Item staticData, int price, bool isPlayerItem)
    {
        myItemData = data;
        myItemStaticData = staticData;
        myPrice = price;
        isOwnedByPlayer = isPlayerItem;
        canSell = staticData.canBeSold; 

        if (staticData.icon != null) 
        {
            iconImage.sprite = staticData.icon;
            iconImage.enabled = true;

            if (!canSell && isPlayerItem)
            {
                iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); 
            }
            else
            {
                iconImage.color = Color.white; 
            }
        }
        else
        {
            iconImage.enabled = false; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShopManager.Instance.ShowTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShopManager.Instance.HideTooltip();
    }

    // --- NOVÁ FUNKCE PRO KLIKÁNÍ ---
    public void OnPointerClick(PointerEventData eventData)
    {
        // Zajistíme, že hráč klikl opravdu LEVÝM tlačítkem
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ShopManager.Instance.OnSlotClicked(this);
        }
    }
}