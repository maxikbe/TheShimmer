using UnityEngine;

[CreateAssetMenu(fileName = "Skills", menuName = "Scriptable Objects/Skills")]
public class Skills : ScriptableObject
{
    public int id;
    public int characterID;
    public string skillName;
    public string description;
    public skillType type;
    public int amount;
    public int pointsToResearch;
    public int animationID;
    public int manaCost;
    public bool isDefault;
    public int mustBeActivedSkillID;
    public Sprite icon;
    public int gridX;
    public int gridY;
}

public enum skillType { Damage, Heal, Buff, Mana}
