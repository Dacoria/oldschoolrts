using UnityEngine;

public abstract class MonoBehaviourCI : MonoBehaviour
{
    protected virtual void Awake()
    {
        this.ComponentInject();
    }
}