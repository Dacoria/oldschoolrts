using UnityEngine;

public class GhostBuildingBehaviour : MonoBehaviour
{
    public Purpose Purpose = Purpose.BUILDING;

    // Start is called before the first frame update

    // Begint uit! Wordt door ander script aangezet als de template mode naar ghost mode overgaat
    private void Start()
    {
        if (ActionEvents.BuilderRequest == null) 
        { 
            Debug.Log("GhostBuildingBehaviour in " + this.transform.gameObject.name + " -> geen GameManager!"); 
            return; 
        }

        var builderRequest = new BuilderRequest
        {
            Status = BuildStatus.NONE,
            Purpose = Purpose,
            GameObject = transform.parent.gameObject
        };
        ActionEvents.BuilderRequest(builderRequest);
        
        // forceert een change --> maakt het bijhouden makkelijker
        builderRequest.Status = BuildStatus.NEEDS_PREPARE;
    }
}