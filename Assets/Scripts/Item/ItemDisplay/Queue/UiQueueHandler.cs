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
    private List<UiQueueItem> placedDisplayItemsOnQueue = new List<UiQueueItem>();

    public class UiQueueItem
    {
        public UiQueueCardBehaviour UiQueueCardBehaviour;
        public QueueItem QueueItem;
    }

    protected override int FramesTillSlowUpdate => 10;
    protected override void SlowUpdate()
    {
        UpdateQueue();
    }

    public TypeProcessing GetCurrentItemProcessed() => CallingQueueForBuildingBehaviour.CallingBuilding.GetCurrentTypeProcessed();

    public void OnCancelQueueItemClick(UiQueueCardBehaviour uiQueueCardBehaviour)
    {
        var item = placedDisplayItemsOnQueue.Single(x => x.UiQueueCardBehaviour == uiQueueCardBehaviour);
        CallingQueueForBuildingBehaviour.RemoveItemFromQueue(item.QueueItem);

        // TODO Items/resources teruggeven? (nog eerst regelen dat resources worden gebruikt.... )
    }

    private QueueForBuildingBehaviour LastKnownQueue;
    private TypeProcessing LastTypeProcessed;

    private void UpdateQueue()
    {
        var queueIsActive = CallingQueueForBuildingBehaviour?.QueueItems.Count > 0;
        QueueTitle.gameObject.SetActive(queueIsActive);
        UIQueueProgressBarScript.gameObject.SetActive(queueIsActive);

        if (CallingQueueForBuildingBehaviour != null && 
            (
                LastKnownQueue != CallingQueueForBuildingBehaviour ||
                (LastTypeProcessed != GetCurrentItemProcessed() && LastTypeProcessed == null) || 
                placedDisplayItemsOnQueue.Count != CallingQueueForBuildingBehaviour.QueueItems.Count)            
            )
        {
            DestroyItemsOnQueue();
            VisualizeCurrentQueue();
            LastKnownQueue = CallingQueueForBuildingBehaviour;
            LastTypeProcessed = GetCurrentItemProcessed();
        }
    }

    private void DestroyItemsOnQueue()
    {
        placedDisplayItemsOnQueue.RemoveAll(x => true);
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
            var removeCancelOptionUnit = !placedDisplayItemsOnQueue.Any() && GetCurrentItemProcessed() != null;  // 1e is niet te cancelen als die al gebouwd wordt
            displayUIQueueCard.CancelButtonGO.SetActive(!removeCancelOptionUnit); 
            placedDisplayItemsOnQueue.Add(displayQueueItem);
        }
    }

    private UiQueueCardBehaviour InstantiateQueueItem(QueueItem queueItem, BuildingType buildingType)
    {
        var queueCardGo = Instantiate(UIQueueCardPrefab, transform);
        var uiCardSettings = buildingType.GetProductionSettings();

        // TODO -> String compare ipv enums --> vanwege enum abstractie....
        var uiCardSetting = uiCardSettings.First(x => x.GetType().ToString() == queueItem.Type.ToString());
        queueCardGo.Image.sprite = uiCardSetting.GetIcon();

        queueCardGo.DisplayQueueUIHandler = this;

        return queueCardGo;
    }

    public float GetBuildTimeInSeconds() => CallingQueueForBuildingBehaviour.GetBuildTimeInSeconds();
}