using UnityEngine;
using System.Collections.Generic;

public class EnemyAnimationData
{
    public int enemyID;
    public int animationID;
    public string animationName;
    public AnimationClip animationClip;
}
[System.Serializable]
public class Hit
{
    public float timeOffset; 
    public int damage;       
    public float parryTimePlayer;  
    public float dodgeTimePlayer; 
    public dodgeType dodgeType;
}

[System.Serializable]
public class EnemyAttack
{
    public int id;
    public string attackName;
    public float totalAnimationDuration; 
    public List<Hit> hits = new List<Hit>();
    public List<EnemyAnimationData> animations = new List<EnemyAnimationData>();
    public float weight;
    public int numberOfCharHits;
}

[System.Serializable]
public class Enemy
{
    public int id;
    public string name;
    public int health;
    public int maxHealth;
    public bool isDead;
    public Sprite sprite;
    public List<EnemyAttack> attacks = new List<EnemyAttack>();

    public EnemyAttack GetRandomAttack() => attacks[Random.Range(0, attacks.Count)];
}

public enum dodgeType 
{

    normal,
    jump
}