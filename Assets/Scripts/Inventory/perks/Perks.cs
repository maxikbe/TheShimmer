using UnityEngine;

[CreateAssetMenu(fileName = "Perks", menuName = "Scriptable Objects/Perks")]
public class Perks : ScriptableObject
{
    public int id;
    public string perkName;
    public perkType perkType;
    public int addingAmount;
    public string description;
    public Sprite icon;
    public int levelOfPerk = 1;
}

public enum perkType
{
    healthAdder, //id: 100
    damageAdder, //id: 200
    critAdder, //id: 300
    speedAdder, //id: 400
    armorAdder //id: 500
}
