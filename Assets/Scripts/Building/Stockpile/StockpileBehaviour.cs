using System;
using System.Linq;
using UnityEngine;

public class StockpileBehaviour : BaseAEMonoCI, IValidateOrder
{
    public ItemAmountBuffer[] InitialItemAmount;
    [HideInInspector] public ItemAmountBuffer[] CurrentItemAmount;
    [ComponentInject] private IOrderDestination orderDestination;

    private new void Awake()
    {
        base.Awake();
        var allItemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));
        CurrentItemAmount = new ItemAmountBuffer[allItemTypes.Length];

        for (int i = 0; i < allItemTypes.Length; i++)
        {
            var itemType = allItemTypes[i];
            var currentAmount = 0;
            foreach (var initialItemType in InitialItemAmount)
            {
                if (initialItemType.ItemType == itemType)
                {
                    currentAmount = initialItemType.Amount;
                }
            }

            CurrentItemAmount[i] = new ItemAmountBuffer { ItemType = itemType, Amount = currentAmount};
        }
    }

    void Start()
    {
        StockPilesManager.Instance.RegisterStockpile(this);
    }

    public bool CanProcessOrder(SerfOrder serfOrder)
    {
        if (serfOrder.Status == Status.IN_PROGRESS_FROM)
        {
            var itemAmount = this.CurrentItemAmount.Single(x => x.ItemType == serfOrder.ItemType);
            return itemAmount.Amount > 0;
        }

        return true;
    }

    protected override void OnOrderStatusChanged(SerfOrder serfOrder, Status prevStatus)
    {
        if (serfOrder.From != null && serfOrder.From.GameObject == orderDestination.GetGO() && serfOrder.Status == Status.IN_PROGRESS_TO)
        {
            // TODO beetje hacky maar het werkt
            var itemAmount = this.CurrentItemAmount.Single(x => x.ItemType == serfOrder.ItemType);
            if(itemAmount.Amount > 0)
            {
                itemAmount.Amount--;
            }
            else
            {
                throw new Exception("CanProcessOrder niet aangeroepen? (Stockpile)");              
            }
        }

        if (serfOrder.To != null && serfOrder.To.GameObject == orderDestination.GetGO() && serfOrder.Status == Status.SUCCESS)
        {

            var itemAmount = this.CurrentItemAmount.Single(x => x.ItemType == serfOrder.ItemType);
            // TODO beetje hacky maar het werkt
            itemAmount.Amount++;
        }
    }

    public Vector3 Location => this == null || this.gameObject == null ? new Vector3(0,0,0) : this.gameObject.EntranceExit();

    private void OnDestroy()
    {
        StockPilesManager.Instance?.TryRemoveStockpile(this);
    }
}