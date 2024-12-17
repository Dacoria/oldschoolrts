using System.Linq;
using UnityEngine;
using System;

public class WarningDisplayAboveHeadBehaviour : BaseAEMono
{
    private GameObject WarningGo;

    public int NoWorkCounterToDisplayWarning = 5;
    private int currentNoWorkCounter;

    [ComponentInject]
    private WorkManager WorkManager;

    private new void Awake()
    {
        base.Awake();
        this.ComponentInject();
    }

    protected override void OnNoWorkerAction(WorkManager workMangerChanged)
    {
        if(WorkManager == workMangerChanged)
        {
            currentNoWorkCounter++;

            if (WarningGo == null && currentNoWorkCounter >= NoWorkCounterToDisplayWarning)
            {
                InitiateWarningBubble();
            }            
        }
    }

    public void InitiateWarningBubble()
    {
        var warningGoPrefab = MonoHelper.Instance.WarningGoPrefab;
        WarningGo = Instantiate(warningGoPrefab, this.transform, false);

        WarningGo.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        WarningGo.transform.localPosition = new Vector3(0, 2.4f, 0); // net voor de borst -> voor nu hardcoded, allemaal cubes
    }

    protected override void OnStartNewWorkerAction(WorkManager workMangerChanged)
    {
        if (WorkManager == workMangerChanged)
        {
            StopWarningDisplay();            
        }
    }

    public void StopWarningDisplay()
    {
        currentNoWorkCounter = 0;
        Destroy(WarningGo);
    }
}
