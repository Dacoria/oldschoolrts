using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardsStockpileInUiBehaviour : MonoBehaviourSlowUpdateFramesCI, ICardCarousselDisplay
{
    public ImageTextBehaviour ImageTextBehaviourPrefab;
    private IUiCallingBuilding callingBuilding;

    private List<ImageTextBehaviour> imageTextBehaviours;    

    public Text Title;
    private bool cardsLoaded;

    private RefillBehaviour refillStockpile; // bij resetten van cards geset

    public bool CardsAreLoaded() => cardsLoaded;

    public int GetCount() => imageTextBehaviours == null ? 0 : imageTextBehaviours.Count;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        imageTextBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }  

    private new void Awake()
    {
        base.Awake();
        callingBuilding = transform.parent.GetComponentInChildren<IUiCallingBuilding>(true);
        if (callingBuilding == null )
            callingBuilding = transform.parent.transform.parent.GetComponentInChildren<IUiCallingBuilding>(true);
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
        refillStockpile = callingBuilding.GetGameObject()?.GetComponent<RefillBehaviour>();
        if (refillStockpile != null)
        {
            foreach (var itemInStockpile in refillStockpile.StockpileOfItemsRequired)
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

        if(imageTextBehaviours.Count == 0)
            Title.gameObject.SetActive(false);
    }    

    private void UpdateValuesOfCard()
    {
        if (cardsLoaded && refillStockpile != null)
        {
            foreach (var card in imageTextBehaviours)
            {
                card.Text.text = refillStockpile.StockpileOfItemsRequired.Single(x => x.ItemType == card.ItemType).Amount.ToString();
            }
        }
    }
}