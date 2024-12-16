﻿// Voor identificeren scripts/go om resource naartoe terug te brengen
using System;
using UnityEngine;

public interface ILocationOfResource
{
    public GameObject GetResourceToRetrieve();
    public int GetMaxRangeForResource();
    public RangeType GetRangeTypeToFindResource();

}