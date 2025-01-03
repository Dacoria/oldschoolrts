using UnityEngine;

public class GhostBuildingBehaviour : MonoBehaviour
{
    public Purpose Purpose = Purpose.BUILDING;    

    // Begint uit! Wordt door ander script aangezet als de template mode naar ghost mode overgaat
    private void Start()
    {
        var builderRequest = new BuilderRequest
        {
            Status = BuildStatus.NONE,
            Purpose = Purpose,
            GameObject = transform.parent.gameObject
        };
        AE.BuilderRequest?.Invoke(builderRequest);
        
        // forceert een change --> maakt het bijhouden makkelijker
        builderRequest.Status = BuildStatus.NEEDS_PREPARE;
    }
}