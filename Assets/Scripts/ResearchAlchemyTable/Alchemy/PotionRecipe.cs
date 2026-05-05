using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Novy Recept", menuName = "Scriptable Objects/Potion Recipe")]
public class PotionRecipe : ScriptableObject
{
    public string recipeName;
    
    [Header("Co hodit do kotlíku")]
    [Tooltip("Itemy, které musí plavat v kotlíku. Na pořadí nezáleží.")]
    public List<Item> requiredIngredients = new List<Item>(); 
    
    [Header("Vaření (Hořák)")]
    public float minBoilTimeSeconds = 10f; // Jak dlouho to musí minimálně bublat
    public float maxBoilTimeSeconds = 15f; // Když to tam necháš déle, vznikne bláto
    
    [Header("Výsledek")]
    public Item resultPotion; // Lektvar, který dostaneš, když to uděláš správně
    public Item failedSludge; // Co dostaneš, když to zkazíš (např. item "Podezřelá břečka")
}