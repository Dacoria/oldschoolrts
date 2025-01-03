using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Army;
using System.Collections;

public class MonoHelper : MonoBehaviour
{
    public static MonoHelper Instance { get; set; }
    public Camera MainCamera;

    public GameObject ProcessingDisplayPrefab;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // check of alle armortypes plaatjes hebben
        foreach (ArmorType armorType in Enum.GetValues(typeof(ArmorType)))
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

    public Sprite GetSpriteForArmorType(ArmorType armorType) => SpriteArmorTypes.Single(x => x.ArmorType == armorType).Sprite;
    public Sprite GetSpriteForAttackType(AttackType attackType) => SpriteDamageTypes.Single(x => x.AttackType == attackType).Sprite;


    public GameObject LeftArrowCarousselUiPrefab;
    public GameObject RightArrowCarousselUiPrefab;
    public RangeDisplayBehaviour RangedDisplayPrefab;
    public GameObject WarningGoPrefab;

    public TavernBehaviour GetClosestTavernWithFood()
    {
        var allTaverns = GameObject.FindGameObjectsWithTag(Constants.TAG_TAVERN);
        TavernBehaviour closestTavern = null;
        var closestDistance = 9999999f;

        foreach (var tavern in allTaverns)
        {
            var distance = Vector3.Distance(tavern.transform.position, transform.position);
            if (distance < closestDistance)
            {
                var tavernScript = tavern.GetComponent<TavernBehaviour>();
                if (tavernScript.HasFoodForRefill())
                {
                    closestTavern = tavernScript;
                    closestDistance = distance;
                }
            }
        }

        return closestTavern;
    }

    public void Do_CR(float waitTimeInSeconds, Action callback)
    {
        StartCoroutine(CR_Do_CR(waitTimeInSeconds, callback));
    }

    private IEnumerator CR_Do_CR(float waitTimeInSeconds, Action callback)
    {
        yield return Wait4Seconds.Get(waitTimeInSeconds);
        callback?.Invoke();
    }

    public T FindChildComponentInParents<T>(GameObject go, bool searchInactiveChilds = true)
    {
        var goToSearch = go;
        for (var i = 0; i < 10; i++)
        {
            var result = goToSearch.GetComponentInChildren<T>(searchInactiveChilds);
            if(result != null)
            {
                return result;
            }

            goToSearch = goToSearch.transform?.parent.gameObject;
            if (goToSearch == null)
            {
                throw new Exception($"No child found in entire GO for type: {typeof(T)}");
            }
        }

        throw new Exception("onwaarschijnlijk dat er zoveel parent-niveaus zijn --> hoe dan?");        
    }

    public GameObject GetHighestParent(GameObject gameObjectToStart)
    {
        var limitTries = 10;
        var highestParentGo = gameObjectToStart;
        for (var i = 1; i <= limitTries; i++)
        {
            if (highestParentGo.transform.parent != null)
            {
                highestParentGo = highestParentGo.transform.parent.gameObject;
            }
            else
            {
                break;
            }
        }

        return highestParentGo;
    }
}