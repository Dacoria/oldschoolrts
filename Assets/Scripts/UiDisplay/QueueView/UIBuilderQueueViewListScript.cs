using System.Collections.Generic;
using UnityEngine;

public class UIBuilderQueueViewListScript : MonoBehaviourSlowUpdateFramesCI
{
    public UIQueueItemViewScript UIQueueItemViewPrefab;
    public Sprite BuildIcon;

    public List<UIQueueItemViewScript> CurrentUIQueueItemViews = new List<UIQueueItemViewScript>();

    protected override int FramesTillSlowUpdate => 40;

    protected override void SlowUpdate()
    {
        DestroyAllQueueItems();

        var builders = VillagerManager.Instance.GetBuilders();
        foreach (var builder in builders)
        {
            if(builder.CurrentBuilderRequest == null)
            {
                continue;
            }
            var goUIQueueItemView = CreateUIQueueItemGo(builder.CurrentBuilderRequest, Colorr.GrassGreen);
            CurrentUIQueueItemViews.Add(goUIQueueItemView);
        }

        foreach (var buildingReq in GameManager.Instance.GetBuilderRequests())
        {
            var goUIQueueItemView = CreateUIQueueItemGo(buildingReq, Colorr.Orange);
            CurrentUIQueueItemViews.Add(goUIQueueItemView);
        }
    }

    private UIQueueItemViewScript CreateUIQueueItemGo(BuilderRequest builderRequest, Color colorIcon)
    {
        var goUIQueueItemView = Instantiate(UIQueueItemViewPrefab, this.transform);
        goUIQueueItemView.Icon.sprite = BuildIcon;

        switch (builderRequest.Purpose)
        {
            case Purpose.ROAD:
                goUIQueueItemView.Text.text = "Road";
                goUIQueueItemView.Icon.color = colorIcon;
                break;
            case Purpose.BUILDING:
                goUIQueueItemView.Text.text = builderRequest.GameObject.name.Replace("(Clone)", "").Replace("Prefab", "");
                goUIQueueItemView.Icon.color = colorIcon;
                break;
            default:
                throw new System.Exception($"buildingReq.Purpose {builderRequest.Purpose} not supported");
        }

        return goUIQueueItemView;
    }

    private void DestroyAllQueueItems()
    {
        foreach (var uiQueueItemView in CurrentUIQueueItemViews)
        {
            Destroy(uiQueueItemView.gameObject);
        }
        CurrentUIQueueItemViews.Clear();
    }
}