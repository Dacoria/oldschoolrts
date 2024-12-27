using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    public GameObject MainCastle;

    public static GameManager Instance;    

    public static int PopulationLimit = 8;
    public static int CurrentPopulation = 0;

    private new void Awake()
    {
        Instance = this;
        base.Awake();
        InitServes();
    }

    private T PopClosest<T>(List<T> behaviours, Vector3 objLocation) where T : MonoBehaviour
    {
        var closestBehaviour = behaviours.OrderBy(x => (x.transform.position - objLocation).sqrMagnitude).First();
        behaviours.Remove(closestBehaviour);
        return closestBehaviour;
    }
}