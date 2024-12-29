using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarracksStockpileUiBehaviour : MonoBehaviour, ICardCarousselDisplay
{
    public ImageTextBehaviour ImageTextBehaviourPrefab;
    private List<ImageTextBehaviour> imageTextBehaviours;

    private CardUiHandler barracksUIHandler;

    public Text Title;
    private bool cardsLoaded;

    public bool CardsAreLoaded() => cardsLoaded;

    public int GetCount() => imageTextBehaviours == null ? 0 : imageTextBehaviours.Count;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        imageTextBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }

    public int GetIndexFirstEnabledCard()
    {
        for (int i = 0; i < imageTextBehaviours.Count; i++)
        {
            var card = imageTextBehaviours[i];
            if (card.gameObject.activeSelf)
            {
                return i;
            }
        }

        return -1;
    }

    private void Awake()
    {
        imageTextBehaviours = new List<ImageTextBehaviour>();
        barracksUIHandler = transform.parent.GetComponentInChildren<CardUiHandler>();
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
        for (int i = imageTextBehaviours.Count - 1; i >= 0; i--)
        {
            var imageTextBehaviour = imageTextBehaviours[i];
            Destroy(imageTextBehaviour.gameObject);
        }
        imageTextBehaviours.RemoveAll(x => true);
        Title.gameObject.SetActive(false);
        cardsLoaded = false;
    }

    private void OnEnable()
    {
        ResetCards();
    }

    private RefillBehaviour BarracksStockpile;

    private void ResetCards()
    {
        ClearAllCards();

        imageTextBehaviours = new List<ImageTextBehaviour>();
        SetBarracksStockpile();
        if (BarracksStockpile != null)
        {
            foreach (var itemInStockpile in BarracksStockpile.StockpileOfItemsRequired)
            {
                var itemCountUiWrapper = Instantiate(ImageTextBehaviourPrefab, transform);
                itemCountUiWrapper.Image.sprite = ResourcePrefabs.Get().Single(x => x.ItemType == itemInStockpile.ItemType).Icon;
                itemCountUiWrapper.Text.text = itemInStockpile.Amount.ToString();
                itemCountUiWrapper.ItemType = itemInStockpile.ItemType;

                imageTextBehaviours.Add(itemCountUiWrapper);
            }

            cardsLoaded = true;
            Title.gameObject.SetActive(true);
        }
    }

    private void SetBarracksStockpile()
    {
        BarracksStockpile = null;
        var barracks = barracksUIHandler?.CallingBuilding;
        if(barracks != null)
        {
            BarracksStockpile = ((BarracksBehaviour)barracks).RefillBehaviour;
        }
    }

    private void UpdateValuesOfCard()
    {
        if (cardsLoaded && BarracksStockpile != null)
        {
            foreach (var card in imageTextBehaviours)
            {
                card.Text.text = BarracksStockpile.StockpileOfItemsRequired.Single(x => x.ItemType == card.ItemType).Amount.ToString();
            }
        }
    }
}