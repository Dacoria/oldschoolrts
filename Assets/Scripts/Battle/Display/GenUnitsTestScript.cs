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

    private void GenUnits(Vector3 pos)
    {
        var type = BarracksUnitType.SWORDFIGHTER;
        var unit = BarrackUnitPrefabs.Get().Single(x => x.Type == type);
        var unitGo = Instantiate(unit.ResourcePrefab);
        unitGo.GetComponent<OwnedByPlayerBehaviour>().Player = Player.PLAYER1;
        unitGo.transform.position = pos;
    }   
}