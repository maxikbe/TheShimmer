using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PerksDatabase", menuName = "Scriptable Objects/PerksDatabase")]
public class PerksDatabase : ScriptableObject
{
    [SerializeField] private List<Perks> _perkDatabase;

    [ContextMenu("Set Perk IDs")]
    public void SetPerkIDs()
    {
        _perkDatabase = new List<Perks>();
        var foundPerks = Resources.LoadAll<Perks>("PerksData").OrderBy(perk => perk.id).ToList();

        var hasIDInRange = foundPerks.Where(perk => perk.id != -1 && perk.id < foundPerks.Count).OrderBy(perk => perk.id).ToList();
        var hasIDNotInRange = foundPerks.Where(perk => perk.id != -1 && perk.id >= foundPerks.Count).OrderBy(perk => perk.id).ToList();
        var noID = foundPerks.Where(perk => perk.id <= -1).ToList();

        var index = 0;
        for(int i = 0; i < foundPerks.Count; i++)
        {
           Perks perkToAdd;
           perkToAdd = hasIDInRange.Find(perk => perk.id == i);
           if(perkToAdd != null)
            {
                _perkDatabase.Add(perkToAdd);
            }
            else if(index < noID.Count)
            {
                noID[index].id = i;
                perkToAdd = noID[index];
                index++;
                _perkDatabase.Add(perkToAdd);
            }
        }

        foreach(var perk in hasIDNotInRange)
        {
            _perkDatabase.Add(perk);
        }
    }   

    public Perks GetPerkByID(int id)
    {
        return _perkDatabase.Find(perk => perk.id == id);
    }
    
    public List<Perks> GetAllPerks()
    {
        return _perkDatabase;
    }
}
