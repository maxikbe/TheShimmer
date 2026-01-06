using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Database", menuName = "Scriptable Objects/Database")]
public class Database : ScriptableObject
{
    [SerializeField] private List<Item> _itemDatabase;

    [ContextMenu("Set Item IDs")]
    public void SetItemIDs()
    {
        _itemDatabase = new List<Item>();
        var foundItems = Resources.LoadAll<Item>("ItemData").OrderBy(item => item.id).ToList();

        var hasIDInRange = foundItems.Where(item => item.id != -1 && item.id < foundItems.Count).OrderBy(item => item.id).ToList();
        var hasIDNotInRange = foundItems.Where(item => item.id != -1 && item.id >= foundItems.Count).OrderBy(item => item.id).ToList();
        var noID = foundItems.Where(item => item.id <= -1).ToList();

        var index = 0;
        for(int i = 0; i < foundItems.Count; i++)
        {
           Item itemToAdd;
           itemToAdd = hasIDInRange.Find(item => item.id == i);
           if(itemToAdd != null)
            {
                _itemDatabase.Add(itemToAdd);
            }
            else if(index < noID.Count)
            {
                noID[index].id = i;
                itemToAdd = noID[index];
                index++;
                _itemDatabase.Add(itemToAdd);
            }
        }

        foreach(var item in hasIDNotInRange)
        {
            _itemDatabase.Add(item);
        }
    }   

    public Item GetItemByID(int id)
    {
        return _itemDatabase.Find(item => item.id == id);
    }
}
