using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUiVisualizeBehaviour : MonoBehaviourCI
{
    [ComponentInject] private SkillUIBehaviour SkillUIBehaviour;

    public SkillUIWrapperBehaviour SkillUIWrapperBehaviourPrefab;
    public Image TransparentImage;
    public TextMeshProUGUI PointLeft;
    public List<SkillUIWrapperBehaviour> SkillUIWrapperBehaviours;

    public Sprite OutlineSelectedSprite;
    public Sprite OutlineNotSelectedSprite;

    private void Start()
    {
        SkillUIWrapperBehaviours = new List<SkillUIWrapperBehaviour>();
        var skilltreeCategories = SkillTreeReader.Instance.GetSkillCategories();

        foreach (var skilltreeCategory in skilltreeCategories)
        {
            CreateSkillTreeCategory(skilltreeCategory);
        }

        UpdateAllSkills();
    }

    public class SkillItemAndDir
    {
        public SkillItem SkillItem;
        public DependencyDirectionType DependencyDirectionType;

        public SkillItemAndDir(SkillItem SkillItem, DependencyDirectionType DependencyDirectionType)
        {
            this.SkillItem = SkillItem;
            this.DependencyDirectionType = DependencyDirectionType;
        }
    }

    private void CreateSkillTreeCategory(SkillItemCategory skilltreeCategory)
    {
        var skillsFirstIteration = GetSkillsToInstantiateFirstIteration(skilltreeCategory);
        InstantiateSkillItems(skillsFirstIteration);

        if (IsOneSkillUnlockingMultipleSkills(skilltreeCategory))
        {
            var skillsSecondIteration = GetSkillsToInstantiateSecondIteration(skilltreeCategory);
            InstantiateSkillItems(skillsSecondIteration);
        }
    }

    private bool IsOneSkillUnlockingMultipleSkills(SkillItemCategory skilltreeCategory)
    {
        return skilltreeCategory.SkillItems
            .Any(
                x => skilltreeCategory.SkillItems.Count(y => y.MaxDepth == x.MaxDepth) > 1
            );
    }

    private List<SkillItemAndDir> GetSkillsToInstantiateFirstIteration(SkillItemCategory skillItemCategory)
    {
        var skillsToAddInReverse = new List<SkillItemAndDir>();
        for (int maxDepth = 4; maxDepth > 0; maxDepth--)
        {
            var skillItemsOnDepth = skillItemCategory.SkillItems.Where(x => x.MaxDepth == maxDepth).ToList();
            if (skillItemsOnDepth.Count > 2)
            {
                throw new Exception("Error, Skill mag max 2 andere skills unlocken (per depth)");
            }

            else if (skillItemsOnDepth.Count == 0)
            {
                skillsToAddInReverse.Add(null);
            }

            else if (skillItemsOnDepth.Count == 1)
            {
                skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[0], DependencyDirectionType.Up));
            }

            else if (skillItemsOnDepth.Count == 2)
            {
                var maxDepthAfter = skillItemCategory.SkillItems.Where(x => x.MaxDepth == maxDepth + 1).ToList();
                if (maxDepthAfter.Count == 0)
                {
                    skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[0], DependencyDirectionType.Up));

                }
                else if (maxDepthAfter.Count == 1)
                {
                    var skillDepedancyIdNextDepth = maxDepthAfter.First().SkillDependencyIds.First();
                    if (skillDepedancyIdNextDepth == skillItemsOnDepth[0].Id)
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[0], DependencyDirectionType.Up));
                    }
                    else if (skillDepedancyIdNextDepth == skillItemsOnDepth[1].Id)
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[1], DependencyDirectionType.Up));
                    }
                    else
                    {
                        throw new Exception("Zou niet moeten kunnen voorkomen");
                    }
                }
                else if (maxDepthAfter.Count == 2)
                {
                    var skillDepedancyIdNextDepth = maxDepthAfter.First().SkillDependencyIds.First();
                    if (skillDepedancyIdNextDepth == skillItemsOnDepth[0].Id)
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[0], DependencyDirectionType.Up));
                    }
                    else
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[1], DependencyDirectionType.Up));
                    }
                }
            }
            else
            {
                throw new Exception("Zou niet moeten kunnen voorkomen");
            }
        }

        // 1e plaatje is altijd beschikbaar & gevuld
        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemCategory.SkillItems.Single(x => x.MaxDepth == 0), DependencyDirectionType.None));
        skillsToAddInReverse.Reverse();
        return skillsToAddInReverse;
    }

    private List<SkillItemAndDir> GetSkillsToInstantiateSecondIteration(SkillItemCategory skillItemCategory)
    {
        var skillsToAddInReverse = new List<SkillItemAndDir>();

        for (int maxDepth = 4; maxDepth > 0; maxDepth--)
        {
            var countSkillItemsLevelBefore = skillItemCategory.SkillItems.Where(x => x.MaxDepth == maxDepth - 1).Count();
            var dir = countSkillItemsLevelBefore == 1 ? DependencyDirectionType.UpLeft : DependencyDirectionType.Up;

            var skillItemsOnDepth = skillItemCategory.SkillItems.Where(x => x.MaxDepth == maxDepth).ToList();
            if (skillItemsOnDepth.Count > 2)
            {
                throw new Exception("Error, Skill mag max 2 andere skills unlocken (per depth)");
            }

            else if (skillItemsOnDepth.Count == 0)
            {
                skillsToAddInReverse.Add(null);
            }

            else if (skillItemsOnDepth.Count == 1)
            {
                skillsToAddInReverse.Add(null);
            }

            else if (skillItemsOnDepth.Count == 2)
            {
                var maxDepthAfter = skillItemCategory.SkillItems.Where(x => x.MaxDepth == maxDepth + 1).ToList();

                if (maxDepthAfter.Count == 0)
                {
                    skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[1], dir));
                }
                else if (maxDepthAfter.Count == 1)
                {
                    var skillDepedancyIdNextDepth = maxDepthAfter.First().SkillDependencyIds.First();                    

                    if (skillDepedancyIdNextDepth == skillItemsOnDepth[0].Id)
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[1], dir));
                    }
                    else if (skillDepedancyIdNextDepth == skillItemsOnDepth[1].Id)
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[0], dir));
                    }
                    else
                    {
                        throw new Exception("Zou niet moeten kunnen voorkomen");
                    }

                }
                else if (maxDepthAfter.Count == 2)
                {
                    var skillDepedancyIdNextDepth = maxDepthAfter.First().SkillDependencyIds.First();
                    if (skillDepedancyIdNextDepth == skillItemsOnDepth[0].Id)
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[1], dir));
                    }
                    else
                    {
                        skillsToAddInReverse.Add(new SkillItemAndDir(skillItemsOnDepth[0], dir));
                    }
                }
            }
            else
            {
                throw new Exception("Zou niet moeten kunnen voorkomen");
            }
        }

        // 2e plaatje is nooit beschikbaar & gevuld
        skillsToAddInReverse.Add(null);
        skillsToAddInReverse.Reverse();
        return skillsToAddInReverse;
    }

    private void InstantiateSkillItems(List<SkillItemAndDir> skillItemsWithDir)
    {
        foreach (var skillItemAndDir in skillItemsWithDir)
        {
            if (skillItemAndDir?.SkillItem != null)
            {
                InstantiateNewSkill(skillItemAndDir.SkillItem, skillItemAndDir.DependencyDirectionType);
            }
            else
            {
                Instantiate(TransparentImage, transform);
            }
        }
    }

    private void InstantiateNewSkill(SkillItem skillItem, DependencyDirectionType dependencyDirectionType)
    {
        var skillUiWrapper = Instantiate(SkillUIWrapperBehaviourPrefab, transform);
        skillUiWrapper.SkillImage.sprite = MonoHelper.Instance.GetSpriteForSkillType(skillItem.SkillType);
        skillUiWrapper.SkillItem = skillItem;
        skillUiWrapper.DependencyDirectionType = dependencyDirectionType;

        skillUiWrapper.SkillUIBehaviour = SkillUIBehaviour;
        SkillUIWrapperBehaviours.Add(skillUiWrapper);
    }    

    public void UpdateAllSkills(int selectedSkillId = -1)
    {
        UpdateAvailablePointsText();
        UpdateSkillsStats();
        UpdateSelectedSkills(selectedSkillId);
    }

    private void UpdateSelectedSkills(int selectedSkillId)
    {
        foreach(var skillui in SkillUIWrapperBehaviours)
        {
            if(selectedSkillId == skillui.SkillItem.Id)
            {
                skillui.CardOutlineImage.sprite = OutlineSelectedSprite;
            }
            else
            {
                skillui.CardOutlineImage.sprite = OutlineNotSelectedSprite;
            }            
        }
    }

    private void OnEnable()
    {
        PointLeft.gameObject.SetActive(true);
    }

    private void UpdateAvailablePointsText()
    {
        PointLeft.text = "Points left: " + SkillTreeReader.Instance.AvailablePoints;
    }

    private void UpdateSkillsStats()
    {
        var allSkillItems = SkillTreeReader.Instance.GetSkillCategories().SelectMany(x => x.SkillItems).ToList();

        foreach (var skillUiWrapper in SkillUIWrapperBehaviours)
        {
            var skillItem = allSkillItems.First(x => x.Id == skillUiWrapper.SkillItem.Id);
            skillUiWrapper.SkillItem = skillItem;
        }
    }    
}
