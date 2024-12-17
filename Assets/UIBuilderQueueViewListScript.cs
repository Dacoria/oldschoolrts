using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIBuilderQueueViewListScript : MonoBehaviour
{
    private int frameCount;
    private int frameLimit = 40;
    public UIQueueItemViewScript UIQueueItemViewPrefab;
    public Sprite BuildIcon;

    public List<UIQueueItemViewScript> CurrentUIQueueItemViews = new List<UIQueueItemViewScript>();

    void Update()
    {
        frameCount++;
        if (frameCount >= frameLimit)
        {
            SlowUpdate();
            frameCount = 0;
        }        
    }

    private void SlowUpdate()
    {
        DestroyAllQueueItems();

        var builders = VillagerManager.Instance.GetBuilders();
        foreach (var builder in builders)
        {
            if(builder._currentBuilderRequest == null)
            {
                continue;
            }
            var goUIQueueItemView = CreateUIQueueItemGo(builder._currentBuilderRequest, Colorr.GrassGreen);
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