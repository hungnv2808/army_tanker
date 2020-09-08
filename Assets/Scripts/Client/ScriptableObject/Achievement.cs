using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement&DailyQuest", menuName = "ScriptableObject/Achievement&DailyQuest", order = 1)]
public class Achievement : ScriptableObject
{
    public AchievementObject[] AchievementNames;
    public AchievementObject[] QuestNames;
}
[System.Serializable]
public class AchievementObject {
    public string Name;
    public int GoldAward;
    public int DiamondAward;
    public string OtherAward;
}
