// Voor identificeren scripts/go om resource naartoe terug te brengen
using System;
using UnityEngine;

internal interface ILocationOfNewResource
{
    public Vector3 GetCoordinatesForNewResource();
    public GameObject GetGameObjectForNewResource();
}