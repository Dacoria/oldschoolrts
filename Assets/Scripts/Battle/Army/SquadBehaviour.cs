using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SquadBehaviour : MonoBehaviour
{
    public List<GameObject> Units;
    public int Width = 5;
    public int Height = 2;
    

    public void Clear()
    {
        Units.Clear();
    }

    public void AddUnit(GameObject unit)
    {
        Units.Add(unit);
    }

    public void SetDestination(Vector3 destination)
    {
        for (int i = 0; i < Units.Count; i++)
        {
            int row = GetRow(i,Height, Width);
            int column = GetColumn(i, Height, Width);
            var unit = Units[i];
            var offset = Vector3.one * 2;
            offset.x *= row;
            offset.z *= column;
            unit.GetComponent<NavMeshAgent>().destination = destination + offset;
        }
    }

    private int GetRow(int index, int height, int width)
    {
        if (width == 0)
        {
            return index / height;
        } if (height == 0)
        {
            return index % width;
        }

        return 0;
    }

    private int GetColumn(int index, int height, int width)
    {
        if (width == 0)
        {
            return index % height;
        }
        if (height == 0)
        {
            return index / width;
        }

        return 0;
    }
}