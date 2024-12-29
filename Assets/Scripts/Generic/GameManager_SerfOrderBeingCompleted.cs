using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    private Dictionary<SerfOrder, SerfShowPackageHandling> SerfOrdersBeingCompleted = new Dictionary<SerfOrder, SerfShowPackageHandling>();
    protected override void OnStartCompletingSerfRequest(SerfOrder order)
    {
        var serfShowPackageHandling = order.Assignee.gameObject.AddComponent<SerfShowPackageHandling>();
        SerfOrdersBeingCompleted.Add(order, serfShowPackageHandling);
    }

    public bool SerfOrderIsBeingCompletedForGo(GameObject go)
    {
        if(SerfOrdersBeingCompleted.Any(x => x.Key.Status == Status.IN_PROGRESS_FROM && x.Key.From.GameObject == go))
        {
            return true;
        }
        if (SerfOrdersBeingCompleted.Any(x => x.Key.Status == Status.IN_PROGRESS_TO && x.Key.To.GameObject == go))
        {
            return true;
        }
        return false;
    }

    private void RemoveSerfOrderBeingCompleted(SerfOrder serfOrder)
    {
        if(SerfOrdersBeingCompleted.TryGetValue(serfOrder, out var serfShowPackageHandling))
        {
            Destroy(SerfOrdersBeingCompleted[serfOrder]); // destroy go SerfShowPackageHandling
            SerfOrdersBeingCompleted.Remove(serfOrder); // verwijder hele order
        }        
    }
}