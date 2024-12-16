using Assets;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class QueueForBuildingBehaviour : MonoBehaviour
{
    public float BuildTimeInSeconds;
    public List<QueueItem> QueueItems;

    public ICardBuilding CallingBuilding;

    void Awake()
    {
        QueueItems = new List<QueueItem>();
        CallingBuilding = GetComponent<ICardBuilding>();
    }    

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
                CheckItemFinished(itemProcessed);
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

    public QueueItem GetCurrentItemProcessed()
    {
        return QueueItems.FirstOrDefault(x => x.IsBeingBuild);
    }

    private void CheckItemFinished(QueueItem item)
    {
        var timeItemFinished = item.StartTimeBeingBuild.Value.AddSeconds(BuildTimeInSeconds);
        if (timeItemFinished <= DateTime.Now)
        {
            HandleItemFinished(item);
        }
    }

    private void HandleItemFinished(QueueItem item)
    {
        // doe actie
        CallingBuilding.AddItem(item.Type);

        // verwijder item van queue
        QueueItems.Remove(item);
    }
}
