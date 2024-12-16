using System;
using System.Collections;
using UnityEngine;

public class RecoverHarvestableMaterialScript : MonoBehaviour
{
    [ComponentInject]
    private HarvestableMaterialScript HarvestableMaterialScript;

    public int RecoverAmount;
    public int RecoveryTimeInSeconds;

    public DateTime? StartDateRefilling;
    public bool isRefillingResources => StartDateRefilling.HasValue;

    private void Awake()
    {
        this.ComponentInject();
    }

    private int checkCounter;    

    void Update()
    {
        if(checkCounter == 0 && !isRefillingResources)
        {
            CheckToRefillResources();
        }

        checkCounter++;
        if(checkCounter >= 100)
        {
            checkCounter = 0;
        }
    }

    private void CheckToRefillResources()
    {
        if(HarvestableMaterialScript.MaterialCount == 0)
        {
            StartCoroutine(RecoverHarvestableMaterial());
        }
    }

    private IEnumerator RecoverHarvestableMaterial()
    {
        StartDateRefilling = DateTime.Now;
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(RecoveryTimeInSeconds);
        HarvestableMaterialScript.MaterialCount = RecoverAmount;
        StartDateRefilling = null;
    }
}
