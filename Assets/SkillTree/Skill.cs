[System.Serializable]

public class Skill {

    public int SkillId;
    public SkillType SkillType;
    public int[] SkillDependencyIds;
    public bool IsUnlocked;
    public int Cost;
    public string Name;
    public string Description;
}
