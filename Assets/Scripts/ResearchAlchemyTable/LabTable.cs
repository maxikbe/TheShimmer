using UnityEngine;
using System.Collections.Generic;

public class LabTable : MonoBehaviour
{
    [Header("=== VÝZKUM (RESEARCH) ===")]
    public bool isResearching = false;
    public bool isFinished = false;
    public float remainingTimeSeconds = 0f;
    
    public ItemSaveData researchItemData; 
    public Item researchItemStatic;

    [Header("=== ALCHYMIE - HMOŽDÍŘ ===")]
    public ItemSaveData mortarItemData;
    public Item mortarItemStatic;
    public int currentCrushes = 0;

    [Header("=== ALCHYMIE - KOTLÍK (Příprava) ===")]
    public List<ItemSaveData> cauldronItemsData = new List<ItemSaveData>();
    public bool isBoiling = false;
    public float boilTimeSeconds = 0f;

    private void Update()
    {
        if (isResearching && remainingTimeSeconds > 0)
        {
            remainingTimeSeconds -= Time.deltaTime;
            if (remainingTimeSeconds <= 0)
            {
                FinishResearch();
            }
        }

        if (isBoiling)
        {
            boilTimeSeconds += Time.deltaTime;
        }
    }

    // ==========================================
    // FUNKCE PRO VÝZKUM (Přidáno)
    // ==========================================

    public void InsertSampleToResearch(ItemSaveData sampleSaveData, Item sampleStaticData)
    {
        if (isResearching || isFinished) return;

        researchItemData = sampleSaveData;
        researchItemStatic = sampleStaticData;
        remainingTimeSeconds = sampleStaticData.researchTimeMinutes * 60f; 
        isResearching = true;
        isFinished = false;

        gameDataManager.currentGameData.OwnedItems.Remove(sampleSaveData);
        gameDataManager.SaveData();
    }

    private void FinishResearch()
    {
        isResearching = false;
        isFinished = true;
        remainingTimeSeconds = 0f;
        
        if (!gameDataManager.currentGameData.unlockedResearches.Contains(researchItemStatic.id))
        {
            gameDataManager.currentGameData.unlockedResearches.Add(researchItemStatic.id);
            gameDataManager.SaveData(); 
        }
        Debug.Log($"Výzkum {researchItemStatic.itemName} dokončen!");
    }

    public ItemSaveData CancelAndRetrieveSample()
    {
        if (!isResearching) return null;

        isResearching = false;
        ItemSaveData retrievedData = researchItemData;
        researchItemData = null;
        researchItemStatic = null;
        remainingTimeSeconds = 0f;

        return retrievedData;
    }

    public ItemSaveData CollectFinishedSample()
    {
        if (!isFinished) return null;

        isFinished = false;
        ItemSaveData retrievedData = researchItemData;
        researchItemData = null;
        researchItemStatic = null;

        return retrievedData;
    }

    public string GetFormattedTimeRemaining()
    {
        if (!isResearching) return "00:00";
        int minutes = Mathf.FloorToInt(remainingTimeSeconds / 60F);
        int seconds = Mathf.FloorToInt(remainingTimeSeconds - minutes * 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // ==========================================
    // SDÍLENÉ FUNKCE (Pro sebrání stolu do inventáře)
    // ==========================================
    
    // Tuto funkci si zavolá kámoš, když sbalí stůl.
    // Vrátí to VŠECHNY itemy, co ve stole zrovna jsou.
    public List<ItemSaveData> RetrieveAllItemsAndReset()
    {
        List<ItemSaveData> rescuedItems = new List<ItemSaveData>();

        if (researchItemData != null) rescuedItems.Add(researchItemData);
        if (mortarItemData != null) rescuedItems.Add(mortarItemData);
        if (cauldronItemsData.Count > 0) rescuedItems.AddRange(cauldronItemsData);

        ResetTableData();
        return rescuedItems;
    }

    // Tato funkce bude volat kámoš zvenčí pro záchranu jen hmoždíře
    public ItemSaveData RetrieveMortarItem()
    {
        if (mortarItemData == null) return null;
        
        ItemSaveData retrievedData = mortarItemData;
        mortarItemData = null;
        mortarItemStatic = null;
        currentCrushes = 0;
        
        return retrievedData;
    }

    public void ResetTableData()
    {
        isResearching = false;
        isFinished = false;
        remainingTimeSeconds = 0f;
        researchItemData = null;
        researchItemStatic = null;

        mortarItemData = null;
        mortarItemStatic = null;
        currentCrushes = 0;

        cauldronItemsData.Clear();
        isBoiling = false;
        boilTimeSeconds = 0f;
    }
}