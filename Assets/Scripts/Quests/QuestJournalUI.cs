using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestJournalUI : MonoBehaviour
{
    [Header("Hlavní Okno")]
    public GameObject journalPanel; // Celý tento Panel

    [Header("Levá Strana - Seznamy")]
    // --- TADY MÁME TEĎ DVA KONTEJNERY ---
    public Transform mainQuestListContainer; 
    public Transform sideQuestListContainer; 
    public GameObject questButtonPrefab;

    [Header("Pravá Strana - Detaily")]
    public TextMeshProUGUI detailTitleText;
    public TextMeshProUGUI detailDescriptionText;
    public Button trackButton; // Tlačítko pro hození do levého horního rohu

    private QuestData selectedQuest; // Který quest máš zrovna rozkliknutý

    void Update()
    {
        // Otevírání deníku klávesou 'J'
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (journalPanel.activeSelf) CloseJournal();
            else OpenJournal();
        }
    }

    public void OpenJournal()
    {
        journalPanel.SetActive(true);
        RefreshQuestList(); // Načteme seznam úkolů
        
        // Vyčistíme pravou stranu, než hráč na něco klikne
        detailTitleText.text = "Choose quest from list";
        detailDescriptionText.text = "";
        trackButton.gameObject.SetActive(false); // Schováme tlačítko Trackeru
    }

    public void CloseJournal()
    {
        journalPanel.SetActive(false);
    }

    private void RefreshQuestList()
    {
        // Čistka obou kontejnerů!
        foreach (Transform child in mainQuestListContainer) Destroy(child.gameObject);
        foreach (Transform child in sideQuestListContainer) Destroy(child.gameObject);

        // Projdeme všechny aktivní questy
        foreach (QuestData quest in QuestManager.Instance.activeQuests)
        {
            // Zjistíme, do jakého kontejneru to máme hodit
            Transform targetContainer = (quest.questType == QuestType.MainQuest) ? mainQuestListContainer : sideQuestListContainer;

            // Spawneme tlačítko do správného seznamu
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

        // --- NOVÁ MAGIE PRO CHYTRÉ TLAČÍTKO ---
        trackButton.gameObject.SetActive(true);
        
        // Najdeme textový komponent uvnitř tlačítka, abychom ho mohli přepisovat
        TextMeshProUGUI btnText = trackButton.GetComponentInChildren<TextMeshProUGUI>();
        
        trackButton.onClick.RemoveAllListeners(); // Smažeme staré akce

        // Zkontrolujeme: Je tenhle rozkliknutý quest ten samý, co zrovna svítí vlevo nahoře?
        if (QuestManager.Instance.trackedQuest == selectedQuest)
        {
            // Pokud ANO, tlačítko bude sloužit k vypnutí
            btnText.text = "Stop Tracking";
            trackButton.onClick.AddListener(() => 
            {
                QuestManager.Instance.UntrackQuest();
                ShowQuestDetails(selectedQuest); // Znovu načteme detaily, ať se tlačítko hned přepíše!
            });
        }
        else
        {
            // Pokud NE, tlačítko bude sloužit k zapnutí
            btnText.text = "Track quest";
            trackButton.onClick.AddListener(() => 
            {
                QuestManager.Instance.TrackQuest(selectedQuest);
                ShowQuestDetails(selectedQuest); // Znovu načteme detaily
            });
        }
    }
}