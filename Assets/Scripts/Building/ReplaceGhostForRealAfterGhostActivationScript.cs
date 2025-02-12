using System.Collections;

public class ReplaceGhostForRealAfterGhostActivationScript : MonoBehaviourCI
{
    public int TimeToWaitInSecondsForReplace = 5;
    [ComponentInject] private GhostBuildingBehaviour GhostBuildingBehaviour;
    [ComponentInject] private BuildingBehaviour BuildingBehaviour;

    private bool ScriptIsActive;
    private bool PreviousCheckIsInGhostMode;

    void Start()
    {
        ScriptIsActive = BuildingBehaviour != null && GhostBuildingBehaviour != null && !GhostBuildingBehaviour.isActiveAndEnabled;
        if (ScriptIsActive)
        {
            PreviousCheckIsInGhostMode = GhostBuildingBehaviour.isActiveAndEnabled;
        }        
    }

    void Update()
    {
        if (ScriptIsActive && GhostBuildingBehaviour != null && PreviousCheckIsInGhostMode != GhostBuildingBehaviour.isActiveAndEnabled)
        {
            StartCoroutine(ReplaceGhostForReal());
        }
    }

    private IEnumerator ReplaceGhostForReal()
    {
        yield return Wait4Seconds.Get(TimeToWaitInSecondsForReplace);
        BuildingBehaviour.Real.gameObject.SetActive(true);
        BuildingBehaviour.GhostBuildingBehaviour.gameObject.SetActive(false);
    }
}