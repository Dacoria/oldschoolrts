using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardsStockpileOutUiBehaviour : MonoBehaviourSlowUpdateFramesCI, ICardCarousselDisplay
{
    [ComponentInject] private GridLayoutGroup gridLayoutGroup;
    public ImageTextBehaviour ImageTextBehaviourPrefab;
    private IUiCallingBuilding callingBuilding;

    private List<ImageTextBehaviour> imageTextBehaviours;    

    public Text Title;
    private bool cardsLoaded;

    private HandleProduceResourceOrderBehaviour produceRscBehaviour; // bij resetten van cards geset

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
        produceRscBehaviour = callingBuilding.GetGameObject()?.GetComponent<HandleProduceResourceOrderBehaviour>();
        if (produceRscBehaviour != null)
        {

            var buildingType = callingBuilding.GetGameObject().GetComponentInParent<BuildingBehaviour>().BuildingType;
            var itemsToProduceSettings = buildingType.GetItemProduceSettings();
                
            imageTextBehaviours = new List<ImageTextBehaviour>();
        
            foreach (var itemSet in itemsToProduceSettings)
            {
                foreach (var itemToProduce in itemSet.ItemsToProduce)
                {
                    var itemCountUiWrapper = Instantiate(ImageTextBehaviourPrefab, transform);
                    itemCountUiWrapper.Image.sprite = ResourcePrefabs.Get().Single(x => x.ItemType == itemToProduce.ItemType).Icon;
                    itemCountUiWrapper.Text.text = "0";
                    itemCountUiWrapper.ItemType = itemToProduce.ItemType;

                    imageTextBehaviours.Add(itemCountUiWrapper);
                }
            }            

            cardsLoaded = true;
            Title.gameObject.SetActive(true);

            if (imageTextBehaviours.Count > 3)
                gridLayoutGroup.constraintCount = 2;
            else 
                gridLayoutGroup.constraintCount = 1;
        }

        if(imageTextBehaviours.Count == 0)
            Title.gameObject.SetActive(false);
    }    

    private void UpdateValuesOfCard()
    {
        if (cardsLoaded && produceRscBehaviour != null)
        {
            foreach (var card in imageTextBehaviours)
            {
                card.Text.text = produceRscBehaviour.OutputOrders.Count(x => x.ItemType == card.ItemType).ToString();
            }
        }
    }
}