using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    public GameObject MainCastle;

    public static GameManager Instance { get; set; }

    public List<ItemFoodRefillValue> ItemFoodRefillValues; // verplaatsen later

    public GameObject GoToTavernBubble; // verplaatsen later

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