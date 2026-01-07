using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        EditorGUILayout.LabelField("ZÁKLADNÍ NASTAVENÍ", EditorStyles.boldLabel);
        item.itemName = EditorGUILayout.TextField("Název", item.itemName);
        item.description = EditorGUILayout.TextField("Popis", item.description);
        item.isOwned = EditorGUILayout.Toggle("Je vlastněný?", item.isOwned);
        item.isResearched = EditorGUILayout.Toggle("Je výzkoumaný?", item.isResearched);
        item.isUsable = EditorGUILayout.Toggle("Je použitelný?", item.isUsable);
        item.itemType = (ItemType)EditorGUILayout.EnumPopup("Typ předmětu", item.itemType);
        
        EditorGUILayout.Space();

        switch (item.itemType)
        {
            case ItemType.Weapon:
                EditorGUILayout.LabelField("NASTAVENÍ ZBRANĚ", EditorStyles.boldLabel);
                
                item.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Základní typ", item.weaponType);
                
                item.isMagical = EditorGUILayout.Toggle("Je Magická?", item.isMagical);
                if (item.isMagical)
                {
                    item.magicalElement = (MagicalElement)EditorGUILayout.EnumPopup("Element Magie", item.magicalElement);
                }

                EditorGUILayout.Space();
                item.Damage = EditorGUILayout.FloatField("Poškození", item.Damage);

                if (item.weaponType == WeaponType.Ranged)
                {
                    item.Range = EditorGUILayout.FloatField("Dosah (Range)", item.Range);
                    item.FireRate = EditorGUILayout.FloatField("Rychlost střelby", item.FireRate);
                    item.AmmoCapacity = EditorGUILayout.IntField("Kapacita munice", item.AmmoCapacity);
                }
                break;

            case ItemType.Armor:
                item.armorType = (ArmorType)EditorGUILayout.EnumPopup("Slot brnění", item.armorType);
                item.Armor = EditorGUILayout.FloatField("Obrana", item.Armor);
                item.durability = EditorGUILayout.IntField("Odolnost", item.durability);
                break;

            case ItemType.Healing:
            case ItemType.Consumable:
                item.HealAmount = EditorGUILayout.IntField("Léčení / Obnova", item.HealAmount);
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("VIZUÁL A DATA", EditorStyles.boldLabel);
        item.icon = (Sprite)EditorGUILayout.ObjectField("Ikona", item.icon, typeof(Sprite), false);
        item.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", item.prefab, typeof(GameObject), false);

        if (GUI.changed) EditorUtility.SetDirty(item);
    }
}