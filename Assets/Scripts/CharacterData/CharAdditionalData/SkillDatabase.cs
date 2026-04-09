using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Scriptable Objects/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    [SerializeField] private List<Skills> _skillDatabase;

    [ContextMenu("Set Skill IDs")]
    public void SetSkillIDs()
    {
        _skillDatabase = new List<Skills>();
        var foundSkills = Resources.LoadAll<Skills>("SkillsData").OrderBy(skill => skill.id).ToList();

        var hasIDInRange = foundSkills.Where(skill => skill.id != -1 && skill.id < foundSkills.Count).OrderBy(skill => skill.id).ToList();
        var hasIDNotInRange = foundSkills.Where(skill => skill.id != -1 && skill.id >= foundSkills.Count).OrderBy(skill => skill.id).ToList();
        var noID = foundSkills.Where(skill => skill.id <= -1).ToList();

        var index = 0;
        for (int i = 0; i < foundSkills.Count; i++)
        {
            Skills skillToAdd;
            skillToAdd = hasIDInRange.Find(skill => skill.id == i);
            if (skillToAdd != null)
            {
                _skillDatabase.Add(skillToAdd);
            }
            else if (index < noID.Count)
            {
                noID[index].id = i;
                skillToAdd = noID[index];
                index++;
                _skillDatabase.Add(skillToAdd);
            }
        }

        foreach (var skill in hasIDNotInRange)
        {
            _skillDatabase.Add(skill);
        }
    }

    public Skills GetSkillByID(int id)
    {
        return _skillDatabase.Find(skill => skill.id == id);
    }

    public List<Skills> GetAllSkills()
    {
        return _skillDatabase;
    }
}