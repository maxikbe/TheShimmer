using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class ShopItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Prvek na Tlačítku")]
    public Image iconImage; // ukona itemu
    
    [HideInInspector] public ItemSaveData myItemData;
    [HideInInspector] public Item myItemStaticData; 
    [HideInInspector] public int myPrice;
    [HideInInspector] public bool isOwnedByPlayer;
    [HideInInspector] public bool canSell; // nova promenna pro logiku prodeje

    // nastaveni vzhledu tlacitka pri vytvareni vola se z Śhopmanageru
    public void SetupSlot(ItemSaveData data, Item staticData, int price, bool isPlayerItem)
    {
        myItemData = data;
        myItemStaticData = staticData;
        myPrice = price;
        isOwnedByPlayer = isPlayerItem;
        canSell = staticData.canBeSold; // natahne si to ze sablony

        // Vykresluje se pouze obrazek
        if (staticData.icon != null) 
        {
            iconImage.sprite = staticData.icon;
            iconImage.enabled = true;

            // logka na zasednuti quest itemu v mojem batohu
            if (!canSell && isPlayerItem)
            {
                iconImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // RGB seda
            }
            else
            {
                iconImage.color = Color.white; // normalni barva
            }
        }
        else
        {
            // pokud tam nic neni tak to nic nevykrasluje 
            iconImage.enabled = false; 
        }
    }

    // najetí myší 
    public void OnPointerEnter(PointerEventData eventData)
    {
        // shop manager uklazuje data 
        ShopManager.Instance.ShowTooltip(this);
    }

    // odjede mys 
    public void OnPointerExit(PointerEventData eventData)
    {
        // ukrije data
        ShopManager.Instance.HideTooltip();
    }
}