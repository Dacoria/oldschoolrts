using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using System;

public class UiQueueHandler : MonoBehaviourSlowUpdateFramesCI, IProcesOneItemUI
{
    // GO om te showen/hiden
    public Text QueueTitle;
    public UIProgressBarScript UIQueueProgressBarScript;

    // settings
    public UiQueueCardBehaviour UIQueueCardPrefab;    

    // setten van queue -> dit zorgt dat de queue ververst voor het gebouw
    [HideInInspector] public QueueForBuildingBehaviour CallingQueueForBuildingBehaviour;

    // display queue (weergave van items uit CallingQueueForBuildingBehaviour)
    private List<UiQueueItem> PlacedDisplayItemsOnQueue = new List<UiQueueItem>();

    public class UiQueueItem
    {
        public UiQueueCardBehaviour UiQueueCardBehaviour;
        public UIItemProcessing QueueItem;
    }

    protected override int FramesTillSlowUpdate => 20;
    protected override void SlowUpdate()
    {
        UpdateQueue();
    }

    public UIItemProcessing GetCurrentItemProcessed() => CallingQueueForBuildingBehaviour?.QueueItems.FirstOrDefault(x => x.IsBeingBuild);

    public void OnCancelQueueItemClick(UiQueueCardBehaviour uiQueueCardBehaviour)
    {
        var item = PlacedDisplayItemsOnQueue.Single(x => x.UiQueueCardBehaviour == uiQueueCardBehaviour);
        CallingQueueForBuildingBehaviour.RemoveItemFromQueue(item.QueueItem);

        // TODO Items/resources teruggeven? (nog eerst regelen dat resources worden gebruikt.... )
    }

    private QueueForBuildingBehaviour LastKnownQueue;    

    private void UpdateQueue()
    {
        QueueTitle.gameObject.SetActive(CallingQueueForBuildingBehaviour?.QueueItems.Count > 0);
        UIQueueProgressBarScript.gameObject.SetActive(CallingQueueForBuildingBehaviour?.QueueItems.Count > 0);

        if (CallingQueueForBuildingBehaviour != null && 
            (
                LastKnownQueue != CallingQueueForBuildingBehaviour || 
                PlacedDisplayItemsOnQueue.Count != CallingQueueForBuildingBehaviour.QueueItems.Count)            
            )
        {
            DestroyItemsOnQueue();
            VisualizeCurrentQueue();
            LastKnownQueue = CallingQueueForBuildingBehaviour;
        }
    }

    private void DestroyItemsOnQueue()
    {
        PlacedDisplayItemsOnQueue.RemoveAll(x => true);
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void VisualizeCurrentQueue()
    {
        var buildingType = CallingQueueForBuildingBehaviour.GetBuildingType();
        foreach (var queueItem in CallingQueueForBuildingBehaviour.QueueItems)
        {
            var displayUIQueueCard = InstantiateQueueItem(queueItem, buildingType);


            var displayQueueItem = new UiQueueItem
            {
                QueueItem = queueItem,
                UiQueueCardBehaviour = displayUIQueueCard
            };
            PlacedDisplayItemsOnQueue.Add(displayQueueItem);
        }
    }

    private UiQueueCardBehaviour InstantiateQueueItem(UIItemProcessing queueItem, BuildingType buildingType)
    {
        var queueCardGo = Instantiate(UIQueueCardPrefab, transform);
        var uiCardSettings = buildingType.GetProductionSettings();

        // TODO -> String compare ipv enums --> vanwege enum abstractie....
        var uiCardSetting = uiCardSettings.First(x => x.GetType().ToString() == queueItem.Type.ToString());
        queueCardGo.Image.sprite = uiCardSetting.GetIcon();

        queueCardGo.DisplayQueueUIHandler = this;

        return queueCardGo;
    }

    public float GetBuildTimeInSeconds(Enum type) => CallingQueueForBuildingBehaviour.GetBuildTimeInSeconds();
}