using Assets.Army;
using UnityEngine;
using UnityEngine.AI;

public class ArmyUnitBehaviour : MonoBehaviourSlowUpdateFramesCI
{
    public float EnemyAttractRadius; // wordt geset voor aanmaken
    public float Reach; // wordt geset voor aanmaken
    public Offence Offence; // wordt geset voor aanmaken
    public Defence Defence; // wordt geset voor aanmaken

    [ComponentInject] public NavMeshAgent NavMeshAgent;
    [ComponentInject] private Animator Animator;    

    private GameObject Target;

    public bool IsRanged => RangedHomingMissilePrefab != null;


    public RangedHomingMissileBehaviour RangedHomingMissilePrefab;
    public Transform RangedHomingMissileSpawnPosition;

    [ComponentInject] private OwnedByPlayerBehaviour OwnedByPlayerBehaviour;

    private void Start()
    {
        // TODO IETS MET REACH?

        NavMeshAgent.stoppingDistance = 0;
        AE.NewBattleUnit?.Invoke(this);
        gameObject.AddComponent<DisplayUnitIsSelected>();
    }


    private GameObject prevTarget;
    protected override int FramesTillSlowUpdate => 5;
    protected override void SlowUpdate()
    {       
        var colliders = Physics.OverlapSphere(this.transform.position, EnemyAttractRadius, 1 << Constants.LAYER_RTS_UNIT);
        
        Animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, !NavMeshAgent.StoppedAtDestination());
        Animator.SetBool(Constants.ANIM_BOOL_IS_ATTACKING, false);

        Target = null;
        foreach (var collider in colliders)
        {
            var otherOwnedByPlayerBehaviour = collider.gameObject.GetComponent<OwnedByPlayerBehaviour>();
            if (otherOwnedByPlayerBehaviour.Player != OwnedByPlayerBehaviour.Player)
            {
                Target = otherOwnedByPlayerBehaviour.gameObject;
                NavMeshAgent.destination = otherOwnedByPlayerBehaviour.transform.position;

                if (NavMeshAgent.StoppedAtDestination())
                {
                    FaceTarget(); // navmesh kan gestopt zijn terwijl je nog gedraait bent of van achteren wordt geraakkt
                    Animator.SetBool(Constants.ANIM_BOOL_IS_ATTACKING, true);
                }

                if (Target == prevTarget)
                {
                    // zelfde unit blijven aanvalllen tot deze niet meer collide
                    break;
                }
            }
        }

        prevTarget = Target;
    }

    private void FaceTarget()
    {
        var turnTowardNavSteeringTarget = NavMeshAgent.steeringTarget;

        Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }

    // Animation event!
    public void AttackHits()
    {
        if (Target != null)
        {
            if (IsRanged)
            {
                SpawnRangedHomingMissle();
            }
            else
            {
                HandleAttack.Handle(this.Offence, Target.gameObject);
            }
        }
    }

    private void SpawnRangedHomingMissle()
    {
        var rangedHomingMissile = Instantiate(RangedHomingMissilePrefab, RangedHomingMissileSpawnPosition.position, Quaternion.identity);
        rangedHomingMissile.SetTarget(Target);
        rangedHomingMissile.Offence = this.Offence;
    }
}