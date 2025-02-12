using System;
using System.Linq;
using System.Collections.Generic;

public class QueueForBuildingBehaviour : BaseAEMonoCI
{
    public float GetBuildTimeInSeconds() => GetCurrentItemProcessed() == null ?
        0f :
        CallingBuilding.GetBuildingType().GetProductionDurationSettings().TimeToProduceResourceInSeconds;
    public List<QueueItem> QueueItems = new List<QueueItem>();

    [ComponentInject] public ICardSelectProdBuilding CallingBuilding;
    [ComponentInject] private BuildingBehaviour buildingBehaviour;

    public BuildingType GetBuildingType() => buildingBehaviour.BuildingType;

    protected override void OnFinishedWaitingAfterProducingAction(BuildingBehaviour building)
    {
        if(building == buildingBehaviour)
        {
            QueueItems.Remove(QueueItems.FirstOrDefault());
        }
    }

    public void AddItemsOnQueue(Enum type, int amount)
    {
        var queuelimit = 14;
        var amountToAdd = Math.Min(queuelimit - QueueItems.Count, amount);

        for (int i = 0; i < amountToAdd; i++)
        {
            QueueItems.Add(new QueueItem { Type = type });
        }
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
            if (itemProcessed == null)
            {
                var itemToCreate = QueueItems.First();
                if (CallingBuilding.CanProces(itemToCreate.Type))
                {
                    CallingBuilding.AddType(itemToCreate.Type);
                }
            }
        }
    }

    public TypeProcessing GetCurrentItemProcessed() => CallingBuilding.GetCurrentTypeProcessed();    
}