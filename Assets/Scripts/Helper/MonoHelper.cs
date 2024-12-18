using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Army;

public class MonoHelper : MonoBehaviour
{
    public static MonoHelper Instance { get; set; }
    public Camera MainCamera;


    void Awake()
    {
        Instance = this;     
    }

    private void Start()
    {
        // check of alle armortypes plaatjes hebben
        foreach(ArmorType armorType in Enum.GetValues(typeof(ArmorType)))
        {
            var x = SpriteArmorTypes.Single(x => x.ArmorType == armorType);
        }
        // check of alle attacktypes plaatjes hebben
        foreach (AttackType attackType in Enum.GetValues(typeof(AttackType)))
        {
            SpriteDamageTypes.Single(x => x.AttackType == attackType);
        }        
    }

    public Vector3 GetMousePositionV3(Vector3 mousePosition) => MainCamera.ScreenToWorldPoint(mousePosition);

    public List<SpriteArmorType> SpriteArmorTypes;
    public List<SpriteAttackType> SpriteDamageTypes;
    public List<SpriteSkillType> SpriteSkillTypes;


    [Serializable]
    public class SpriteArmorType
    {
        public ArmorType ArmorType;
        public Sprite Sprite;
    }

    [Serializable]
    public class SpriteAttackType
    {
        public AttackType AttackType;
        public Sprite Sprite;
    }

    [Serializable]
    public class SpriteSkillType
    {
        public SkillType SkillType;
        public Sprite Sprite;
    }

    public Sprite GetSpriteForArmorType(ArmorType armorType) => SpriteArmorTypes.Single(x => x.ArmorType == armorType).Sprite;
    public Sprite GetSpriteForAttackType(AttackType attackType) => SpriteDamageTypes.Single(x => x.AttackType == attackType).Sprite;
    public Sprite GetSpriteForSkillType(SkillType skillType)
    {
        var skillSprit = SpriteSkillTypes.FirstOrDefault(x => x.SkillType == skillType);
        if(skillSprit != null)
        {
            return skillSprit.Sprite;
        }

        return SpriteSkillTypes.Single(x => x.SkillType == SkillType.None).Sprite;
    }

    public GameObject LeftArrowCarousselUiPrefab;
    public GameObject RightArrowCarousselUiPrefab;
    public RangeDisplayBehaviour RangedDisplayPrefab;
    public GameObject WarningGoPrefab;
    public GameObject GetClosestTavernWithFood()
    {
        var allTaverns = GameObject.FindGameObjectsWithTag(Constants.TAG_TAVERN);
        GameObject closestTavern = null;
        var closestDistance = 9999999f;

        foreach (var tavern in allTaverns)
        {
            var distance = Vector3.Distance(tavern.transform.position, transform.position);
            if (distance < closestDistance)
            {
                var tavernScript = tavern.GetComponent<TavernBehaviour>();
                if (tavernScript.HasFoodForRefill())
                {
                    closestTavern = tavern;
                    closestDistance = distance;
                }
            }
        }

        return closestTavern;
    }

    private Dictionary<float, WaitForSeconds> WaitForSecondsCache = new Dictionary<float, WaitForSeconds>();
    public WaitForSeconds GetCachedWaitForSeconds(float seconds)
    {
        WaitForSeconds result;
        if(WaitForSecondsCache.TryGetValue(seconds, out result))
        {
            return result;
        }
        else
        {
            result = new WaitForSeconds(seconds);
            WaitForSecondsCache.Add(seconds, result);
            return result;
        }
    }
}