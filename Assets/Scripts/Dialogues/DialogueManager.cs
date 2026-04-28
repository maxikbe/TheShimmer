using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject dialoguePanel; // cele okno 
    public TextMeshProUGUI npcNameText; // kdo mluvi
    public TextMeshProUGUI dialogueText; 
    
    [Header("Tlačítka")]
    public GameObject buttonPrefab; 
    public Transform buttonContainer; // container pro ty tacitka

    private string currentSpeakerName;
    private Merchant currentMerchant;
    private GameObject currentNPCObject; // Uložení aktuálního NPC pro Companion příkazy

    // Vola script na NPC pro start konverzace (přidán parametr npcObject)
    public void StartConversation(string npcName, DialogueNode firstNode, Merchant merchant = null, GameObject npcObject = null)
    {
        currentSpeakerName = npcName; 
        currentMerchant = merchant;
        currentNPCObject = npcObject; // Zapamatujeme si, kdo to je
        
        ContinueDialogue(firstNode);
    }

    public void ContinueDialogue(DialogueNode node)
    {
        dialoguePanel.SetActive(true);
        npcNameText.text = currentSpeakerName; 
        dialogueText.text = node.dialogueText;

        // maze stara tlacitka z minuleho pouziti
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // --- PŘÍPRAVA PRO COMPANION LOGIKU ---
        Animal_movement[] allCompanions = FindObjectsOfType<Animal_movement>();
        Animal_movement currentAnimal = currentNPCObject != null ? currentNPCObject.GetComponent<Animal_movement>() : null;

        // vytvori tlacitka pro kazdou choice
        foreach (DialogueChoice choice in node.choices)
        {
            // --- KONTROLA PODMÍNEK (ZAKLÍNAČSKÝ UPDATE) ---
            bool canShowChoice = true;

            if (choice.conditions != null && choice.conditions.Count > 0)
            {
                foreach (QuestCondition condition in choice.conditions)
                {
                    if (condition.quest != null && condition.quest.currentState != condition.requiredState)
                    {
                        canShowChoice = false; 
                        break; 
                    }
                }
            }

            // --- KONTROLA COMPANION TLAČÍTEK (DYNAMICKÉ SKRÝVÁNÍ) ---
            if (canShowChoice && choice.npcCommand != CommandType.None)
            {
                switch (choice.npcCommand)
                {
                    case CommandType.WaitHere:
                        // Zobrazí se jen když zvíře NEČEKÁ
                        if (currentAnimal == null || currentAnimal.IsWaiting()) canShowChoice = false;
                        break;
                    case CommandType.FollowMe:
                        // Zobrazí se jen když zvíře ČEKÁ
                        if (currentAnimal == null || !currentAnimal.IsWaiting()) canShowChoice = false;
                        break;
                    case CommandType.AllWait:
                        // Zobrazí se jen pokud aspoň někdo NEČEKÁ
                        bool anyoneFollowing = false;
                        foreach (var c in allCompanions) if (c.behavior == Ghost_movement.MobBehavior.Companion && !c.IsWaiting()) anyoneFollowing = true;
                        if (!anyoneFollowing) canShowChoice = false;
                        break;
                    case CommandType.AllFollow:
                        // Zobrazí se jen pokud aspoň někdo ČEKÁ
                        bool anyoneWaiting = false;
                        foreach (var c in allCompanions) if (c.behavior == Ghost_movement.MobBehavior.Companion && c.IsWaiting()) anyoneWaiting = true;
                        if (!anyoneWaiting) canShowChoice = false;
                        break;
                }
            }

            // Pokud podmínky neprošly, přeskočíme zbytek kódu a jdeme na další volbu
            if (!canShowChoice) continue;
            // ----------------------------------------------

            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            newButton.GetComponent<Button>().onClick.AddListener(() => 
            {
                if (choice.npcCommand != CommandType.None)
                {
                    // Posíláme přímo GameObject toho, s kým mluvíme
                    ExecuteCompanionCommand(choice.npcCommand, currentNPCObject); 
                }
                
                if (choice.opensShop)
                {
                    dialoguePanel.SetActive(false);

                    if (currentMerchant != null)
                    {
                        ShopManager.Instance.OpenShop(currentMerchant);
                    }
                    else
                    {
                        Debug.Log("Někdo se snaží obchodovat s někým, kdo není prodejce");
                    }
                    return;
                }
                else
                {
                    if (choice.questToStart != null)
                    {
                        QuestManager.Instance.StartQuest(choice.questToStart);
                    }

                    if (choice.nextNode != null)
                    {
                        ContinueDialogue(choice.nextNode); 
                    }
                    else
                    {
                        dialoguePanel.SetActive(false);
                    }
                }
            });
        }
    }
    
    private void ExecuteCompanionCommand(CommandType command, GameObject currentNPC)
    {
        Animal_movement[] allCompanions = FindObjectsOfType<Animal_movement>();
        // Najdeme hráče pro měření vzdálenosti (Doslech/Aggro radius)
        Transform playerPos = GameObject.FindGameObjectWithTag("Player").transform;

        foreach (Animal_movement companion in allCompanions)
        {
            if (companion.behavior != Ghost_movement.MobBehavior.Companion) continue;

            // Změříme vzdálenost mezi hráčem a companionem
            float distanceToPlayer = Vector3.Distance(playerPos.position, companion.transform.position);

            // Hromadné příkazy s dosahem (když jsi moc daleko, neuslyší tě)
            if (command == CommandType.AllWait)
            {
                if (distanceToPlayer <= companion.visionRadius) companion.SetWaitState(true);
            }
            else if (command == CommandType.AllFollow)
            {
                if (distanceToPlayer <= companion.visionRadius) companion.SetWaitState(false);
            }
            // Příkazy pro jedno konkrétní NPC
            else if (currentNPC != null && companion.gameObject == currentNPC)
            {
                if (command == CommandType.WaitHere)
                {
                    companion.SetWaitState(true);
                }
                else if (command == CommandType.FollowMe)
                {
                    // Když mu to říkáš do ksichtu, měl bys být u něj, ale raději to zkontrolujeme taky
                    if (distanceToPlayer <= companion.visionRadius) companion.SetWaitState(false);
                }
            }
        }
    }
}