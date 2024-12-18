using Assets.Army;
using Assets.CrossCutting;
using UnityEngine;
using UnityEngine.AI;

public class ArmyUnitBehaviour : MonoBehaviourCI
{
    public float EnemyAttractRadius = 10;
    public float Reach = 1.5f;
    [ComponentInject] public NavMeshAgent NavMeshAgent;
    [ComponentInject] private Animator Animator;

    public Offence Offence;
    public Defence Defence;

    private GameObject Target;

    public bool IsRanged => RangedHomingMissilePrefab != null;

    public RangedHomingMissileBehaviour RangedHomingMissilePrefab;
    public Transform RangedHomingMissileSpawnPosition;

    [ComponentInject] private OwnedByPlayerBehaviour OwnedByPlayerBehaviour;

    private void Start()
    {
        NavMeshAgent.stoppingDistance = Reach;
    }

    private void Update()
    {
        var colliders = Physics.OverlapSphere(this.transform.position, EnemyAttractRadius, LayerMaskManager.RtsUnitMask);
        
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