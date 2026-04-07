using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Základní UI")]
    public GameObject shopPanel;
    public Transform playerInventoryContainer;
    public Transform merchantInventoryContainer;
    public GameObject itemSlotPrefab;

    [Header("Tooltip (Spodní lišta)")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipName;
    public TextMeshProUGUI tooltipPrice;
    public TextMeshProUGUI tooltipType;
    public Image tooltipIcon;

    [Header("Inspect Okno (Velký detail)")]
    public GameObject inspectPanel;
    public TextMeshProUGUI inspectName;
    public TextMeshProUGUI inspectDescription;
    public TextMeshProUGUI inspectStats;
    public Image inspectIcon;

    [Header("Databáze všech Itemů")]
    // pristup ke vsem itemum, abychom zjistovali ikonu a dalsi veci
    public Database itemDatabase; // tahame primo z database scriptablew objektu

    private Merchant currentMerchant;
    private ShopItemSlot hoveredSlot; // ví na cem mys prebyva

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // pokud je otevreny slot a jsem najety na necem
        if (shopPanel.activeSelf && hoveredSlot != null)
        {
            // F nebo prave tlacitko
            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1))
            {
                ShowInspectWindow(hoveredSlot);
            }
        }

        // zavreni pres esc nebo klikntuti kdekoliv nebo znova F
        if (inspectPanel.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F)))
        {
            inspectPanel.SetActive(false);
        }
    }

    public void OpenShop(Merchant merchant)
    {
        currentMerchant = merchant;
        shopPanel.SetActive(true);
        RefreshShopUI();
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        tooltipPanel.SetActive(false);
        inspectPanel.SetActive(false);
        currentMerchant = null;
    }

    public void RefreshShopUI()
    {
        // maze tlacitka
        foreach (Transform child in playerInventoryContainer) Destroy(child.gameObject);
        foreach (Transform child in merchantInventoryContainer) Destroy(child.gameObject);

        // vykresluje obchodnikuv invent
        foreach (ItemSaveData itemData in currentMerchant.currentInventory)
        {
            Item staticData = itemDatabase.GetItemByID(itemData.id);
            if (staticData == null) continue;

            // vypocitava nakupni cenu pres prirazku obchodnika
            int buyPrice = Mathf.RoundToInt(staticData.basePrice * currentMerchant.sellModifier);
            CreateItemSlot(itemData, staticData, buyPrice, false, merchantInventoryContainer);
        }

        // vykresluje muj batoh 
        foreach (ItemSaveData itemData in gameDataManager.currentGameData.OwnedItems)
        {
            Item staticData = itemDatabase.GetItemByID(itemData.id);
            
            // uz neignoruje quest itemi, proste jedeme dal
            if (staticData == null) continue; 

            // vypocitava nakupniu cenu
            int sellPrice = Mathf.RoundToInt(staticData.basePrice * currentMerchant.buyModifier);
            CreateItemSlot(itemData, staticData, sellPrice, true, playerInventoryContainer);
        }
    }

    private void CreateItemSlot(ItemSaveData data, Item staticData, int price, bool isPlayer, Transform container)
    {
        GameObject newSlot = Instantiate(itemSlotPrefab, container);
        ShopItemSlot slotScript = newSlot.GetComponent<ShopItemSlot>();
        slotScript.SetupSlot(data, staticData, price, isPlayer);
    }

    // --- TOOLTIP LOGIKA ---
    public void ShowTooltip(ShopItemSlot slot)
    {
        hoveredSlot = slot;
        tooltipPanel.SetActive(true);
        
        tooltipName.text = slot.myItemStaticData.itemName;
        tooltipType.text = "Typ: " + slot.myItemStaticData.itemType.ToString();
        
        // logika pro prodejnost
        if (!slot.canSell && slot.isOwnedByPlayer)
        {
            tooltipPrice.text = "Neprodejné";
            tooltipPrice.color = Color.red; // prepise barvu na cervenou
        }
        else
        {
            tooltipPrice.text = "Cena: " + slot.myPrice.ToString() + " G";
            tooltipPrice.color = Color.white; // vrati zpet bilou
        }
        
        if (slot.myItemStaticData.icon != null) 
        {
            tooltipIcon.sprite = slot.myItemStaticData.icon;
        }
    }

    public void HideTooltip()
    {
        hoveredSlot = null;
        tooltipPanel.SetActive(false);
    }

    // --- INSPECT LOGIKA ---
    private void ShowInspectWindow(ShopItemSlot slot)
    {
        inspectPanel.SetActive(true);
        Item data = slot.myItemStaticData;

        inspectName.text = data.itemName;
        inspectDescription.text = data.description;
        if (data.icon != null) inspectIcon.sprite = data.icon;

        // formatovani podle typu itemu
        string statsText = $"Level: {slot.myItemData.level}\n\n";
        
        if (data.itemType == ItemType.Weapon)
        {
            statsText += $"Damage: {data.Damage}\nRange: {data.Range}\nFire Rate: {data.FireRate}";
        }
        else if (data.itemType == ItemType.Armor)
        {
            statsText += $"Armor: {data.Armor}\nDurability: {data.durability}";
        }
        else if (data.itemType == ItemType.Healing || data.itemType == ItemType.Consumable)
        {
            statsText += $"Heal Amount: {data.HealAmount}";
        }

        inspectStats.text = statsText;
    }
}