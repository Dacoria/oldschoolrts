using UnityEngine;
using UnityEngine.AI;

public class VillagerBuilderScript : MonoBehaviour
{
    // Start is called before the first frame update

    [ComponentInject] private NavMeshAgent NavMeshAgent;
    [ComponentInject] private Animator Animator;

    private bool IsBuilding;

    public GameObject ToLocation;


    void Awake()
    {
        this.ComponentInject();
    }

    void Start()
    {
        ActionEvents.BuilderRequestStatusChanged += BuilderRequestStatusChanged;
    }

    private void BuilderRequestStatusChanged(BuilderRequest builderRequest, BuildStatus previousStatus)
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
        Animator.SetBool(StaticHelper.ANIM_BOOL_IS_WALKING, NavMeshAgent.enabled);
        Animator.SetBool(StaticHelper.ANIM_BOOL_IS_WORKING, IsBuilding);
        Animator.SetBool(StaticHelper.ANIM_BOOL_IS_IDLE, !NavMeshAgent.enabled);
    }
}
