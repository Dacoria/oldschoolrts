using System;
using UnityEngine;


public class SerfOrder: IComparable<SerfOrder>
{
    public SerfRequest From { 
        get; 
        set;
    }

    public SerfRequest To
    {
        get; 
        set;
    }

    public float TimeCreated = Time.time;

    public ItemType ItemType => Current.ItemType;

    public SerfRequest Current
    {
        get
        {
            return Status switch
            {
                Status.IN_PROGRESS_FROM => From,
                _ => To
            };
        }
    }

    public Vector3 Location => Current?.Location ?? default;

    public int Priority =>Mathf.Min(From?.RequestPriority ?? int.MaxValue, To?.RequestPriority ?? int.MaxValue);

    private Status _requestStatus;
    public Status Status
    {
        get => _requestStatus;
        set
        {
            if (_requestStatus != value)
            {
                _requestStatus = value;
                AE.OrderStatusChanged?.Invoke(this);
            }
        }
    }

    public Purpose Purpose
    {
        get
        {
            Purpose p = Purpose.LOGISTICS;
            if (To != null)
            {
                if (To.Purpose < p)
                {
                    p = To.Purpose;
                }
            }

            if (From != null)
            {
                if (From.Purpose < p)
                {
                    p = From.Purpose;
                }
            }

            return p;
        }
    }

    public int CompareTo(SerfOrder other)
    {
        return this.To.CompareTo(other.To);
    }

    public SerfBehaviour Assignee { get; set; }

    public bool Has(SerfRequest serfRequest)
    {
        return From == serfRequest || To == serfRequest;
    }
}
