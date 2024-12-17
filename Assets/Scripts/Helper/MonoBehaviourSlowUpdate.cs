using UnityEngine;

public abstract class MonoBehaviourSlowUpdate : MonoBehaviour
{
    protected abstract int FramesTillSlowUpdate { get; }

    private int frameCounter;

    protected void Update()
    {
        frameCounter++;
        if(frameCounter >= FramesTillSlowUpdate)
        {
            frameCounter = 0;
            SlowUpdate();
        }
    }

    protected abstract void SlowUpdate();
}