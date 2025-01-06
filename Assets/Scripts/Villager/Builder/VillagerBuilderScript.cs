using UnityEngine;
using UnityEngine.AI;

public class VillagerBuilderScript : BaseAEMonoCI
{
    [ComponentInject] private NavMeshAgent navMeshAgent;
    [ComponentInject] private Animator animator;

    private bool isBuilding;

    public GameObject ToLocation;

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (previousStatus == BuildStatus.NEEDS_PREPARE && builderRequest.Status == BuildStatus.PREPARING)
        {
            isBuilding = true;
        }
        if (previousStatus == BuildStatus.NEEDS_BUILDING && builderRequest.Status == BuildStatus.BUILDING)
        {
            isBuilding = true;
        }
        if (builderRequest.Status == BuildStatus.COMPLETED_BUILDING || builderRequest.Status == BuildStatus.COMPLETED_PREPARING)
        {
            isBuilding = false;
        }
    }

    public void Update()
    {
        animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, navMeshAgent.enabled);
        animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, isBuilding);
        animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, !navMeshAgent.enabled);
    }
}