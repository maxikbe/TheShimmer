using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PlantData
{
    public PlantType plantType; // Z tvého enumu
    public Sprite codexSprite; // Fotka do deníku
    public string displayName; // Hezký název
    [TextArea(5, 10)]
    public string description; // Zápisky biologa
}

[CreateAssetMenu(fileName = "PlantDatabase", menuName = "Scriptable Objects/Plant Database")]
public class PlantDatabase : ScriptableObject
{
    public List<PlantData> plants = new List<PlantData>();

    public PlantData GetPlantData(PlantType type)
    {
        return plants.Find(p => p.plantType == type);
    }
}