using TMPro;

public class SetTextOfBuilding : MonoBehaviourCI
{
    [ComponentInject] private TextMeshPro TextMeshPro;

    void Start()
    {
        var highestParentGO = MonoHelper.Instance.GetHighestParent(this.gameObject);
        TextMeshPro.text = highestParentGO.name.Replace("Prefab", "").Replace("(Clone)", "");
    }
}