using System.Collections.Generic;
using System.Linq;

public partial class GameManager : BaseAEMonoCI
{
    private SerfOrder TryCreateNewOrder()
    {
        SerfOrder order = null;

        // If there's no free serfs, theres no need to create an order
        if (freeSerfs.Count > 0)
        {
            var poppedSerfRequests = new List<SerfRequest>();
            while (order == null && serfRequests.Count > 0)
            {
                var serfRequest = serfRequests.Pop();
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
                serfRequests.Add(poppedSerfRequest);
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
            var match = currentSerfOrders.FirstOrDefault(x =>
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
                currentSerfOrders.Remove(order);
                order.To = serfRequest;
                currentSerfOrders.Add(order);
            }
        }
        else if (serfRequest.Direction == Direction.PUSH)
        {
            var match = currentSerfOrders.FirstOrDefault(x =>
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
                currentSerfOrders.Remove(order);
                match.From = serfRequest;
                currentSerfOrders.Add(order);
            }
        }

        return order != null;
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
            
            var match = serfRequests.FirstOrDefault(x =>
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
                var countItemsAlreadyBeingPickedUp = currentSerfOrders.Count(x => x.From.GameObject == MainCastle && x.ItemType == serfRequest.ItemType && x.Status == Status.IN_PROGRESS_FROM);

                var stockpileWithStock = StockPilesManager.Instance.GetStockpiles().FirstOrDefault(x =>
                    x.CurrentItemAmount.Any(x => x.ItemType == serfRequest.ItemType && x.Amount - countItemsAlreadyBeingPickedUp > 0)
                    && (serfRequest.Purpose == Purpose.ROAD || HasPathForBuilding(x, serfRequest.Location, serfRequest.Purpose.ToAreaMask(), path)));
                if (stockpileWithStock != null)
                {
                    order.From = serfRequest.CloneOpposite();
                    order.From.OrderDestination = stockpileWithStock.GetComponentInParent<IOrderDestination>();
                }
            }
        }
        else
        {
            order.From = serfRequest;

            var match = serfRequests.FirstOrDefault(x =>
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
                var stockpile = StockPilesManager.Instance.GetStockpiles().FirstOrDefault(x =>
                    serfRequest.Purpose == Purpose.ROAD || 
                    UnityEngine.AI.NavMesh.CalculatePath(x.Location, serfRequest.Location, serfRequest.Purpose.ToAreaMask(), path));
                if (stockpile != null)
                {
                    order.To = order.From.CloneOpposite();
                    order.To.OrderDestination = mainCastleOrderDestination;
                }
            }
        }

        if (order.From == null || order.To == null)
        {
            order = null;
        }

        if (order != null)
        {
            currentSerfOrders.Add(order);
            serfRequests.Remove(order.From);
            serfRequests.Remove(order.To);
        }

        return order;
    }
}