using Assets.CrossCutting;
using System;
using System.Linq;
using UnityEngine;

public class GenUnitsTestScript : MonoBehaviour
{
    public Vector3 StartPosFirstUnit;
    public int UnitsToGenerateCount;
    public float DistanceBetweenUnits;
    public int NumbersPerRow;

    // Button
    public void GenUnits()
    {
        var pos = StartPosFirstUnit;

        var rowDiff = 0;
        var colDiff = 0;

        for (int i = 1; i <= UnitsToGenerateCount; i++)
        {
            GenUnits(StartPosFirstUnit + new Vector3(rowDiff * DistanceBetweenUnits, 0, colDiff - DistanceBetweenUnits));

            if (i % NumbersPerRow == 0)
            {
                rowDiff = 0;
                colDiff++;
            }
            else
            {
                rowDiff++;
            }
        }
    }


    public void GenUnits(Vector3 pos)
    {
        var type = BarracksUnitType.SWORDFIGHTER;
        var unit = BarrackUnitPrefabs.Get().Single(x => x.Type == type);
        var unitGo = Instantiate(unit.ResourcePrefab);
        unitGo.GetComponent<OwnedByPlayerBehaviour>().Player = Player.PLAYER1;
        unitGo.transform.position = pos;
        SetUnitStats(unitGo, unit);        
    }

    private void SetUnitStats(GameObject unitGo, BarracksUnitSetting unitSettings)
    {
        var armyUnitBehaviour = unitGo.GetComponent<ArmyUnitBehaviour>();
        if (armyUnitBehaviour != null)
        {
            armyUnitBehaviour.Offence = unitSettings.UnitStats.Offence.DeepClone();
            armyUnitBehaviour.Defence = unitSettings.UnitStats.Defence.DeepClone();
            armyUnitBehaviour.EnemyAttractRadius = unitSettings.UnitStats.RangeToAttractEnemies;
            armyUnitBehaviour.Reach = unitSettings.UnitStats.RangeToAttack;
            armyUnitBehaviour.NavMeshAgent.stoppingDistance = unitSettings.UnitStats.RangeToAttack;
            armyUnitBehaviour.NavMeshAgent.speed = unitSettings.UnitStats.Speed;

            var healthBehaviour = unitGo.GetComponent<HealthBehaviour>();
            if (healthBehaviour != null)
            {
                healthBehaviour.InitialHeath = unitSettings.UnitStats.Health.DeepClone();
                healthBehaviour.CurrentHealth = unitSettings.UnitStats.Health.DeepClone();
            }
        }
        else
        {
            throw new Exception($"Unit '{unitSettings.Type} -> {unitGo.name}' vereist ArmyUnitBehaviour");
        }
    }
}
