using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
[CanEditMultipleObjects]
public class ItemEditor : Editor
{
    // Serializované vlastnosti
    SerializedProperty isDefaultItem, defaultAmount, defaultLevel, allowedCharacterIDs;
    SerializedProperty itemName, description, itemType, icon, prefab;
    SerializedProperty isResearched, isUsable, maxStack;
    SerializedProperty canBeSold, basePrice;

    void OnEnable()
    {
        // Propojíme proměnné z Item.cs s editorem
        isDefaultItem = serializedObject.FindProperty("isDefaultItem");
        defaultAmount = serializedObject.FindProperty("defaultAmount");
        defaultLevel = serializedObject.FindProperty("defaultLevel");
        allowedCharacterIDs = serializedObject.FindProperty("allowedCharacterIDs");
        
        itemName = serializedObject.FindProperty("itemName");
        description = serializedObject.FindProperty("description");
        itemType = serializedObject.FindProperty("itemType");
        icon = serializedObject.FindProperty("icon");
        prefab = serializedObject.FindProperty("prefab");
        
        isResearched = serializedObject.FindProperty("isResearched");
        isUsable = serializedObject.FindProperty("isUsable");
        maxStack = serializedObject.FindProperty("maxStack");
        
        canBeSold = serializedObject.FindProperty("canBeSold");
        basePrice = serializedObject.FindProperty("basePrice");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Item item = (Item)target;

        // --- SEKCE PRO SAVE DATA ---
        EditorGUILayout.Space();
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.normal.textColor = new Color(0.2f, 0.7f, 0.2f); // Zelený text
        
        EditorGUILayout.LabelField("VÝCHOZÍ STAV PRO SAVE (JSON)", headerStyle);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(isDefaultItem, new GUIContent("Získat při startu?"));
        EditorGUILayout.PropertyField(defaultAmount, new GUIContent("Počáteční množství"));
        EditorGUILayout.PropertyField(defaultLevel, new GUIContent("Počáteční level"));
        
        EditorGUILayout.Space(2);
        // Vložení listu ID postav přímo do zelené sekce
        EditorGUILayout.PropertyField(allowedCharacterIDs, new GUIContent("Povolené ID postav", "Seznam ID postav, které mohou tento předmět použít"), true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // --- ZÁKLADNÍ NASTAVENÍ ---
        EditorGUILayout.LabelField("STATICKÁ DATA PŘEDMĚTU", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(itemName, new GUIContent("Název"));
        EditorGUILayout.PropertyField(description, new GUIContent("Popis"));
        EditorGUILayout.PropertyField(itemType, new GUIContent("Typ předmětu"));
        
        // Pro obchodníky
        EditorGUILayout.PropertyField(canBeSold, new GUIContent("Lze prodat obchodníkovi?"));

        // Pokud je povolen prodej
        if (canBeSold.boolValue)
        {
            EditorGUILayout.PropertyField(basePrice, new GUIContent("Základní cena (Coiny)"));
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(isResearched, new GUIContent("Je vyzkoumaný?"));
        EditorGUILayout.PropertyField(isUsable, new GUIContent("Je použitelný?"));
        EditorGUILayout.PropertyField(maxStack, new GUIContent("Max Stack"));

        EditorGUILayout.Space();

        // --- SPECIFICKÉ NASTAVENÍ PODLE TYPU ---
        switch ((ItemType)itemType.enumValueIndex)
        {
            case ItemType.Weapon:
                DrawWeaponSettings(item);
                break;
            case ItemType.Armor:
                DrawArmorSettings(item);
                break;
            case ItemType.Healing:
            case ItemType.Consumable:
                DrawConsumableSettings(item);
                break;
        }

        EditorGUILayout.Space();

        // --- VIZUÁL ---
        EditorGUILayout.LabelField("VIZUÁL A DATA", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(icon, new GUIContent("Ikona"));
        EditorGUILayout.PropertyField(prefab, new GUIContent("Prefab"));

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawWeaponSettings(Item item)
    {
        EditorGUILayout.LabelField("NASTAVENÍ ZBRANĚ", EditorStyles.boldLabel);
        item.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Základní typ", item.weaponType);
        item.isTurnedBaseWeapon = EditorGUILayout.Toggle("Je zbraní pro Turn Based Combat?", item.isTurnedBaseWeapon);
        item.firstCharID = EditorGUILayout.IntField("Který char má tuto zbraň defaultně? (pro každého hráče 1, takže prosím neopakujte to, 0 je že nikdo)", item.firstCharID);
        item.Damage = EditorGUILayout.FloatField("Poškození", item.Damage);
        
        item.isMagical = EditorGUILayout.Toggle("Je Magická?", item.isMagical);
        if (item.isMagical)
            item.magicalElement = (MagicalElement)EditorGUILayout.EnumPopup("Element", item.magicalElement);

        if (item.weaponType == WeaponType.Ranged)
        {
            item.Range = EditorGUILayout.FloatField("Dosah", item.Range);
            item.FireRate = EditorGUILayout.FloatField("Rychlost střelby", item.FireRate);
            item.AmmoCapacity = EditorGUILayout.IntField("Kapacita munice", item.AmmoCapacity);
        }
    }

    private void DrawArmorSettings(Item item)
    {
        EditorGUILayout.LabelField("NASTAVENÍ BRNĚNÍ", EditorStyles.boldLabel);
        item.armorType = (ArmorType)EditorGUILayout.EnumPopup("Slot brnění", item.armorType);
        item.Armor = EditorGUILayout.FloatField("Obrana", item.Armor);
        item.durability = EditorGUILayout.IntField("Odolnost", item.durability);
    }

    private void DrawConsumableSettings(Item item)
    {
        EditorGUILayout.LabelField("NASTAVENÍ KONZUMACE", EditorStyles.boldLabel);
        item.HealAmount = EditorGUILayout.IntField("Léčení / Obnova", item.HealAmount);
    }
}