using UnityEngine;

public class CheckResourceCollisionForBuilding : MonoBehaviour
{
    public MaterialResourceType MaterialResourceTypeToCheck;
   

    public bool IsCollidingWithRequiredResource(Collider other)
    {  
        if (other.transform.tag == StaticHelper.TAG_RESOURCE  && 
        other.transform.gameObject.layer != StaticHelper.LAYER_TERRAIN)
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
