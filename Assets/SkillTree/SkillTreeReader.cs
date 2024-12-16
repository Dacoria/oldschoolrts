using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class SkillTreeReader : MonoBehaviour 
{
    public static SkillTreeReader Instance;
    
    // Array with all the skills in our skilltree
    public Skill[] _skillTree;

    // Dictionary with the skills in our skilltree
    private Dictionary<int, Skill> _skills;

    // Variable for caching the currently being inspected skill
    private Skill _skillInspected;
    

    public int AvailablePoints = 100;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            SetUpSkillTree();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public List<Skill> GetSkillTree() => _skillTree.ToList();

    public SkillItem GetSkillItem(int skillId)
    {
        var skillItems = GetSkillCategories().SelectMany(x => x.SkillItems).ToList();
        return skillItems.FirstOrDefault(x => x.Id == skillId);
    }

    public List<SkillItemCategory> GetSkillCategories()
    {
        var result = new List<SkillItemCategory>();
        var rootSkills = _skillTree.Where(x => !x.SkillDependencyIds.Any()).ToList();

        for(int i = 0; i < rootSkills.Count; i++)
        {
            var skillItemsOfCategory = new List<SkillItem>();

            var skillsInCategory = DetermineAllSkillsRelatedToRootSkill(rootSkills[i]);

            foreach(Skill skill in skillsInCategory)
            {
                var rootSkillItem = ConvertToSkillItem(skill, rootSkills[i]);
                skillItemsOfCategory.Add(rootSkillItem);
            }

            var skillItemCategory = new SkillItemCategory
            {
                CatergoryIndex = i,
                SkillItems = skillItemsOfCategory
            };

            result.Add(skillItemCategory);
        }

        return result;
    }

    private List<Skill> DetermineAllSkillsRelatedToRootSkill(Skill rootSkill)
    {
        var result = new List<Skill>();

        foreach(var skill in _skillTree)
        {
            if(HasSkillAsDependency(skill, rootSkill))
            {
                result.Add(skill);
            }
        }

        return result;
    }

    private bool HasSkillAsDependency(Skill skill, Skill rootSkill) {

        var skillDepth = GetSkillDepth(skill, rootSkill);
        return skillDepth >= 0;
    }


    private int GetSkillDepth(Skill skill, Skill rootSkill)
    {
        if(skill.SkillId == rootSkill.SkillId)
        {
            return 0;
        }

        var finishedAllDependencies = false;
        var foundRootSkill = false;

        var skillsToEvaluate = new List<Skill> { skill };
        var dependancySkills = _skillTree
            .Where(y => 
                skillsToEvaluate.SelectMany(x => x.SkillDependencyIds)
                .Any(x => x == y.SkillId)).ToList();

        var currentDepth = 1;
        for (currentDepth = 1; !finishedAllDependencies && !foundRootSkill; currentDepth++)
        {
            
            if(dependancySkills.Any(x => x.SkillId == rootSkill.SkillId))
            {
                foundRootSkill = true;
                break;
            }

            // niet gevonden? opnieuw verder gaan
            skillsToEvaluate = dependancySkills;
            dependancySkills = _skillTree
                .Where(y =>
                    skillsToEvaluate.SelectMany(x => x.SkillDependencyIds)
                    .Any(x => x == y.SkillId)).ToList();

            finishedAllDependencies = !dependancySkills.Any();
        }

        return foundRootSkill ? currentDepth : -1;
    }

    private SkillItem ConvertToSkillItem(Skill skill, Skill rootSkill)
    {
        var skillItem = new SkillItem
        {
            Id = skill.SkillId,
            SkillType = skill.SkillType,
            Description = skill.Description,
            Cost = skill.Cost,
            IsUnlocked = skill.IsUnlocked,
            SkillDependencyIds = skill.SkillDependencyIds.ToList(),
            MaxDepth = GetSkillDepth(skill, rootSkill)
        };

        return skillItem;
    }

    // Use this for initialization of the skill tree
    void SetUpSkillTree ()
    {
        _skills = new Dictionary<int, Skill>();
        LoadSkillTree();
    }	

    public void LoadSkillTree()
    {
        string path = "Assets/SkillTree/Data/skilltree.json";
        string dataAsJson;
        if (File.Exists(path))
        {
            // Read the json from the file into a string
            dataAsJson = File.ReadAllText(path);

            // Pass the json to JsonUtility, and tell it to create a SkillTree object from it
            SkillTree loadedData = JsonUtility.FromJson<SkillTree>(dataAsJson);

            // Store the SkillTree as an array of Skill
            _skillTree = new Skill[loadedData.skilltree.Length];
            _skillTree = loadedData.skilltree;

            // Populate a dictionary with the skill id and the skill data itself
            for (int i = 0; i < _skillTree.Length; ++i)
            {
                _skills.Add(_skillTree[i].SkillId, _skillTree[i]);
            }
        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }        
    }

    public bool IsSkillUnlocked(int id_skill)
    {
        if (_skills.TryGetValue(id_skill, out _skillInspected))
        {
            return _skillInspected.IsUnlocked;
        }
        else
        {
            return false;
        }
    }

    public bool CanSkillBeUnlocked(int id_skill)
    {
        bool canUnlock = true;
        if(_skills.TryGetValue(id_skill, out _skillInspected)) // The skill exists
        {
            if(_skillInspected.Cost <= AvailablePoints) // Enough points available
            {
                int[] dependencies = _skillInspected.SkillDependencyIds;
                for (int i = 0; i < dependencies.Length; ++i)
                {
                    if (_skills.TryGetValue(dependencies[i], out _skillInspected))
                    {
                        if (!_skillInspected.IsUnlocked)
                        {
                            canUnlock = false;
                            break;
                        }
                    }
                    else // If one of the dependencies doesn't exist, the skill can't be unlocked.
                    {
                        return false;
                    }
                }
            }
            else // If the player doesn't have enough skill points, can't unlock the new skill
            {
                return false;
            }
            
        }
        else // If the skill id doesn't exist, the skill can't be unlocked
        {
            return false;
        }
        return canUnlock;
    }

    public bool UnlockSkill(int id_Skill)
    {
        if(_skills.TryGetValue(id_Skill, out _skillInspected))
        {
            if (_skillInspected.Cost <= AvailablePoints)
            {
                AvailablePoints -= _skillInspected.Cost;
                _skillInspected.IsUnlocked = true;

                // We replace the entry on the dictionary with the new one (already unlocked)
                _skills.Remove(id_Skill);
                _skills.Add(id_Skill, _skillInspected);

                return true;
            }
            else
            {
                return false;   // The skill can't be unlocked. Not enough points
            }
        }
        else
        {
            return false;   // The skill doesn't exist
        }
    }
}
