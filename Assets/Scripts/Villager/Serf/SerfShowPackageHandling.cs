using UnityEngine;
using UnityEngine.AI;

public class SerfShowPackageHandling : MonoBehaviourCI
{
    [ComponentInject] private NavMeshAgent navMeshAgent;
    [ComponentInject] private NavMeshObstacle navMeshObstacle;

    private GameObject ProcessingPackageGO;

    private new void Awake()
    {
        base.Awake();
        if (Load.GoMapUI.TryGetValue(Constants.GO_PREFAB_UI_SERF_PROCESSING_DISPLAY, out GameObject processingDisplayPrefab))
        {
            ProcessingPackageGO = Instantiate(processingDisplayPrefab, transform);
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
        }
    }

    private void OnDestroy()
    {
        if(ProcessingPackageGO != null)
        {
            Destroy(ProcessingPackageGO);
            navMeshObstacle.enabled = false;
            navMeshAgent.enabled = true;
        }
    }
}