using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestJournalUI : MonoBehaviour
{
    [Header("Hlavní Okno")]
    public GameObject journalPanel; 

    [Header("Levá Strana - Seznamy")]
    public Transform mainQuestListContainer; 
    public Transform sideQuestListContainer; 
    public GameObject questButtonPrefab;

    [Header("Pravá Strana - Detaily")]
    public TextMeshProUGUI detailTitleText;
    public TextMeshProUGUI detailDescriptionText;
    public Button trackButton; 

    private QuestData selectedQuest; // zrovna rozkliknuty quest

    void Update()
    {
        // otevirani journalu pomoci J
        if (Input.GetKeyDown(KeyBoardSetting.Journal))
        {
            if (journalPanel.activeSelf) CloseJournal();
            else OpenJournal();
        }
    }

    public void OpenJournal()
    {
        journalPanel.SetActive(true);
        RefreshQuestList(); // nacte seznam ukolu
        
        // vycisti pravou stranu nez na neco klikne
        detailTitleText.text = "Choose quest from list";
        detailDescriptionText.text = "";
        trackButton.gameObject.SetActive(false); // schovava tracker tlacitko
    }

    public void CloseJournal()
    {
        journalPanel.SetActive(false);
    }

    private void RefreshQuestList()
    {
        // Kontejnery se seznamem cisti
        foreach (Transform child in mainQuestListContainer) Destroy(child.gameObject);
        foreach (Transform child in sideQuestListContainer) Destroy(child.gameObject);

        // projede vsechny aktivni quest
        foreach (QuestData quest in QuestManager.Instance.activeQuests)
        {
            // zjisti jestli je main nebo side
            Transform targetContainer = (quest.questType == QuestType.MainQuest) ? mainQuestListContainer : sideQuestListContainer;

            // hodi ho do spravneho seznamu
            GameObject newBtn = Instantiate(questButtonPrefab, targetContainer);
            newBtn.GetComponentInChildren<TextMeshProUGUI>().text = quest.questName;
            
            newBtn.GetComponent<Button>().onClick.AddListener(() => 
            {
                ShowQuestDetails(quest);
            });
        }
    }

    private void ShowQuestDetails(QuestData quest)
    {
        selectedQuest = quest;
        detailTitleText.text = quest.questName;

        string storySoFar = ""; 
        foreach (QuestStep step in quest.questSteps)
        {
            storySoFar += step.logText + "\n\n"; 
            if (!step.isCompleted) break; 
        }
        detailDescriptionText.text = storySoFar;


        trackButton.gameObject.SetActive(true);
        
        //najde text uprostred track tlacitka
        TextMeshProUGUI btnText = trackButton.GetComponentInChildren<TextMeshProUGUI>();
        
        trackButton.onClick.RemoveAllListeners();

        // kontroluje jestli je ted trackován nebo ne, podle toho mení text a trackuje/untrackuje podle toho co tam zrovna je 
        if (QuestManager.Instance.trackedQuest == selectedQuest)
        {
            btnText.text = "Stop Tracking";
            trackButton.onClick.AddListener(() => 
            {
                QuestManager.Instance.UntrackQuest();
                ShowQuestDetails(selectedQuest); 
            });
        }
        else
        {
            btnText.text = "Track quest";
            trackButton.onClick.AddListener(() => 
            {
                QuestManager.Instance.TrackQuest(selectedQuest);
                ShowQuestDetails(selectedQuest); // Znovu nacteme detaily
            });
        }
    }
}