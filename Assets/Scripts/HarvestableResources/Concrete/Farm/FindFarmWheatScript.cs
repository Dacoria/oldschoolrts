using UnityEngine;

public class FindFarmWheatScript : MonoBehaviour, ILocationOfNewResource, ILocationOfResource
{
    public int MinRangeForWheatField = 1;
    public int MaxRangeForWheatField = 20;

    public int GetMaxRangeForResource() => MaxRangeForWheatField;
    public RangeType GetRangeTypeToFindResource() => RangeType.Circle;

    public Vector3 GetCoordinatesForNewResource()
    {
        var go = GetGameObjectForNewResource();
        if(go == null)
        {
            return new Vector3(0, 0, 0);
        }

        return go.transform.position;
    }

    public GameObject GetGameObjectForNewResource()
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
                    return wheatField;                    
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
