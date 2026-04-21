using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QuestSaveData
{
    public string questID;
    public QuestState currentState;
    public List<bool> stepsCompleted = new List<bool>();
}
