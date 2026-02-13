using System;
using System.Collections.Generic;

[Serializable] 
public class ItemSaveData
{
    public int id;
    public bool isOwned;
    public int level;
    public int amount;
    public List<int> allowedCharacterIDs = new List<int>();
}


