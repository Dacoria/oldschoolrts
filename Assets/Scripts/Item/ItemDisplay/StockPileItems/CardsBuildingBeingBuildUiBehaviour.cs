using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardsBuildingBeingBuildUiBehaviour : MonoBehaviourSlowUpdateFramesCI
{
    public ImageTextBehaviour ImageTextBehaviourPrefab;
    public GhostBuildingBehaviour CallingGhostBuildingBehaviour; // wordt geset voor activeren
    public Image BuildingImage; // wordt geset voor activeren

    private List<ImageTextBehaviour> imageTextBehaviours = new List<ImageTextBehaviour>();

    public GameObject ItemsNeededGrid;

    private bool cardsLoaded;

    private BuildingBehaviour buildingBehaviour; // bij resetten van cards geset

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
        this.gameObject.SetDirectChildrenInactive();
        cardsLoaded = false;
    }

    private void ResetCards()
    {
        ClearAllCards();

        imageTextBehaviours = new List<ImageTextBehaviour>();
        if (CallingGhostBuildingBehaviour != null)
        {
            buildingBehaviour = CallingGhostBuildingBehaviour.GetComponentInParent<BuildingBehaviour>();
            foreach (var buildItem in buildingBehaviour.BuildingType.GetBuildCosts())
            {
                var itemCountUiWrapper = Instantiate(ImageTextBehaviourPrefab, ItemsNeededGrid.transform);
                itemCountUiWrapper.Image.sprite = ResourcePrefabs.Get().Single(x => x.ItemType == buildItem.ItemType).Icon;
                itemCountUiWrapper.ItemType = buildItem.ItemType;
                itemCountUiWrapper.Text.text = "";
                imageTextBehaviours.Add(itemCountUiWrapper);
            }

            BuildingImage.sprite = BuildingPrefabs.Get().Single(x => x.BuildingType == buildingBehaviour.BuildingType).Icon;
            this.gameObject.SetDirectChildrenActive();
            cardsLoaded = true;
            
        }
    }

    private void UpdateValuesOfCard()
    {
        if (cardsLoaded && CallingGhostBuildingBehaviour != null)
        {            
            var buildItems = buildingBehaviour.BuildingType.GetBuildCosts();
            foreach (var card in imageTextBehaviours)
            {
                var countItemsBrought = GameManager.Instance.CompletedOrdersIncFailed.Count(x => 
                    x.Status == Status.SUCCESS && x.
                    To.OrderDestination == (IOrderDestination)buildingBehaviour &&
                    x.ItemType == card.ItemType);

                card.Text.text = $"{countItemsBrought}/{buildItems.Single(x => x.ItemType == card.ItemType).Amount}";
            }
        }
    }
}