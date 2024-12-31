using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardsStockpileUiBehaviour : MonoBehaviourSlowUpdateFramesCI, ICardCarousselDisplay
{
    public ImageTextBehaviour ImageTextBehaviourPrefab;
    public CardUiHandler CardUIHandler;

    private List<ImageTextBehaviour> imageTextBehaviours;    

    public Text Title;
    private bool cardsLoaded;

    private RefillBehaviour RefillStockpile; // bij resetten van cards geset

    public bool CardsAreLoaded() => cardsLoaded;

    public int GetCount() => imageTextBehaviours == null ? 0 : imageTextBehaviours.Count;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        imageTextBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }  

    private new void Awake()
    {
        base.Awake();
        imageTextBehaviours = new List<ImageTextBehaviour>();
    }

    private void OnEnable()
    {
        ResetCards();
    }

    private void OnDisable()
    {
        ClearAllCards();
    }

    protected override int FramesTillSlowUpdate => 30;
    protected override void SlowUpdate()
    {
        UpdateValuesOfCard();
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

    private void ResetCards()
    {
        ClearAllCards();

        imageTextBehaviours = new List<ImageTextBehaviour>();
        RefillStockpile = CardUIHandler.CallingBuilding?.GetGameObject().GetComponent<RefillBehaviour>();
        if (RefillStockpile != null)
        {
            foreach (var itemInStockpile in RefillStockpile.StockpileOfItemsRequired)
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

    private void UpdateValuesOfCard()
    {
        if (cardsLoaded && RefillStockpile != null)
        {
            foreach (var card in imageTextBehaviours)
            {
                card.Text.text = RefillStockpile.StockpileOfItemsRequired.Single(x => x.ItemType == card.ItemType).Amount.ToString();
            }
        }
    }
}