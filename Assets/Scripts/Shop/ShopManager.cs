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

    [Header("Košík UI")]
    public GameObject cartWindow; // Cele okno kosiku co se schovava
    public Transform cartContainer; // misto na itemi v kosiku
    public TextMeshProUGUI cartTotalValueText; // Suma ve velkem okne
    public TextMeshProUGUI cartMiniTotalText; // Suma pod ikonkou
    
    [Header("Tlačítka v Košíku")]
    public UnityEngine.UI.Button removeAllItemsButton;
    public UnityEngine.UI.Button directTradeButton; // koupit rovnou"
    public UnityEngine.UI.Button startHaggleButton; // smlouvat"
    
    [Header("Smlouvání UI")]
    public GameObject hagglePanel; // panel smlouvání
    public Slider haggleSlider;
    public TextMeshProUGUI merchantOfferText;
    public TextMeshProUGUI playerOfferText;
    public TextMeshProUGUI patienceText;
    public UnityEngine.UI.Button submitOfferButton; // navrhnout cenu
    

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
    
    private int currentMerchantOffer;
    private int currentMinAcceptable;
    private int currentMaxAcceptable;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        priceColor = tooltipPrice.color;
        
        if (cartToggleButton != null) cartToggleButton.onClick.AddListener(ToggleCartWindow);

        if (shopCloseButton != null) shopCloseButton.onClick.AddListener(CloseShop);

        if (removeAllItemsButton  != null) removeAllItemsButton.onClick.AddListener(RemoveAllItemsFromCart);
        
        
        if (directTradeButton != null) directTradeButton.onClick.AddListener(ExecuteTrade);
        
        if (startHaggleButton != null) startHaggleButton.onClick.AddListener(StartHaggling);
        
        
        if (haggleSlider != null) haggleSlider.onValueChanged.AddListener(UpdateHaggleSliderUI);
        
        if (submitOfferButton != null) submitOfferButton.onClick.AddListener(SubmitHaggleOffer);
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

        // sleva podle reputace
        MerchantReputation rep = GetCurrentMerchantReputation();
        float repValue = rep != null ? rep.reputationValue : 50f;
        
        // vzorec: (50 - 50) * 0.002 = 0. (100 - 50) * 0.002 = +0.1 (10% bonus)
        float repModifier = (repValue - 50f) * 0.002f; 

        // Vykreslení obchodu
        foreach (ItemSaveData itemData in currentMerchant.currentInventory)
        {
            Item staticData = itemDatabase.GetItemByID(itemData.id);
            if (staticData == null) continue;

            // Obchodník prodává ( menší číslo):
            float finalSellMod = currentMerchant.sellModifier - repModifier; 
            int buyPrice = Mathf.RoundToInt(staticData.basePrice * finalSellMod);
            
            CreateItemSlot(itemData, staticData, buyPrice, false, merchantInventoryContainer);
        }

        // Vykreslení hráče
        foreach (ItemSaveData itemData in gameDataManager.currentGameData.OwnedItems)
        {
            Item staticData = itemDatabase.GetItemByID(itemData.id);
            if (staticData == null) continue; 

            // Obchodník kupuje ( větší číslo):
            float finalBuyMod = currentMerchant.buyModifier + repModifier;
            int sellPrice = Mathf.RoundToInt(staticData.basePrice * finalBuyMod);
            
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
    
    
    // --- SMLOUVÁNÍ ---

    public void StartHaggling()
    {
        if (cartItems.Count == 0 || currentCartMode == CartMode.None) return;

        hagglePanel.SetActive(true);
        currentMerchantOffer = cartTotalSum; // zakladni cena
        
        // maximální mozna sleva u obchodnika 0.15 - 15%
        float tolerance = currentMerchant.haggleTolerance; 

        // kupuješ - chces cena DOLŮ, prodáváš - chces  NAHORU.
        if (currentCartMode == CartMode.Buying)
        {
            currentMinAcceptable = Mathf.RoundToInt(currentMerchantOffer * (1f - tolerance));
            haggleSlider.minValue = Mathf.RoundToInt(currentMerchantOffer * 0.5f); // max 50% sleva
            haggleSlider.maxValue = currentMerchantOffer;
        }
        else //prodavani
        {
            currentMaxAcceptable = Mathf.RoundToInt(currentMerchantOffer * (1f + tolerance));
            haggleSlider.minValue = currentMerchantOffer;
            haggleSlider.maxValue = Mathf.RoundToInt(currentMerchantOffer * 1.5f); // jenom 50 % navic
        }

        haggleSlider.value = currentMerchantOffer;
        UpdateHaggleUI();
    }

    private void UpdateHaggleSliderUI(float value)
    {
        playerOfferText.text = $"Tvoje nabídka: {Mathf.RoundToInt(value)} G";
    }

    private void UpdateHaggleUI()
    {
        merchantOfferText.text = $"Obchodník žádá: {currentMerchantOffer} G";
        playerOfferText.text = $"Tvoje nabídka: {Mathf.RoundToInt(haggleSlider.value)} G";
        patienceText.text = $"Trpělivost: {currentMerchant.currentPatience} / {currentMerchant.maxPatience}";
    }

    public void SubmitHaggleOffer()
    {
        int playerOffer = Mathf.RoundToInt(haggleSlider.value);
        bool dealAccepted = false;

        // vyhodnoceni nabydky
        if (currentCartMode == CartMode.Buying && playerOffer >= currentMinAcceptable) dealAccepted = true;
        else if (currentCartMode == CartMode.Selling && playerOffer <= currentMaxAcceptable) dealAccepted = true;

        if (dealAccepted)
        {
            Debug.Log("Obchodník přijal tvoji nabídku!");
            cartTotalSum = playerOffer; // prepise se suma na usmlouvanou
            
            ExecuteTrade(); // prodej
        }
        else
        {
            // nasrali jsme ho, ale zatim jen trochu
            currentMerchant.currentPatience--;
            
            if (currentMerchant.currentPatience <= 0)
            {
                Debug.Log("Zebraku, nemam te rad GRRR. Konec hsoppu.");
                
                // Trest za ztrátu trpělivosti (-10 bodů)
                ModifyReputation(-10f); 
                gameDataManager.SaveData(); 
                
                hagglePanel.SetActive(false);
                CloseShop(); // di do ksa
            }
            else
            {
                // trosku slevi, ne uplne to co chci
                if (currentCartMode == CartMode.Buying)
                {
                    currentMerchantOffer = Random.Range(currentMinAcceptable, currentMerchantOffer);
                    haggleSlider.maxValue = currentMerchantOffer;
                }
                else
                {
                    currentMerchantOffer = Random.Range(currentMerchantOffer, currentMaxAcceptable);
                    haggleSlider.minValue = currentMerchantOffer;
                }
                
                haggleSlider.value = currentMerchantOffer;
                UpdateHaggleUI();
            }
        }
    }

    // --- FINAL PRESUN ITEMU A PENEZ ---
    public void ExecuteTrade()
    {
        if (cartItems.Count == 0) return;

        // jsem dostatecny zid??
        if (currentCartMode == CartMode.Buying && gameDataManager.currentGameData.player.numberOfCoins < cartTotalSum)
        {
            Debug.LogWarning("Nemáš dost peněz!");
            return; // Obchod se zruší
        }

        // currentMerchant.currentPatience =  currentMerchant.maxPatience; // resetuju patience
        
        // presun do listu
        foreach (ShopItemSlot slot in cartItems)
        {
            if (currentCartMode == CartMode.Buying)
            {
                // zmizi obchodnikovy obevi se hraci (kupuju)
                currentMerchant.currentInventory.Remove(slot.myItemData);
                gameDataManager.currentGameData.OwnedItems.Add(slot.myItemData);
            }
            else
            {
                // zmizi hgraci obevi se obchodnikovi (prodavam)
                gameDataManager.currentGameData.OwnedItems.Remove(slot.myItemData);
                currentMerchant.currentInventory.Add(slot.myItemData);
            }
        }

        // pohyb penez
        if (currentCartMode == CartMode.Buying)
        {
            gameDataManager.currentGameData.player.numberOfCoins -= cartTotalSum;
            Debug.Log($"Koupeno! Zůstatek: {gameDataManager.currentGameData.player.numberOfCoins}");
        }
        else
        {
            gameDataManager.currentGameData.player.numberOfCoins += cartTotalSum;
            Debug.Log($"Prodáno! Zůstatek: {gameDataManager.currentGameData.player.numberOfCoins}");
        }

        // Value-based růst: Např. za každých 100 G přidá 1 bod reputace
        float repGain = cartTotalSum * 0.01f;
        ModifyReputation(repGain);
        
        gameDataManager.SaveData(); // ulozime do jsonu

        // reloadneme shop
        Debug.Log("Mažu věechny itemy v košíku");
        
        RemoveAllItemsFromCart();
        hagglePanel.SetActive(false);
        UpdateCartSum();
        RefreshShopUI();
    }
    
    private MerchantReputation GetCurrentMerchantReputation()
    {
        // Místo currentMerchant.merchantID použijeme občanku
        NPCController merchantIDCard = currentMerchant.GetComponent<NPCController>();
        if (merchantIDCard == null || string.IsNullOrEmpty(merchantIDCard.uniqueID)) return null;

        MerchantReputation rep = gameDataManager.currentGameData.merchantReputations
            .FirstOrDefault(r => r.merchantID == merchantIDCard.uniqueID);
        
        // kdyz poprve tak zakladame reputaci
        if (rep == null)
        {
            rep = new MerchantReputation();
            rep.merchantID = currentMerchant.merchantID;
            rep.reputationValue = 50f; 
            gameDataManager.currentGameData.merchantReputations.Add(rep);
        }
        return rep;
    }

    private void ModifyReputation(float amount)
    {
        MerchantReputation rep = GetCurrentMerchantReputation();
        if (rep != null)
        {
            rep.reputationValue += amount;
            // min 0 max 100
            rep.reputationValue = Mathf.Clamp(rep.reputationValue, 0f, 100f);
            Debug.Log($"Reputace upravena o {amount}. Aktuální: {rep.reputationValue}/100");
        }
    }
}