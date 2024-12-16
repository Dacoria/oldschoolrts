using System.Linq;
using UnityEngine;
using System;

public class WarningDisplayAboveHeadBehaviour : MonoBehaviour
{
    private GameObject WarningGo;

    public int NoWorkCounterToDisplayWarning = 5;
    private int currentNoWorkCounter;

    [ComponentInject]
    private WorkManager WorkManager;

    private void Awake()
    {
        this.ComponentInject();
    }

    private void Start()
    {
        ActionEvents.NoWorkerAction += OnNoWorkerAction;
        ActionEvents.StartNewWorkerAction += OnStartNewWorkerAction;
    }

    private void OnNoWorkerAction(WorkManager workMangerChanged)
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

    private void OnStartNewWorkerAction(WorkManager workMangerChanged)
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

    private void OnDestroy()
    {
        ActionEvents.NoWorkerAction -= OnNoWorkerAction;
        ActionEvents.StartNewWorkerAction -= OnStartNewWorkerAction;
    }
}
