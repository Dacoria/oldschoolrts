using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundTarget : MonoBehaviour
{
    public GameObject TargetToMoveAround;

    public float Speed = 20;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(TargetToMoveAround.transform.position, Vector3.down, Speed * Time.deltaTime);
    }
}
