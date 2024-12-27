using UnityEngine;

public class RotateScript : MonoBehaviourCI
{
    public float Speed;
        
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0.1f * Speed, 0, 0));
    }
}