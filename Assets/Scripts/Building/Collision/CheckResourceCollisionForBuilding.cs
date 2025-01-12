using UnityEngine;

public class CheckResourceCollisionForBuilding : MonoBehaviour
{
    public MaterialResourceType MaterialResourceTypeToCheck;   

    public bool IsCollidingWithRequiredResource(Collider other)
    {  
        if (other.transform.tag == Constants.TAG_RESOURCE  && 
        other.transform.gameObject.layer != Constants.LAYER_TERRAIN)
        {            
            var resourceScript = other.gameObject.GetComponent<HarvestableMaterialScript>();
            if(resourceScript != null)
            {
                return resourceScript.MaterialType == MaterialResourceTypeToCheck;
            }
            
        }

        return false;
    }    
}