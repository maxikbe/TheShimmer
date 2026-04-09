using System.Collections.Generic;
[System.Serializable]
public class GameData
{
    public playerData player = new playerData();
    public List<Character> characters = new List<Character>();
    public List<ItemSaveData> OwnedItems = new List<ItemSaveData>();
    public List<Enemy> enemies = new List<Enemy>();
    public List<CharacterAnimationData> characterAnimations = new List<CharacterAnimationData>();
    public List<EnemyAnimationData> enemyAnimations = new List<EnemyAnimationData>();
    public List<SkillSaveData> Skills = new List<SkillSaveData>();
    public List<MerchantReputation> merchantReputations = new List<MerchantReputation>();
}