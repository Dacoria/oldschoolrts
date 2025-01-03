using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WIP_CardsBuildingBeingBuildUiBehaviour : MonoBehaviourSlowUpdateFramesCI, ICardCarousselDisplay
{
    public ImageTextBehaviour ImageTextBehaviourPrefab;
    private IUiCallingBuilding callingBuilding;

    private List<ImageTextBehaviour> imageTextBehaviours;

    public GameObject ItemsNeededBuildGrid;

    private bool cardsLoaded;

    private RefillBehaviour refillStockpile; // bij resetten van cards geset
    private BuildingBehaviour buildingBehaviour; // bij resetten van cards geset
    private GhostBuildingBehaviour ghostBuildingBehaviour; // bij resetten van cards geset

    public bool CardsAreLoaded() => cardsLoaded;

    public int GetCount() => imageTextBehaviours == null ? 0 : imageTextBehaviours.Count;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        imageTextBehaviours[indexOfCard].gameObject.SetActive(activeYN);
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
        ItemsNeededBuildGrid.transform.parent.gameObject.SetActive(false);
        cardsLoaded = false;
    }

    private void ResetCards()
    {
        ClearAllCards();

        imageTextBehaviours = new List<ImageTextBehaviour>();
        refillStockpile = callingBuilding.GetGameObject()?.GetComponent<RefillBehaviour>();
        if (refillStockpile != null)
        {
            buildingBehaviour = callingBuilding.GetGameObject().GetComponent<BuildingBehaviour>();
            ghostBuildingBehaviour = callingBuilding.GetGameObject().GetComponent<GhostBuildingBehaviour>();

            foreach (var itemInStockpile in refillStockpile.StockpileOfItemsRequired)
            {
                var itemCountUiWrapper = Instantiate(ImageTextBehaviourPrefab, transform);
                itemCountUiWrapper.Image.sprite = ResourcePrefabs.Get().Single(x => x.ItemType == itemInStockpile.ItemType).Icon;
                itemCountUiWrapper.ItemType = itemInStockpile.ItemType;

                imageTextBehaviours.Add(itemCountUiWrapper);
            }

            cardsLoaded = true;
            ItemsNeededBuildGrid.transform.parent.gameObject.SetActive(true);
        }
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