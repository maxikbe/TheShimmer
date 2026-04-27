using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
[CanEditMultipleObjects]
public class ItemEditor : Editor
{
    SerializedProperty isDefaultItem, defaultAmount, defaultLevel, allowedCharacterIDs;
    SerializedProperty itemName, description, itemType, icon, prefab;
    SerializedProperty isResearched, isUsable, maxStack, isTurnedBaseItem;
    SerializedProperty canBeSold, basePrice;
    SerializedProperty rarity, originMobName, potionHeal, potionAditionalHealth, potionBonusSpeed, potionBonusStamina, potionBonusFOV, potionBonushungerSpeed, potionBonusdamage, hilightResources;
    SerializedProperty HealAmount, consumeAmount, waterAmount, sleepAmount;

    void OnEnable()
    {
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
        isTurnedBaseItem = serializedObject.FindProperty("isTurnedBaseItem");
        
        canBeSold = serializedObject.FindProperty("canBeSold");
        basePrice = serializedObject.FindProperty("basePrice");
        
        rarity = serializedObject.FindProperty("rarity");
        originMobName = serializedObject.FindProperty("originMobName");
        potionHeal = serializedObject.FindProperty("potionHeal");
        potionAditionalHealth = serializedObject.FindProperty("potionAditionalHealth");
        potionBonusSpeed = serializedObject.FindProperty("potionBonusSpeed");
        potionBonusStamina = serializedObject.FindProperty("potionBonusStamina");
        potionBonusFOV =  serializedObject.FindProperty("potionBonusFOV");
        potionBonushungerSpeed =  serializedObject.FindProperty("potionBonushungerSpeed");
        potionBonusdamage =  serializedObject.FindProperty("potionBonusdamage");
        hilightResources =  serializedObject.FindProperty("hilightResources");

        HealAmount = serializedObject.FindProperty("HealAmount");
        consumeAmount = serializedObject.FindProperty("consumeAmount");
        waterAmount = serializedObject.FindProperty("waterAmount");
        sleepAmount = serializedObject.FindProperty("sleepAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Item item = (Item)target;

        EditorGUILayout.Space();
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.normal.textColor = new Color(0.2f, 0.7f, 0.2f);
        
        EditorGUILayout.LabelField("VÝCHOZÍ STAV PRO SAVE (JSON)", headerStyle);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(isDefaultItem, new GUIContent("Získat při startu?"));
        EditorGUILayout.PropertyField(defaultAmount, new GUIContent("Počáteční množství"));
        EditorGUILayout.PropertyField(defaultLevel, new GUIContent("Počáteční level"));
        EditorGUILayout.Space(2);
        EditorGUILayout.PropertyField(allowedCharacterIDs, new GUIContent("Povolené ID postav", "Seznam ID postav, které mohou tento předmět použít"), true);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("STATICKÁ DATA PŘEDMĚTU", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(itemName, new GUIContent("Název"));
        EditorGUILayout.PropertyField(description, new GUIContent("Popis"));
        EditorGUILayout.PropertyField(itemType, new GUIContent("Typ předmětu"));
        EditorGUILayout.PropertyField(canBeSold, new GUIContent("Lze prodat obchodníkovi?"));

        if (canBeSold.boolValue)
        {
            EditorGUILayout.PropertyField(basePrice, new GUIContent("Základní cena (Coiny)"));
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(isResearched, new GUIContent("Je vyzkoumaný?"));
        EditorGUILayout.PropertyField(isUsable, new GUIContent("Je použitelný?"));
        EditorGUILayout.PropertyField(maxStack, new GUIContent("Max Stack"));
        EditorGUILayout.PropertyField(isTurnedBaseItem, new GUIContent("Je Turn-Based předmět?"));

        if (isTurnedBaseItem.boolValue)
        {
            DrawTurnBasedItemSettings(item);
        }

        EditorGUILayout.Space();

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
            case ItemType.Sample:
                DrawSampleSettings(item);
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("VIZUÁL A DATA", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(icon, new GUIContent("Ikona"));
        EditorGUILayout.PropertyField(prefab, new GUIContent("Prefab"));

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTurnBasedItemSettings(Item item)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("TURN-BASED VLASTNOSTI", EditorStyles.boldLabel);
        item.turnBaseItemType = (TurnBaseItemType)EditorGUILayout.EnumPopup("Typ efektu", item.turnBaseItemType);
        item.turnBaseItemEffectAmount = EditorGUILayout.IntField("Síla efektu", item.turnBaseItemEffectAmount);
        item.turnBaseItemDuration = EditorGUILayout.IntField("Trvání (tahy)", item.turnBaseItemDuration);
        EditorGUILayout.EndVertical();
    }

    private void DrawWeaponSettings(Item item)
    {
        EditorGUILayout.LabelField("NASTAVENÍ ZBRANĚ", EditorStyles.boldLabel);
        item.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Základní typ", item.weaponType);
        item.isTurnedBaseWeapon = EditorGUILayout.Toggle("Zbraň pro Turn-Based?", item.isTurnedBaseWeapon);
        item.firstCharID = EditorGUILayout.IntField("Defaultní majitel (ID)", item.firstCharID);
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
        EditorGUILayout.LabelField("NASTAVENÍ KONZUMACE / LÉČENÍ", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(HealAmount, new GUIContent("Obnova HP"));
        EditorGUILayout.PropertyField(consumeAmount, new GUIContent("Obnova Jídla"));
        EditorGUILayout.PropertyField(waterAmount, new GUIContent("Obnova Vody"));
        EditorGUILayout.PropertyField(sleepAmount, new GUIContent("Obnova Spánku"));
    }
    
    private void DrawSampleSettings(Item item)
    {
        EditorGUILayout.LabelField("NASTAVENÍ VZORKU (SAMPLE)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(rarity, new GUIContent("Rarita vzorku"));
        EditorGUILayout.PropertyField(originMobName, new GUIContent("Původní monstrum"));
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Tyto staty se hráči ukážou v UI až když bude vzorek vyzkoumaný.", MessageType.Info);
        
        EditorGUILayout.PropertyField(potionHeal, new GUIContent("HP potion (heal do max)"));
        EditorGUILayout.PropertyField(potionAditionalHealth, new GUIContent("Bonusový health (nad max)"));
        EditorGUILayout.PropertyField(potionBonusSpeed, new GUIContent("Bonusový speed"));
        EditorGUILayout.PropertyField(potionBonusStamina, new GUIContent("Bonusová stamina"));
        EditorGUILayout.PropertyField(potionBonusFOV, new GUIContent("Bonusové FOV"));
        EditorGUILayout.PropertyField(potionBonushungerSpeed, new GUIContent("Rychlost hungeru"));
        EditorGUILayout.PropertyField(potionBonusdamage, new GUIContent("Bonusový damage"));
        EditorGUILayout.PropertyField(hilightResources, new GUIContent("Zvýraznění resources"));
    }
}