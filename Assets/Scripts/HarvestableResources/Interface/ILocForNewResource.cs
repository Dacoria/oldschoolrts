// Voor identificeren scripts/go om resource naartoe terug te brengen
using UnityEngine;

internal interface ILocForNewResource
{
    public LocForRsc GetLocationForNewResource();
}

public class LocForRsc
{
    public GameObject GO;
    public Vector3 V3;
}