using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Collections;

[System.Serializable]

public class CampFire
{
    public string id;
    public Vector2 pos;
    public bool isBlueprint;
    public int woodLeft;
    public int stoneLeft;
    public bool isLit;
    public int woodFuelAmount;
}

[System.Serializable]
public class playerData
{
    public String playerName;
    public int numberOfGunUpgraders;
    public int numberOfMaterial;
    public int numberOfCoins;
    public float thirstLevel;
    public float maxThirstLevel;
    public float hungerLevel;
    public float maxHungerLevel;
    public float staminaLevel;
    public float maxStaminaLevel;
    public float sleepLevel;
    public float maxSleepLevel;
    public List<int> unFoundPerks = new List<int>();
    public List<int> foundPerks = new List<int>();
    public Vector2 playerPos;
    public float time;
    public int dayNumber;
    public string currentScene;
    public bool isTentPlaced;
    public Vector2 tentPos;
    public List<CampFire> campFires = new List<CampFire>();
    
    public float gasSecondsLeft = 300f;
   
}
