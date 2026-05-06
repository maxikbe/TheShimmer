using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SampleEntry : MonoBehaviour
{
    public TextMeshProUGUI sampleNameText;
    public Image sampleIcon;
    public Button myButton;

    private Item mySample;
    private CodexUIManager myManager;

    public void Setup(Item sample, CodexUIManager manager)
    {
        mySample = sample;
        myManager = manager;

        sampleNameText.text = sample.itemName;
        if (sample.icon != null)
        {
            sampleIcon.sprite = sample.icon;
            
            // Logika: Ztmaví ikonu, pokud to ještě není vyzkoumané
            bool isResearched = gameDataManager.currentGameData != null && 
                                gameDataManager.currentGameData.unlockedResearches.Contains(sample.id); 
            sampleIcon.color = isResearched ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f); 
        }

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        myManager.OpenSampleInSampler(mySample);
    }
}