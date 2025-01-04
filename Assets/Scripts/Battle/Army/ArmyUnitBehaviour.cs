using Assets.Army;
using UnityEngine;
using UnityEngine.AI;

public class ArmyUnitBehaviour : MonoBehaviourCI
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
        NavMeshAgent.stoppingDistance = Reach;
        AE.NewBattleUnit?.Invoke(this);
        gameObject.AddComponent<DisplayUnitIsSelected>();
    }

    private void Update()
    {
        var colliders = Physics.OverlapSphere(this.transform.position, EnemyAttractRadius, 1 << Constants.LAYER_RTS_UNIT);
        
        Animator.SetBool(Constants.ANIM_BOOL_IS_WALKING, !NavMeshAgent.StoppedAtDestination());
        Animator.SetBool(Constants.ANIM_BOOL_IS_ATTACKING, false);

        foreach (var collider in colliders)
        {
            var otherOwnedByPlayerBehaviour = collider.gameObject.GetComponent<OwnedByPlayerBehaviour>();
            if (otherOwnedByPlayerBehaviour.Player != OwnedByPlayerBehaviour.Player)
            {
                Target = otherOwnedByPlayerBehaviour.gameObject;
                NavMeshAgent.destination = otherOwnedByPlayerBehaviour.transform.position;

                if (NavMeshAgent.StoppedAtDestination())
                {
                    Animator.SetBool(Constants.ANIM_BOOL_IS_ATTACKING, true);
                }
            }
        }
    }

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
        var rangedHomingMissile = Instantiate(RangedHomingMissilePrefab, RangedHomingMissileSpawnPosition.position, Quaternion.identity) as RangedHomingMissileBehaviour;
        rangedHomingMissile.SetTarget(Target);
        rangedHomingMissile.Offence = this.Offence;
    }    
}