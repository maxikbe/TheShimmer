using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class QuestManager : MonoBehaviour
{
    // aby byl script pristupny odevsad
    public static QuestManager Instance;

    [Header("Databáze")]
    public QuestDatabase questDatabase; 

    [Header("Aktivní Questy")]
    public List<QuestData> activeQuests = new List<QuestData>();
    public QuestData trackedQuest; 

    [Header("UI Tracker (vlevo nahoře)")]
    public GameObject trackerPanel; 
    public TextMeshProUGUI trackerNameText; 
    public TextMeshProUGUI trackerObjectiveText;


    private void Start()
    {
        // Za předpokladu, že gameDataManager.currentGameData už existuje
        LoadQuestsFromData();
    }


    private void Awake()
    {
        // ošetření aby nebyl dvakrat
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartQuest(QuestData questToStart)
    {
        // zjistime jestli neni nahodou zacaty
        if (questToStart.currentState == QuestState.NotStarted)
        {
            questToStart.currentState = QuestState.Active; // aktivujeme
            activeQuests.Add(questToStart); // pridame do aktivnich
            
            TrackQuest(questToStart); // a dame trackovat
            Debug.Log("Quest přijat: " + questToStart.questName);
        }
        else
        {
            Debug.LogWarning("Quest " + questToStart.questName + " už máš nebo je hotový.");
        }
    }

    public void AddQuest(QuestData questToAdd)
    {
        if (questToAdd.currentState == QuestState.NotStarted)
        {
            questToAdd.currentState = QuestState.Active;
            activeQuests.Add(questToAdd);
            
            Debug.Log("Quest byl přidat do menu: " +  questToAdd.questName);
        }
        else
        {
            Debug.LogWarning("Quest " + questToAdd.questName + " už máš nebo je hotový.");
        }
    }

    // aktulizuje trackovaný quest
    public void TrackQuest(QuestData quest)
    {
        trackedQuest = quest;
        trackerPanel.SetActive(true); 
        
        trackerNameText.text = quest.questName;

        if (quest.questType == QuestType.MainQuest)
        {
            trackerNameText.color = new Color(1f, 0.8f, 0f); // zlatá pro main
            trackerNameText.fontStyle = FontStyles.Bold; // ztucneni
        }
        else
        {
            trackerNameText.color = Color.white; // bila pro side
            trackerNameText.fontStyle = FontStyles.Normal;
        }

        // najdeme prvni nesplneni step a vypiseme ho
        foreach (QuestStep step in quest.questSteps)
        {
            if (!step.isCompleted)
            {
                trackerObjectiveText.text = "- " + step.objectiveDescription;
                return; 
            }
        }
        
        //Pokud neni zadny dalsi
        trackerObjectiveText.text = "- Vrať se pro odměnu!";
    }
    
    // pokud chceme schovat trackované questy
    public void UntrackQuest()
    {
        trackedQuest = null; 
        trackerPanel.SetActive(false); 
    }
    
    
    // pokdu hrac neco dela
    public void AdvanceQuest(QuestData quest)
    {
        // najde prvni nesplneni krok
        foreach (QuestStep step in quest.questSteps)
        {
            if (!step.isCompleted)
            {
                // odfajfkne ho jako splneny
                step.isCompleted = true; 
                Debug.Log("Krok splněn: " + step.objectiveDescription);

                // PRO PRIDAVANI QUESTU UPROSTRED KROKU
                
                // pouze prida do journalu, netrackuje
                foreach(QuestData newQuest in step.questsToAddOnComplete)
                {
                    if(newQuest != null) AddQuest(newQuest); 
                }

                // pokud je nejaky nastaveni aby ho to zrova trackovalo
                if(step.questToStartOnComplete != null)
                {
                    StartQuest(step.questToStartOnComplete); // prida se a rovnou ukaze
                }
                // pokud tam nic neni tak se pouze updatne aktualni
                else if (trackedQuest == quest)
                {
                    TrackQuest(quest); 
                }

                
                return; 
            }
        }

        // pokud quest nenasel nic nesplneneho quest je hotovy
        Debug.Log("Quest " + quest.questName + " je už kompletně hotový.");
        quest.currentState = QuestState.Completed;
        
        // tady kdyztak dalsi funkce pro odmeny
    }
    
    // Tuto metodu zavoláš PŘEDTÍM, než se zavolá samotné ukládání do JSONu
    public void SaveQuestsToData()
    {
        gameDataManager.currentGameData.savedQuests.Clear();

        // Projdeme všechny questy v databázi
        foreach (QuestData quest in questDatabase.allQuests) // <-- "allQuests" si uprav podle tvé databáze
        {
            // Ukládáme jen ty, které už hráč začal nebo dokončil
            if (quest.currentState != QuestState.NotStarted)
            {
                QuestSaveData qData = new QuestSaveData();
                qData.questID = quest.questID;
                qData.currentState = quest.currentState;
                
                // Uložíme postup jednotlivých kroků
                foreach (QuestStep step in quest.questSteps)
                {
                    qData.stepsCompleted.Add(step.isCompleted);
                }
                
                gameDataManager.currentGameData.savedQuests.Add(qData);
            }
        }

        // Uložíme si, co hráč zrovna trackuje
        if (trackedQuest != null)
            gameDataManager.currentGameData.trackedQuestID = trackedQuest.questID;
        else
            gameDataManager.currentGameData.trackedQuestID = "";
    }

    // Tuto metodu zavoláš POTÉ, co se JSON načte do currentGameData
    public void LoadQuestsFromData()
    {
        activeQuests.Clear();
        UntrackQuest();

        // DŮLEŽITÉ: ScriptableObjecty si v Unity Editoru pamatují stav mezi zapnutím hry. 
        // Musíme je všechny natvrdo resetovat, než na ně aplikujeme uložená data.
        foreach (QuestData quest in questDatabase.allQuests)
        {
            quest.currentState = QuestState.NotStarted;
            foreach (QuestStep step in quest.questSteps) step.isCompleted = false;
        }

        // Pokud nemáme žádná data (nová hra), končíme
        if (gameDataManager.currentGameData.savedQuests == null) return;

        // Načítání z JSON dat
        foreach (QuestSaveData qData in gameDataManager.currentGameData.savedQuests)
        {
            // Najdeme odpovídající quest v databázi podle unikátního ID
            QuestData quest = questDatabase.GetQuestByID(qData.questID);
            if (quest != null)
            {
                quest.currentState = qData.currentState;
                
                // Nahrajeme stav kroků
                for (int i = 0; i < qData.stepsCompleted.Count; i++)
                {
                    if (i < quest.questSteps.Length)
                        quest.questSteps[i].isCompleted = qData.stepsCompleted[i];
                }

                // Pokud je quest aktivní, hodíme ho do listu activeQuests
                if (quest.currentState == QuestState.Active)
                    activeQuests.Add(quest);

                // Obnovení trackeru
                if (gameDataManager.currentGameData.trackedQuestID == quest.questID)
                    TrackQuest(quest);
            }
        }
    }
}