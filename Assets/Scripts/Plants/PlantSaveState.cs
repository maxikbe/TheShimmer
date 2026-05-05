using UnityEngine;

[System.Serializable]
public class PlantSaveState
{
    public string uniqueID;
    public Vector3 position;
    public bool isLooted;
    public bool isDestroyed; // Pro případy, kdy jsi na kytce zaškrtl "destroyOnLoot"
}