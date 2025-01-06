using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    public GameObject MainCastle;
    private GameObject buildingParentGo;
    private IOrderDestination mainCastleOrderDestination;    

    public static GameManager Instance;

    private new void Awake()
    {
        Instance = this;
        mainCastleOrderDestination = MainCastle.GetComponent<IOrderDestination>();
        base.Awake();
        InitServes();
    }       
}