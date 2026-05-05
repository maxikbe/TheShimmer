using UnityEngine;

public class PlantController : MonoBehaviour
{
    [Header("Typ Kytky")]
    public PlantType plantType = PlantType.None;
    
    [Header("Master Identifikace (Základ pro Save)")]
    public string uniqueID; // Musí být unikátní ve scéně! (např. "Houba_Jeskyně_01")

    [HideInInspector] public bool isLooted = false;
    [HideInInspector] public bool isDestroyed = false;

    private void Start()
    {
        // Jakmile kytka spawnne, zkontroluje, jestli už nemá vybílené kapsy
        LoadMyState();
    }

    private void LoadMyState()
    {
        if (gameDataManager.currentGameData == null) return;

        // Podíváme se do tvého nového listu
        var state = gameDataManager.currentGameData.savedWorldPlants.Find(p => p.uniqueID == uniqueID);
        
        if (state != null)
        {
            this.isLooted = state.isLooted;
            this.isDestroyed = state.isDestroyed;

            // Pokud měla kytka destroyOnLoot = true, rovnou ji pošleme do prázdna
            if (this.isDestroyed)
            {
                Destroy(gameObject);
            }
            // Pokud tu měla zůstat (jen okrasa), vypneme collider, ať tě UI nespamuje "E"čkem
            else if (this.isLooted)
            {
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }
        }
    }

    // Metoda pro uložení stavu do tvého JSONu
    public void SaveMyState(bool destroyedValue)
    {
        this.isDestroyed = destroyedValue;

        if (gameDataManager.currentGameData == null) return;

        // Najdeme nebo vytvoříme záznam v JSONu
        var state = gameDataManager.currentGameData.savedWorldPlants.Find(p => p.uniqueID == uniqueID);
        if (state == null)
        {
            state = new PlantSaveState { uniqueID = uniqueID };
            gameDataManager.currentGameData.savedWorldPlants.Add(state);
        }
        
        // Uložíme aktuální hodnoty
        state.isLooted = this.isLooted;
        state.isDestroyed = this.isDestroyed;
        state.position = transform.position;
    }
}