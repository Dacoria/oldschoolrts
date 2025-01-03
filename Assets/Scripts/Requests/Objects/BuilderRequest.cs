using System;
using System.Collections.Generic;
using UnityEngine;

public class BuilderRequest : IComparable<BuilderRequest>
{
    private static readonly Dictionary<Purpose, int> PurposeWeight =
        new Dictionary<Purpose, int>
        {
            {Purpose.ROAD, 1},
            {Purpose.BUILDING, 2},
            {Purpose.LOGISTICS, 3},
            {Purpose.FIELD, 4}
        };

    public int BufferDepth = 0;

    public Vector3 Location => GameObject.transform.position;

    public GameObject GameObject;
    public Purpose Purpose;

    private BuildStatus _status;
    public BuildStatus Status
    {
        get => _status;
        set
        {
            if (_status != value)
            {
                var previousValue = _status;
                _status = value;
                AE.BuilderRequestStatusChanged?.Invoke(this, previousValue);
            }
        }
    }

    public int RequestPriority =>
        PurposeWeight[Purpose] * 100 +
        BufferDepth * 10000;

    public int Priority =>
            PurposeWeight[Purpose] +
            BufferDepth * 10;

    public float TimeCreated = Time.time;

    public int CompareTo(BuilderRequest other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;

        if (RequestPriority == other.RequestPriority)
        {
            if (TimeCreated == other.TimeCreated)
            {
                return this.GetHashCode().CompareTo(other.GetHashCode());
            }
            return TimeCreated.CompareTo(other.TimeCreated);
        }
        return RequestPriority.CompareTo(other.RequestPriority);
    }
}