using Assets.Army;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ArmyUnitBehaviour : MonoBehaviourSlowUpdateFramesCI
{
    public float EnemyAttractRadius { get; private set; } 
    public float Reach { get; private set; }
    public Offence Offence { get; private set; }
    public Defence Defence { get; private set; }

    [ComponentInject] private NavMeshAgent navMeshAgent;
    [ComponentInject] private Animator animator;
    [ComponentInject] private HealthBehaviour healthBehaviour;

    public BarracksUnitType BarracksUnitType;

    private GameObject target;

    public bool isRanged => RangedHomingMissilePrefab != null;

    public RangedHomingMissileBehaviour RangedHomingMissilePrefab;
    public Transform RangedHomingMissileSpawnPosition;

    [ComponentInject] private OwnedByPlayerBehaviour ownedByPlayerBehaviour;
    private CompassDirection activeDirection;

    private void Start()
    {
        SetUnitStats();
        AE.NewBattleUnit?.Invoke(this);
        gameObject.AddComponent<DisplayUnitIsSelected>();
    }


    private GameObject prevTarget;
    protected override int FramesTillSlowUpdate => 5;
    protected override void SlowUpdate()
    {       
        var colliders = Physics.OverlapSphere(this.transform.position, EnemyAttractRadius, 1 << Constants.LAYER_RTS_UNIT);
        
        animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, !navMeshAgent.StoppedAtDestination());
        animator.SetBool(Constants.ANIM_BOOL_IS_ATTACKING, false);

        target = null;
        var isAttacking = false;
        foreach (var collider in colliders)
        {
            var otherOwnedByPlayerBehaviour = collider.gameObject.GetComponent<OwnedByPlayerBehaviour>();
            if (otherOwnedByPlayerBehaviour.Player != ownedByPlayerBehaviour.Player)
            {
                target = otherOwnedByPlayerBehaviour.gameObject;
                navMeshAgent.destination = otherOwnedByPlayerBehaviour.transform.position;

                if (navMeshAgent.StoppedAtDestination())
                {
                    isAttacking = true;
                    FaceTarget(); // navmesh kan gestopt zijn terwijl je nog gedraait bent of van achteren wordt geraakkt
                    animator.SetBool(Constants.ANIM_BOOL_IS_ATTACKING, true);
                }

                if (target == prevTarget)
                {
                    // zelfde unit blijven aanvalllen tot deze niet meer collide
                    break;
                }
            }
        }

        if(RtsUnitSelectionManager.Instance.CurrentSelected.GetUnits().Any(x => x == gameObject))
        {
            activeDirection = RtsUnitSelectionManager.Instance.CurrentSelected.CurrentDirection;
        }
        if(!isAttacking && navMeshAgent.StoppedAtDestination())
        {
            FaceActiveDirection();
        }

        prevTarget = target;
    }

    private void FaceTarget()
    {
        var turnTowardNavSteeringTarget = navMeshAgent.steeringTarget;

        Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }

    private void FaceActiveDirection()
    {
        var lDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * activeDirection.GetAngle()), 0, Mathf.Cos(Mathf.Deg2Rad * activeDirection.GetAngle()));
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lDirection.x, 0, lDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }

    // Animation event!
    public void AttackHits()
    {
        if (target != null)
        {
            if (isRanged)
            {
                SpawnRangedHomingMissle();
            }
            else
            {
                HandleAttack.Handle(this.Offence, target.gameObject);
            }
        }
    }

    private void SpawnRangedHomingMissle()
    {
        var rangedHomingMissile = Instantiate(RangedHomingMissilePrefab, RangedHomingMissileSpawnPosition.position, Quaternion.identity);
        rangedHomingMissile.SetTarget(target);
        rangedHomingMissile.Offence = this.Offence;
    }

    private void SetUnitStats()
    {
        var unitStats = BarracksUnitType.GetUnitStats();

        Offence = unitStats.Offence.DeepClone();
        Defence = unitStats.Defence.DeepClone();
        EnemyAttractRadius = unitStats.RangeToAttractEnemies;
        Reach = unitStats.RangeToAttack;
        //NavMeshAgent.stoppingDistance = unitStats.RangeToAttack;
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.speed = unitStats.Speed;       
    }
}