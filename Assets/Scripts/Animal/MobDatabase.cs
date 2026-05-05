using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MobData
{
    public MobType mobType; // Např. Wolf, Bear
    public Sprite journalSprite; // Ten hezký obrázek pro Bestiář
    public string displayName; // Např. "Zmutovaný Medvěd"
    [TextArea(5, 10)]
    public string description; // Ten Witcher-style lore text
}

[CreateAssetMenu(fileName = "MobDatabase", menuName = "Scriptable Objects/Mob Database")]
public class MobDatabase : ScriptableObject
{
    public List<MobData> mobs = new List<MobData>();

    public MobData GetMobData(MobType type)
    {
        return mobs.Find(m => m.mobType == type);
    }
}