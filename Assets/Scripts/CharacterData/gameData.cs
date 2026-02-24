using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public playerData player = new playerData();
    public List<Character> characters = new List<Character>();
    public List<ItemSaveData> OwnedItems = new List<ItemSaveData>();
}