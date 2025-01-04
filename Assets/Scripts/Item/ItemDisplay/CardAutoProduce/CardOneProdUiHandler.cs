using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class CardOneProdUiHandler : MonoBehaviour, ICardCarousselDisplay, IProcesOneItemUI, IUiCallingBuilding
{   
    public UiCardBehaviour UiCardBehaviourPrefab;
    [HideInInspector] public List<UiCardBehaviour> UiCardBehaviours =  new List<UiCardBehaviour>();

    [HideInInspector] public bool CardsAreLoaded;

    [HideInInspector] public ICardOneProdBuilding CallingBuilding;
    private BuildingType buildingType;

    void OnEnable()
    {
        if (CallingBuilding == null)
            return;

        UiCardBehaviours.Clear();
        buildingType = CallingBuilding.GetGameObject().GetComponentInParent<BuildingBehaviour>().BuildingType;
        var produceSettings = buildingType.GetItemProduceSettings().First();
        
        if (buildingType.GetCategory() == BuildingCategory.OneProductOverTime)
        {
            foreach (var itemToConsume in produceSettings.ItemsConsumedToProduce)
            {
                var rscPrefab = ResourcePrefabs.Get().First(x => x.ItemType == itemToConsume.ItemType);
                InitNewCardGo(rscPrefab.Icon, rscPrefab.ItemType, itemToConsume.Amount.ToString());
            }            
        }

        var rightArrowGo = Instantiate(Load.GoMapUI[Constants.GO_PREFAB_UI_PRODUCE_ITEM_RIGHT_ARROW], transform);
        var cardUIRightArrow = rightArrowGo.GetComponent<UiCardBehaviour>();
        //if(buildingType.GetCategory() == BuildingCategory.Manual)
        //    cardUIRightArrow.CountText.text = "";
        //else
        //    cardUIRightArrow.CountText.text = $"{buildingType.GetProductionDurationSettings().TimeToProduceResourceInSeconds}";

        UiCardBehaviours.Add(cardUIRightArrow);

        foreach (var itemToProduce in produceSettings.ItemsToProduce)
        {
            var rscPrefab = ResourcePrefabs.Get().First(x => x.ItemType == itemToProduce.ItemType);
            InitNewCardGo(rscPrefab.Icon, rscPrefab.ItemType, itemToProduce.ProducedPerProdCycle.ToString());
        }

        CardsAreLoaded = true;
    }

    private void InitNewCardGo(Sprite icon, Enum enumType, string countText)
    {
        var uiCardBehaviourGo = Instantiate(UiCardBehaviourPrefab, transform);
        uiCardBehaviourGo.Image.sprite = icon;

        var enumString = enumType.ToString();
        UiCardBehaviours.Add(uiCardBehaviourGo);

        uiCardBehaviourGo.Type = enumType;
        uiCardBehaviourGo.CountText.text = countText;
    }

    private void OnDisable()
    {
        foreach (var uiCard in UiCardBehaviours)
        {
            Destroy(uiCard.gameObject);
        }
        UiCardBehaviours.Clear();
        CardsAreLoaded = false;
    }
  
    public int GetCount() => UiCardBehaviours.Count;

    bool ICardCarousselDisplay.CardsAreLoaded() => CardsAreLoaded;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        UiCardBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }

    public TypeProcessing GetCurrentItemProcessed() => CallingBuilding?.GetCurrentTypesProcessed()?.FirstOrDefault();
    public float GetBuildTimeInSeconds() => buildingType.GetProductionDurationSettings().TimeToProduceResourceInSeconds;

    public GameObject GetGameObject() => CallingBuilding?.GetGameObject();
}