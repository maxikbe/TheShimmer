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
    // Vola script na NPC pro start konverzace
    public void StartConversation(string npcName, DialogueNode firstNode, Merchant merchant = null)
    {
        // uklada jmeno pro dalsi pouziti
        currentSpeakerName = npcName; 
        currentMerchant = merchant;
        
        // pusti dialog
        ContinueDialogue(firstNode);
    }


    public void ContinueDialogue(DialogueNode node)
    {
        // zapne dialog okno
        dialoguePanel.SetActive(true);

        // pouze ono jmeno
        npcNameText.text = currentSpeakerName; 
        dialogueText.text = node.dialogueText;

        // maze stara tlacitka z minuleho pouziti
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // vytvori tlacitka pro kazdou choice
        foreach (DialogueChoice choice in node.choices)
        {
            // --- KONTROLA PODMÍNEK (ZAKLÍNAČSKÝ UPDATE) ---
            bool canShowChoice = true;

            if (choice.conditions != null && choice.conditions.Count > 0)
            {
                foreach (QuestCondition condition in choice.conditions)
                {
                    // Pokud máme nastavenou podmínku na quest a jeho stav neodpovídá tomu, co vyžadujeme...
                    if (condition.quest != null && condition.quest.currentState != condition.requiredState)
                    {
                        canShowChoice = false; // ... tak volbu zakážeme
                        break; 
                    }
                }
            }

            // Pokud podmínky neprošly, přeskočíme zbytek kódu a jdeme na další volbu
            if (!canShowChoice) continue;
            // ----------------------------------------------

            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;

            // Pro qeuesty a dalsi veci
            newButton.GetComponent<Button>().onClick.AddListener(() => 
            {
                if (choice.opensShop)
                {
                    dialoguePanel.SetActive(false);

                    if (currentMerchant != null)
                    {
                        ShopManager.Instance.OpenShop(currentMerchant);
                    }
                    else
                    {
                        Debug.Log("Někdo se snaží obchodovat s někým kdo není prodejce");
                    }
                    return;
                }
                else
                {
                    // pokud je u odpovedi choice, spusti quest
                    if (choice.questToStart != null)
                    {
                        QuestManager.Instance.StartQuest(choice.questToStart);
                    }

                    // kontroluje jeslti je dalsi dialog pokracujici
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
}