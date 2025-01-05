using System;
using UnityEngine;

public class DestroyGoOnRightClick : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(gameObject);
        }
    }
}
