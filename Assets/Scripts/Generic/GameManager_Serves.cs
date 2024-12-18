using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    private static SortedSet<SerfRequest> SerfRequests = new SortedSet<SerfRequest>();
    private SortedSet<SerfOrder> CurrentSerfOrders = new SortedSet<SerfOrder>();
    private static List<SerfBehaviour> freeSerfs = new List<SerfBehaviour>();

    public SortedSet<SerfOrder> CompletedOrders = new SortedSet<SerfOrder>();   

    public SortedSet<SerfRequest> GetSerfRequests() => SerfRequests;
    public SortedSet<SerfOrder> GetCurrentSerfOrders() => CurrentSerfOrders;


    private void InitServes()
    {
        StartCoroutine(CheckOrdersEverySec());
    }

    private DateTime lastTimeCheckSerfOrders;
    private IEnumerator CheckOrdersEverySec()
    {
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(1);        
        var msSinceLastCheck = (DateTime.Now - lastTimeCheckSerfOrders).TotalMilliseconds;
        if (msSinceLastCheck > 1000)
        {
            ReevaluateCurrentOrders();
        }        

        StartCoroutine(CheckOrdersEverySec());
    }
    
    protected override void OnOrderStatusChanged(SerfOrder serfOrder)
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

        AE.SerfRequest?.Invoke(serfRequest);
    } 
   
    public void ReevaluateCurrentOrders()
    {
        ManagePossibleNewSerfOrders();
    }

    protected override void OnSerfRequest(SerfRequest serfRequest)
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
        lastTimeCheckSerfOrders = DateTime.Now;
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

    protected override void OnFreeSerf(SerfBehaviour serf)
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
                    && (serfRequest.Purpose == Purpose.ROAD || HasPathForBuilding(x, serfRequest.Location, serfRequest.Purpose.ToAreaMask(), path)));
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