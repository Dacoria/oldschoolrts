using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeDisplayBehaviour : MonoBehaviour
{
    public int cirlceSegments = 50;
    LineRenderer line;

    public RangeType RangeType;
    public int MaxRangeForResource = 20;

    [ComponentInject]
    private BoxCollider boxCollider;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (RangeType == RangeType.None)
        {
            return;
        }

        line = gameObject.GetComponent<LineRenderer>();        
        line.useWorldSpace = false;
        if (RangeType == RangeType.Circle)
        {
            line.positionCount = cirlceSegments + 1;
            CreateCirlcePoints();
        }
        else if(RangeType == RangeType.BoxColliderExpand)
        {
            line.positionCount = 5;
            CreateBoxColliderPoints();
        }
    }

    private void CreateBoxColliderPoints()
    {
        this.ComponentInject(); // box collider

        var trans = boxCollider.transform;
        var min = boxCollider.center - boxCollider.size * 0.5f;
        var max = boxCollider.center + boxCollider.size * 0.5f;

        var values = new List<Vector2>
        {
            trans.TransformPoint(new Vector2(min.x - MaxRangeForResource, min.z - MaxRangeForResource)),
            trans.TransformPoint(new Vector2(min.x - MaxRangeForResource, max.z + MaxRangeForResource)),
            trans.TransformPoint(new Vector2(max.x + MaxRangeForResource, max.z + MaxRangeForResource)),
            trans.TransformPoint(new Vector2(max.x + MaxRangeForResource, min.z - MaxRangeForResource)),
            trans.TransformPoint(new Vector2(min.x - MaxRangeForResource, min.z - MaxRangeForResource))
        };

        for (int i = 0; i < values.Count; i++)
        {
            line.SetPosition(i, new Vector3(values[i].x, values[i].y, 0));
            line.transform.localPosition = new Vector3(0, 0.01f, 0);
        }
    }

    void CreateCirlcePoints()
    {
        float angle = 20f;

        for (int i = 0; i < (cirlceSegments + 1); i++)
        {
            var x = Mathf.Sin(Mathf.Deg2Rad * angle) * MaxRangeForResource;
            var y = Mathf.Cos(Mathf.Deg2Rad * angle) * MaxRangeForResource;

            line.SetPosition(i, new Vector3(x, y, 0));
            line.transform.localPosition = new Vector3(0, 0.01f, 0);
            angle += (360f / cirlceSegments);
        }
    }
}
