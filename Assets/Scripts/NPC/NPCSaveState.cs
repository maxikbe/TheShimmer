using UnityEngine;

[System.Serializable]
public class NPCSaveState
{
    public string uniqueID;
    public Vector3 position;
    public bool isDead;
    public bool isInCombat; // Přidáno: abychom věděli, jestli tahle konkrétní mobka zrovna bojuje
}