using UnityEngine;

public class CharacterIdentity : MonoBehaviour
{
    // Toto jsou data, která se načtou z JSONu
    public Character myData; 

    // Metoda, kterou zavoláš po načtení JSONu
    public void Setup(Character loadedData)
    {
        myData = loadedData;
        // Tady můžeš např. nastavit jméno nad hlavu postavy
        Debug.Log("Postava " + myData.name + " byla nastavena.");
    }
}