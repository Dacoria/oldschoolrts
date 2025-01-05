using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SquadBehaviour : MonoBehaviour
{
    // voor perf; store navmesh
    public Dictionary<GameObject, NavMeshAgent> UnitsDict { get; private set; } = new Dictionary<GameObject, NavMeshAgent>();
    public List<GameObject> GetUnits() => UnitsDict.Keys.ToList();       

    [HideInInspector] public SquadMovementBehaviour Movement;
    
    public CompassDirection CurrentDirection;

    public int UnitWidth;

    private void Start()
    {
        Movement = gameObject.AddComponent<SquadMovementBehaviour>();
        CurrentDirection = RtsUnitSelectionManager.Instance.DefaultDirection;
        if(UnitWidth <= 0)
        {
            UnitWidth = 1;
        }        
    }

    public void Clear()
    {
        UnitsDict.Clear();
    }

    public void AddUnit(GameObject unit)
    {
        UnitsDict.Add(unit, unit.GetComponent<NavMeshAgent>());

        if (UnitWidth <= 6 && GetUnits().Count <= 6)
        {
            UnitWidth = GetUnits().Count;
        }
    }
}