using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResearchUI : MonoBehaviour
{
    [Header("Hlavní Reference")]
    public Database database;
    public GameObject canvasPanel; 
    
    [Header("Obrazovka 1: Inventář (Když je stůl prázdný)")]
    public GameObject inventoryScreen; 
    public Transform sampleContainer; 
    public GameObject slotPrefab;     
    public Button toggleHideButton;
    public TextMeshProUGUI toggleHideText;
    
    [Header("Obrazovka 1: Detail a Vložit")]
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailDescText;
    public Image detailIcon;
    public Button insertButton; 

    [Header("Obrazovka 2: Aktivní Výzkum (Když stůl pracuje)")]
    public GameObject progressScreen; 
    public TextMeshProUGUI activeItemNameText;
    public Image activeItemIcon;
    public Slider progressBar; 
    public TextMeshProUGUI timeRemainingText;
    public TextMeshProUGUI percentageText;
    
    [Header("Obrazovka 2: Vyzvednutí a Zrušení")]
    public Button collectButton; 
    public TextMeshProUGUI collectButtonText;
    public Button cancelResearchButton; // NOVÉ: Tlačítko pro přerušení

    [Header("Alert Popup (Ověření zrušení)")]
    public GameObject alertPanel; // Celý panel vyskakovacího okna
    public Button alertYesButton; // Tlačítko "Ano, přerušit"
    public Button alertNoButton;  // Tlačítko "Ne, nechat zkoumat"

    private ResearchTable currentTable; 
    private bool hideResearched = false;

    private ItemSaveData selectedData;
    private Item selectedStatic;

    private void Start()
    {
        if (toggleHideButton != null) toggleHideButton.onClick.AddListener(ToggleHideResearched);
        if (insertButton != null) insertButton.onClick.AddListener(OnInsertClicked);
        if (collectButton != null) collectButton.onClick.AddListener(OnCollectClicked);
        
        // Napojení nových tlačítek
        if (cancelResearchButton != null) cancelResearchButton.onClick.AddListener(ShowAlert);
        if (alertYesButton != null) alertYesButton.onClick.AddListener(ConfirmCancelResearch);
        if (alertNoButton != null) alertNoButton.onClick.AddListener(HideAlert);
    }

    private void Update()
    {
        if (canvasPanel.activeSelf && currentTable != null && progressScreen.activeSelf)
        {
            UpdateProgressUI();
        }
    }

    public void OpenCanvas(ResearchTable table)
    {
        currentTable = table;
        canvasPanel.SetActive(true);
        HideAlert(); // Ujistíme se, že alert na začátku nesvítí
        
        if (currentTable.isResearching || currentTable.isFinished)
        {
            ShowProgressScreen();
        }
        else
        {
            ShowInventoryScreen();
        }
    }

    public void CloseCanvas()
    {
        canvasPanel.SetActive(false);
        HideAlert();
        currentTable = null;
    }

    // ==========================================
    // LOGIKA OBRAZOVKY 1: VÝBĚR A VLOŽENÍ
    // ==========================================

    private void ShowInventoryScreen()
    {
        inventoryScreen.SetActive(true);
        progressScreen.SetActive(false);
        ClearSelection();
        RefreshInventory();
    }

    private void ToggleHideResearched()
    {
        hideResearched = !hideResearched;
        toggleHideText.text = hideResearched ? "Zobrazit všechny" : "Skrýt vyzkoumané";
        RefreshInventory();
    }

    private void RefreshInventory()
    {
        foreach (Transform child in sampleContainer) Destroy(child.gameObject);
        if (gameDataManager.currentGameData == null) return;

        foreach (ItemSaveData saveData in gameDataManager.currentGameData.OwnedItems)
        {
            if (!saveData.isOwned) continue;

            // Kontrola databáze
            if (database == null) { Debug.LogError("Kokkotte, nemáš přiřazenou databázi v ResearchUI!"); return; }

            Item staticData = database.GetItemByID(saveData.id);
            if (staticData == null) 
            { 
                Debug.LogWarning($"Našel jsem saveData s ID {saveData.id}, ale v Databázi ten item není!"); 
                continue; 
            }

            if (staticData.itemType != ItemType.Sample) continue;

            bool isAlreadyResearched = gameDataManager.currentGameData.unlockedResearches.Contains(staticData.id);
            if (hideResearched && isAlreadyResearched) continue;

            GameObject slot = Instantiate(slotPrefab, sampleContainer);
            
            // POJISTKA PRO TEXT:
            var tmpro = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpro != null) 
            { 
                tmpro.text = staticData.itemName; 
            }
            else 
            { 
                Debug.LogError("Tvůj prefab ItemSlot nemá v sobě žádný TextMeshProUGUI!"); 
            }
            
            Transform iconTransform = slot.transform.Find("Icon");
            if (iconTransform != null && staticData.icon != null) 
            {
                iconTransform.GetComponent<Image>().sprite = staticData.icon;
            }

            if (isAlreadyResearched && tmpro != null)
            {
                tmpro.text += " (Hotovo)";
                tmpro.color = Color.gray;
            }

            // POJISTKA PRO TLAČÍTKO:
            Button btn = slot.GetComponentInChildren<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SelectSample(saveData, staticData, isAlreadyResearched));
            }
            else
            {
                Debug.LogError("Tvůj prefab ItemSlot nemá na sobě komponentu Button!");
            }
        }
    }

    private void SelectSample(ItemSaveData saveItem, Item staticItem, bool isAlreadyResearched)
    {
        selectedData = saveItem;
        selectedStatic = staticItem;

        detailNameText.text = staticItem.itemName;
        
        if (isAlreadyResearched)
            detailDescText.text = "Tenhle vzorek už máš zapsaný v deníku. Nemá smysl ho zkoumat znovu.";
        else
            detailDescText.text = $"Rarita: {staticItem.rarity}\nPotřebný čas: {staticItem.researchTimeMinutes} minut.";
        
        if (staticItem.icon != null) 
        {
            detailIcon.sprite = staticItem.icon;
            detailIcon.enabled = true;
        }

        insertButton.interactable = !isAlreadyResearched;
    }

    private void ClearSelection()
    {
        selectedData = null;
        selectedStatic = null;
        detailNameText.text = "Vyber vzorek";
        detailDescText.text = "Klikni na vzorek vlevo pro detaily.";
        detailIcon.enabled = false;
        insertButton.interactable = false;
    }

    private void OnInsertClicked()
    {
        if (selectedData != null && selectedStatic != null && currentTable != null)
        {
            currentTable.InsertSampleToResearch(selectedData, selectedStatic);
            ShowProgressScreen();
        }
    }

    // ==========================================
    // LOGIKA OBRAZOVKY 2: PROGRESS A VYZVEDNUTÍ
    // ==========================================

    private void ShowProgressScreen()
    {
        inventoryScreen.SetActive(false);
        progressScreen.SetActive(true);

        if (currentTable.currentSampleStaticData != null)
        {
            activeItemNameText.text = currentTable.currentSampleStaticData.itemName;
            if (currentTable.currentSampleStaticData.icon != null)
            {
                activeItemIcon.sprite = currentTable.currentSampleStaticData.icon;
                activeItemIcon.enabled = true;
            }
        }

        UpdateProgressUI(); 
    }

    private void UpdateProgressUI()
    {
        if (currentTable.isResearching)
        {
            float totalSeconds = currentTable.currentSampleStaticData.researchTimeMinutes * 60f;
            float elapsedSeconds = totalSeconds - currentTable.remainingTimeSeconds;
            float progressPercentage = elapsedSeconds / totalSeconds;
            
            if (progressBar != null) progressBar.value = progressPercentage;
            if (percentageText != null) percentageText.text = $"{Mathf.RoundToInt(progressPercentage * 100)}%";
            
            timeRemainingText.text = $"Zbývá: {currentTable.GetFormattedTimeRemaining()}";
            
            collectButton.interactable = false;
            collectButtonText.text = "Zkoumám...";
            
            // Zobrazíme tlačítko pro zrušení, protože výzkum běží
            if (cancelResearchButton != null) cancelResearchButton.gameObject.SetActive(true);
        }
        else if (currentTable.isFinished)
        {
            if (progressBar != null) progressBar.value = 1f;
            if (percentageText != null) percentageText.text = "100%";
            timeRemainingText.text = "Analýza dokončena.";
            
            collectButton.interactable = true;
            collectButtonText.text = "Vyzvednout Výsledek";
            
            // Schováme tlačítko pro zrušení, už je hotovo
            if (cancelResearchButton != null) cancelResearchButton.gameObject.SetActive(false);
        }
    }

    private void OnCollectClicked()
    {
        if (currentTable != null && currentTable.isFinished)
        {
            ItemSaveData finishedData = currentTable.CollectFinishedSample();
            if (finishedData != null)
            {
                gameDataManager.currentGameData.OwnedItems.Add(finishedData);
                gameDataManager.SaveData();
            }
            ShowInventoryScreen();
        }
    }

    // ==========================================
    // LOGIKA ALERTU (ZRUŠENÍ VÝZKUMU)
    // ==========================================

    private void ShowAlert()
    {
        if (alertPanel != null) alertPanel.SetActive(true);
    }

    private void HideAlert()
    {
        if (alertPanel != null) alertPanel.SetActive(false);
    }

    private void ConfirmCancelResearch()
    {
        HideAlert();
        
        if (currentTable != null && currentTable.isResearching)
        {
            // Vytáhneme nedodělaný vzorek
            ItemSaveData retrievedData = currentTable.CancelAndRetrieveSample();
            
            if (retrievedData != null)
            {
                // Vrátíme ho zpět do inventáře
                gameDataManager.currentGameData.OwnedItems.Add(retrievedData);
                gameDataManager.SaveData();
                Debug.Log("Výzkum zrušen! Vzorek byl vrácen do inventáře.");
            }
            
            // Přepneme zpět na výběr vzorků
            ShowInventoryScreen();
        }
    }
}