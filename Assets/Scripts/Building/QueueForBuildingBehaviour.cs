using System;
using System.Linq;
using System.Collections.Generic;

public class QueueForBuildingBehaviour : MonoBehaviourCI
{
    public float GetBuildTimeInSeconds() => GetCurrentItemProcessed() == null ? 0f : CallingBuilding.GetProductionTime(GetCurrentItemProcessed().Type);
    public List<UIItemProcessing> QueueItems = new List<UIItemProcessing>();

    [ComponentInject] public ICardBuilding CallingBuilding;
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;

    public void AddItemOnQueue(Enum type)
    {
        var itemToPutOnQueue = new UIItemProcessing
        {
            Type = type,
        };

        QueueItems.Add(itemToPutOnQueue);
    }

    public void RemoveItemFromQueue(UIItemProcessing queueItem)
    {
        QueueItems.Remove(queueItem);
    }

    public void Update()
    {
        if (QueueItems.Count > 0)
        {
            var itemProcessed = GetCurrentItemProcessed();
            if (itemProcessed != null)
            {
                var isFinished = CheckItemFinished(itemProcessed);
                if (isFinished)
                {
                    HandleItemFinished(itemProcessed);
                }
            }
            else
            {
                var itemToStartWith = QueueItems.First();
                if (CallingBuilding.CanProces(itemToStartWith.Type))
                {
                    itemToStartWith.StartTimeBeingBuild = DateTime.Now;
                }
            }
        }
    }

    public UIItemProcessing GetCurrentItemProcessed() => QueueItems.FirstOrDefault(x => x.IsBeingBuild);

    private bool CheckItemFinished(UIItemProcessing item)
    {
        var timeItemFinished = item.StartTimeBeingBuild.Value.AddSeconds(GetBuildTimeInSeconds());
        return timeItemFinished <= DateTime.Now;       
    }

    private void HandleItemFinished(UIItemProcessing item)
    {
        // doe actie
        CallingBuilding.AddItem(item.Type);

        // verwijder item van queue
        QueueItems.Remove(item);
    }
}