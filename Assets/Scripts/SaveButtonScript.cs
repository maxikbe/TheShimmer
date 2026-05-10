using UnityEngine;
using UnityEngine.UI; // Nezapomeň na tohle, jinak Button nenajdeš!

public class SaveButtonScript : MonoBehaviour
{
    [SerializeField] private Button saveButton; // Tady v Inspectoru přiřadíš to tlačítko

    void Start()
    {
        // Pojistka, kdybys na to v Inspectoru zapomněl (známe se, že?)
        if (saveButton != null)
        {
            // Tímhle příkazem přidáš metodu do OnClick eventu přímo kódem
            saveButton.onClick.AddListener(OnSaveClicked);
        }
        else
        {
            Debug.LogError("Kokkotte, zapomněl jsi v Inspectoru přiřadit to tlačítko do skriptu SaveButtonScript!");
        }
    }

    private void OnSaveClicked()
    {
        InitializeGameJson.CreateSave("Data.json");
        
        //gameDataManager.SaveData();
        Debug.Log("SaveData zavoláno přes automatický AddListener.");
    }

    // Dobrá praxe: Odstranit listener, když se objekt zničí, aby ti nevznikaly memory leaky
    void OnDestroy()
    {
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(OnSaveClicked);
        }
    }
}