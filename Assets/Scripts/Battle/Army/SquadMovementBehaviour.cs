using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SquadMovementBehaviour : MonoBehaviourCI
{
    [ComponentInject] private SquadBehaviour squadBehaviour;

    private HashSet<GameObject> assignedUnits;
    private float offsetDistanceBetweenUnits = 1.5f;
    public void SetDestination(Vector3 mainDestination)
    {
        assignedUnits = new HashSet<GameObject>();
        var positions = GenPositions(mainDestination, squadBehaviour.UnitsDict.Count, Mathf.Max(squadBehaviour.UnitWidth, 1), offsetDistanceBetweenUnits);

        foreach (var position in positions)
        {
            var availableUnitsToAssign = squadBehaviour.UnitsDict.Where(unitDict => !assignedUnits.Any(assignedUnit => unitDict.Key == assignedUnit)).ToList(); ;
            var closestUnit = availableUnitsToAssign.OrderBy(unit => Vector3.Distance(position, unit.Key.transform.position)).First();
            closestUnit.Value.SetDestination(position);
            assignedUnits.Add(closestUnit.Key);
        }

        gameObject.AddComponent<SquadShowDestinationBehaviour>().ShowPositions(positions);
    }

    public List<Vector3> GenPositions(Vector3 destination, int positionCountToGenerate, int positionsPerRow, float offsetDistance)
    {
        var result = new List<Vector3>();

        var rowDiff = 0;
        var colDiff = 0;

        for (int i = 1; i <= positionCountToGenerate; i++)
        {
            result.Add(destination + new Vector3(rowDiff * offsetDistance, 0, colDiff - offsetDistance));

            if (i % positionsPerRow == 0)
            {
                rowDiff = 0;
                colDiff++;
            }
            else
            {
                rowDiff++;
            }
        }

        return result;
    }
}