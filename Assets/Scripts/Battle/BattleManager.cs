using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static bool ToggleInstaFreeUnits_Active = false;

    public static BattleManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
