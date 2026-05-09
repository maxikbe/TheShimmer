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

        Animal_movement[] allCompanions = FindObjectsOfType<Animal_movement>();
        Animal_movement currentAnimal = currentNPCObject != null ? currentNPCObject.GetComponent<Animal_movement>() : null;

        // vytvori tlacitka pro kazdou choice
        foreach (DialogueChoice choice in node.choices)
        {
            bool canShowChoice = true;

            // 1. KONTROLA QUESTŮ
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

            // 2. KONTROLA ITEMŮ V INVENTÁŘI
            if (canShowChoice && choice.itemConditions != null && choice.itemConditions.Count > 0)
            {
                if (gameDataManager.currentGameData == null) canShowChoice = false;
                else
                {
                    foreach (ItemCondition itemCond in choice.itemConditions)
                    {
                        if (itemCond.requiredItem == null) continue;

                        ItemSaveData foundItem = gameDataManager.currentGameData.OwnedItems.Find(i => i.id == itemCond.requiredItem.id && i.isOwned);
                        
                        if (foundItem == null || foundItem.amount < itemCond.requiredAmount)
                        {
                            canShowChoice = false; // Nemáš loot, volbu vůbec neukážeme
                            break;
                        }
                    }
                }
            }

            // 3. KONTROLA COMPANION TLAČÍTEK
            if (canShowChoice && choice.npcCommand != CommandType.None)
            {
                switch (choice.npcCommand)
                {
                    case CommandType.WaitHere:
                        if (currentAnimal == null || currentAnimal.IsWaiting()) canShowChoice = false;
                        break;
                    case CommandType.FollowMe:
                        if (currentAnimal == null || !currentAnimal.IsWaiting()) canShowChoice = false;
                        break;
                    case CommandType.AllWait:
                        bool anyoneFollowing = false;
                        foreach (var c in allCompanions) if (c.behavior == Ghost_movement.MobBehavior.Companion && !c.IsWaiting()) anyoneFollowing = true;
                        if (!anyoneFollowing) canShowChoice = false;
                        break;
                    case CommandType.AllFollow:
                        bool anyoneWaiting = false;
                        foreach (var c in allCompanions) if (c.behavior == Ghost_movement.MobBehavior.Companion && c.IsWaiting()) anyoneWaiting = true;
                        if (!anyoneWaiting) canShowChoice = false;
                        break;
                }
            }

            if (!canShowChoice) continue;

            // Vytvoření samotného tlačítka
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            newButton.GetComponent<Button>().onClick.AddListener(() => 
            {
                // Odevzdání itemů z inventáře
                if (choice.itemConditions != null && gameDataManager.currentGameData != null)
                {
                    foreach (ItemCondition itemCond in choice.itemConditions)
                    {
                        if (itemCond.consumeItem && itemCond.requiredItem != null)
                        {
                            ItemSaveData invItem = gameDataManager.currentGameData.OwnedItems.Find(i => i.id == itemCond.requiredItem.id && i.isOwned);
                            if (invItem != null)
                            {
                                invItem.amount -= itemCond.requiredAmount;
                                if (invItem.amount <= 0) invItem.isOwned = false; // Už ho nemá
                                Debug.Log($"Item odevzdán: {itemCond.requiredItem.itemName}");
                            }
                        }
                    }
                    gameDataManager.SaveData(); // Save inventáře
                }

                if (choice.npcCommand != CommandType.None) ExecuteCompanionCommand(choice.npcCommand, currentNPCObject); 
                
                // SPEEDRUN START FIGHTU
                if (choice.triggerCombat && currentNPCObject != null)
                {
                    Animal_movement npcAnimal = currentNPCObject.GetComponent<Animal_movement>();
                    if (npcAnimal != null)
                    {
                        npcAnimal.MakeAggressive(); // Hodí NPC do agresivního stavu
                        dialoguePanel.SetActive(false);
                        return; 
                    }
                }
                
                // Posouvání a zapínání questů
                if (choice.questToStart != null) QuestManager.Instance.StartQuest(choice.questToStart);
                if (choice.questToAdvance != null) QuestManager.Instance.AdvanceQuest(choice.questToAdvance);

                if (choice.opensShop)
                {
                    dialoguePanel.SetActive(false);
                    // V původním kódu je currentMerchant definovaný mimo, u tebe taky.
                    // Jen bacha, jestli máš referenci správně.
                    ShopManager.Instance.OpenShop(currentMerchant);
                }
                else
                {
                    if (choice.nextNode != null) ContinueDialogue(choice.nextNode); 
                    else dialoguePanel.SetActive(false);
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