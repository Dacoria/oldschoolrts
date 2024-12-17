using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;

public class GameManager : MonoBehaviour
{
    public GameObject MainCastle;
    public GameObject RoadNavMeshSurfacePrefab;
    public GameObject RoadNavMeshSurface;

    private static SortedSet<SerfRequest> SerfRequests;
    private SortedSet<SerfOrder> CurrentSerfOrders;
    private static List<SerfBehaviour> freeSerfs;

    private static List<BuilderBehaviour> freeBuilders;
    private static SortedSet<BuilderRequest> BuilderRequests;

    public SortedSet<SerfRequest> GetSerfRequests() => SerfRequests;
    public SortedSet<SerfOrder> GetCurrentSerfOrders() => CurrentSerfOrders;
    public SortedSet<BuilderRequest> GetBuilderRequests() => BuilderRequests;


    public SortedSet<SerfOrder> CompletedOrders;
    public static GameManager Instance { get; set; }


    private List<StockpileBehaviour> stockpiles;
    public List<ResourcePrefabItem> ResourcePrefabItems;
    public List<BuildingPrefabItem> BuildingPrefabItems;

    public List<ItemFoodRefillValue> ItemFoodRefillValues;

    public GameObject GoToTavernBubble;
    public Sprite UnknownSprite;
    public GameObject UnknownGameObjectPrefab;


    public static int PopulationLimit = 8;
    public static int CurrentPopulation = 0;

    internal void RegisterStockpile(StockpileBehaviour stockpileBehaviour)
    {
        stockpiles.Add(stockpileBehaviour);
    }

    internal void TryRemoveStockpile(StockpileBehaviour stockpileBehaviour)
    {
        var stockpile = stockpiles.FirstOrDefault(x => x == stockpileBehaviour);
        if (stockpile != null)
        {
            stockpiles.Remove(stockpile);
        }
    }

    void Awake()
    {
        Instance = this;

        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            var countItemTypeInDict = ResourcePrefabItems.Count(x => x.ItemType == itemType);
            if (countItemTypeInDict != 1)
            {
                throw new Exception("ItemType " + itemType + " komt " + countItemTypeInDict + "x voor ipv 1 -- > Zie Grass -> ResourcePrefabDictionary");
            }

            var resourceItem = ResourcePrefabItems.Single(x => x.ItemType == itemType);
            if (resourceItem.Icon == null)
            {
                resourceItem.Icon = UnknownSprite;
            }
            if (resourceItem.ResourcePrefab == null)
            {
                resourceItem.ResourcePrefab = UnknownGameObjectPrefab;
            }
        }
        foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
        {
            var countItemTypeInDict = BuildingPrefabItems.Count(x => x.BuildingType == buildingType);
            if (buildingType != BuildingType.NONE && countItemTypeInDict != 1)
            {
                throw new Exception("BuildingType " + buildingType + " komt " + countItemTypeInDict + "x voor ipv 1 -- > Zie Grass -> BuildingPrefabItems");
            }
        }

        if (RoadNavMeshSurface == null)
        {
            RoadNavMeshSurface = Instantiate(RoadNavMeshSurfacePrefab);
        }

        stockpiles = new List<StockpileBehaviour>();

        BuilderRequests = new SortedSet<BuilderRequest>();
        SerfRequests = new SortedSet<SerfRequest>();
        CurrentSerfOrders = new SortedSet<SerfOrder>();
        CompletedOrders = new SortedSet<SerfOrder>();

        freeSerfs = new List<SerfBehaviour>();
        freeBuilders = new List<BuilderBehaviour>();

