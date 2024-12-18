using UnityEngine;

public class ShowHideExitEntranceScript : BaseAEMonoCI
{
    public GameObject Mark;   

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderReq, BuildStatus previousStatus)
    {
        // bij initieren v/e gebouwen verversen (normaal of via insta build)
        if(builderReq.Status == BuildStatus.COMPLETED_BUILDING || builderReq.Status == BuildStatus.NEEDS_PREPARE)
        {
            // refresh alle gebouwen
            HideShowDisplayEntranceExit(KeyCodeStatusSettings.ToggleEntranceExitDisplay_Active, builderReq.GameObject);
        }
    }

    protected override void OnKeyCodeAction(KeyCodeAction keyCodeAction)
    {
        if (keyCodeAction.KeyCodeActionType == KeyCodeActionType.ToggleEntranceExitDisplay)
        {
            HideShowDisplayEntranceExit(keyCodeAction.Active);
        }
    }

    private void HideShowDisplayEntranceExit(bool showEntranceExit, GameObject goJustFinishedBuilding = null)
    {
        var allEntranceExits = GameObject.FindGameObjectsWithTag(Constants.TAG_ENTRANCE_EXIT);
        foreach (var entranceExitGo in allEntranceExits)
        {
            if (showEntranceExit)
            {
                DestroyEntranceExitGo(entranceExitGo);
                CreateEntranceExitGo(entranceExitGo, goJustFinishedBuilding);
            }
            else
            {
                DestroyEntranceExitGo(entranceExitGo);
            }
        }
    }

    private void DestroyEntranceExitGo(GameObject entranceExitGo)
    {
        foreach (Transform child in entranceExitGo.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateEntranceExitGo(GameObject entranceExitGo, GameObject goJustFinishedBuilding = null)
    {
        var buildingScript = entranceExitGo.GetComponentInParent<BuildingBehaviour>();
        if (buildingScript != null || entranceExitGo.transform.parent.name == "MainBuilding")
        {
            var mark = Instantiate(Mark, new Vector3(entranceExitGo.transform.position.x, 0.03f, entranceExitGo.transform.position.z), Quaternion.identity);
            mark.transform.parent = entranceExitGo.transform;
        }
    }
}
