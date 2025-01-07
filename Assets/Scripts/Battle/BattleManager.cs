using UnityEngine;

public class BattleManager : BaseAEMonoCI
{
    public static bool ToggleInstaFreeUnits_Active = false;

    public static BattleManager Instance;
    private GameObject armyUnitParentGo;

    private new void Awake()
    {
        base.Awake();
        Instance = this;
        armyUnitParentGo = GameObject.Find(Constants.GO_SCENE_BATTLE_UNITS);
    }

    protected override void OnNewBattleUnit(ArmyUnitBehaviour armyUnit)
    {
        armyUnit.transform.parent = armyUnitParentGo.transform;
    }
}