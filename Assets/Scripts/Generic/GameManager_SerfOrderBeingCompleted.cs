using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GameManager : BaseAEMonoCI
{
    private List<SerfOrder> SerfOrdersBeingCompleted = new List<SerfOrder>();
    protected override void OnStartCompletingSerfRequest(SerfOrder order)
    {
        SerfOrdersBeingCompleted.Add(order);
    }

    public bool SerfOrderIsBeingCompletedForGo(GameObject go)
    {
        if(SerfOrdersBeingCompleted.Any(x => x.Status == Status.IN_PROGRESS_FROM && x.From.GameObject == go))
        {
            return true;
        }
        if (SerfOrdersBeingCompleted.Any(x => x.Status == Status.IN_PROGRESS_TO && x.To.GameObject == go))
        {
            return true;
        }
        return false;
    }

    private void RemoveSerfOrderBeingCompleted(SerfOrder serfOrder)
    {
        SerfOrdersBeingCompleted.Remove(serfOrder);
    }
}