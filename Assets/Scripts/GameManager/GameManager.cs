using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    public GameObject MainCastle;
    private GameObject BuildingParentGo;
    private IOrderDestination MainCastleOrderDestination;    

    public static GameManager Instance;

    private new void Awake()
    {
        Instance = this;
        MainCastleOrderDestination = MainCastle.GetComponent<IOrderDestination>();
        base.Awake();
        InitServes();
    }       
}