using System.Linq;
using UnityEngine;

public class SkillUIBehaviour : MonoBehaviourCI
{

    [HideInInspector] public ChurchBehaviour CallingChurch;

    [ComponentInject] private SkillUiVisualizeBehaviour SkillUIVisualizeBehaviour;

    public SelectedSkillUiItemScript SelectedSkillUiItem;

    private void OnDisable()
    {
        SkillUIVisualizeBehaviour.UpdateAllSkills();
    }

    // aangeroepen vanuit wrapper
    public void UnlockSkillItem(SkillItem skillItem)
    {
        if(skillItem.Cost <= SkillTreeReader.Instance.AvailablePoints)
        {
            SkillTreeReader.Instance.UnlockSkill(skillItem.Id);
            SelectedSkillUiItem.SkillItem = SkillTreeReader.Instance.GetSkillItem(skillItem.Id);
            SkillUIVisualizeBehaviour.UpdateAllSkills(SelectedSkillUiItem.SkillItem.Id);
        }
    }

    public void SelectSkill(SkillUIWrapperBehaviour skillUIWrapper)
    {
        SelectedSkillUiItem.SkillItem = skillUIWrapper.SkillItem;
        SkillUIVisualizeBehaviour.UpdateAllSkills(SelectedSkillUiItem.SkillItem.Id);
    }

    public bool SkillCanBeUnlocked(SkillItem skillItem)
    {
        if(skillItem.IsUnlocked)
        {
            return false;
        }

        if (skillItem.MaxDepth == 0)
        {
            return true;
        }

        var allSkillItems = SkillTreeReader.Instance.GetSkillCategories().SelectMany(x => x.SkillItems).ToList();
        return allSkillItems.First(x => x.Id == skillItem.SkillDependencyIds.First()).IsUnlocked;
    }    
}