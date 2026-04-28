using UnityEngine;

public class KeyBoardSetting : MonoBehaviour
{
    public static KeyCode keyUp = KeyCode.W;
    public static KeyCode keyDown = KeyCode.S;
    public static KeyCode keyLeft = KeyCode.A;
    public static KeyCode keyRight = KeyCode.D;
    public static KeyCode keyRun = KeyCode.LeftShift;
    public static KeyCode TBinventory = KeyCode.Tab;
    public static KeyCode NormalInventory = KeyCode.I;

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

    
}
