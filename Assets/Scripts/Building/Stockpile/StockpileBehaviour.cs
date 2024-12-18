using System;
using System.Linq;
using UnityEngine;

public class StockpileBehaviour : BaseAEMonoCI
{
    public ItemAmountBuffer[] InitialItemAmount;
    [HideInInspector] public ItemAmountBuffer[] CurrentItemAmount;

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
        GameManager.Instance.RegisterStockpile(this);
    }

    protected override void OnOrderStatusChanged(SerfOrder serfOrder)
    {
        if (serfOrder.From != null && serfOrder.From.GameObject == gameObject && serfOrder.Status == Status.IN_PROGRESS_TO)
        {

            var itemAmount = this.CurrentItemAmount.Single(x => x.ItemType == serfOrder.ItemType);
            // TODO beetje hacky maar het werkt
            itemAmount.Amount--;
        }

        if (serfOrder.To != null && serfOrder.To.GameObject == this.gameObject && serfOrder.Status == Status.SUCCESS)
        {

            var itemAmount = this.CurrentItemAmount.Single(x => x.ItemType == serfOrder.ItemType);
            // TODO beetje hacky maar het werkt
            itemAmount.Amount++;
        }
    }

    public Vector3 Location => this == null || this.gameObject == null ? new Vector3(0,0,0) : this.gameObject.EntranceExit();

    private void OnDestroy()
    {
        GameManager.Instance.TryRemoveStockpile(this);
    }
}