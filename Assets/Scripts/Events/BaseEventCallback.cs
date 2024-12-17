using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class BaseEventCallback : MonoBehaviour
{
    protected void Awake()
    {
        this.ComponentInject();
    }

    protected void OnEnable()
    {
        if (IsOverwritten("OnVillagerUnitCreated")) AE.VillagerUnitCreated += OnVillagerUnitCreated;
    }

    protected void OnDisable()
    {
        if (IsOverwritten("OnVillagerUnitCreated")) AE.VillagerUnitCreated += OnVillagerUnitCreated;
    }

    protected virtual void OnVillagerUnitCreated(VillagerUnitType villagerUnitType) { }


    // GEEN ABSTRACTE CLASSES!
    private bool IsOverwritten(string functionName)
    {
        var type = GetType();
        var method = type.GetMember(functionName, BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        return method.Length > 0;
    }
}