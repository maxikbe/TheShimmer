using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CodexUIManager : MonoBehaviour
{
    public static CodexUIManager Instance;

    [Header("Odkazy na Systém")]
    public Database itemDatabase;
    public MobDatabase mobDatabase;
    public PlantDatabase plantDatabase;
    public List<PotionRecipe> allPotionRecipes; // Sem v Inspektoru přetáhneš všechny své recepty!

    [Header("Hlavní Okno")]
    public GameObject codexCanvas;

    [Header("Tlačítka (Záložky)")]
    public Button tabBestiaryBtn;
    public Button tabHerbariumBtn;
    public Button tabSamplesBtn;
    public Button tabRecipesBtn;

    [Header("Stránky (Panely)")]
    public GameObject pageBestiary;
    public GameObject pageHerbarium;
    public GameObject pageSamples;
    public GameObject pageRecipes;

    [Header("=== BESTIÁŘ: LEVÁ STRANA ===")]
    public Transform bestiaryListContainer;
    public GameObject bestiaryEntryPrefab;

    [Header("=== BESTIÁŘ: PRAVÁ STRANA ===")]
    public GameObject bestiaryDetailPanel;
    public TextMeshProUGUI detailMobNameText;
    public Image detailMobLargeImage; 
    public TextMeshProUGUI detailMobLoreText;
    public Transform detailDropsContainer;
    public GameObject detailDropItemPrefab;
    
    [Header("=== HERBÁŘ: LEVÁ STRANA ===")]
    public Transform herbariumListContainer;
    public GameObject herbariumEntryPrefab;

    [Header("=== HERBÁŘ: PRAVÁ STRANA ===")]
    public GameObject herbariumDetailPanel;
    public TextMeshProUGUI detailPlantNameText;
    public Image detailPlantLargeImage;
    public TextMeshProUGUI detailPlantLoreText;
    public Transform detailPlantDropsContainer;

    [Header("=== VZORKOVNÍK: LEVÁ STRANA ===")]
    public Transform samplesListContainer;
    public GameObject sampleEntryPrefab;

    [Header("=== VZORKOVNÍK: PRAVÁ STRANA ===")]
    public GameObject sampleDetailPanel;
    public TextMeshProUGUI detailSampleNameText;
    public Image detailSampleLargeImage;
    public TextMeshProUGUI detailSampleStatusText;
    public TextMeshProUGUI detailSampleStatsText;
    public Transform detailSampleSourcesContainer; // "Získává se z:"
    public Transform detailSampleUsesContainer;    // "Využití v alchymii:"
    
    
    
    [Header("=== RECEPTY: LEVÁ STRANA ===")]
    public Transform recipesListContainer;
    public GameObject recipeEntryPrefab;

    [Header("=== RECEPTY: PRAVÁ STRANA ===")]
    public GameObject recipeDetailPanel;
    public TextMeshProUGUI detailRecipeNameText;
    public Image detailRecipeLargeImage;
    public TextMeshProUGUI detailRecipeStatsText;
    public Transform detailRecipeIngredientsContainer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (tabBestiaryBtn != null) tabBestiaryBtn.onClick.AddListener(() => SwitchTab(0));
        if (tabHerbariumBtn != null) tabHerbariumBtn.onClick.AddListener(() => SwitchTab(1));
        if (tabSamplesBtn != null) tabSamplesBtn.onClick.AddListener(() => SwitchTab(2));
        if (tabRecipesBtn != null) tabRecipesBtn.onClick.AddListener(() => SwitchTab(3));

        CloseCodex();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (codexCanvas.activeSelf) CloseCodex();
            else OpenCodex();
        }
    }

    public void OpenCodex()
    {
        codexCanvas.SetActive(true);
        Time.timeScale = 0f;
        SwitchTab(0);
    }

    public void CloseCodex()
    {
        codexCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SwitchTab(int tabIndex)
    {
        pageBestiary.SetActive(false);
        pageHerbarium.SetActive(false);
        pageSamples.SetActive(false);
        pageRecipes.SetActive(false);

        switch (tabIndex)
        {
            case 0:
                pageBestiary.SetActive(true);
                RefreshBestiary(); 
                break;
            case 1:
                pageHerbarium.SetActive(true);
                RefreshHerbarium(); 
                break;
            case 2:
                pageSamples.SetActive(true);
                RefreshSamples(); 
                break;
            case 3:
                pageRecipes.SetActive(true);
                RefreshRecipes(); 
                break;
        }
    }

    // ==========================================
    // LOGIKA PRO BESTIÁŘ
    // ==========================================
    private void RefreshBestiary()
    {
        foreach (Transform child in bestiaryListContainer) Destroy(child.gameObject);
        bestiaryDetailPanel.SetActive(false);

        if (gameDataManager.currentGameData == null) return;

        foreach (MobType unlockedMob in gameDataManager.currentGameData.unlockedBestiary)
        {
            if (unlockedMob == MobType.None) continue;
            GameObject newEntry = Instantiate(bestiaryEntryPrefab, bestiaryListContainer);
            BestiaryEntry entryScript = newEntry.GetComponent<BestiaryEntry>();
            if (entryScript != null) entryScript.Setup(unlockedMob, this);
        }
    }

    public void ShowBestiaryDetails(MobType mob)
    {
        bestiaryDetailPanel.SetActive(true);
        MobData visualData = mobDatabase.GetMobData(mob);
    
        if (visualData != null)
        {
            detailMobNameText.text = visualData.displayName;
            detailMobLoreText.text = visualData.description; 
            detailMobLargeImage.sprite = visualData.codexSprite;
            detailMobLargeImage.color = Color.white;
        }
        else
        {
            detailMobNameText.text = mob.ToString();
            detailMobLargeImage.color = new Color(1, 1, 1, 0); 
        }

        foreach (Transform child in detailDropsContainer) Destroy(child.gameObject);

        foreach (Item item in itemDatabase.GetAllItems())
        {
            if (item.originMobs != null && item.originMobs.Contains(mob)) 
            {
                SpawnDropItemUI(item, detailDropsContainer);
            }
        }
    }

    // ==========================================
    // LOGIKA PRO HERBÁŘ
    // ==========================================
    private void RefreshHerbarium()
    {
        foreach (Transform child in herbariumListContainer) Destroy(child.gameObject);
        herbariumDetailPanel.SetActive(false);

        if (gameDataManager.currentGameData == null) return;

        foreach (PlantType unlockedPlant in gameDataManager.currentGameData.unlockedHerbarium)
        {
            if (unlockedPlant == PlantType.None) continue;
            GameObject newEntry = Instantiate(herbariumEntryPrefab, herbariumListContainer);
            HerbariumEntry entryScript = newEntry.GetComponent<HerbariumEntry>();
            if (entryScript != null) entryScript.Setup(unlockedPlant, this, plantDatabase);
        }
    }

    public void ShowPlantDetails(PlantType plant)
    {
        herbariumDetailPanel.SetActive(true);
        PlantData visualData = plantDatabase.GetPlantData(plant);
    
        if (visualData != null)
        {
            detailPlantNameText.text = visualData.displayName;
            detailPlantLoreText.text = visualData.description; 
            detailPlantLargeImage.sprite = visualData.codexSprite;
            detailPlantLargeImage.color = Color.white;
        }
        else
        {
            detailPlantNameText.text = plant.ToString();
            detailPlantLargeImage.color = new Color(1, 1, 1, 0); 
        }

        foreach (Transform child in detailPlantDropsContainer) Destroy(child.gameObject);

        foreach (Item item in itemDatabase.GetAllItems())
        {
            if (item.originPlants != null && item.originPlants.Contains(plant)) 
            {
                SpawnDropItemUI(item, detailPlantDropsContainer);
            }
        }
    }

    // Univerzální spawn dropů z monster/kytek s proklikem do Vzorkovníku
    private void SpawnDropItemUI(Item itemToSpawn, Transform targetContainer)
    {
        GameObject dropSlot = Instantiate(detailDropItemPrefab, targetContainer);
        Image icon = dropSlot.transform.Find("Icon").GetComponent<Image>();
        icon.sprite = itemToSpawn.icon;
        
        TextMeshProUGUI dropName = dropSlot.GetComponentInChildren<TextMeshProUGUI>();
        if (dropName != null) dropName.text = itemToSpawn.itemName;

        Button btn = dropSlot.GetComponent<Button>();
        if (btn != null && itemToSpawn.itemType == ItemType.Sample)
        {
            btn.onClick.AddListener(() => {
                OpenSampleInSampler(itemToSpawn);
            });
        }
    }

    // ==========================================
    // LOGIKA PRO VZORKOVNÍK (Nové)
    // ==========================================
    private void RefreshSamples()
    {
        foreach (Transform child in samplesListContainer) Destroy(child.gameObject);
        sampleDetailPanel.SetActive(false);

        if (gameDataManager.currentGameData == null) return;

        List<int> encounteredSampleIDs = new List<int>();

        foreach (ItemSaveData owned in gameDataManager.currentGameData.OwnedItems)
        {
            if (!encounteredSampleIDs.Contains(owned.id)) encounteredSampleIDs.Add(owned.id);
        }
        
        foreach (int resId in gameDataManager.currentGameData.unlockedResearches)
        {
            if (!encounteredSampleIDs.Contains(resId)) encounteredSampleIDs.Add(resId);
        }

        foreach (int id in encounteredSampleIDs)
        {
            Item item = itemDatabase.GetItemByID(id);
            if (item != null && item.itemType == ItemType.Sample)
            {
                GameObject newEntry = Instantiate(sampleEntryPrefab, samplesListContainer);
                SampleEntry entryScript = newEntry.GetComponent<SampleEntry>();
                if (entryScript != null) entryScript.Setup(item, this);
            }
        }
    }

    public void OpenSampleInSampler(Item sampleItem)
    {
        SwitchTab(2); // Přepne na záložku Vzorkovníku
        sampleDetailPanel.SetActive(true);

        detailSampleNameText.text = sampleItem.itemName;
        if (sampleItem.icon != null)
        {
            detailSampleLargeImage.sprite = sampleItem.icon;
            detailSampleLargeImage.color = Color.white;
        }

        foreach (Transform child in detailSampleSourcesContainer) Destroy(child.gameObject);
        foreach (Transform child in detailSampleUsesContainer) Destroy(child.gameObject);

        bool isResearched = gameDataManager.currentGameData != null && 
                            gameDataManager.currentGameData.unlockedResearches.Contains(sampleItem.id); 

        if (isResearched)
        {
            detailSampleStatusText.text = "<color=green>STAV: VYZKOUMÁNO</color>";
            
            string stats = "Účinky v lektvarech:\n";
            if (sampleItem.potionHeal > 0) stats += $"+{sampleItem.potionHeal} Doplňuje HP\n"; 
            if (sampleItem.potionAditionalHealth > 0) stats += $"+{sampleItem.potionAditionalHealth} Bonus HP\n"; 
            if (sampleItem.potionBonusStamina > 0) stats += $"+{sampleItem.potionBonusStamina} Max Stamina\n"; 
            detailSampleStatsText.text = stats;

            if (sampleItem.originMobs != null)
            {
                foreach(MobType m in sampleItem.originMobs) 
                {
                    MobData md = mobDatabase.GetMobData(m); 
                    if (md != null) SpawnVisualBox(md.displayName, md.codexSprite, detailSampleSourcesContainer);
                }
            }
            if (sampleItem.originPlants != null)
            {
                foreach(PlantType p in sampleItem.originPlants) 
                {
                    PlantData pd = plantDatabase.GetPlantData(p); 
                    if (pd != null) SpawnVisualBox(pd.displayName, pd.codexSprite, detailSampleSourcesContainer);
                }
            }

            foreach (PotionRecipe recipe in allPotionRecipes)
            {
                if (recipe.requiredIngredients != null && recipe.requiredIngredients.Contains(sampleItem))
                {
                    if (recipe.resultPotion != null)
                    {
                        SpawnVisualBox(recipe.resultPotion.itemName, recipe.resultPotion.icon, detailSampleUsesContainer);
                    }
                }
            }
        }
        else
        {
            detailSampleStatusText.text = "<color=red>STAV: NEZNÁMÁ TKÁŇ (Vyžaduje mikroskopickou analýzu)</color>";
            detailSampleStatsText.text = "Chemické složení: ???\nPotenciální efekty: ???";
        }
    }

    private void SpawnVisualBox(string boxName, Sprite boxIcon, Transform targetContainer)
    {
        GameObject box = Instantiate(detailDropItemPrefab, targetContainer);
        Image icon = box.transform.Find("Icon").GetComponent<Image>();
        if (boxIcon != null) 
        {
            icon.sprite = boxIcon;
            icon.color = Color.white;
        }
        else 
        {
            icon.color = new Color(1, 1, 1, 0); 
        }

        TextMeshProUGUI text = box.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null) text.text = boxName;

        Button btn = box.GetComponent<Button>();
        if (btn != null) Destroy(btn); 
    }

    // ==========================================
    // LOGIKA PRO RECEPTY (The Annihilation Puzzle)
    // ==========================================
    private void RefreshRecipes()
    {
        foreach (Transform child in recipesListContainer) Destroy(child.gameObject);
        recipeDetailPanel.SetActive(false);

        if (gameDataManager.currentGameData == null) return;

        foreach (PotionRecipe recipe in allPotionRecipes)
        {
            int totalSamples = 0;
            int researchedSamples = 0;

            // 1. Zjistíme stav vzorků v receptu
            foreach (Item ingredient in recipe.requiredIngredients)
            {
                if (ingredient.itemType == ItemType.Sample)
                {
                    totalSamples++;
                    if (gameDataManager.currentGameData.unlockedResearches.Contains(ingredient.id))
                    {
                        researchedSamples++;
                    }
                }
            }

            // 2. Pokud jsme vyzkoumali alespoň 1 vzorek, ukážeme recept v seznamu
            if (totalSamples > 0 && researchedSamples > 0)
            {
                bool isFullyDiscovered = (totalSamples == researchedSamples);
                
                GameObject newEntry = Instantiate(recipeEntryPrefab, recipesListContainer);
                RecipeEntry entryScript = newEntry.GetComponent<RecipeEntry>();
                if (entryScript != null) entryScript.Setup(recipe, this, isFullyDiscovered);
            }
        }
    }

    public void ShowRecipeDetails(PotionRecipe recipe)
    {
        recipeDetailPanel.SetActive(true);

        int totalSamples = 0;
        int researchedSamples = 0;

        foreach (Item ingredient in recipe.requiredIngredients)
        {
            if (ingredient.itemType == ItemType.Sample)
            {
                totalSamples++;
                if (gameDataManager.currentGameData.unlockedResearches.Contains(ingredient.id))
                    researchedSamples++;
            }
        }

        bool isFullyDiscovered = (totalSamples == researchedSamples);

        // Vyčistíme kontejner na ingredience
        foreach (Transform child in detailRecipeIngredientsContainer) Destroy(child.gameObject);

        if (isFullyDiscovered)
        {
            // MÁME VŠE - UKÁŽEME VŠE!
            detailRecipeNameText.text = recipe.resultPotion.itemName;
            detailRecipeLargeImage.sprite = recipe.resultPotion.icon;
            detailRecipeLargeImage.color = Color.white;
            detailRecipeStatsText.text = "Plně objevený recept.\nVšechny biologické složky stabilizovány.";

            foreach (Item ingredient in recipe.requiredIngredients)
            {
                // Zde můžeme použít starou funkci z Vzorkovníku a ukázat normálně i s proklikem
                SpawnDropItemUI(ingredient, detailRecipeIngredientsContainer);
            }
        }
        else
        {
            // ČÁSTEČNÝ VÝZKUM - TAJÍME DATA!
            detailRecipeNameText.text = "Neznámý lektvar";
            detailRecipeLargeImage.sprite = recipe.resultPotion.icon;
            detailRecipeLargeImage.color = Color.black; // Zatmavíme hlavní obrázek
            detailRecipeStatsText.text = "Recept je nekompletní. Objevte další vzorky z mutovaných organizmů pro dokončení syntézy.";

            foreach (Item ingredient in recipe.requiredIngredients)
            {
                if (ingredient.itemType == ItemType.Sample)
                {
                    if (gameDataManager.currentGameData.unlockedResearches.Contains(ingredient.id))
                    {
                        // Tento vzorek známe - ukážeme ho normálně
                        SpawnDropItemUI(ingredient, detailRecipeIngredientsContainer);
                    }
                    else
                    {
                        // Tento vzorek NEZNÁME - ukážeme černou siluetu a schováme název
                        SpawnVisualBox("???", ingredient.icon, detailRecipeIngredientsContainer);
                        
                        // Najdeme ten vygenerovaný box a přebarvíme ho na černo
                        Transform lastBox = detailRecipeIngredientsContainer.GetChild(detailRecipeIngredientsContainer.childCount - 1);
                        Image icon = lastBox.Find("Icon").GetComponent<Image>();
                        if (icon != null) icon.color = Color.black;
                    }
                }
                // Pokud to není vzorek (je to uhlí, voda, atd.) a recept není hotový -> VŮBEC HO NEVYKRESLÍME (Skryto)
            }
        }
    }
}