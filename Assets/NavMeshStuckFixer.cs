using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshStuckFixer : MonoBehaviourSlowUpdate
{
    [ComponentInject] private NavMeshAgent NavMashAgent;

    private void Awake()
    {
        this.ComponentInject();
    }

    protected override int FramesTillSlowUpdate => 20;

    protected override void SlowUpdate()
    {
        throw new System.NotImplementedException();
    }
}
