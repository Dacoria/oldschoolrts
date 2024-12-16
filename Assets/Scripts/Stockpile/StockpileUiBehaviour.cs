using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class StockpileUiBehaviour : MonoBehaviour, ICardCarousselDisplay
{
    [HideInInspector]
    public StockpileBehaviour CallingStockpile;

    public StockpileResourceWrapperBehaviour StockpileResourceWrapperPrefab;
    public List<StockpileResourceWrapperBehaviour> StockpileResourceWrappers;

    private bool CardsLoaded;


    private void Start()
    {
        StockpileResourceWrappers = new List<StockpileResourceWrapperBehaviour>();
        foreach (var item in GameManager.Instance.ResourcePrefabItems.OrderBy(x => x.ItemType))
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

    // Update is called once per frame

    private int frame;

    private void Update()
    {
        frame++;
        if (frame == 20)
        {
            frame = 0;
            if (CallingStockpile != null)
            {
                foreach (var itemSprite in GameManager.Instance.ResourcePrefabItems)
                {
                    var itemType = CallingStockpile.CurrentItemAmount.Single(x => x.ItemType == itemSprite.ItemType);
                    var wrapper = StockpileResourceWrappers.Single(x => x.ItemType == itemSprite.ItemType);

                    wrapper.SetAmount(itemType.Amount);
                }
            }
        }
    }

    public int GetCount() => StockpileResourceWrappers.Count;

    public bool CardsAreLoaded() => CardsLoaded;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        StockpileResourceWrappers[indexOfCard].gameObject.SetActive(activeYN);
    }

    public int GetIndexFirstEnabledCard()
    {
        for (int i = 0; i < StockpileResourceWrappers.Count; i++)
        {
            var card = StockpileResourceWrappers[i];
            if (card.gameObject.activeSelf)
            {
                return i;
            }
        }

        return -1;
    }
}