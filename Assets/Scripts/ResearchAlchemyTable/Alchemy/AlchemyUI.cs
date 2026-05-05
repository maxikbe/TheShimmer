using System.Collections.Generic;
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
    public GameObject dragSlotPrefab; 

    [Header("Info Panel (Tooltip dole)")]
    public Image tooltipIcon; 
    public TextMeshProUGUI tooltipNameText;
    public TextMeshProUGUI tooltipDescText;

    [Header("Pravá Strana - Hmoždíř (Mortar)")]
    public GameObject pestleObject; 
    public Image mortarItemIcon;
    public Slider crushProgressBar;
    // SMAZÁNO: collectMortarButton
    
    [Header("Pravá Strana - Kotlík a Plyn")]
    public TextMeshProUGUI gasRemainingText;
    public TextMeshProUGUI boilTimeText;
    public Button toggleGasButton;
    public TextMeshProUGUI toggleGasButtonText;

    [Header("Pravá Strana - Kohoutek a Lahvička")]
    public Image flaskSlotIcon; 
    public Button faucetButton; // Kohoutek
    public List<PotionRecipe> allRecipes; // Sem v Unity přetáhneš všechny své recepty!


    private void Start()
    {
        HideTooltip(); 
        
        // Připojení tlačítek
        if (toggleGasButton != null) toggleGasButton.onClick.AddListener(ToggleGas);
        if (faucetButton != null) faucetButton.onClick.AddListener(OnFaucetClicked);
    }
    
    private void Update()
    {
        // UI se musí updatovat každý frame, aby byl vidět čas a ubývání plynu
        if (currentTable != null && currentTable.isBoiling)
        {
            UpdateCauldronUI();
        }
    }

    public void OpenAlchemy(LabTable tableToUse)
    {
        currentTable = tableToUse; 
        if (alchemyScreenPanel != null) alchemyScreenPanel.SetActive(true);
        RefreshInventory();
        UpdateMortarUI();
        UpdateCauldronUI();
    }

    public void CloseAlchemy()
    {
        if (alchemyScreenPanel != null) alchemyScreenPanel.SetActive(false);
        currentTable = null;
        HideTooltip();
    }

    public void RefreshInventory()
    {
        foreach (Transform child in inventoryContainer) Destroy(child.gameObject);
        if (gameDataManager.currentGameData == null) return;

        foreach (ItemSaveData saveData in gameDataManager.currentGameData.OwnedItems)
        {
            if (!saveData.isOwned) continue;

            Item staticData = database.GetItemByID(saveData.id);
            if (staticData == null || !staticData.canBeUsedInAlchemy) continue;

            GameObject slot = Instantiate(dragSlotPrefab, inventoryContainer);
            Transform iconTransform = slot.transform.Find("Icon");
            if (iconTransform != null && staticData.icon != null) 
            {
                iconTransform.GetComponent<Image>().sprite = staticData.icon;
            }

            DraggableItem dragScript = slot.AddComponent<DraggableItem>();
            dragScript.Setup(saveData, staticData, this);
        }
    }

    public void ShowTooltip(Item item)
    {
        if (tooltipNameText != null) tooltipNameText.text = item.itemName;
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
                tooltipDescText.text = "Již nadrceno nebo tekuté. Tohle už je připravené do kotlíku.";
        }
    }

    public void HideTooltip()
    {
        if (tooltipNameText != null) tooltipNameText.text = "";
        if (tooltipDescText != null) tooltipDescText.text = "";
        if (tooltipIcon != null) tooltipIcon.enabled = false; 
    }

    public void DropItemIntoMortar(ItemSaveData dropData, Item dropStatic)
    {
        currentTable.mortarItemData = dropData;
        currentTable.mortarItemStatic = dropStatic;
        currentTable.currentCrushes = 0;

        gameDataManager.currentGameData.OwnedItems.Remove(dropData);
        gameDataManager.SaveData();

        HideTooltip(); 
        RefreshInventory();
        UpdateMortarUI();
    }

    public void UpdateMortarUI()
    {
        if (currentTable == null) return;

        if (currentTable.mortarItemData == null || currentTable.mortarItemStatic == null)
        {
            mortarItemIcon.color = new Color(1, 1, 1, 0); 
            if (crushProgressBar != null) crushProgressBar.value = 0;
            if (pestleObject != null) pestleObject.SetActive(false);
        }
        else
        {
            mortarItemIcon.sprite = currentTable.mortarItemStatic.icon;
            mortarItemIcon.color = new Color(1, 1, 1, 1); 
            mortarItemIcon.enabled = true; 

            float progress = (float)currentTable.currentCrushes / currentTable.mortarItemStatic.requiredCrushes;
            if (crushProgressBar != null) crushProgressBar.value = progress;

            if (!currentTable.mortarItemStatic.isCrushable)
            {
                if (pestleObject != null) pestleObject.SetActive(false);
            }
            else
            {
                if (pestleObject != null) pestleObject.SetActive(true);
            }
        }
    }

    public void ManualCrush() 
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

    // ZMĚNA: Tohle je teď PUBLIC a volá to náš DropZone inventáře!
    public void CollectFromMortar()
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
    
    // ==========================================
    // KOTLÍK: VHAZOVÁNÍ ITEMŮ
    // ==========================================
    public void DropItemIntoCauldron(ItemSaveData dropData, Item dropStatic)
    {
        currentTable.cauldronItemsData.Add(dropData);
        currentTable.cauldronItemsStatic.Add(dropStatic);

        gameDataManager.currentGameData.OwnedItems.Remove(dropData);
        gameDataManager.SaveData();

        RefreshInventory();
    }

    // ==========================================
    // KOTLÍK: PLYN A ČAS
    // ==========================================
    private void ToggleGas()
    {
        if (currentTable == null) return;
        
        if (gameDataManager.currentGameData.player.gasSecondsLeft <= 0)
        {
            Debug.Log("Nemáš žádný plyn!");
            return;
        }

        currentTable.isBoiling = !currentTable.isBoiling;
        UpdateCauldronUI();
    }

    private void UpdateCauldronUI()
    {
        if (currentTable == null) return;

        // Plyn UI
        float gasLeft = gameDataManager.currentGameData.player.gasSecondsLeft;
        int gasMin = Mathf.FloorToInt(gasLeft / 60f);
        int gasSec = Mathf.FloorToInt(gasLeft % 60f);
        if (gasRemainingText != null) gasRemainingText.text = $"Plyn: {gasMin:00}:{gasSec:00}";

        // Čas vaření UI
        int boilMin = Mathf.FloorToInt(currentTable.boilTimeSeconds / 60f);
        int boilSec = Mathf.FloorToInt(currentTable.boilTimeSeconds % 60f);
        if (boilTimeText != null) boilTimeText.text = $"{boilMin:00}:{boilSec:00}";

        if (toggleGasButtonText != null) 
            toggleGasButtonText.text = currentTable.isBoiling ? "Vypnout hořák" : "Zapnout hořák";
            
        // Ikonka lahvičky pod kohoutkem
        if (currentTable.flaskItemStatic != null)
        {
            flaskSlotIcon.sprite = currentTable.flaskItemStatic.icon;
            flaskSlotIcon.color = new Color(1, 1, 1, 1);
        }
        else
        {
            flaskSlotIcon.color = new Color(1, 1, 1, 0); // Průhledná, když tam nic není
        }
    }

    // ==========================================
    // KOHOUTEK A LAHVIČKA
    // ==========================================
    public void DropFlaskUnderFaucet(ItemSaveData dropData, Item dropStatic)
    {
        currentTable.flaskItemData = dropData;
        currentTable.flaskItemStatic = dropStatic;

        gameDataManager.currentGameData.OwnedItems.Remove(dropData);
        gameDataManager.SaveData();

        RefreshInventory();
        UpdateCauldronUI();
    }

    // ==========================================
    // KOHOUTEK A VÝROBA LEKTVARU (S DEBUG LOGY)
    // ==========================================
    private void OnFaucetClicked()
    {
        Debug.Log("🚰 Kliknuto na kohoutek!");

        if (currentTable == null) 
        {
            Debug.LogError("❌ currentTable je NULL! Skript vůbec neví, u kterého jsi stolu.");
            return;
        }

        if (currentTable.cauldronItemsData.Count == 0)
        {
            Debug.LogWarning("⚠️ V kotlíku nejsou žádné ingredience. Přeci nebudeš stáčet horkou vodu!");
            return;
        }

        // Není lahvička -> Alert
        if (currentTable.flaskItemData == null)
        {
            Debug.Log("⚠️ Pod kohoutkem není lahvička, volám Alert pro vylití!");
            AlertManager.Instance.ShowAlert(
                "Pod kohoutkem chybí lahvička! Chceš obsah kotlíku vylít na zem?", 
                SpillCauldronOnGround
            );
            return;
        }

        Debug.Log("✅ Ingredience i lahvička jsou připraveny. Vypínám hořák a jdu vařit!");
        currentTable.isBoiling = false; // Vypneme plyn automaticky
        CheckRecipeAndFillFlask();
    }

    private void CheckRecipeAndFillFlask()
    {
        Debug.Log($"🔍 Začínám kontrolu. Počet načtených receptů v AlchemyUI: {allRecipes.Count}");
        Item resultItemStatic = null;

        foreach (PotionRecipe recipe in allRecipes)
        {
            Debug.Log($"🥣 Zkoumám recept: '{recipe.recipeName}'");
            Debug.Log($"⏱ Tvá doba varu: {currentTable.boilTimeSeconds}s | Recept vyžaduje: {recipe.minBoilTimeSeconds}s až {recipe.maxBoilTimeSeconds}s");

            // Kontrola času vaření
            if (currentTable.boilTimeSeconds >= recipe.minBoilTimeSeconds && 
                currentTable.boilTimeSeconds <= recipe.maxBoilTimeSeconds)
            {
                Debug.Log($"⏱ ČAS SEDÍ! Jdu kontrolovat ingredience pro '{recipe.recipeName}'...");
                
                // Kontrola ingrediencí 
                if (AreIngredientsMatching(recipe.requiredIngredients, currentTable.cauldronItemsStatic))
                {
                    Debug.Log($"🎉 BINGO! Ingredience sedí. Vznikne: {recipe.resultPotion.itemName}");
                    resultItemStatic = recipe.resultPotion;
                    break; 
                }
                else
                {
                    Debug.LogWarning($"❌ Ingredience nesedí s receptem '{recipe.recipeName}'.");
                }
            }
            else
            {
                Debug.LogWarning($"❌ Čas varu byl mimo limit pro recept '{recipe.recipeName}'.");
            }
        }

        // Pokud to nevyšlo, vznikne "Břečka"
        if (resultItemStatic == null)
        {
            Debug.LogWarning("☠️ Žádný recept nevyšel! Snažím se vytvořit Břečku (Failed Sludge)...");
            if (allRecipes.Count > 0 && allRecipes[0].failedSludge != null)
            {
                resultItemStatic = allRecipes[0].failedSludge; 
            }
            else
            {
                Debug.LogError("❌ Břečku se nepodařilo vytvořit! Nemáš v 1. receptu přiřazený item 'failedSludge'.");
            }
        }

        // Změníme lahvičku na výsledek
        if (resultItemStatic != null)
        {
            currentTable.flaskItemData.id = resultItemStatic.id;
            currentTable.flaskItemStatic = resultItemStatic;
            Debug.Log($"🧪 HOTŮVKO! V lahvičce je nyní: {resultItemStatic.itemName}");
        }

        // Vyčistíme kotel
        currentTable.ClearCauldron();
        UpdateCauldronUI();
    }

    // Bezpečnější porovnání přes ID předmětů místo celých objektů!
    private bool AreIngredientsMatching(List<Item> recipeReqs, List<Item> cauldronItems)
    {
        Debug.Log($"   -> Porovnávám: Recept vyžaduje {recipeReqs.Count} věcí, v kotli je {cauldronItems.Count} věcí.");
        
        if (recipeReqs.Count != cauldronItems.Count) return false;

        // Uděláme si seznam IDček toho, co potřebujeme
        List<int> tempReqIDs = new List<int>();
        foreach(Item i in recipeReqs) tempReqIDs.Add(i.id);

        foreach (Item item in cauldronItems)
        {
            if (tempReqIDs.Contains(item.id)) 
            {
                tempReqIDs.Remove(item.id); // Odškrtneme si nalezenou věc
            }
            else 
            {
                Debug.LogWarning($"   -> ❌ V kotli plave {item.itemName}, který do tohoto receptu vůbec nepatří!");
                return false;
            }
        }
        return tempReqIDs.Count == 0;
    }

    // ==========================================
    // VYLITÍ NA ZEM (Z Alertu)
    // ==========================================
    private void SpillCauldronOnGround()
    {
        if (currentTable != null)
        {
            currentTable.ClearCauldron();
            UpdateCauldronUI();
        }
    }

    
    public void CollectFlaskFromFaucet()
    {
        if (currentTable != null && currentTable.flaskItemData != null)
        {
            // Vezmeme data lahvičky (ať už je prázdná, nebo je v ní hotový potion)
            ItemSaveData retrievedData = currentTable.flaskItemData;
            gameDataManager.currentGameData.OwnedItems.Add(retrievedData);
            
            // Vymažeme ji zpod kohoutku
            currentTable.flaskItemData = null;
            currentTable.flaskItemStatic = null;

            gameDataManager.SaveData();
            
            RefreshInventory();
            UpdateCauldronUI(); // Tohle schová obrázek lahvičky
        }
    }
}