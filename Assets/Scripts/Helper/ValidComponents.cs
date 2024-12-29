using System.Collections.Generic;
using System;
using UnityEngine;

public class ValidComponents : MonoBehaviour
{
    public void DoCheck(List<Type> actives = null, List<Type> inactives = null)
    {
        if (actives != null)
        {
            foreach (var type in actives)
            {
                if (gameObject.GetComponent(type) == null)
                {
                    throw new Exception($"Component '{type}' NIET active on go '{gameObject.name}' -> not allowed");
                }
            }
        }
        if (inactives != null)
        {
            foreach (var type in inactives)
            {
                if (gameObject.GetComponent(type) != null)
                {
                    throw new Exception($"Component '{type}' WEL active on go '{gameObject.name}' -> not allowed");
                }
            }
        }

        Destroy(this);
    }
}