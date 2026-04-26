using UnityEngine;
using System.Collections.Generic;
using System;

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
}
