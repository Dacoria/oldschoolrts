using UnityEngine;
using UnityEngine.AI;

public class VillagerBuilderScript : BaseAEMonoCI
{
    [ComponentInject] private NavMeshAgent NavMeshAgent;
    [ComponentInject] private Animator Animator;

    private bool IsBuilding;

    public GameObject ToLocation;

    protected override void OnBuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
    {
        if (previousStatus == BuildStatus.NEEDS_PREPARE && builderRequest.Status == BuildStatus.PREPARING)
        {
            IsBuilding = true;
        }
        if (previousStatus == BuildStatus.NEEDS_BUILDING && builderRequest.Status == BuildStatus.BUILDING)
        {
            IsBuilding = true;
        }
        if (builderRequest.Status == BuildStatus.COMPLETED_BUILDING || builderRequest.Status == BuildStatus.COMPLETED_PREPARING)
        {
            IsBuilding = false;
        }
    }

    public void Update()
    {
        Animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, NavMeshAgent.enabled);
        Animator.SetBool(Constants.ANIM_BOOL_IS_WORKING, IsBuilding);
        Animator.SetBool(Constants.ANIM_BOOL_IS_IDLE, !NavMeshAgent.enabled);
    }
}