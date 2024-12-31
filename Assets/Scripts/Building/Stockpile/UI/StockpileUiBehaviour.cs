using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StockpileUiBehaviour : MonoBehaviourSlowUpdateFramesCI, ICardCarousselDisplay
{
    [HideInInspector] public StockpileBehaviour CallingStockpile;

    public StockpileResourceWrapperBehaviour StockpileResourceWrapperPrefab;
    public List<StockpileResourceWrapperBehaviour> StockpileResourceWrappers;

    private bool CardsLoaded;


    private void Start()
    {
        StockpileResourceWrappers = new List<StockpileResourceWrapperBehaviour>();
        foreach (var item in ResourcePrefabs.Get().Where(x => x.ItemType != ItemType.NONE).OrderBy(x => x.ItemType))
        {
            var stockpileResourceWrapper = Instantiate(StockpileResourceWrapperPrefab, transform);
            stockpileResourceWrapper.Image.sprite = item.Icon;
            stockpileResourceWrapper.ItemType = item.ItemType;
            stockpileResourceWrapper.StockpileUiBehaviour = this;
            StockpileResourceWrappers.Add(stockpileResourceWrapper);
        }

        CardsLoaded = true;
    }

    // aangeroepen door wrapper
    public void IncreaseStockpileAmountOnClick(ItemType itemType)
    {
        if (CallingStockpile != null)
        {
            var stockpileOfItem = CallingStockpile.CurrentItemAmount.Single(x => x.ItemType == itemType);

            // TODO => iets met buffers? (initieel niet ingesteld, resulteert nu in een max buffer van 0)
            stockpileOfItem.Amount++;
            
            GameManager.Instance.ReevaluateCurrentOrders();

            // TODO -> na klikken kan aan 1 item meerdere orders worden gekoppeld!!!
        }
    }

    protected override int FramesTillSlowUpdate => 20;
    protected override void SlowUpdate()
    {        
        if (CallingStockpile != null)
        {
            foreach (var itemSprite in ResourcePrefabs.Get().Where(x => x.ItemType != ItemType.NONE))
            {
                var itemType = CallingStockpile.CurrentItemAmount.Single(x => x.ItemType == itemSprite.ItemType);
                var wrapper = StockpileResourceWrappers.Single(x => x.ItemType == itemSprite.ItemType);

                wrapper.SetAmount(itemType.Amount);
            }
        }        
    }

    public int GetCount() => StockpileResourceWrappers.Count;

    public bool CardsAreLoaded() => CardsLoaded;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        StockpileResourceWrappers[indexOfCard].gameObject.SetActive(activeYN);
    }
}