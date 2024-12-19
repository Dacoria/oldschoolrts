using System;
using TMPro;
using UnityEngine;

public class BuildProgressTextScript : MonoBehaviourCI
{
    public TextMeshPro BuildProgressText;
    public GameObject BuildProgressImage;
    [ComponentInject] private BuildingBehaviour BuildingBehaviour;

    void Start()
    {
        if (BuildingBehaviour == null)
        {
            throw new Exception("Progressie met text vereist BuildingBehaviour in parent!");
        }
        BuildProgressText.transform.gameObject.SetActive(true); // Workaround --> direct enablen zorgt ervoor dat de text blokken wordt...... bug Unity
    }

    void Update()
    {
        switch(BuildingBehaviour.CurrentBuildStatus)
        {
            case BuildStatus.BUILDING:
            case BuildStatus.PREPARING:
                BuildProgressImage.SetActive(true);
                UpdateBuildItemProgress();
                break;
            case BuildStatus.COMPLETED_BUILDING:
                this.enabled = false; // disable deze GO weer (stopt ook de updates)
                break;
            default:
                BuildProgressImage.SetActive(false);
                BuildProgressText.text = "";
                break;            
        }
    }

    private void UpdateBuildItemProgress()
    {
        // BS: meerere bouwers? doen we nu niks mee
        var durationBuildingInMs = BuildingBehaviour.CurrentBuildStatus == BuildStatus.PREPARING ?
            BuildingBehaviour.TimeToPrepareBuildingInSeconds * 1000 : 
            BuildingBehaviour.TimeToBuildRealInSeconds * 1000;

        var timeSpendBuildingInMs = (DateTime.Now - BuildingBehaviour.StartTimeBuildingTheBuilding).TotalMilliseconds;
        var percBuild = (timeSpendBuildingInMs / (durationBuildingInMs)) * 100;

        BuildProgressText.text = percBuild.ToString("F0") + " %";
    }
}