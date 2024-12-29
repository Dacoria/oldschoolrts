using System;
using System.Linq;
using System.Collections.Generic;

public class QueueForBuildingBehaviour : MonoBehaviourCI
{
    public float GetBuildTimeInSeconds() => GetCurrentItemProcessed() == null ? 0f : CallingBuilding.GetProductionTime(GetCurrentItemProcessed().Type);
    public List<QueueItem> QueueItems = new List<QueueItem>();

    [ComponentInject] public ICardBuilding CallingBuilding;

    public BuildingType GetBuildingType()
    {
        var buildingBehaviour = transform.GetComponentInParent<BuildingBehaviour>();
        return buildingBehaviour.BuildingType;
    }

    public void AddItemOnQueue(Enum type)
    {
        var itemToPutOnQueue = new QueueItem
        {
            Type = type,
        };

        QueueItems.Add(itemToPutOnQueue);
    }

    public void RemoveItemFromQueue(QueueItem queueItem)
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

    public QueueItem GetCurrentItemProcessed() => QueueItems.FirstOrDefault(x => x.IsBeingBuild);

    private bool CheckItemFinished(QueueItem item)
    {
        var timeItemFinished = item.StartTimeBeingBuild.Value.AddSeconds(GetBuildTimeInSeconds());
        return timeItemFinished <= DateTime.Now;       
    }

    private void HandleItemFinished(QueueItem item)
    {
        // doe actie
        CallingBuilding.AddItem(item.Type);

        // verwijder item van queue
        QueueItems.Remove(item);
    }
}