
using System.Linq;
using UnityEngine.UI;

public class UiCardItemBeingBuild : MonoBehaviourSlowUpdateFramesCI
{
    public Image ItemToBuildImage;

    private IProcesOneItemUI procesOneItemUI;    

    void Start()
    {
        procesOneItemUI = MonoHelper.Instance.FindChildComponentInParents<IProcesOneItemUI>(gameObject);
    }

    protected override int FramesTillSlowUpdate => 20;

    protected override void SlowUpdate()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(procesOneItemUI.ItemIsBeingProcessed);
            if(procesOneItemUI.ItemIsBeingProcessed)
            {
                var rscPrefab = ResourcePrefabs.Get().Single(x => x.ItemType.ToString() == procesOneItemUI.GetCurrentItemProcessed().Type.ToString());
                ItemToBuildImage.sprite = rscPrefab.Icon;
            }
        }
    }
}