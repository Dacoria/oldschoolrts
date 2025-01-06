using System;
using UnityEngine;
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