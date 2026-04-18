using UnityEngine;

[CreateAssetMenu(fileName = "Skills", menuName = "Scriptable Objects/Skills")]
public class Skills : ScriptableObject
{
    public int id;
    public int characterID;
    public string name;
    public string description;
    public skillType type;
    public int amount;
    public int skillLevel;
    public int pointsToResearch;
    public int animationID;
    public int manaCost;
}

public enum skillType { Damage, Heal, Buff, Mana}
