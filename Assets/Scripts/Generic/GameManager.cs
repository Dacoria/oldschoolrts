using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    public GameObject MainCastle;
    private IOrderDestination MainCastleOrderDestination;

    public static GameManager Instance;

    private new void Awake()
    {
        Instance = this;
        MainCastleOrderDestination = MainCastle.GetComponent<IOrderDestination>();
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