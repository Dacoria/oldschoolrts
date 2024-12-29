using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Assets;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public static class MyExtensions
{
    public static int ToAreaMask(this Purpose purpose)
    {
        switch (purpose)
        {
            case Purpose.ROAD:
                return 1 << 0;
            default:
                return 1 << 3;
        }
    }

    public static T Pop<T>(this SortedSet<T> sortedSet)
    {
        var obj = sortedSet.Min();
        if (obj != null)
        {
            sortedSet.Remove(obj);
        }

        return obj;
    }

    public static string TitleCase(this string text)
    {
        var textInfo = new CultureInfo("en-US", false).TextInfo;
        var test = textInfo.ToTitleCase(text.ToLower());
        return textInfo.ToTitleCase(text.ToLower());
    }

    public static bool StoppedAtDestination(this NavMeshAgent myNavMeshAgent, float additionalStoppingDistance = 0)
    {
        return myNavMeshAgent.isActiveAndEnabled &&
                !myNavMeshAgent.pathPending &&
                myNavMeshAgent.pathStatus != NavMeshPathStatus.PathInvalid &&
                myNavMeshAgent.remainingDistance <= myNavMeshAgent.stoppingDistance + additionalStoppingDistance &&
                myNavMeshAgent.velocity.sqrMagnitude == 0f;
    }

    public static bool IsSameXAndZ(this Vector3 a, Vector3 b)
    {
        return a.x == b.x && a.z == b.z;
    }

    public static bool IsSameVector3(this Vector3 a, Vector3 b)
    {
        var sqrDiff = Vector3.SqrMagnitude(a - b);
        return sqrDiff < 0.001;
    }

    public static bool IsEmptyVector(this Vector3 a)
    {
        return a.x == 0 && a.y == 0 && a.z == 0;
    }
    public static bool IsAlmostEmptyVector(this Vector3 a)
    {
        return Mathf.Abs(a.x) <= 0.1 && Mathf.Abs(a.y) <= 0.1 && Mathf.Abs(a.z) <= 0.1;
    }

    public static Vector3 MultiplyVector(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vector3 Absolute(this Vector3 a)
    {
        return new Vector3(math.abs(a.x), math.abs(a.y), math.abs(a.z));
    }

    public static Bounds Copy(this Bounds b)
    {
        return new Bounds(new Vector3(b.center.x, b.center.y, b.center.z), new Vector3(b.size.x, b.size.y, b.size.z));
    }

    public static Vector3 EntranceExit(this GameObject go)
    {
        if (go == null)
        {
            return new Vector3(0, 0, 0);
        }

        Transform root = null;
        if (go.transform.parent != null)
        {
            root = go.transform.parent;
        } else
        {
            root = go.transform;
        }
        foreach (Transform child in root)
        {
            if (child.tag == Constants.TAG_ENTRANCE_EXIT)
            {
                return child.position;
            }
        }

        return go.transform.position;
    }

    public static bool IsRoad(this GameObject go)
    {
        return RootNameGoStartsWith(go, "Road");
    }

    public static bool IsFarmField(this GameObject go)
    {
        return RootNameGoStartsWith(go, "FarmField");
    }

    private static bool RootNameGoStartsWith(GameObject go, string startsWithText)
    {
        Transform root = null;
        do
        {
            root = go.transform.parent != null ? go.transform.parent : go.transform;

        } while (root.parent != null);

        return root.gameObject.name.StartsWith(startsWithText);
    }

    public static List<KeyCodeAction> KeyCodeActionList =
    new List<KeyCodeAction>
    {
            new KeyCodeAction(KeyCode.U, KeyCodeActionType.ToggleInputOutputDisplay),
            new KeyCodeAction(KeyCode.I, KeyCodeActionType.ToggleBuildingProgressDisplay),
            new KeyCodeAction(KeyCode.O, KeyCodeActionType.ToggleEntranceExitDisplay),
            new KeyCodeAction(KeyCode.P, KeyCodeActionType.ToggleBuildingNameImgDisplay)
    };

    public static List<ProductionSetting> GetProductionSettings(BuildingType type)
    {
        var building = BuildingPrefabs.Get().Single(x => x.BuildingType == type);
        switch (type)
        {
            case BuildingType.BLACKSMITH:
            case BuildingType.WEAPONMAKER:
            case BuildingType.LEATHERARMORY:
            case BuildingType.CLOTHARMORMAKER:
                return building.BuildingPrefab.GetComponentInChildren<CardItemsProduceBehaviour>(true).ItemProductionSettings.ConvertAll<ProductionSetting>(x => x);
            case BuildingType.BARRACKS:
                return BarrackUnitPrefabs.Get().ConvertAll<ProductionSetting>(x => x);
            case BuildingType.SCHOOL:
                return VillagerPrefabs.Get().ConvertAll<ProductionSetting>(x => x);

            default:
                throw new Exception("Cards for " + type.ToString() + " not specified");
        }
    }

    public static ProductionSetting GetProductionSetting(BuildingType buildingType, Enum enumType)
    {
        var prodSettings = GetProductionSettings(buildingType);

        switch (buildingType)
        {
            case BuildingType.BARRACKS:
                return prodSettings.Single(x => (BarracksUnitType)x.GetType() == Enum.Parse<BarracksUnitType>(enumType.ToString()));
            case BuildingType.SCHOOL:
                return prodSettings.Single(x => (VillagerUnitType)x.GetType() == Enum.Parse<VillagerUnitType>(enumType.ToString()));
            case BuildingType.BLACKSMITH:
            case BuildingType.WEAPONMAKER:
            case BuildingType.LEATHERARMORY:
            case BuildingType.CLOTHARMORMAKER:
                return prodSettings.Single(x => (ItemType)x.GetType() == Enum.Parse<ItemType>(enumType.ToString()));

            default:
                throw new Exception("Cards/prod settings for " + buildingType.ToString() + " not specified");
        }
    }

    public static Vector2 GetRandomVector(int minRange, int maxRange)
    {
        var randomX = UnityEngine.Random.Range(minRange, maxRange);
        var randomZ = UnityEngine.Random.Range(minRange, maxRange);

        var positiveNegativeX = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;
        var positiveNegativeZ = UnityEngine.Random.Range(0, 2) == 1 ? 1 : -1;

        var x = Mathf.RoundToInt(randomX * positiveNegativeX);
        var z = Mathf.RoundToInt(randomZ * positiveNegativeZ);

        return new Vector2(x, z);
    }

    public static List<ItemProduceSetting> ConvertToSingleProduceItem(this List<ProductionSetting> ProductionSettings)
    {
        var result = new List<ItemProduceSetting>();
        foreach (var produceSetting in ProductionSettings)
        {
            var itemResult = produceSetting.ConvertToSingleProduceItem();
            result.Add(itemResult);
        }

        return result;
    }
    
    public static Vector2 ConvertToVector2(this Vector3 vector3) => new Vector2(vector3.x, vector3.z);    

    public static ItemProduceSetting ConvertToSingleProduceItem(this ProductionSetting productionSetting)
    {
        var itemResult = new ItemProduceSetting
        {
            ItemsConsumedToProduce = productionSetting.ItemsConsumedToProduce,
            ItemsToProduce = new List<ItemOutput>
            {
                new ItemOutput
                {
                    ItemType = (ItemType)productionSetting.GetType(),
                    MaxBuffer = 5,
                    ProducedPerProdCycle = 1
                }
            }
        };

        return itemResult;
    }

    public static string Capitalize(this string input, bool everyWord = true, string delimiter = "_")
    {
        if (everyWord)
        {
            var words = input.Split(delimiter);
            var result = words[0].Capitalize(false); ;
            for (int i = 1; i < words.Length; i++)
            {
                result += delimiter;
                result += words[i].Capitalize(false);                
                
            }

            return result;
        }
        else
        {
            return string.Concat(input[0].ToString().ToUpper(), input.ToLower().Substring(1, input.Length - 1));
        }        
    }
}

