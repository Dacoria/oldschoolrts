using UnityEngine;

public class NoEnemiesLeftVictoryCondition : MonoBehaviourSlowUpdateFramesCI
{
    public GameObject Sprite;
    protected override int FramesTillSlowUpdate => 100;

    protected override void SlowUpdate()
    {
        if(this.GetComponentsInChildren<Transform>().Length <= 1)
        {
            Sprite.SetActive(true);
        }
    }
}