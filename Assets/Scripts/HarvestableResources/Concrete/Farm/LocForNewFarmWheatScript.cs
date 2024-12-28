using UnityEngine;

public class LocForNewFarmWheatScript : MonoBehaviour, ILocForNewResource, ILocationOfResource
{
    public int MinRangeForWheatField = 1;
    public int MaxRangeForWheatField = 20;

    public int GetMaxRangeForResource() => MaxRangeForWheatField;
    public RangeType GetRangeTypeToFindResource() => RangeType.Circle;

    public LocForRsc GetLocationForNewResource()
    {
        var wheatFields = GameObject.FindGameObjectsWithTag(Constants.TAG_WHEATFIELD);

        foreach (var wheatField in wheatFields)
        {
            var distance = Vector3.Distance(wheatField.transform.position, transform.position);

            if (distance < MaxRangeForWheatField)
            {
                var farmFieldScript = wheatField.GetComponent<FarmFieldScript>();
                if (farmFieldScript != null && 
                    farmFieldScript.enabled && 
                    farmFieldScript.ObjectGrownOnField == null &&
                    !farmFieldScript.HasObjectGrownOnField
                    )
                {                   
                    return new LocForRsc { GO=wheatField, V3=wheatField.transform.position };
                }
            }
        }

        return null;
    }

    public GameObject GetResourceToRetrieve()
    {
        var wheatFields = GameObject.FindGameObjectsWithTag(Constants.TAG_WHEATFIELD);
        GameObject closestWheatField = null;
        var closestDistance = 9999999f;

        foreach (var wheatField in wheatFields)
        {
            var distance = Vector3.Distance(wheatField.transform.position, transform.position);

            if (distance < MaxRangeForWheatField && distance < closestDistance)
            {
                var farmFieldScript = wheatField.GetComponent<FarmFieldScript>();
                if(farmFieldScript != null && 
                    farmFieldScript.enabled && 
                    farmFieldScript.HasObjectGrownOnFieldFinishedGrowing &&
                    farmFieldScript.CanRetrieveResource()
                    )
                {
                    closestWheatField = wheatField;
                    closestDistance = distance;
                }               
            }
        }

        return closestWheatField;
    }
}