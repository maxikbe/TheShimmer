using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public List<Character> characters = new List<Character>();
    public List<ItemSaveData> OwnedItems = new List<ItemSaveData>();
}