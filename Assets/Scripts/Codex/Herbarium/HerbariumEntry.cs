using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HerbariumEntry : MonoBehaviour
{
    public TextMeshProUGUI plantNameText;
    public Image plantIcon;
    public Button myButton;

    private PlantType myPlant;
    private CodexUIManager myManager;

    public void Setup(PlantType plant, CodexUIManager manager, PlantDatabase plantDB)
    {
        myPlant = plant;
        myManager = manager;

        PlantData data = plantDB.GetPlantData(plant);
        if (data != null)
        {
            plantNameText.text = !string.IsNullOrEmpty(data.displayName) ? data.displayName : plant.ToString();
            if (data.codexSprite != null)
            {
                plantIcon.sprite = data.codexSprite;
                plantIcon.color = Color.white;
            }
        }
        else
        {
            plantNameText.text = plant.ToString();
        }

        myButton.onClick.RemoveAllListeners();
        myButton.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        myManager.ShowPlantDetails(myPlant);
    }
}