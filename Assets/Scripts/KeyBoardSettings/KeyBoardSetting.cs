using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Reflection;

public class KeyBoardSetting : MonoBehaviour
{
    public static KeyCode keyUp = KeyCode.W;
    public static KeyCode keyDown = KeyCode.S;
    public static KeyCode keyLeft = KeyCode.A;
    public static KeyCode keyRight = KeyCode.D;
    public static KeyCode keyRun = KeyCode.LeftShift;
    
    //ingame use (setting, inventory,...)
    public static KeyCode Pause =  KeyCode.Escape;
    public static KeyCode MenuRight = KeyCode.RightArrow;
    public static KeyCode MenuLeft = KeyCode.LeftArrow;
    public static KeyCode Cancel = KeyCode.Escape;
    
    public static KeyCode TBinventory = KeyCode.Tab;
    public static KeyCode NormalInventory = KeyCode.I;
    
    public static KeyCode Journal =  KeyCode.J;
    public static KeyCode Codex = KeyCode.B;
    public static KeyCode Interact = KeyCode.E;
    public static KeyCode Craft = KeyCode.C;
    public static KeyCode Tent =  KeyCode.T;
    public static KeyCode Pack =  KeyCode.F;
    public static KeyCode Map = KeyCode.G;
    public static KeyCode InspectItem = KeyCode.F;
    public static KeyCode LightenUp = KeyCode.L;
    

    // TurnBased ShortCuts
    public static KeyCode chooseSpecialSpell = KeyCode.Q; 
    public static KeyCode chooseNormalSpell = KeyCode.W;   
    public static KeyCode chooseItem = KeyCode.E;
    public static KeyCode doAccept = KeyCode.Return;
    public static KeyCode doBack = KeyCode.Backspace;
    public static KeyCode swapUp = KeyCode.W;
    public static KeyCode swapDown = KeyCode.S;
    public static KeyCode swapLeft = KeyCode.A;
    public static KeyCode swapRight = KeyCode.D;
    
    public static KeyCode swapAliveUp = KeyCode.Tab;
    public static KeyCode swapAliveDown = KeyCode.LeftShift;

    public static KeyCode jump = KeyCode.Space;
    public static KeyCode dodge = KeyCode.D;
    public static KeyCode parry = KeyCode.F;

    
    
    public static Dictionary<string, KeyCode> defaultKeysBackup = new Dictionary<string, KeyCode>();

    // Statický konstruktor - spustí se dřív, než se načtou savy z gameDataManageru!
    static KeyBoardSetting()
    {
        // Wallhack: Oskenujeme tenhle skript a najdeme všechny proměnné typu KeyCode
        FieldInfo[] fields = typeof(KeyBoardSetting).GetFields(BindingFlags.Public | BindingFlags.Static);
        
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(KeyCode))
            {
                // Uložíme si přesný název proměnné (např. "keyUp") a její výchozí klávesu (např. KeyCode.W)
                defaultKeysBackup.Add(field.Name, (KeyCode)field.GetValue(null));
            }
        }
    }
    
}
