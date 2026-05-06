using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BestiaryEntry : MonoBehaviour
{
    [Header("UI Prvky uvnitř Prefabu")]
    public TextMeshProUGUI mobNameText;
    public Image mobIcon;
    public Button myButton;

    private MobType myMob;
    private CodexUIManager myManager;

    public void Setup(MobType mob, CodexUIManager manager)
    {
        myMob = mob;
        myManager = manager;

        // Zatím nemáme databázi mobek s ikonkami a jmény, 
        // takže použijeme jen název enumu. Později si můžeme udělat MobDatabase!
        mobNameText.text = mob.ToString(); 

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        myManager.ShowBestiaryDetails(myMob);
    }
}