using System.Collections;
using UnityEngine;

public abstract class MonoBehaviourSlowUpdateTime : MonoBehaviour
{
    protected abstract int MsTillSlowUpdate { get; }

    protected void Start()
    {
        StartCoroutine(InitSlowUpdate());
    }

    private IEnumerator InitSlowUpdate()
    {
        yield return new WaitForSeconds(MsTillSlowUpdate / 1000f);
        SlowUpdate();

        StartCoroutine(InitSlowUpdate());
    }

    protected abstract void SlowUpdate();
}