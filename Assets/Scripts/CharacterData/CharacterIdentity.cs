using UnityEngine;

public class CharacterIdentity : MonoBehaviour
{
    public Character myData; 

    public void Setup(Character loadedData)
    {
        myData = loadedData;
        Debug.Log("Postava " + myData.name + " byla nastavena.");
    }
}