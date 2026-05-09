using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class KeybindConfig
{
    [Tooltip("Přesný název z KeyBoardSetting, např. 'TBinventory' nebo 'jump'")]
    public string variableName; 
    
    [Tooltip("Co uvidí hráč, např. 'Skok' nebo 'Tahový inventář'")]
    public string displayName;  
}

public class KeybindingManager : MonoBehaviour
{
    [Header("Settings")]
    public List<KeybindConfig> keybinds = new List<KeybindConfig>
    {
        // --- Basic Movement ---
        new KeybindConfig { variableName = "keyUp", displayName = "Move Up" },
        new KeybindConfig { variableName = "keyDown", displayName = "Move Down" },
        new KeybindConfig { variableName = "keyLeft", displayName = "Move Left" },
        new KeybindConfig { variableName = "keyRight", displayName = "Move Right" },
        new KeybindConfig { variableName = "keyRun", displayName = "Sprint" },

        // --- Ingame Use ---
        new KeybindConfig { variableName = "Pause", displayName = "Pause" },
        new KeybindConfig { variableName = "MenuRight", displayName = "Menu Right" },
        new KeybindConfig { variableName = "MenuLeft", displayName = "Menu Left" },
        new KeybindConfig { variableName = "Cancel", displayName = "Cancel" },
        new KeybindConfig { variableName = "TBinventory", displayName = "Combat Inventory" },
        new KeybindConfig { variableName = "NormalInventory", displayName = "Inventory" },
        new KeybindConfig { variableName = "Journal", displayName = "Journal" },
        new KeybindConfig { variableName = "Codex", displayName = "Codex" },
        new KeybindConfig { variableName = "Interact", displayName = "Interact" },
        new KeybindConfig { variableName = "Craft", displayName = "Crafting" },
        new KeybindConfig { variableName = "Tent", displayName = "Pitch Tent" },
        new KeybindConfig { variableName = "Pack", displayName = "Pack Up" },
        new KeybindConfig { variableName = "Map", displayName = "Map" },
        new KeybindConfig { variableName = "InspectItem", displayName = "Inspect Item" },
        new KeybindConfig { variableName = "LightenUp", displayName = "Flashlight" },

        // --- TurnBased ShortCuts ---
        new KeybindConfig { variableName = "chooseSpecialSpell", displayName = "Special Spell" },
        new KeybindConfig { variableName = "chooseNormalSpell", displayName = "Normal Spell" },
        new KeybindConfig { variableName = "chooseItem", displayName = "Choose Item" },
        new KeybindConfig { variableName = "doAccept", displayName = "Confirm" },
        new KeybindConfig { variableName = "doBack", displayName = "Back" },
        new KeybindConfig { variableName = "swapUp", displayName = "Swap Up" },
        new KeybindConfig { variableName = "swapDown", displayName = "Swap Down" },
        new KeybindConfig { variableName = "swapLeft", displayName = "Swap Left" },
        new KeybindConfig { variableName = "swapRight", displayName = "Swap Right" },
        new KeybindConfig { variableName = "swapAliveUp", displayName = "Next Character" },
        new KeybindConfig { variableName = "swapAliveDown", displayName = "Previous Character" },
        
        // --- Combat Mechanics ---
        new KeybindConfig { variableName = "jump", displayName = "Jump" },
        new KeybindConfig { variableName = "dodge", displayName = "Dodge" },
        new KeybindConfig { variableName = "parry", displayName = "Parry" }
    };
    
    [Header("UI Reference")]
    public GameObject keybindRowPrefab;
    public Transform contentPanel;

    private bool isRebinding = false;
    private string variableToRebind;
    private TMP_Text buttonTextToUpdate;

    void Start()
    {
        GenerateUI();
    }

    public void GenerateUI()
    {
        // Vyčistíme staré řádky
        foreach (Transform child in contentPanel) { Destroy(child.gameObject); }

        foreach (var bind in keybinds)
        {
            GameObject row = Instantiate(keybindRowPrefab, contentPanel);
            TMP_Text actionNameText = row.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text buttonText = row.transform.GetChild(1).GetComponentInChildren<TMP_Text>();
            Button rebindButton = row.transform.GetChild(1).GetComponent<Button>();

            // Hráč vidí hezké jméno ("Tahový inventář")
            actionNameText.text = bind.displayName;

            // Získáme aktuální klávesu přes Reflection přímo ze tvého skriptu
            FieldInfo field = typeof(KeyBoardSetting).GetField(bind.variableName, BindingFlags.Public | BindingFlags.Static);
            if (field != null)
            {
                buttonText.text = ((KeyCode)field.GetValue(null)).ToString();
                
                // Připravíme čudlík na bindování
                rebindButton.onClick.AddListener(() => StartRebind(bind.variableName, buttonText));
            }
            else
            {
                Debug.LogError($"Kokkotte, máš překlep! Proměnná {bind.variableName} ve skriptu KeyBoardSetting neexistuje.");
            }
        }
    }

    // --- OBNOVENÍ VÝCHOZÍCH HODNOT ---
    public void RestoreDefaults()
    {
        foreach (var bind in keybinds)
        {
            // Podíváme se do naší zálohy, jaká klávesa tam byla původně
            if (KeyBoardSetting.defaultKeysBackup.TryGetValue(bind.variableName, out KeyCode originalKey))
            {
                // Vrátíme ji tam!
                FieldInfo field = typeof(KeyBoardSetting).GetField(bind.variableName, BindingFlags.Public | BindingFlags.Static);
                field.SetValue(null, originalKey);
            }
        }

        GenerateUI(); // Zaktualizujeme UI
        
        // Uložíme do tvého save systému
        gameDataManager.CaptureCurrentSettings();
        gameDataManager.SaveData(null, false);
    }

    private void StartRebind(string variableName, TMP_Text buttonText)
    {
        if (isRebinding) return;
        variableToRebind = variableName;
        buttonTextToUpdate = buttonText;
        buttonTextToUpdate.text = "???";
        isRebinding = true;
    }

    void Update()
    {
        if (isRebinding && Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || key == KeyCode.Mouse2) return;

                    // Uložíme novou klávesu přímo do proměnné
                    FieldInfo field = typeof(KeyBoardSetting).GetField(variableToRebind, BindingFlags.Public | BindingFlags.Static);
                    field.SetValue(null, key);
                    
                    buttonTextToUpdate.text = key.ToString();
                    isRebinding = false;
                    
                    // Uložíme data
                    gameDataManager.CaptureCurrentSettings();
                    gameDataManager.SaveData(null, false);
                    break;
                }
            }
        }
    }
}