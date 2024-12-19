using System.Collections;

public abstract class MonoBehaviourSlowUpdateTimeCI : MonoBehaviourCI
{
    protected abstract int MsTillSlowUpdate { get; }

    protected void Start()
    {
        StartCoroutine(InitSlowUpdate());
    }

    private IEnumerator InitSlowUpdate()
    {
        yield return Wait4Seconds.Get(MsTillSlowUpdate / 1000f);
        SlowUpdate();

        StartCoroutine(InitSlowUpdate());
    }

    protected abstract void SlowUpdate();
}