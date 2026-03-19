using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Hit
{
    public float timeOffset; 
    public int damage;       
}

[System.Serializable]
public class EnemyAttack
{
    public int id;
    public string attackName;
    public float totalAnimationDuration; 
    public List<Hit> hits = new List<Hit>();
    [Range(0, 100)] public float weight;
}

[System.Serializable]
public class Enemy
{
    public int id;
    public string name;
    public int health;
    public List<EnemyAttack> attacks = new List<EnemyAttack>();

    public EnemyAttack GetRandomAttack() => attacks[Random.Range(0, attacks.Count)];
}