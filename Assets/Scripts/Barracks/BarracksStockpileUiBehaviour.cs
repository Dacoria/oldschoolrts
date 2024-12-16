using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarracksStockpileUiBehaviour : MonoBehaviour, ICardCarousselDisplay
{
    public ImageTextBehaviour ImageTextBehaviourPrefab;
    private List<ImageTextBehaviour> ImageTextBehaviours;

    private CardUiHandler BarracksUIHandler;

    public Text Title;

    [HideInInspector]
    private bool CardsLoaded;

    public bool CardsAreLoaded() => CardsLoaded;

    public int GetCount() => ImageTextBehaviours == null ? 0 : ImageTextBehaviours.Count;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        ImageTextBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }

    public int GetIndexFirstEnabledCard()
    {
        for (int i = 0; i < ImageTextBehaviours.Count; i++)
        {
            var card = ImageTextBehaviours[i];
            if (card.gameObject.activeSelf)
            {
                return i;
            }
        }

        return -1;
    }

    private void Awake()
    {
        ImageTextBehaviours = new List<ImageTextBehaviour>();
        BarracksUIHandler = transform.parent.GetComponentInChildren<CardUiHandler>();
    }

    private void OnDisable()
    {
        ClearAllCards();
    }

    private int updateCounter;

    private void Update()
    {
        if(updateCounter == 0)
        {
            UpdateValuesOfCard();
        }
        updateCounter++;
        if(updateCounter > 30)
        {
            updateCounter = 0;
        }
    }    

    private void ClearAllCards()
    {
        for (int i = ImageTextBehaviours.Count - 1; i >= 0; i--)
        {
            var imageTextBehaviour = ImageTextBehaviours[i];
            Destroy(imageTextBehaviour.gameObject);
        }
        ImageTextBehaviours.RemoveAll(x => true);
        Title.gameObject.SetActive(false);
        CardsLoaded = false;
    }

    private void OnEnable()
    {
        ResetCards();
    }

    private RefillBehaviour BarracksStockpile;

    private void ResetCards()
    {
        ClearAllCards();

        ImageTextBehaviours = new List<ImageTextBehaviour>();

        // ja, omslachtig :')
        BarracksStockpile = BarracksUIHandler?.CallingBuilding?.GetQueueForBuildingBehaviour()?.GetComponent<RefillBehaviour>();
        if (BarracksStockpile != null)
        {
            foreach (var itemInStockpile in BarracksStockpile.StockpileOfItemsRequired)
            {
                var itemCountUiWrapper = Instantiate(ImageTextBehaviourPrefab, transform);
                itemCountUiWrapper.Image.sprite = GameManager.Instance.ResourcePrefabItems.Single(x => x.ItemType == itemInStockpile.ItemType).Icon;
                itemCountUiWrapper.Text.text = itemInStockpile.Amount.ToString();
                itemCountUiWrapper.ItemType = itemInStockpile.ItemType;

                ImageTextBehaviours.Add(itemCountUiWrapper);
            }

            CardsLoaded = true;
            Title.gameObject.SetActive(true);
        }
    }

    private void UpdateValuesOfCard()
    {
        if (CardsLoaded && BarracksStockpile != null)
        {
            foreach (var card in ImageTextBehaviours)
            {
                card.Text.text = BarracksStockpile.StockpileOfItemsRequired.Single(x => x.ItemType == card.ItemType).Amount.ToString();
            }
        }
    }
}