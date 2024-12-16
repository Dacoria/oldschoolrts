
using System.Collections.Generic;

public class SkillItem 
{
    public int Id;
    public SkillType SkillType;
    public string Description;
    public List<int> SkillDependencyIds;
    public bool IsUnlocked;
    public int Cost;
    public int MaxDepth;
}