        ActionEvents.FreeBuilder += OnFreeBuilder;
        ActionEvents.BuilderRequest += OnBuilderRequest;
        ActionEvents.SerfRequest += OnSerfRequest;
        ActionEvents.FreeSerf += OnFreeSerf;
        ActionEvents.OrderStatusChanged += OnOrderStatusChanged;
    }

    private NavMeshSurface _roadNavMeshSurfaceComponent;
    private NavMeshSurface RoadNavMeshSurfaceComponent
    {
        get {
            if (_roadNavMeshSurfaceComponent == null)
            {
                if (RoadNavMeshSurface == null)
                {
                    RoadNavMeshSurface = Instantiate(RoadNavMeshSurfacePrefab);
                }                
                _roadNavMeshSurfaceComponent = RoadNavMeshSurface.GetComponent<NavMeshSurface>();
            }
            return _roadNavMeshSurfaceComponent;
        }
        set => _roadNavMeshSurfaceComponent = value;
    }

    private void OnOrderStatusChanged(SerfOrder serfOrder)
    {
        switch (serfOrder.Status)
        {
            case Status.SUCCESS:
                if (!CurrentSerfOrders.Remove(serfOrder))
                {
                    throw new Exception("Kon serforder niet verwijderen. Dit is 'DE' bug, die we gezien hadden met debuggen.");
                }
                CompletedOrders.Add(serfOrder);
                break;
            case Status.FAILED:
                if (!CurrentSerfOrders.Remove(serfOrder))
                {
                    throw new Exception(
                        "Kon serforder niet verwijderen. Dit is 'DE' bug, die we gezien hadden met debuggen.");
                }

                CompletedOrders.Add(serfOrder);
                CreateNewSerfReqAfterFailing(serfOrder);
                break;
        }
    }

    private void CreateNewSerfReqAfterFailing(SerfOrder origSerfOrder)
    {
        var serfRequest = new SerfRequest
        {
            Purpose = origSerfOrder.Purpose,
            ItemType = origSerfOrder.ItemType,
            GameObject = origSerfOrder.Current.GameObject,
            Direction = origSerfOrder.Current.Direction,
            IsOriginator = true
        };

        ActionEvents.SerfRequest(serfRequest);
    }

    public void RegisterRoad(Transform transform)
    {
        transform.SetParent(RoadNavMeshSurface.transform);
        RoadNavMeshSurfaceComponent.BuildNavMesh();
        ManagePossibleNewSerfOrders();
    }

    private void OnBuilderRequest(BuilderRequest builderRequest)
    {
        if (freeBuilders.Count > 0)
        {
            var builder = PopClosest(freeBuilders, builderRequest.Location);
            builder.AssignBuilderRequest(builderRequest);
        }
        else
        {
            BuilderRequests.Add(builderRequest);
        }
    }

    private void OnFreeBuilder(BuilderBehaviour builder)
    {
        var request = BuilderRequests.Pop();
        if (request != null)
        {
            builder.AssignBuilderRequest(request);
        }
        else
        {
            if(!IsFreeBuilder(builder))
            {
                freeBuilders.Add(builder);
            }
        }
    }


    public void ReevaluateCurrentOrders()
    {
        ManagePossibleNewSerfOrders();
    }

    private void OnSerfRequest(SerfRequest serfRequest)
    {
        ManagePossibleNewSerfOrders(serfRequest:serfRequest);
    }

    private void ManagePossibleNewSerfOrders(SerfRequest serfRequest = null, SerfBehaviour serfBehaviour = null)
    {
        AddPotentialFreeSerf(serfBehaviour);
        if (serfRequest != null)
        {
            var combined = TryCombineWithExistingOrder(serfRequest);
            if (!combined)
            {
                SerfRequests.Add(serfRequest);
            }
        }

        ManageStaleSerfOrders();
    }   

    private void ManageStaleSerfOrders()
    {
        // Eventueel hier wat omdraaien: order creation is 'duur/complex' en freeserfs.count is cheapo
        var order = TryCreateNewOrder();

        while (order != null && freeSerfs.Count > 0)
        {
            order.Assignee = PopClosest(freeSerfs, order.Location);
            order.Assignee.Assign(order);
            CurrentSerfOrders.Add(order);

            order = TryCreateNewOrder();
        }
    }

    private void AddPotentialFreeSerf(SerfBehaviour newSerfBehaviour)
    {
        if (newSerfBehaviour != null && !IsFreeSerf(newSerfBehaviour))
        {
            freeSerfs.Add(newSerfBehaviour);
        }        
    }

    public bool IsFreeSerf(SerfBehaviour serf)
    {
        return freeSerfs.Any(x => x == serf);
    }

    public bool TryRemoveSerfFromFreeSerfList(SerfBehaviour serf)
    {
        if(IsFreeSerf(serf))
        {
            return freeSerfs.Remove(serf);
        }

        return false;
    }

    public bool IsFreeBuilder(BuilderBehaviour builder)
    {
        return freeBuilders.Any(x => x == builder);
    }

    public bool TryRemoveBuilderFromFreeBuilderList(BuilderBehaviour builder)
    {
        if (IsFreeBuilder(builder))
        {
            return freeBuilders.Remove(builder);
        }

        return false;
    }

    private void OnFreeSerf(SerfBehaviour serf)
    {
        ManagePossibleNewSerfOrders(serfBehaviour:serf);
    }

    private SerfOrder TryCreateNewOrder()
    {
        SerfOrder order = null;

        // If there's no free serfs, theres no need to create an order
        if (freeSerfs.Count > 0)
        {
            var poppedSerfRequests = new List<SerfRequest>();
            while (order == null && SerfRequests.Count > 0)
            {
                var serfRequest = SerfRequests.Pop();
                poppedSerfRequests.Add(serfRequest);

                order = CreateNewOrder(serfRequest);
            }

            // Re-add all serfrequests that were popped and weren't able to create orders from.
            if (order != null)
            {
                poppedSerfRequests.Remove(order.To);
                poppedSerfRequests.Remove(order.From);
            }
            
            foreach (var poppedSerfRequest in poppedSerfRequests.Where(x => x.IsOriginator))
            {
                SerfRequests.Add(poppedSerfRequest);
            }
        }

        return order;
    }

    private bool TryCombineWithExistingOrder(SerfRequest serfRequest)
    {
        SerfOrder order = null;

        var path = new UnityEngine.AI.NavMeshPath();
        // Find any ongoing orders we can combine
        if (serfRequest.Direction == Direction.PULL)
        {
            var match = CurrentSerfOrders.FirstOrDefault(x =>
                !x.To.IsOriginator // This 'To' was created just to create an order, we can safely overrule it and forget the unoriginal request
                && x.To.ItemType == serfRequest.ItemType // We only care about an itemmatch
                && UnityEngine.AI.NavMesh.CalculatePath( // We need a valid path
                    x.From.Location, // From the match
                    serfRequest.Location, // To the pullrequest
                    serfRequest.Purpose.ToAreaMask(),
                    path)
            );

            order = match;
            if (order != null)
            {
                // Change priority of the order, so remove and add
                CurrentSerfOrders.Remove(order);
                order.To = serfRequest;
                CurrentSerfOrders.Add(order);
            }
        }
        else if (serfRequest.Direction == Direction.PUSH)
        {
            var match = CurrentSerfOrders.FirstOrDefault(x =>
                !x.From.IsOriginator // This 'From' was created just to create an order, we can safely overrule it and forget the unoriginal request
                && x.To.ItemType == serfRequest.ItemType // We only care about an itemmatch
                && x.Status ==
                Status.IN_PROGRESS_FROM // We can only take over orders that are still going to their from
                && UnityEngine.AI.NavMesh.CalculatePath( // We need a valid path
                    serfRequest.Location, // From the pushrequest
                    x.To.Location, // To the match
                    serfRequest.Purpose.ToAreaMask(),
                    path)
            );

            order = match;
            if (match != null)
            {
                // Change priority of the order, so remove and add
                CurrentSerfOrders.Remove(order);
                match.From = serfRequest;
                CurrentSerfOrders.Add(order);
            }
        }

        return order != null;
    }

    private T PopClosest<T>(List<T> behaviours, Vector3 objLocation) where T : MonoBehaviour
    {
        var closestBehaviour = behaviours.OrderBy(x => (x.transform.position - objLocation).sqrMagnitude).First();
        behaviours.Remove(closestBehaviour);
        return closestBehaviour;
    }

    private SerfOrder CreateNewOrder(SerfRequest serfRequest)
    {
        var path = new UnityEngine.AI.NavMeshPath();
        if (serfRequest == null)
        {
            return null;
        }

        var order = new SerfOrder
        {
            Status = Status.IN_PROGRESS_FROM
        };

        if (serfRequest.Direction == Direction.PULL)
        {
            order.To = serfRequest;
            
            var match = SerfRequests.FirstOrDefault(x =>
                x.Direction == Direction.PUSH 
                && x.ItemType == serfRequest.ItemType
                && UnityEngine.AI.NavMesh.CalculatePath(x.Location, serfRequest.Location, serfRequest.Purpose.ToAreaMask(), path));
            if (match != null)
            {
                order.From = match;
            }
            
            if (order.From == null)
            {
                // ter voorkoming dat 1 item in de stockpile 3x kan worden opgehaald
                var countItemsAlreadyBeingPickedUp = CurrentSerfOrders.Count(x => x.From.GameObject == MainCastle && x.ItemType == serfRequest.ItemType);

                var stockpileWithStock = stockpiles.FirstOrDefault(x =>
                    x.CurrentItemAmount.Any(x => x.ItemType == serfRequest.ItemType && x.Amount - countItemsAlreadyBeingPickedUp > 0) 
                    && (serfRequest.Purpose == Purpose.ROAD || UnityEngine.AI.NavMesh.CalculatePath(x.Location, serfRequest.Location, serfRequest.Purpose.ToAreaMask(), path)));
                if (stockpileWithStock != null)
                {
                    order.From = serfRequest.CloneOpposite();
                    order.From.GameObject = stockpileWithStock.gameObject;
                }
            }
        }
        else
        {
            order.From = serfRequest;

            var match = SerfRequests.FirstOrDefault(x =>
                x.Direction == Direction.PULL
                && x.ItemType == serfRequest.ItemType
                && UnityEngine.AI.NavMesh.CalculatePath(serfRequest.Location, x.Location, x.Purpose.ToAreaMask(), path));
            if (match != null)
            {
                order.To = match;
            }

            // Geen uitstaande pullrequests, dump naar de castle
            // TODO dump naar dichts bijzijnde, nvamashbereikbare stockpile
            if (order.To == null)
            {
                var stockpile = stockpiles.FirstOrDefault(x =>
                    serfRequest.Purpose == Purpose.ROAD || 
                    UnityEngine.AI.NavMesh.CalculatePath(x.Location, serfRequest.Location, serfRequest.Purpose.ToAreaMask(), path));
                if (stockpile != null)
                {
                    order.To = order.From.CloneOpposite();
                    order.To.GameObject = MainCastle;
                }
            }
        }

        if (order.From == null || order.To == null)
        {
            order = null;
        }

        if (order != null)
        {
            CurrentSerfOrders.Add(order);
            SerfRequests.Remove(order.From);
            SerfRequests.Remove(order.To);
        }

        return order;
    }
}