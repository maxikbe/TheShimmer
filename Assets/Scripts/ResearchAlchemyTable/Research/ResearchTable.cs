using UnityEngine;

public class ResearchTable : MonoBehaviour
{
    [Header("Stav zkoumání (Pro UI)")]
    public bool isResearching = false;
    public bool isFinished = false;
    public float remainingTimeSeconds = 0f;

    [Header("Aktuální Vzorek na stole")]
    public ItemSaveData currentSampleData; 
    public Item currentSampleStaticData;

    private void Update()
    {
        // Klasický odpočet, běží jen když se něco reálně zkoumá
        if (isResearching && remainingTimeSeconds > 0)
        {
            remainingTimeSeconds -= Time.deltaTime;

            if (remainingTimeSeconds <= 0)
            {
                FinishResearch();
            }
        }
    }

    // ==========================================
    // TVOJE FUNKCE (VOLÁ TVŮJ CANVAS)
    // ==========================================

    public void InsertSampleToResearch(ItemSaveData sampleSaveData, Item sampleStaticData)
    {
        // 1. Ochrana: Stůl už pracuje nebo čeká na vybrání
        if (isResearching || isFinished)
        {
            Debug.LogWarning("Kokkotte, stůl už něco dělá nebo je plný!");
            return;
        }

        // 2. Ochrana: Není to Sample
        if (sampleStaticData.itemType != ItemType.Sample)
        {
            Debug.LogWarning("Tohle není vzorek k vyzkoumání!");
            return;
        }

        // 3. Ochrana: Je to už vyzkoumané? (Kontrolujeme JSON Master Save)
        if (gameDataManager.currentGameData.unlockedResearches.Contains(sampleStaticData.id)) 
        {
            Debug.Log($"Vzorek {sampleStaticData.itemName} už máš vyzkoumaný! (Tady hoď UI popup)");
            return;
        }

        // Vše OK -> Zahajujeme proces
        currentSampleData = sampleSaveData;
        currentSampleStaticData = sampleStaticData;
        
        // Převod minut na sekundy
        remainingTimeSeconds = sampleStaticData.researchTimeMinutes * 60f; 
        
        isResearching = true;
        isFinished = false;

        Debug.Log($"Začínám zkoumat {sampleStaticData.itemName}. Bude to trvat {sampleStaticData.researchTimeMinutes} minut.");
        
        // Odebereme item z hráčova inventáře (aby ho nenaklonoval)
        gameDataManager.currentGameData.OwnedItems.Remove(sampleSaveData);
        gameDataManager.SaveData();
    }

    private void FinishResearch()
    {
        isResearching = false;
        isFinished = true;
        remainingTimeSeconds = 0f;
        
        // Zápis do historie: Tohle ID je odteď vyzkoumané pro celou hru
        if (!gameDataManager.currentGameData.unlockedResearches.Contains(currentSampleStaticData.id))
        {
            gameDataManager.currentGameData.unlockedResearches.Add(currentSampleStaticData.id);
            gameDataManager.SaveData(); // Uložíme postup
        }
        
        Debug.Log($"Výzkum {currentSampleStaticData.itemName} dokončen! Teď znáš jeho staty.");
    }

    // ==========================================
    // FUNKCE PRO KÁMOŠE (VOLÁ JEHO 3D UI / INTERAKCE)
    // ==========================================

    // Hráč zvedne stůl BĚHEM výzkumu (vrátíme nevyzkoumaný vzorek)
    public ItemSaveData CancelAndRetrieveSample()
    {
        if (!isResearching) return null;

        isResearching = false;
        ItemSaveData retrievedData = currentSampleData;
        
        // Reset stolu
        currentSampleData = null;
        currentSampleStaticData = null;
        remainingTimeSeconds = 0f;

        return retrievedData; // Kámošův skript tohle musí vzít a hodit zpět do inventáře
    }

    // Hráč klikne na stůl a výzkum je HOTOVÝ (vyzvedne si item)
    public ItemSaveData CollectFinishedSample()
    {
        if (!isFinished) return null;

        isFinished = false;
        ItemSaveData retrievedData = currentSampleData;
        
        // Reset stolu
        currentSampleData = null;
        currentSampleStaticData = null;

        return retrievedData;
    }

    // Kámoš si tohle zavolá pro zobrazení nad stolem
    public string GetFormattedTimeRemaining()
    {
        if (!isResearching) return "00:00";
        
        int minutes = Mathf.FloorToInt(remainingTimeSeconds / 60F);
        int seconds = Mathf.FloorToInt(remainingTimeSeconds - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}