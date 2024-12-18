using UnityEngine;

public abstract class MonoBehaviourCI : MonoBehaviour
{
    protected void Awake()
    {
        this.ComponentInject();
    }
}