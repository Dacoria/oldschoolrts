using UnityEngine;

public interface IVillagerWorkAction
{
    public void SetReturnTargetForAction(GameObject objectToBringResourceBackTo);
    public bool CanDoAction();
    public bool IsActive();
    public void Init();
    public AnimationStatus GetAnimationStatus();
    public int GetPrio();
}