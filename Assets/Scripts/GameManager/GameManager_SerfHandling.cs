using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : BaseAEMonoCI
{
    private static SortedSet<SerfRequest> serfRequests = new SortedSet<SerfRequest>();
    private SortedSet<SerfOrder> currentSerfOrders = new SortedSet<SerfOrder>();
    private static List<SerfBehaviour> freeSerfs = new List<SerfBehaviour>();

    public SortedSet<SerfOrder> CompletedOrdersIncFailed = new SortedSet<SerfOrder>();   

    public SortedSet<SerfRequest> GetSerfRequests() => serfRequests;
    public SortedSet<SerfOrder> GetCurrentSerfOrders() => currentSerfOrders;

    private void InitServes()
    {
        StartCoroutine(CheckOrdersEverySec());
    }

    private DateTime lastTimeCheckSerfOrders;
    private IEnumerator CheckOrdersEverySec()
    {
        yield return Wait4Seconds.Get(1);        
        var msSinceLastCheck = (DateTime.Now - lastTimeCheckSerfOrders).TotalMilliseconds;
        if (msSinceLastCheck > 1000)
        {
            ReevaluateCurrentOrders();
        }        

        StartCoroutine(CheckOrdersEverySec());
    }
    
    protected override void OnOrderStatusChanged(SerfOrder serfOrder, Status prevStatus)
    {
        RemoveSerfOrderBeingCompleted(serfOrder); // blijkbaar een status afgerond; anders kom je hier niet

        switch (serfOrder.Status)
        {
            case Status.SUCCESS:
                if (!currentSerfOrders.Remove(serfOrder))
                {
                    // Vermoedelijke oorzaak -> tijdens een OnOrderStatusChanged wordt de status geupdate naar een nieuwe --> oplossing: mini delay voor nieuwe update
                    throw new Exception("Kon serforder niet verwijderen. Dit is 'DE' bug, die we gezien hadden met debuggen.");
                }
                CompletedOrdersIncFailed.Add(serfOrder);
                break;
            case Status.FAILED:
                if (!currentSerfOrders.Remove(serfOrder))
                {
                    // Vermoedelijke oorzaak -> tijdens een OnOrderStatusChanged wordt de status geupdate naar een nieuwe --> oplossing: mini delay voor nieuwe update
                    throw new Exception(
                        "Kon serforder niet verwijderen. Dit is 'DE' bug, die we gezien hadden met debuggen.");
                }

                CompletedOrdersIncFailed.Add(serfOrder);
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
            OrderDestination = origSerfOrder.Current.OrderDestination,
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

    protected override void OnFreeSerf(SerfBehaviour serf)
    {
        ManagePossibleNewSerfOrders(serfBehaviour: serf);
    }

    private void ManagePossibleNewSerfOrders(SerfRequest serfRequest = null, SerfBehaviour serfBehaviour = null)
    {
        AddPotentialFreeSerf(serfBehaviour);
        if (serfRequest != null)
        {
            var combined = TryCombineWithExistingOrder(serfRequest);
            if (!combined)
            {
                serfRequests.Add(serfRequest);
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
            order.Assignee = freeSerfs.PopClosest(order.Location);
            order.Assignee.Assign(order);
            currentSerfOrders.Add(order);

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

    public bool IsFreeSerf(SerfBehaviour serf) => freeSerfs.Any(x => x == serf);

    public bool TryRemoveSerfFromFreeSerfList(SerfBehaviour serf)
    {
        if(IsFreeSerf(serf))
        {
            return freeSerfs.Remove(serf);
        }

        return false;
    }
}