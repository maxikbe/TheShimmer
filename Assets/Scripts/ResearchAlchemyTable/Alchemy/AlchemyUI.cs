using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlchemyUI : MonoBehaviour
{
    [Header("Reference")]
    public Database database;
    public LabTable currentTable;
    
    [Header("UI Panely (Když je skript na GameManageru)")]
    public GameObject alchemyScreenPanel;

    [Header("Levá Strana - Inventář")]
    public Transform inventoryContainer;
    public GameObject dragSlotPrefab; // Sem dáš ten NOVÝ OŘEZANÝ prefab

    [Header("Info Panel (Tooltip dole)")]
    public Image tooltipIcon; // NOVÉ: Ikonka v tooltipu
    public TextMeshProUGUI tooltipNameText;
    public TextMeshProUGUI tooltipDescText;

    [Header("Pravá Strana - Hmoždíř (Mortar)")]
    public Image mortarItemIcon;
    public Button crushButton; 
    public TextMeshProUGUI crushButtonText;
    public Slider crushProgressBar;
    public Button collectMortarButton; 

    private void Start()
    {
        if (crushButton != null) crushButton.onClick.AddListener(CrushItem);
        if (collectMortarButton != null) collectMortarButton.onClick.AddListener(CollectFromMortar);
        HideTooltip(); 
    }

    // ZMĚNA: Tohle už není OnEnable(). Budeme to volat ručně!
    public void OpenAlchemy(LabTable tableToUse)
    {
        currentTable = tableToUse; 
        
        // Zapneme panel, ať je vidět
        if (alchemyScreenPanel != null) alchemyScreenPanel.SetActive(true);
        
        RefreshInventory();
        UpdateMortarUI();
    }

    public void CloseAlchemy()
    {
        if (alchemyScreenPanel != null) alchemyScreenPanel.SetActive(false);
        currentTable = null;
        HideTooltip();
    }

    private void RefreshInventory()
    {
        foreach (Transform child in inventoryContainer) Destroy(child.gameObject);
        if (gameDataManager.currentGameData == null) return;

        foreach (ItemSaveData saveData in gameDataManager.currentGameData.OwnedItems)
        {
            if (!saveData.isOwned) continue;

            Item staticData = database.GetItemByID(saveData.id);
            if (staticData == null) continue;

            if (!staticData.canBeUsedInAlchemy) continue;

            // Spawnujeme náš čistý prefab
            GameObject slot = Instantiate(dragSlotPrefab, inventoryContainer);
            
            Transform iconTransform = slot.transform.Find("Icon");
            if (iconTransform != null && staticData.icon != null) 
            {
                iconTransform.GetComponent<Image>().sprite = staticData.icon;
            }

            // Nahodíme mu skript a předáme mu item a referenci na sebe (this)
            DraggableItem dragScript = slot.AddComponent<DraggableItem>();
            dragScript.Setup(saveData, staticData, this);
        }
    }

    // ==========================================
    // TOOLTIP (Voláno z DraggableItem)
    // ==========================================
    public void ShowTooltip(Item item)
    {
        if (tooltipNameText != null) tooltipNameText.text = item.itemName;
        
        // Zobrazení ikonky v tooltipu
        if (tooltipIcon != null) 
        {
            tooltipIcon.sprite = item.icon;
            tooltipIcon.enabled = true;
        }

        if (tooltipDescText != null) 
        {
            if (item.isCrushable)
                tooltipDescText.text = $"Vyžaduje úderů: {item.requiredCrushes}\nKdyž do toho budeš mlátit, vznikne prach.";
            else
                tooltipDescText.text = "Již nadrceno. Tohle už je připravené do kotlíku.";
        }
    }

    public void HideTooltip()
    {
        if (tooltipNameText != null) tooltipNameText.text = "";
        if (tooltipDescText != null) tooltipDescText.text = "";
        if (tooltipIcon != null) tooltipIcon.enabled = false; // Schováme ikonku
    }

    // ==========================================
    // DROP ZONE (Voláno z MortarDropZone)
    // ==========================================
    public void DropItemIntoMortar(ItemSaveData dropData, Item dropStatic)
    {
        currentTable.mortarItemData = dropData;
        currentTable.mortarItemStatic = dropStatic;
        currentTable.currentCrushes = 0;

        gameDataManager.currentGameData.OwnedItems.Remove(dropData);
        gameDataManager.SaveData();

        HideTooltip(); // Reset po dropnutí
        RefreshInventory();
        UpdateMortarUI();
    }

    private void UpdateMortarUI()
    {
        if (currentTable == null) return;

        if (currentTable.mortarItemData == null || currentTable.mortarItemStatic == null)
        {
            // MÍSTO: mortarItemIcon.enabled = false;
            // DÁME TOHLE:
            mortarItemIcon.color = new Color(1, 1, 1, 0); // Úplně průhledná (neviditelná)
            
            if (crushProgressBar != null) crushProgressBar.value = 0;
            if (crushButton != null) crushButton.interactable = false;
            if (collectMortarButton != null) collectMortarButton.interactable = false;
        }
        else
        {
            // Nastavíme ikonu a vrátíme jí plnou viditelnost
            mortarItemIcon.sprite = currentTable.mortarItemStatic.icon;
            mortarItemIcon.color = new Color(1, 1, 1, 1); // Plně viditelná
            mortarItemIcon.enabled = true; // Pro jistotu, kdyby byla vypnutá z minula

            float progress = (float)currentTable.currentCrushes / currentTable.mortarItemStatic.requiredCrushes;
            if (crushProgressBar != null) crushProgressBar.value = progress;

            if (collectMortarButton != null) collectMortarButton.interactable = true;

            if (!currentTable.mortarItemStatic.isCrushable)
            {
                if (crushButton != null) crushButton.interactable = false;
                if (crushButtonText != null) crushButtonText.text = "Hotovo!";
            }
            else
            {
                if (crushButton != null) crushButton.interactable = true;
                if (crushButtonText != null) crushButtonText.text = $"Bouchej! ({currentTable.currentCrushes}/{currentTable.mortarItemStatic.requiredCrushes})";
            }
        }
    }

    private void CrushItem()
    {
        if (currentTable == null || currentTable.mortarItemData == null || !currentTable.mortarItemStatic.isCrushable) return;

        currentTable.currentCrushes++;

        if (currentTable.currentCrushes >= currentTable.mortarItemStatic.requiredCrushes)
        {
            if (currentTable.mortarItemStatic.crushedVersion != null)
            {
                currentTable.mortarItemData.id = currentTable.mortarItemStatic.crushedVersion.id;
                currentTable.mortarItemStatic = currentTable.mortarItemStatic.crushedVersion;
                currentTable.currentCrushes = 0; 
            }
        }

        UpdateMortarUI();
    }

    private void CollectFromMortar()
    {
        if (currentTable != null && currentTable.mortarItemData != null)
        {
            ItemSaveData retrievedData = currentTable.RetrieveMortarItem();
            
            gameDataManager.currentGameData.OwnedItems.Add(retrievedData);
            gameDataManager.SaveData();
            
            RefreshInventory();
            UpdateMortarUI();
        }
    }
}