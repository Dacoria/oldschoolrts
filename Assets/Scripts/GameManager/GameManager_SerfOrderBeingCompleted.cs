using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    private Dictionary<SerfOrder, SerfShowPackageHandling> serfOrdersBeingCompleted = new Dictionary<SerfOrder, SerfShowPackageHandling>();
    protected override void OnStartCompletingSerfRequest(SerfOrder order)
    {
        var serfShowPackageHandling = order.Assignee.gameObject.AddComponent<SerfShowPackageHandling>();
        serfOrdersBeingCompleted.Add(order, serfShowPackageHandling);
    }

    public bool SerfOrderIsBeingCompletedForGo(GameObject go)
    {
        if(serfOrdersBeingCompleted.Any(x => x.Key.Status == Status.IN_PROGRESS_FROM && x.Key.From.GameObject == go))
        {
            return true;
        }
        if (serfOrdersBeingCompleted.Any(x => x.Key.Status == Status.IN_PROGRESS_TO && x.Key.To.GameObject == go))
        {
            return true;
        }
        return false;
    }

    private void RemoveSerfOrderBeingCompleted(SerfOrder serfOrder)
    {
        if(serfOrdersBeingCompleted.TryGetValue(serfOrder, out var serfShowPackageHandling))
        {
            Destroy(serfOrdersBeingCompleted[serfOrder]); // destroy go SerfShowPackageHandling
            serfOrdersBeingCompleted.Remove(serfOrder); // verwijder hele order
        }        
    }
}