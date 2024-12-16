using System;
using System.Collections.Generic;
using UnityEngine;

public class SerfRequest : IComparable<SerfRequest>
{
    public static Action<SerfOrder> OrderStatusChanged;

    private static readonly Dictionary<Purpose, int> PurposeWeight =
        new Dictionary<Purpose, int>
        {
            {Purpose.ROAD, 1},
            {Purpose.BUILDING, 2},
            {Purpose.LOGISTICS, 3},
            {Purpose.FIELD, 4}
        };

    public int BufferDepth = 0;

    public Vector3 Location => this.GameObject.EntranceExit();

    public GameObject GameObject;
    public Purpose Purpose = Purpose.LOGISTICS;
    public ItemType ItemType;

    public int RequestPriority =>
        PurposeWeight[Purpose] +
        BufferDepth * 10;

    public Direction Direction { get; set; }
    public bool IsOriginator { get; set; }

    public float TimeCreated = Time.time;

    // TODO push & pull heeft zelfde prio --> volle koolmijn belangrijker dan goudsmelter die goudore nodig heeft -> hoort niet (PULL belangrijker dan PUSH)
    // TODO 2 (next level?), iets lagere prio, maar serf is wel veel dichterbij -> pak die
    // TODO 3 PurpuseWeigth belangrijker maken?
    // Let op; limiet van 99 in compare
    public int CompareTo(SerfRequest other)
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

    public SerfRequest CloneOpposite()
    {
        return new SerfRequest
        {
            Purpose = this.Purpose,
            ItemType = this.ItemType,
            GameObject = this.GameObject,
            IsOriginator = false,
            Direction = this.Direction == Direction.PULL ? Direction.PUSH : Direction.PULL,
            TimeCreated = this.TimeCreated,
            BufferDepth = this.BufferDepth,
        };
    }
}

