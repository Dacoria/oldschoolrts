using UnityEngine;

public class MoveAroundTarget : MonoBehaviour
{
    public GameObject TargetToMoveAround;

    public float Speed = 20;

    void Update()
    {
        transform.RotateAround(TargetToMoveAround.transform.position, Vector3.down, Speed * Time.deltaTime);
    }
}
