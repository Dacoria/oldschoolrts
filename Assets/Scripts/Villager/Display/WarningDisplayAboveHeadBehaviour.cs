using UnityEngine;

public class WarningDisplayAboveHeadBehaviour : BaseAEMonoCI
{

    public int NoWorkCounterToDisplayWarning = 5;
    private int currentNoWorkCounter;
    private GameObject warningGo;

    [ComponentInject] private WorkManager workManager;

    protected override void OnNoWorkerAction(WorkManager workMangerChanged)
    {
        if(workManager == workMangerChanged)
        {
            currentNoWorkCounter++;

            if (warningGo == null && currentNoWorkCounter >= NoWorkCounterToDisplayWarning)
            {
                InitiateWarningBubble();
            }            
        }
    }

    public void InitiateWarningBubble()
    {
        var warningGoPrefab = Load.GoMapUI[Constants.GO_PREFAB_UI_WARNING_DISPLAY];
        warningGo = Instantiate(warningGoPrefab, this.transform, false);

        warningGo.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        warningGo.transform.localPosition = new Vector3(0, 2.4f, 0); // net voor de borst -> voor nu hardcoded, allemaal cubes
    }

    protected override void OnStartNewWorkerAction(WorkManager workMangerChanged)
    {
        if (workManager == workMangerChanged)
        {
            StopWarningDisplay();            
        }
    }

    public void StopWarningDisplay()
    {
        currentNoWorkCounter = 0;
        Destroy(warningGo);
    }
}