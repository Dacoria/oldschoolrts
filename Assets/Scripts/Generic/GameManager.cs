using System.Collections.Generic;
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
        InitResourcesStockpile();
        InitCheckBuildingTypes();        
    }
}