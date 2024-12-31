using System;
using UnityEngine;

public class UIItemProcessing
{
    public Enum Type;
    public DateTime? StartTimeBeingBuild;
    public bool IsBeingBuild => StartTimeBeingBuild.HasValue;
}