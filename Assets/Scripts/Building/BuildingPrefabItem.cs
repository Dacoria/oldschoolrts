using System;
using UnityEngine;

[Serializable]
public class BuildingPrefabItem
{
    public BuildingType BuildingType;
    public GameObject BuildingPrefab;
    public Vector3 DisplayOffset;
    public Sprite Icon;
}