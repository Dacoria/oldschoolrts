using System.Collections;
using UnityEngine;

public abstract class MonoBehaviourSlowUpdateTimeCI : MonoBehaviourCI
{
    protected abstract int MsTillSlowUpdate { get; }

    protected void Start()
    {
        StartCoroutine(InitSlowUpdate());
    }

    private IEnumerator InitSlowUpdate()
    {
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(MsTillSlowUpdate / 1000f);
        SlowUpdate();

        StartCoroutine(InitSlowUpdate());
    }

    protected abstract void SlowUpdate();
}