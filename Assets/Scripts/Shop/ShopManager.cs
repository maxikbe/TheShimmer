using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public enum CartMode { None, Buying, Selling }
    [HideInInspector] public CartMode currentCartMode = CartMode.None;

    [Header("Základní UI")]
    public GameObject shopPanel;
    public Transform playerInventoryContainer;
    public Transform merchantInventoryContainer;
    public GameObject itemSlotPrefab;
    public UnityEngine.UI.Button cartToggleButton;
    public UnityEngine.UI.Button shopCloseButton;

    [Header("Košík UI (Novinka!)")]
    public GameObject cartWindow; // Cele okno kosiku co se schovava
    public Transform cartContainer; // Misto kam se sází itemy v košíku
    public TextMeshProUGUI cartTotalValueText; // Suma ve velkem okne
    public TextMeshProUGUI cartMiniTotalText; // Suma pod ikonkou
    public UnityEngine.UI.Button removeAllItemsButton;

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
    public Database itemDatabase; // Tahame primo ze Scriptable Object databaze

    private Merchant currentMerchant;
    private ShopItemSlot hoveredSlot;

    private List<ShopItemSlot> cartItems = new List<ShopItemSlot>(); // Pamatuje si, co je v kosiku
    private int cartTotalSum = 0;
    private Color priceColor;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        priceColor = tooltipPrice.color;
        
        if (cartToggleButton != null)
        {
            cartToggleButton.onClick.AddListener(ToggleCartWindow);
        }

        if (shopCloseButton != null)
        {
            shopCloseButton.onClick.AddListener(CloseShop);
        }

        if (removeAllItemsButton  != null)
        {
            removeAllItemsButton.onClick.AddListener(RemoveAllItemsFromCart);
        }
    }

    void Update()
    {
        if (shopPanel.activeSelf && hoveredSlot != null)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1))
            {
                ShowInspectWindow(hoveredSlot);
            }
        }

        if (inspectPanel.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F)))
        {
            inspectPanel.SetActive(false);
        }
    }

    public void OpenShop(Merchant merchant)
    {
        currentMerchant = merchant;
        shopPanel.SetActive(true);
        cartWindow.SetActive(false); // Defaultně kosik schovame
        RefreshShopUI();
    }

    public void CloseShop()
    {
        // Pred zavrenim vycistime kosik a vratime veci tam, kam patri
        RemoveAllItemsFromCart();

        shopPanel.SetActive(false);
        tooltipPanel.SetActive(false);
        inspectPanel.SetActive(false);
        cartWindow.SetActive(false);
        merchantInventoryContainer.gameObject.SetActive(true);
        playerInventoryContainer.gameObject.SetActive(true);
        currentMerchant = null;
    }

    public void RemoveAllItemsFromCart()
    {
        foreach(var item in new List<ShopItemSlot>(cartItems))
        {
            RemoveFromCart(item);
        }
    }

    // --- FUNKCE PRO TLAČÍTKO KOŠÍKU ---
    public void ToggleCartWindow()
    {
        if(cartWindow != null)
        {
            merchantInventoryContainer.gameObject.SetActive(!merchantInventoryContainer.gameObject.activeSelf);
            playerInventoryContainer.gameObject.SetActive(!playerInventoryContainer.gameObject.activeSelf);
            cartWindow.SetActive(!cartWindow.activeSelf);
        }
    }

    // --- LOGIKA KOŠÍKU ---
    public void OnSlotClicked(ShopItemSlot slot)
    {
        // Nepustí neprodejný item do košíku
        if (slot.isOwnedByPlayer && !slot.canSell) 
        {
            Debug.Log("Toto je quest item, neprodávej ho!");
            return;
        }

        // Nastavíme mód podle prvního kliknutí
        if (currentCartMode == CartMode.None)
        {
            currentCartMode = slot.isOwnedByPlayer ? CartMode.Selling : CartMode.Buying;
        }
        else
        {
            // Ochrana proti míchání prodeje a nákupu
            if ((currentCartMode == CartMode.Selling && !slot.isOwnedByPlayer) ||
                (currentCartMode == CartMode.Buying && slot.isOwnedByPlayer))
            {
                Debug.LogWarning("Nemůžeš najednou kupovat a prodávat! Vyprázdni košík.");
                return;
            }
        }

        // Přesouvání sem a tam
        if (!cartItems.Contains(slot))
        {
            cartItems.Add(slot);
            slot.transform.SetParent(cartContainer); // Fyzicky hodi tlacitko do kosiku
            UpdateCartSum();
        }
        else
        {
            RemoveFromCart(slot); // Kdyz na nej kliknes v kosiku, vrati ho zpatky
        }
    }

    private void RemoveFromCart(ShopItemSlot slot)
    {
        cartItems.Remove(slot);
        
        // Vratime do spravneho okna podle toho, ci to je
        Transform originalParent = slot.isOwnedByPlayer ? playerInventoryContainer : merchantInventoryContainer;
        slot.transform.SetParent(originalParent);
        
        UpdateCartSum();
    }

    private void UpdateCartSum()
    {
        cartTotalSum = 0;
        foreach (var item in cartItems)
        {
            cartTotalSum += item.myPrice;
        }

        // Updatneme oba texty (ve velkem okne i ten maly pod ikonkou)
        if(cartTotalValueText != null) cartTotalValueText.text = $"Suma: {cartTotalSum} G";
        if(cartMiniTotalText != null) cartMiniTotalText.text = $"{cartTotalSum} G";

        if (cartItems.Count == 0) currentCartMode = CartMode.None; // Reset modu kdyz je prazdno
    }

    // --- VYKRESLOVÁNÍ OBCHODU ---
    public void RefreshShopUI()
    {
        foreach (Transform child in playerInventoryContainer) Destroy(child.gameObject);
        foreach (Transform child in merchantInventoryContainer) Destroy(child.gameObject);

        foreach (ItemSaveData itemData in currentMerchant.currentInventory)
        {
            Item staticData = itemDatabase.GetItemByID(itemData.id);
            if (staticData == null) continue;

            int buyPrice = Mathf.RoundToInt(staticData.basePrice * currentMerchant.sellModifier);
            CreateItemSlot(itemData, staticData, buyPrice, false, merchantInventoryContainer);
        }

        foreach (ItemSaveData itemData in gameDataManager.currentGameData.OwnedItems)
        {
            Item staticData = itemDatabase.GetItemByID(itemData.id);
            if (staticData == null) continue; 

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
        
        if (!slot.canSell && slot.isOwnedByPlayer)
        {
            tooltipPrice.text = "Neprodejné";
            tooltipPrice.color = Color.red; 
        }
        else
        {
            tooltipPrice.text = "Cena: " + slot.myPrice.ToString() + " G";
            tooltipPrice.color = priceColor; 
        }
        
        if (slot.myItemStaticData.icon != null) tooltipIcon.sprite = slot.myItemStaticData.icon;
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