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
        if (Load.GoMap.TryGetValue(Constants.RSC_NAME_GO_SERF_PROCESSING, out GameObject processingDisplayPrefab))
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
            navMeshAgent.enabled = true;
            navMeshObstacle.enabled = false;
        }
    }
}