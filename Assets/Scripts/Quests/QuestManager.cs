using System.Collections.Generic;
using UnityEngine;
using TMPro; // Pro ty texty na obrazovce

public class QuestManager : MonoBehaviour
{
    // Tohle z něj udělá Singleton (přístupný odevšad přes QuestManager.Instance)
    public static QuestManager Instance;

    [Header("Databáze")]
    public QuestData[] allGameQuests; // Sem přetáhneš VŠECHNY své ScriptableObjecty questů

    [Header("Aktivní Questy")]
    public List<QuestData> activeQuests = new List<QuestData>();
    public QuestData trackedQuest; // Ten, který ti právě svítí na obrazovce vlevo nahoře

    [Header("UI Tracker (vlevo nahoře)")]
    public GameObject trackerPanel; // Celé to pozadí trackeru
    public TextMeshProUGUI trackerNameText; // Název questu
    public TextMeshProUGUI trackerObjectiveText; // Co máš zrovna udělat

    private void Awake()
    {
        // Ošetření Singletonu - aby se nám tenhle mozek nespawnul dvakrát
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Místo 'string questID' teď vyžadujeme přímo ten konkrétní ScriptableObject!
    public void StartQuest(QuestData questToStart)
    {
        // Už nemusíme nic složitě hledat v databázi. 
        // Rovnou se zeptáme toho konkrétního questu, jestli už není náhodou začatý.
        if (questToStart.currentState == QuestState.NotStarted)
        {
            questToStart.currentState = QuestState.Active; // Změníme stav
            activeQuests.Add(questToStart); // Přidáme ho do batohu aktivních úkolů
            
            TrackQuest(questToStart); // Hodíme ho do UI
            Debug.Log("Quest přijat: " + questToStart.questName);
        }
        else
        {
            Debug.LogWarning("Bacha! Quest " + questToStart.questName + " už máš nebo je hotový.");
        }
    }

    // Funkce, která aktualizuje ten text v levém horním rohu
    public void TrackQuest(QuestData quest)
    {
        trackedQuest = quest;
        trackerPanel.SetActive(true); // Ukážeme UI
        
        trackerNameText.text = quest.questName;

        // Najdeme první nesplněný krok (Step) a vypíšeme jeho zadání
        foreach (QuestStep step in quest.questSteps)
        {
            if (!step.isCompleted)
            {
                trackerObjectiveText.text = "- " + step.objectiveDescription;
                return; // Našli jsme, co jsme potřebovali, dál nehledáme
            }
        }
        
        // Pokud cyklus dojel až sem, znamená to, že všechny kroky jsou hotové
        trackerObjectiveText.text = "- Vrať se pro odměnu!";
    }
    
    // Tuhle funkci zavoláme, když chceme UI tracker úplně schovat
    public void UntrackQuest()
    {
        trackedQuest = null; // Vymažeme paměť
        trackerPanel.SetActive(false); // Vypneme to okno vlevo nahoře
    }
}