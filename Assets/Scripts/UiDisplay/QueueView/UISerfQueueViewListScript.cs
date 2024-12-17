using System.Collections.Generic;
using UnityEngine;

public class UISerfQueueViewListScript : MonoBehaviourSlowUpdateFrames
{
    public UIQueueItemViewScript UIQueueItemViewPrefab;

    public Sprite ServeOrderIcon;
    public Sprite ServeRequestIcon;
    public Sprite ServeRoadIcon;
    public Sprite ServeBuildIcon;

    public List<UIQueueItemViewScript> CurrentUIQueueItemViews = new List<UIQueueItemViewScript>();

    protected override int FramesTillSlowUpdate => 40;
    protected override void SlowUpdate()
    {
        DestroyAllQueueItems();

        var serves = VillagerManager.Instance.GetSerfs();
        foreach (var serf in serves)
        {
            if(serf._currentSerfOrder == null)
            {
                continue;
            }
            var goUIQueueItemView = CreateUIQueueItemGo(serf._currentSerfOrder, Colorr.GrassGreen);
            CurrentUIQueueItemViews.Add(goUIQueueItemView);
        }

        foreach (var serfReq in GameManager.Instance.GetSerfRequests())
        {
            var goUIQueueItemView = CreateUIQueueItemGo(serfReq, Colorr.Orange);
            CurrentUIQueueItemViews.Add(goUIQueueItemView);
        }
    }

    private UIQueueItemViewScript CreateUIQueueItemGo(SerfOrder serfOrder, Color colorIcon)
    {
        var goUIQueueItemView = Instantiate(UIQueueItemViewPrefab, this.transform);
        switch (serfOrder.Purpose)
        {
            case Purpose.ROAD:
                goUIQueueItemView.Text.text = $"{serfOrder.To.ItemType.ToString().TitleCase()} -> Road";
                goUIQueueItemView.Icon.color = colorIcon;
                goUIQueueItemView.Icon.sprite = ServeRoadIcon;
                break;
            case Purpose.BUILDING:
                goUIQueueItemView.Text.text = $"{serfOrder.To.ItemType.ToString().TitleCase()} -> {serfOrder.To.GameObject.transform.parent.name.Replace("(Clone)", "").Replace("Prefab", "")}";
                goUIQueueItemView.Icon.color = colorIcon;
                goUIQueueItemView.Icon.sprite = ServeBuildIcon;
                break;
            case Purpose.LOGISTICS:
                goUIQueueItemView.Text.text = $"{serfOrder.From.ItemType.ToString().TitleCase()} -> {serfOrder.To.ItemType.ToString().TitleCase()}";
                goUIQueueItemView.Icon.color = colorIcon;
                goUIQueueItemView.Icon.sprite = ServeOrderIcon;
                break;            
            default:
                throw new System.Exception($"SerfRequest.Purpose {serfOrder.Purpose} not supported");
        }

        return goUIQueueItemView;
    }

    private UIQueueItemViewScript CreateUIQueueItemGo(SerfRequest serfRequest, Color colorIcon)
    {
        var goUIQueueItemView = Instantiate(UIQueueItemViewPrefab, this.transform);
        switch (serfRequest.Purpose)
        {
            case Purpose.ROAD:
                goUIQueueItemView.Text.text = $"{serfRequest.ItemType.ToString().TitleCase()} -> Road";
                goUIQueueItemView.Icon.color = colorIcon;
                goUIQueueItemView.Icon.sprite = ServeRequestIcon;
                break;
            case Purpose.BUILDING:
            case Purpose.LOGISTICS:
                goUIQueueItemView.Text.text = $"{serfRequest.ItemType.ToString().TitleCase()} -> {serfRequest.GameObject.transform.parent.name.Replace("(Clone)", "").Replace("Prefab", "")}";
                goUIQueueItemView.Icon.color = colorIcon;
                goUIQueueItemView.Icon.sprite = ServeRequestIcon;
                break;          
            default:
                throw new System.Exception($"SerfRequest.Purpose {serfRequest.Purpose} not supported");
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